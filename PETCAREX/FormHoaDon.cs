using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PETCAREX
{
    public partial class FormHoaDon : Form
    {
        // ====== CẤU HÌNH HỆ THỐNG ======
        string strCon = @"Data Source=.;Initial Catalog=QLTC;Integrated Security=True";
        int currentMaKH = 0;
        decimal phanTramKM = 0;
        int currentMaNV = UserSession.MaUser;

        // ====== CONTROLS ======
        FlowLayoutPanel flowMain;
        Panel panelCustomer, panelLoaiHD, panelDichVu, panelSanPham, panelThanhToan;
        TextBox txtSDT, txtTimSP;
        Label lblTenKH, lblTrangThai, lblTamTinh, lblTongTien;
        RadioButton rdoDichVu, rdoSanPham;
        DataGridView dgvThuCung, dgvLichHen, dgvSanPham;
        ComboBox cboThanhToan;
        Button btnTimKH, btnXacNhan;
        // Controls

        public FormHoaDon()
        {
            InitializeComponent();
            InitUI();
        }

        #region UI Initialization
        void InitUI()
        {
            this.Text = "LẬP HÓA ĐƠN";
            this.Size = new Size(1100, 850);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(250, 250, 250);

            flowMain = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoScroll = true, Padding = new Padding(20) };
            this.Controls.Add(flowMain);

            InitCustomerPanel();
            InitLoaiHDPanel();
            InitDichVuPanel();
            InitSanPhamPanel();
            InitThanhToanPanel();
        }
        void MoveCheckedRowToTop(DataGridView dgv, int viewRowIndex)
        {
            if (!(dgv.DataSource is DataTable dt)) return;

            // 🔥 LẤY DataRowView THẬT
            DataRowView drv = dgv.Rows[viewRowIndex].DataBoundItem as DataRowView;
            if (drv == null) return;

            DataRow realRow = drv.Row;

            // Clone row
            DataRow newRow = dt.NewRow();
            newRow.ItemArray = realRow.ItemArray;

            // Xóa row gốc – insert lên đầu DataTable
            dt.Rows.Remove(realRow);
            dt.Rows.InsertAt(newRow, 0);

            // Refresh
            dgv.ClearSelection();
            dgv.Rows[0].Selected = true;
        }



        void InitCustomerPanel()
        {
            panelCustomer = CreateGroup("1. Thông tin khách hàng", 130);
            txtSDT = new TextBox
            {
                Location = new Point(20, 50),
                Width = 250,
                Font = new Font("Segoe UI", 12),
                Text = "Nhập số điện thoại khách hàng...",
                ForeColor = Color.Gray
            };
            // Thêm sự kiện Placeholder
            txtSDT.Enter += (s, e) => {
                if (txtSDT.Text == "Nhập số điện thoại khách hàng...")
                {
                    txtSDT.Text = "";
                    txtSDT.ForeColor = Color.Black;
                }
            };
            txtSDT.Leave += (s, e) => {
                if (string.IsNullOrWhiteSpace(txtSDT.Text))
                {
                    txtSDT.Text = "Nhập số điện thoại khách hàng...";
                    txtSDT.ForeColor = Color.Gray;
                }
            };
            btnTimKH = CreateButton("TÌM KIẾM", 280, 48);
            btnTimKH.Click += BtnTimKH_Click;
            lblTenKH = CreateLabel("Khách hàng: Chưa xác định", 20, 90);
            lblTrangThai = CreateLabel("", 400, 90);
            panelCustomer.Controls.AddRange(new Control[] { txtSDT, btnTimKH, lblTenKH, lblTrangThai });
            flowMain.Controls.Add(panelCustomer);
        }

        void InitLoaiHDPanel()
        {
            panelLoaiHD = CreateGroup("2. Loại hóa đơn", 90);
            rdoDichVu = new RadioButton { Text = "Thanh toán Dịch vụ", Location = new Point(20, 50), Width = 200, Checked = true };
            rdoSanPham = new RadioButton { Text = "Bán Sản phẩm lẻ", Location = new Point(250, 50), Width = 200 };
            rdoDichVu.CheckedChanged += (s, e) => { panelDichVu.Visible = rdoDichVu.Checked; panelSanPham.Visible = rdoSanPham.Checked; TinhTongTien(); };
            panelLoaiHD.Controls.AddRange(new Control[] { rdoDichVu, rdoSanPham });
            flowMain.Controls.Add(panelLoaiHD);
        }

        void InitDichVuPanel()
        {
            panelDichVu = CreateGroup("3. Danh sách Dịch vụ", 450); // Tăng chiều cao panel

            dgvThuCung = CreateGrid(20, 50, 300, 350); // Bảng thú cưng dài hơn
            dgvThuCung.SelectionChanged += (s, e) => LoadDichVuCUaPet();

            dgvLichHen = CreateGrid(340, 50, 680, 350);
            dgvLichHen.ScrollBars = ScrollBars.Both; // Luôn có thanh trượt nếu nhiều dòng
            dgvLichHen.CellValueChanged += (s, e) => TinhTongTien();
            dgvLichHen.CurrentCellDirtyStateChanged += (s, e) => { if (dgvLichHen.IsCurrentCellDirty) dgvLichHen.CommitEdit(DataGridViewDataErrorContexts.Commit); };

            panelDichVu.Controls.AddRange(new Control[] { CreateLabel("Chọn Thú cưng:", 20, 30), dgvThuCung, CreateLabel("Dịch vụ khả dụng:", 340, 30), dgvLichHen });
            flowMain.Controls.Add(panelDichVu);
        }

        void InitSanPhamPanel()
        {
            panelSanPham = CreateGroup("3. Danh mục Sản phẩm", 400);
            panelSanPham.Visible = false;

            txtTimSP = new TextBox { Location = new Point(20, 40), Width = 300, Font = new Font("Segoe UI", 10) };
            // Sửa lại đoạn code trong InitSanPhamPanel
            txtTimSP.TextChanged += (s, e) => {
                if (dgvSanPham.DataSource is DataTable dt)
                {
                    // Trước khi lọc, kết thúc việc edit để đảm bảo dữ liệu đã vào DataTable
                    dgvSanPham.EndEdit();

                    string keyword = txtTimSP.Text.Trim().Replace("'", "''");

                    if (string.IsNullOrEmpty(keyword))
                        dt.DefaultView.RowFilter = "";
                    else
                        dt.DefaultView.RowFilter = $"TENSP LIKE '%{keyword}%'";
                }
            };
            dgvSanPham = CreateGrid(20, 75, 1000, 300);
            dgvSanPham.CellValueChanged += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                if (dgvSanPham.Columns[e.ColumnIndex].Name != "Selected") return;

                dgvSanPham.EndEdit(); // 🔥 LUÔN ĐẨY DATA

                bool isChecked = Convert.ToBoolean(
                    dgvSanPham.Rows[e.RowIndex].Cells["Selected"].Value
                );

                if (isChecked)
                {
                    MoveCheckedRowToTop(dgvSanPham, e.RowIndex);
                }

                TinhTongTien();
            };

            dgvSanPham.CurrentCellDirtyStateChanged += (s, e) => { if (dgvSanPham.IsCurrentCellDirty) dgvSanPham.CommitEdit(DataGridViewDataErrorContexts.Commit); };
            dgvSanPham.CellEndEdit += (s, e) => {
                if (dgvSanPham.Columns[e.ColumnIndex].Name == "SLMua")
                {
                    var row = dgvSanPham.Rows[e.RowIndex];
                    int slTon = Convert.ToInt32(row.Cells["SLTONKHO"].Value);
                    int slMua;

                    if (!int.TryParse(row.Cells["SLMua"].Value?.ToString(), out slMua) || slMua < 1)
                    {
                        MessageBox.Show("Số lượng mua phải là số nguyên dương!", "Lỗi");
                        row.Cells["SLMua"].Value = 1;
                    }
                    else if (slMua > slTon)
                    {
                        MessageBox.Show($"Lỗi: Số lượng mua ({slMua}) vượt quá tồn kho ({slTon})!", "Cảnh báo");
                        row.Cells["SLMua"].Value = 1; // Reset về 1 hoặc slTon tùy ông
                    }
                    TinhTongTien();
                }
            };
            panelSanPham.Controls.AddRange(new Control[] { CreateLabel("Nhập tên sản phẩm để tìm:", 20, 20), txtTimSP, dgvSanPham });
            flowMain.Controls.Add(panelSanPham);
        }
        private bool ShowQRDialog()
        {
            using (Form qrForm = new Form())
            {
                qrForm.Text = "QUÉT MÃ THANH TOÁN QR";
                qrForm.Size = new Size(400, 550); // Tăng kích thước tí cho dễ nhìn
                qrForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                qrForm.StartPosition = FormStartPosition.CenterParent;
                qrForm.MaximizeBox = false;

                PictureBox pb = new PictureBox
                {
                    Dock = DockStyle.Top,
                    Height = 350,
                    SizeMode = PictureBoxSizeMode.Zoom, // Chỉnh Zoom để không bị vỡ hình
                    Padding = new Padding(10)
                };

                // LẤY HÌNH TỪ THƯ MỤC ASSETS
                try
                {
                    // Đường dẫn chạy từ thư mục thực thi (bin/Debug) ra ngoài thư mục gốc Project
                    string projectPath = Application.StartupPath;
                    // Nếu chạy trong Visual Studio, hình nằm ở Assets của Project
                    string imagePath = System.IO.Path.Combine(projectPath, "..", "..", "Assets", "ma-qr-code-la-gi.png");

                    if (System.IO.File.Exists(imagePath))
                    {
                        pb.Image = Image.FromFile(imagePath);
                    }
                    else
                    {
                        // Nếu không tìm thấy file theo đường dẫn tương đối (khi đóng gói app)
                        // Ông có thể chép file hình vào cùng thư mục với file .exe
                        pb.Image = Image.FromFile("ma-qr-code-la-gi.png");
                    }
                }
                catch
                {
                    // Nếu lỗi thì hiện icon cảnh báo mặc định
                    pb.Image = SystemIcons.Question.ToBitmap();
                }
                Label lbl = new Label { Text = "Vui lòng yêu cầu khách quét mã QR\nSố tiền: " + lblTongTien.Text, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 10) };
                Button btnXacNhanQR = new Button { Text = "XÁC NHẬN ĐÃ NHẬN TIỀN", Dock = DockStyle.Bottom, Height = 50, BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

                btnXacNhanQR.Click += (s, e) => { qrForm.DialogResult = DialogResult.OK; qrForm.Close(); };

                qrForm.Controls.Add(lbl);
                qrForm.Controls.Add(pb);
                qrForm.Controls.Add(btnXacNhanQR);

                return qrForm.ShowDialog() == DialogResult.OK;
            }
        }

        void InitThanhToanPanel()
        {
            panelThanhToan = CreateGroup("4. Thanh toán", 150);
            lblTamTinh = CreateLabel("Tạm tính: 0đ", 20, 50);
            lblTongTien = CreateLabel("TỔNG TIỀN: 0đ", 20, 80);
            lblTongTien.Font = new Font("Segoe UI", 14, FontStyle.Bold); lblTongTien.ForeColor = Color.Red;
            lblTamTinh.AutoSize = false;
            lblTamTinh.Size = new Size(300, 30);

            lblTongTien.AutoSize = false;
            lblTongTien.Size = new Size(300, 40);
            cboThanhToan = new ComboBox { Location = new Point(400, 75), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            cboThanhToan.Items.AddRange(new string[] { "Tiền mặt", "QR" });
            cboThanhToan.SelectedIndex = 0;

            btnXacNhan = CreateButton("XÁC NHẬN", 620, 70);
            btnXacNhan.BackColor = Color.FromArgb(46, 204, 113);
            btnXacNhan.Click += BtnXacNhan_Click;

            // Thêm nút Xem Toa vào Panel Thanh Toán
            Button btnXemToa = CreateButton("XEM TOA", 740, 70);
            btnXemToa.BackColor = Color.FromArgb(52, 152, 219); // Màu xanh dương
            btnXemToa.ForeColor = Color.White;
            btnXemToa.Click += (s, e) => {
                if (rdoDichVu.Checked) HienThiBangToaThuoc();
                else MessageBox.Show("Vui lòng chọn thanh toán Dịch vụ để xem toa!");
            };
            panelThanhToan.Controls.Add(btnXemToa);
            panelThanhToan.Controls.AddRange(new Control[] { lblTamTinh, lblTongTien, CreateLabel("Hình thức:", 400, 50), cboThanhToan, btnXacNhan });
            flowMain.Controls.Add(panelThanhToan);
        }
        #endregion

        #region Data Logic
        private void BtnTimKH_Click(object sender, EventArgs e)
        {
            string sdt = txtSDT.Text;
            if (sdt == "Nhập số điện thoại khách hàng..." || sdt.Length < 10) return;
            try
            {
                using (SqlConnection conn = new SqlConnection(strCon))
                {
                    SqlCommand cmd = new SqlCommand("SP_LAYKHACHHANG", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SDT", txtSDT.Text.Trim());
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        currentMaKH = (int)dt.Rows[0]["MAKH"];
                        phanTramKM = Convert.ToDecimal(dt.Rows[0]["PhanTramKM"]);
                        lblTenKH.Text = $"Khách hàng: {dt.Rows[0]["HOTEN"]} - Hạng: {dt.Rows[0]["MACAP"]}";
                        lblTrangThai.Text = $"✔ Giảm giá: {phanTramKM * 100}%";
                        LoadThuCung(conn);
                        LoadSanPham(conn);
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        void LoadThuCung(SqlConnection conn)
        {
            string sql = "SELECT MATC, TENTC FROM THU_CUNG WHERE MAKH = @makh";
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            da.SelectCommand.Parameters.AddWithValue("@makh", currentMaKH);
            DataTable dt = new DataTable(); da.Fill(dt);
            dgvThuCung.DataSource = dt;

            dgvThuCung.DataSource = dt;


        }

        void EnsureSelectedColumn(DataTable dt)
        {
            if (!dt.Columns.Contains("Selected"))
            {
                DataColumn col = new DataColumn("Selected", typeof(bool));
                col.DefaultValue = false;
                dt.Columns.Add(col);
            }
        }

        private void FormHoaDon_Load(object sender, EventArgs e)
        {

        }
        DataTable dtAllDichVu = new DataTable();
        void LoadDichVuCUaPet()
        {
            if (dgvThuCung.CurrentRow == null) return;

            int maTC = (int)dgvThuCung.CurrentRow.Cells["MATC"].Value;

            try
            {
                using (SqlConnection conn = new SqlConnection(strCon))
                {
                    SqlCommand cmd = new SqlCommand("SP_DICHVUPHUHOP", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MaTC", maTC);
                    cmd.Parameters.AddWithValue("@MaNV", currentMaNV);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Thêm cột MATC nếu chưa có trong dt
                    if (!dt.Columns.Contains("MATC"))
                    {
                        dt.Columns.Add("MATC", typeof(int));
                    }

                    // Gán MATC cho tất cả rows
                    foreach (DataRow r in dt.Rows)
                    {
                        r["MATC"] = maTC;
                    }

                    // Khởi tạo dtAllDichVu nếu chưa có cột
                    if (dtAllDichVu.Columns.Count == 0)
                    {
                        foreach (DataColumn c in dt.Columns)
                        {
                            dtAllDichVu.Columns.Add(c.ColumnName, c.DataType);
                        }
                    }

                    // Thêm dữ liệu mới vào dtAllDichVu, tránh trùng STT + MATC
                    foreach (DataRow newRow in dt.Rows)
                    {
                        bool isExists = false;

                        string loai = newRow["Loai"].ToString(); // "TP" hoặc "KB"
                        string newMaDV = newRow["MaDV"].ToString();
                        int newSTT = Convert.ToInt32(newRow["STT"]);
                        int newMaTC = Convert.ToInt32(newRow["MATC"]);

                        int? newMaTP = null;
                        if (loai == "TP" && newRow["MaTP"] != DBNull.Value)
                            newMaTP = Convert.ToInt32(newRow["MaTP"]);

                        foreach (DataRow oldRow in dtAllDichVu.Rows)
                        {
                            string oldLoai = oldRow["Loai"].ToString();

                            if (oldLoai != loai) continue;

                            // ===== TC_KB: KHÓA (MADV + STT + MATC) =====
                            if (loai == "KB")
                            {
                                if (oldRow["MaDV"].ToString() == newMaDV &&
       Convert.ToInt32(oldRow["STT"]) == newSTT &&
       Convert.ToInt32(oldRow["MATC"]) == newMaTC)
                                {
                                    isExists = true;
                                    break;
                                }
                            }
                            // ===== TC_TP: KHÓA (MATP + STT + MATC) =====
                            else if (loai == "TP")
                            {
                                if (oldRow["MaTP"] == DBNull.Value) continue;

                                int oldMaTP = Convert.ToInt32(oldRow["MaTP"]);

                                if (newMaTP.HasValue &&
                                    oldMaTP == newMaTP.Value &&
                                    Convert.ToInt32(oldRow["STT"]) == newSTT &&
                                    Convert.ToInt32(oldRow["MATC"]) == newMaTC)
                                {
                                    isExists = true;
                                    break;
                                }
                            }
                        }
                        if (!isExists)
                            dtAllDichVu.ImportRow(newRow);
                    }

                    // Hiển thị lên grid
                    dgvLichHen.DataSource = null;
                    dgvLichHen.Columns.Clear();
                    dgvLichHen.DataSource = dtAllDichVu.Copy();

                    // Header
                    if (dgvLichHen.Columns.Contains("Gia"))
                        dgvLichHen.Columns["Gia"].HeaderText = "Giá (sau ưu đãi)";
                    if (dgvLichHen.Columns.Contains("TENDV"))
                        dgvLichHen.Columns["TENDV"].HeaderText = "Tên dịch vụ";
                    if (dgvLichHen.Columns.Contains("ThoiGianHen"))
                        dgvLichHen.Columns["ThoiGianHen"].HeaderText = "Thời gian hẹn";

                    // Ẩn cột
                    if (dgvLichHen.Columns.Contains("MADV")) dgvLichHen.Columns["MADV"].Visible = false;
                    if (dgvLichHen.Columns.Contains("STT")) dgvLichHen.Columns["STT"].Visible = false;
                    if (dgvLichHen.Columns.Contains("MATC")) dgvLichHen.Columns["MATC"].Visible = false;
                }

                // Cập nhật tổng tiền
                TinhTongTien();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }




        void LoadSanPham(SqlConnection conn)
        {
            string sql = @"SELECT sp.MASP, sp.TENSP, sp.GIABAN, cs.SLTONKHO 
                           FROM SAN_PHAM sp JOIN CN_SP cs ON sp.MASP = cs.MASP
                           WHERE cs.MACN = (SELECT TOP 1 MACN FROM CN_NV WHERE MANS = @manv ORDER BY NGAYBDMOI DESC)
                           ";
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            da.SelectCommand.Parameters.AddWithValue("@manv", currentMaNV);
            DataTable dt = new DataTable(); da.Fill(dt);
            dgvSanPham.DataSource = dt;
            if (!dt.Columns.Contains("Selected"))
                dt.Columns.Add("Selected", typeof(bool));

            if (!dt.Columns.Contains("SLMua"))
            {
                DataColumn col = new DataColumn("SLMua", typeof(int));
                col.AllowDBNull = false;
                col.DefaultValue = 1;
                dt.Columns.Add(col);
            }

            dgvSanPham.DataSource = dt;

            // CHỈ CHO EDIT 2 CỘT
            foreach (DataGridViewColumn col in dgvSanPham.Columns)
            {
                col.ReadOnly = !(col.Name == "Selected" || col.Name == "SLMua");
            }
        }

        void TinhTongTien()
        {
            long tamTinh = 0;

            // ================= DỊCH VỤ =================
            if (rdoDichVu.Checked && dgvLichHen.DataSource != null)
            {
                foreach (DataGridViewRow r in dgvLichHen.Rows)
                {
                    if (r.IsNewRow) continue;
                    if (r.Cells["Gia"].Value != DBNull.Value)
                        tamTinh += Convert.ToInt64(r.Cells["Gia"].Value);
                }
            }
            // ================= SẢN PHẨM =================
            else if (rdoSanPham.Checked && dgvSanPham.DataSource is DataTable dt)
            {
                dgvSanPham.EndEdit(); // Đẩy dữ liệu UI xuống DataTable

                foreach (DataRow r in dt.Rows) // ❗ DUYỆT DATA TABLE
                {
                    if (r["Selected"] != DBNull.Value &&
                        Convert.ToBoolean(r["Selected"]))
                    {
                        int sl = (r["SLMua"] != DBNull.Value)
                                 ? Convert.ToInt32(r["SLMua"])
                                 : 1;

                        tamTinh += Convert.ToInt64(r["GIABAN"]) * sl;
                    }
                }
            }

            lblTamTinh.Text = $"Tạm tính: {tamTinh:N0}đ";
            lblTongTien.Text = $"TỔNG TIỀN: {(tamTinh * (1 - phanTramKM)):N0}đ";
        }


        #endregion

        #region Execution
        private async void BtnXacNhan_Click(object sender, EventArgs e)
        {
            if (currentMaKH == 0)
            {
                MessageBox.Show("Vui lòng tìm kiếm khách hàng trước!", "Thông báo");
                return;
            }
            // --- ĐOẠN CHECK 10 PHÚT ĐÃ ĐƯỢC SỬA ---
            if (rdoDichVu.Checked)
            {
                DateTime bayGio = DateTime.Now;
                bool biQuaHan = false;
                string chiTietLoi = "";

                foreach (DataGridViewRow r in dgvLichHen.Rows)
                {
                    // 1. Bỏ qua dòng mới (dòng trống cuối grid)
                    if (r.IsNewRow) continue;


                    if (rdoDichVu.Checked && dgvLichHen.Rows.Count == 0)
                    {
                        // 3. Kiểm tra null cho cột GIOHEN để tránh lỗi ép kiểu (Crack tại đây)
                        if (r.Cells["ThoiGianHen"].Value != null && r.Cells["ThoiGianHen"].Value != DBNull.Value)
                        {
                            DateTime thoiGianHen = Convert.ToDateTime(r.Cells["ThoiGianHen"].Value);
                            double lechPhut = (bayGio - thoiGianHen).TotalMinutes;

                            if (lechPhut <= 10)
                            {
                                MessageBox.Show("Dịch vụ còn trong thời gian check-in, không thể thanh toán ngay!");
                                return;
                            }

                            if (lechPhut >= 60)
                            {
                                MessageBox.Show("Dịch vụ đã quá hạn thanh toán (quá 60 phút)!");
                                return;
                            }

                        }
                    }
                }

                if (biQuaHan)
                {
                    MessageBox.Show($"Không thể thanh toán! \nLịch hẹn lúc: {chiTietLoi} đã quá hạn 10 phút. \nVui lòng yêu cầu khách hàng đặt lại lịch mới.",
                                    "Quá hạn lịch hẹn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Dừng hàm tại đây một cách an toàn
                }
            }
            // --- Commit edit trên DataGridView trước khi Build XML ---
            dgvSanPham.EndEdit();
            dgvSanPham.CommitEdit(DataGridViewDataErrorContexts.Commit);

            dgvLichHen.EndEdit();
            dgvLichHen.CommitEdit(DataGridViewDataErrorContexts.Commit);

            // --- Chuẩn bị XML ---
            string xmlDV = null;
            string xmlSP = null;
            int loaiHD = rdoDichVu.Checked ? 1 : 0;

            if (rdoDichVu.Checked)
            {
                xmlDV = BuildXML(dgvLichHen);
                if (xmlDV == null) { MessageBox.Show("Chưa chọn dịch vụ nào!"); return; }
            }
            else
    {
        xmlSP = BuildXML(dgvSanPham);
        if (xmlSP == null)
        {
            MessageBox.Show("Chưa chọn sản phẩm lẻ!");
            return;
        }
    }

            // 2. XỬ LÝ XÁC NHẬN THANH TOÁN QR
            if (cboThanhToan.Text.Contains("QR"))
            {
                // Hiện mã QR cho khách quét
                if (!ShowQRDialog()) return;

                // Sau khi đóng Dialog QR, hiện thêm 1 câu hỏi xác thực cuối cùng
                DialogResult dr = MessageBox.Show("Xác nhận khách hàng đã chuyển khoản thành công?",
                                                "Xác nhận thanh toán",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Question);
                if (dr == DialogResult.No) return;
            }
            else // Đối với tiền mặt cũng nên hỏi 1 câu cho chắc
            {
                if (MessageBox.Show("Xác nhận khách đã trả tiền mặt?", "Xác nhận",
                    MessageBoxButtons.YesNo) == DialogResult.No) return;
            }

            // 3. GỌI STORE PROCEDURE
            try
            {
                using (SqlConnection conn = new SqlConnection(strCon))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("SP_LAPHOADONTONGHOP", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@MaKH", currentMaKH);
                        cmd.Parameters.AddWithValue("@MaNV", currentMaNV);
                        cmd.Parameters.AddWithValue("@HTThanhToan", cboThanhToan.Text);
                        cmd.Parameters.AddWithValue("@LoaiHD", loaiHD);

                        // Gửi XML tương ứng, cái còn lại truyền DBNull để Store biết đường mà bỏ qua
                        cmd.Parameters.Add("@XML_DichVu", SqlDbType.Xml).Value = (object)xmlDV ?? DBNull.Value;
                        cmd.Parameters.Add("@XML_SanPham", SqlDbType.Xml).Value = (object)xmlSP ?? DBNull.Value;

                        await cmd.ExecuteNonQueryAsync();

                        MessageBox.Show("Thanh toán thành công & Đã ghi nhận hóa đơn!",
                                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ResetForm();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("LỖI HỆ THỐNG: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void HienThiBangToaThuoc()
        {
            string xmlDV = BuildXML(dgvLichHen);
            if (dgvLichHen.Rows.Count == 0)
            {
                MessageBox.Show("Không có dịch vụ khám nào để xem toa thuốc!");
                return;
            }

            DataTable dtToa = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(strCon))
                {
                    SqlCommand cmd = new SqlCommand("SP_LAYTOATHUOC", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@XML_DichVu", SqlDbType.Xml).Value = xmlDV;
                    new SqlDataAdapter(cmd).Fill(dtToa);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi SQL: " + ex.Message);
                return;
            }

            if (dtToa.Rows.Count == 0)
            {
                MessageBox.Show("Không có thuốc trong các ca khám đã chọn!");
                return;
            }

            // --- TẠO GIAO DIỆN (Giữ nguyên phần Panel của bạn) ---
            Panel pnlOverlay = new Panel
            {
                Name = "pnlToaThuocOverlay",
                Size = new Size(750, 500),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            pnlOverlay.Location = new Point((this.Width - pnlOverlay.Width) / 2, (this.Height - pnlOverlay.Height) / 2);

            Label lblTitle = new Label
            {
                Text = "CHI TIẾT TOA THUỐC",
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = Color.FromArgb(44, 62, 80),
                ForeColor = Color.White
            };

            DataGridView dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                DataSource = dtToa,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            Button btnClose = new Button
            {
                Text = "ĐÓNG (CLICK ĐỂ THOÁT)",
                Dock = DockStyle.Bottom,
                Height = 40,
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnClose.Click += (s, e) => { this.Controls.Remove(pnlOverlay); };

            pnlOverlay.Controls.Add(dgv);
            pnlOverlay.Controls.Add(lblTitle);
            pnlOverlay.Controls.Add(btnClose);
            this.Controls.Add(pnlOverlay);
            pnlOverlay.BringToFront();

            // --- ĐỊNH DẠNG CỘT AN TOÀN (Sửa lỗi crash ở đây) ---
            string[] cols = { "TENTHUOC", "SOLUONG", "DON_GIA", "THAN_TIEN" };
            string[] names = { "Tên thuốc", "SL", "Đơn giá", "Thành tiền" };

            for (int i = 0; i < cols.Length; i++)
            {
                if (dgv.Columns.Contains(cols[i]))
                {
                    dgv.Columns[cols[i]].HeaderText = names[i];
                    if (cols[i].Contains("GIA") || cols[i].Contains("TIEN"))
                        dgv.Columns[cols[i]].DefaultCellStyle.Format = "N0";
                }
            }
        }
        string BuildXML(DataGridView dgv)
        {
            // Lấy DataTable gốc từ DataSource
            DataTable dt = dgv.DataSource as DataTable;
            if (dt == null) return null;

            // Đảm bảo các thay đổi cuối cùng trên Grid đã được đẩy xuống DataTable
            dgv.EndEdit();

            StringBuilder sb = new StringBuilder("<root>");
            bool hasData = false;

            // DUYỆT TRÊN DATATABLE (Duyệt tất cả dòng, kể cả dòng đang bị ẩn do lọc)
            foreach (DataRow dr in dt.Rows)
            {
                if (dgv == dgvLichHen)
                {
                    string loai = dr["Loai"].ToString();
                    string maTC = dr["MATC"].ToString();
                    string maDV = dr["MADV"].ToString();
                    string stt = dr["STT"].ToString();

                    if (loai == "TP")
                    {
                        string maTP = dr["MATP"].ToString();
                        sb.AppendFormat(
                            "<item loai=\"TP\" madv=\"{0}\" matp=\"{1}\" stt=\"{2}\" matc=\"{3}\" />",
                            maDV, maTP, stt, maTC
                        );
                    }
                    else // KB
                    {
                        sb.AppendFormat(
                            "<item loai=\"KB\" madv=\"{0}\" stt=\"{1}\" matc=\"{2}\" />",
                            maDV, stt, maTC
                        );
                    }

                    hasData = true;
                }
                else if (dgv == dgvSanPham)
                {
                    // 🔥 LỖI Ở ĐÂY: Phải kiểm tra xem dòng này có được TICK hay không
                    if (dr["Selected"] == DBNull.Value || Convert.ToBoolean(dr["Selected"]) == false)
                    {
                        continue; // Nếu không tick thì bỏ qua dòng này, chuyển sang dòng kế tiếp
                    }

                    int sl = 1;
                    if (dt.Columns.Contains("SLMua") && dr["SLMua"] != DBNull.Value)
                    {
                        if (int.TryParse(dr["SLMua"].ToString(), out int tmp) && tmp > 0)
                        {
                            sl = tmp;
                        }
                    }

                    // Nhớ sửa 'sl' thành 'soluong' như tui nói ở câu trước để tránh lỗi NULL nhé
                    sb.AppendFormat(
                        "<item masp=\"{0}\" soluong=\"{1}\" />",
                        dr["MASP"], sl
                    );
                    hasData = true;
                }
            }

            sb.Append("</root>");
            return hasData ? sb.ToString() : null;
        }
        #endregion

        #region Helpers
        private Panel CreateGroup(string title, int h)
        {
            Panel p = new Panel { Width = 1080, Height = h, BackColor = Color.White, Margin = new Padding(0, 0, 0, 15) };
            p.Controls.Add(new Label { Text = title.ToUpper(), Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(255, 107, 107), Location = new Point(10, 5), AutoSize = true });
            return p;
        }

        void ResetForm()
        {
            // 1. Reset thông tin khách hàng
            currentMaKH = 0;
            txtSDT.Text = "Nhập số điện thoại khách hàng...";
            txtSDT.ForeColor = Color.Gray;
            lblTenKH.Text = "Khách hàng: Chưa xác định";
            lblTrangThai.Text = "";

            // 2. Xóa dữ liệu trên các bảng
            if (dgvThuCung.DataSource is DataTable dtPet) dtPet.Clear();
            if (dgvLichHen.DataSource is DataTable dtDV) dtDV.Clear();
            // Riêng bảng sản phẩm thì giữ nguyên danh sách nhưng bỏ tích chọn
            foreach (DataGridViewRow row in dgvSanPham.Rows)
            {
                row.Cells["Selected"].Value = false;
                row.Cells["SLMua"].Value = 1;
            }

            // 3. Reset tiền tệ
            lblTamTinh.Text = "Tạm tính: 0đ";
            lblTongTien.Text = "TỔNG TIỀN: 0đ";

            // 4. Đưa về tab mặc định (Dịch vụ)
            rdoDichVu.Checked = true;
            panelDichVu.Visible = true;
            panelSanPham.Visible = false;

            // Cuốn màn hình lên đầu
            flowMain.AutoScrollPosition = new Point(0, 0);
        }
        private Button CreateButton(string t, int x, int y) => new Button { Text = t, Location = new Point(x, y), Size = new Size(110, 35), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(255, 107, 107), ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
        private Label CreateLabel(string t, int x, int y) => new Label { Text = t, Location = new Point(x, y), AutoSize = true, Font = new Font("Segoe UI", 10) };
        private DataGridView CreateGrid(int x, int y, int w, int h)
        {
            DataGridView dgv = new DataGridView
            {
                Location = new Point(x, y),
                Size = new Size(w, h),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect, // Chọn nguyên dòng
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                AllowUserToResizeRows = false, // KHÔNG cho đổi chiều cao hàng
                MultiSelect = false,
                GridColor = Color.LightGray
            };

            // Bỏ màu xanh mặc định, đổi thành màu nhẹ nhàng hơn khi chọn
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 230, 230); // Màu hồng nhạt
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;

            // Chặn Sort khi click vào Header
            dgv.DataBindingComplete += (s, e) => {
                foreach (DataGridViewColumn column in dgv.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }
            };

            return dgv;
        }

        void AddCheckBoxColumn(DataGridView dgv)
        {
            if (!dgv.Columns.Contains("Selected"))
            {
                DataGridViewCheckBoxColumn checkCol = new DataGridViewCheckBoxColumn
                {
                    Name = "Selected",
                    HeaderText = "Chọn",
                    DataPropertyName = "Selected", // 🔥 DÒNG QUYẾT ĐỊNH
                    Width = 50
                };
                dgv.Columns.Insert(0, checkCol);
            }
        }


        #endregion
    }
}
