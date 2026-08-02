using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PETCAREX
{
    public partial class FormDieuChuyen : Form
    {
        // --- CẤU HÌNH HỆ THỐNG ---
        string strCon = @"Data Source=.;Initial Catalog=QLTC;Integrated Security=True";
        DataTable dtNhanSu = new DataTable();

        // --- COLORS (Đồng bộ với FormDatLich) ---
        Color colorMain = Color.FromArgb(220, 70, 70);       // Đỏ chính
        Color colorSoft = Color.FromArgb(255, 180, 180);     // Hồng nhạt
        Color colorBg = Color.FromArgb(255, 242, 242);       // Nền form

        // --- CONTROLS ---
        Panel pnlLeft, pnlRight;
        DataGridView dgvNhanSu;
        TextBox txtSearch;
        ComboBox cboChiNhanhMoi;
        DateTimePicker dtpNgayBD, dtpNgayKT;
        Label lblTargetName, lblTargetCurrentCN;
        Button btnConfirm, btnCancel;

        public FormDieuChuyen()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            // --- THÊM 2 DÒNG NÀY ---
            this.AutoScroll = true;
            this.AutoScrollMinSize = new Size(1150, 650); // Đảm bảo vùng làm việc luôn đủ 1150x650


            this.Size = new Size(1150, 650);
            this.Text = "PETCAREX - ĐIỀU CHUYỂN NHÂN SỰ";
            this.BackColor = colorBg;
            this.Font = new Font("Segoe UI", 10);

            InitUI();
            LoadDataToRAM();
            LoadDSChiNhanh();
        }

        private void InitUI()
        {
            // 1. TIÊU ĐỀ FORM
            Label lblTitle = new Label()
            {
                Text = "♻️ ĐIỀU CHUYỂN NHÂN SỰ",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = colorMain,
                Location = new Point(25, 15),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            // 2. PANEL BÊN TRÁI (DANH SÁCH)-
            pnlLeft = CreateRoundedPanel(25, 80, 520, 500, Color.White);
            pnlLeft.Resize += pnlLeft_Resize;
            this.Controls.Add(pnlLeft);
            pnlLeft.SendToBack(); // Ép nó nằm dưới cùng để không che icon Sidebar

            Label lblSearchHint = new Label() { Text = "Tìm theo tên hoặc SĐT:", Location = new Point(20, 15), AutoSize = true, ForeColor = Color.Gray };
            pnlLeft.Controls.Add(lblSearchHint);

            txtSearch = new TextBox()
            {
                Location = new Point(20, 38),
                Size = new Size(480, 35), // Thu hẹp TextBox search
                Font = new Font("Segoe UI", 12),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;
            pnlLeft.Controls.Add(txtSearch);

            dgvNhanSu = new DataGridView()
            {
                Location = new Point(20, 85),
                Size = new Size(610, 390),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 9)
            };
            dgvNhanSu.CellClick += DgvNhanSu_CellClick;
            StyleGrid(dgvNhanSu);
            pnlLeft.Controls.Add(dgvNhanSu);

            // 3. PANEL BÊN PHẢI (THÔNG TIN CHI TIẾT)
            pnlRight = CreateRoundedPanel(560, 80, 400, 500, Color.White);
            pnlRight.Enabled = false; // Chỉ mở khi chọn nhân sự
            this.Controls.Add(pnlRight);

            Label lblRightTitle = new Label() { Text = "CẤU HÌNH ĐIỀU CHUYỂN", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = colorMain, Location = new Point(20, 20), AutoSize = true };
            pnlRight.Controls.Add(lblRightTitle);

            // Thông tin nhân sự đang chọn
            lblTargetName = new Label() { Text = "Nhân sự: chưa chọn", Font = new Font("Segoe UI", 11, FontStyle.Bold), Location = new Point(20, 65), Size = new Size(360, 25) };
            lblTargetCurrentCN = new Label() { Text = "Tại: ...", Font = new Font("Segoe UI", 10), Location = new Point(20, 90), Size = new Size(360, 25), ForeColor = Color.DimGray };
            pnlRight.Controls.Add(lblTargetName); pnlRight.Controls.Add(lblTargetCurrentCN);

            // Nhập liệu
            AddLabelAndControl(pnlRight, "Chọn chi nhánh đến:", cboChiNhanhMoi = new ComboBox() { DropDownStyle = ComboBoxStyle.DropDownList }, 140);
            AddLabelAndControl(pnlRight, "Ngày kết thúc chi nhánh cũ:", dtpNgayKT = new DateTimePicker() { Format = DateTimePickerFormat.Short }, 210);
            dtpNgayKT.MinDate = DateTime.Today.AddDays(1); // Chỉ cho phép chọn từ ngày mai trở đi
            AddLabelAndControl(pnlRight, "Ngày bắt đầu chi nhánh mới:", dtpNgayBD = new DateTimePicker() { Format = DateTimePickerFormat.Short }, 280);
            dtpNgayBD.MinDate = DateTime.Today.AddDays(1);
            // Nút bấm
            btnConfirm = CreateStyledButton("XÁC NHẬN ĐIỀU CHUYỂN", 20, 380, colorMain, Color.White);
            btnConfirm.Click += BtnConfirm_Click;
            pnlRight.Controls.Add(btnConfirm);

            btnCancel = CreateStyledButton("HỦY THAO TÁC", 20, 435, Color.Silver, Color.Black);
            btnCancel.Click += (s, e) => { pnlRight.Enabled = false; txtSearch.Clear(); };
            pnlRight.Controls.Add(btnCancel);
        }

        #region XỬ LÝ DỮ LIỆU
        private void LoadDataToRAM()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(strCon))
                {
                    SqlDataAdapter da = new SqlDataAdapter("SP_NHANSUDIEUCHUYEN", conn);
                    dtNhanSu.Clear();
                    da.Fill(dtNhanSu);

                    // --- LOGIC XỬ LÝ LOAINV ---
                    foreach (DataRow row in dtNhanSu.Rows)
                    {
                        // Vì SQL đã lọc nên ở đây chắc chắn là nhân viên
                        if (row["LOAINV"].ToString() == "NV")
                            row["LOAINV"] = "Nhân viên";
                    }

                    dgvNhanSu.DataSource = dtNhanSu;
                    // --- ẨN CÁC CỘT KHÔNG CẦN THIẾT ---
                    if (dgvNhanSu.Columns["MaCNHienTai"] != null) dgvNhanSu.Columns["MaCNHienTai"].Visible = false;
                    if (dgvNhanSu.Columns["CALAMVIEC"] != null) dgvNhanSu.Columns["CALAMVIEC"].Visible = false; // Bỏ cột Ca làm việc


                    pnlLeft.SendToBack();
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message); }
        }

        private void LoadDSChiNhanh()
        {
            using (SqlConnection conn = new SqlConnection(strCon))
            {
                SqlDataAdapter da = new SqlDataAdapter(
                    @"SELECT 
                MACN, 
                TENCN + N' - ' + DIACHI AS TenHienThi
              FROM CHI_NHANH", conn);

                DataTable dt = new DataTable();
                da.Fill(dt);

                cboChiNhanhMoi.DataSource = dt;
                cboChiNhanhMoi.DisplayMember = "TenHienThi";
                cboChiNhanhMoi.ValueMember = "MACN";
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            string search = txtSearch.Text.Trim().Replace("'", "''");
            dtNhanSu.DefaultView.RowFilter = $"HOTEN LIKE '%{search}%' OR SDT LIKE '%{search}%'";
        }

        private void DgvNhanSu_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                pnlRight.Enabled = true;
                var row = dgvNhanSu.Rows[e.RowIndex];
                lblTargetName.Text = "Nhân sự: " + row.Cells["HOTEN"].Value.ToString();
                lblTargetCurrentCN.Text = "Tại: " + row.Cells["ChiNhanhHienTai"].Value.ToString();
            }
        }

        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            if (dgvNhanSu.CurrentRow == null) return;

            int maNS = Convert.ToInt32(dgvNhanSu.CurrentRow.Cells["MANS"].Value);
            string maCNMoi = cboChiNhanhMoi.SelectedValue.ToString();
            string maCNCu = dgvNhanSu.CurrentRow.Cells["MaCNHienTai"].Value.ToString();

            if (maCNMoi == maCNCu)
            {
                MessageBox.Show("Chi nhánh mới trùng với chi nhánh hiện tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (dtpNgayKT.Value.Date <= DateTime.Today)
            {
                MessageBox.Show("Nhân viên phải làm việc hết ngày hôm nay. Vui lòng chọn ngày kết thúc từ ngày mai trở đi!",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // 2. Chặn ngày bắt đầu mới phải từ ngày mai trở đi
            if (dtpNgayBD.Value.Date <= DateTime.Today)
            {
                MessageBox.Show("Ngày bắt đầu tại chi nhánh mới sớm nhất phải là ngày mai!",
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // 3. Kiểm tra Ngày bắt đầu mới phải >= Ngày kết thúc cũ
            if (dtpNgayBD.Value.Date < dtpNgayKT.Value.Date)
            {
                MessageBox.Show("\"Ngày bắt đầu mới không được trước ngày kết thúc cũ!",
                    "Lỗi nghiệp vụ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(strCon))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SP_THUCHIENDIEUCHUYEN", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MaNS", maNS);
                    cmd.Parameters.AddWithValue("@MaCN_Moi", maCNMoi);
                    cmd.Parameters.AddWithValue("@NgayBD_Moi", dtpNgayBD.Value.Date);
                    cmd.Parameters.AddWithValue("@NgayKT_Cu", dtpNgayKT.Value.Date);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Điều chuyển thành công nhân sự " + dgvNhanSu.CurrentRow.Cells["HOTEN"].Value.ToString(), "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadDataToRAM();        // reload data
                    pnlRight.Enabled = false;

                    // ÉP VẼ LẠI UI (GIỮ AUTOSCROLL)
                    ForceRedraw(pnlLeft);
                    ForceRedraw(pnlRight);
                    ForceRedraw(this);
                }
            }
            catch (SqlException ex)
            {
                // Kiểm tra nếu lỗi bắt nguồn từ Trigger của mình
                if (ex.Message.Contains("[TRIGGER ERROR]"))
                {
                    // Hiển thị thông báo rõ ràng là từ Trigger
                    MessageBox.Show("PHÁT HIỆN VI PHẠM RÀNG BUỘC (TRIGGER):\n\n" + ex.Message.Replace("[TRIGGER ERROR]:", "").Trim(),
                                    "Lỗi từ Database Trigger",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Stop);
                }
                else
                {
                    // Các lỗi khác từ Store hoặc SQL (Ví dụ lỗi 50003, 50004...)
                    MessageBox.Show("LỖI NGHIỆP VỤ:\n" + ex.Message,
                                    "Thông báo lỗi",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi không xác định: " + ex.Message);
            }

        }
        #endregion

        #region ĐỒ HỌA & STYLE
        private Panel CreateRoundedPanel(int x, int y, int w, int h, Color bgColor)
        {
            Panel p = new Panel() { Location = new Point(x, y), Size = new Size(w, h), BackColor = bgColor };

            // Gán Region ngay lập tức để chặn góc vuông che icon
            GraphicsPath path = GetRoundedPath(new Rectangle(0, 0, w, h), 25);
            p.Region = new Region(path);

            p.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath pth = GetRoundedPath(new Rectangle(0, 0, p.Width - 1, p.Height - 1), 25))
                {
                    using (Pen pen = new Pen(colorSoft, 2)) e.Graphics.DrawPath(pen, pth);
                }
            };
            return p;
        }

        private Button CreateStyledButton(string text, int x, int y, Color backColor, Color foreColor)
        {
            Button btn = new Button()
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(360, 45),
                BackColor = backColor,
                ForeColor = foreColor,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Paint += (s, e) => {
                btn.Region = new Region(GetRoundedPath(new Rectangle(0, 0, btn.Width, btn.Height), 20));
            };
            return btn;
        }

        private void AddLabelAndControl(Panel p, string labelText, Control ctrl, int y)
        {
            Label lbl = new Label() { Text = labelText, Location = new Point(20, y), AutoSize = true, ForeColor = Color.FromArgb(64, 64, 64) };
            ctrl.Location = new Point(20, y + 22);
            ctrl.Width = 360;
            p.Controls.Add(lbl);
            p.Controls.Add(ctrl);
        }

        private void StyleGrid(DataGridView d)
        {
            d.ScrollBars = ScrollBars.Both; // Hiện cả ngang và dọc
            d.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells; // Tự dãn theo nội dung chữ
            d.AllowUserToResizeColumns = false; // <<< KHÔNG cho người dùng kéo cột
            d.AllowUserToResizeRows = false;    // KHÔNG cho kéo dòng


            d.EnableHeadersVisualStyles = false;
            d.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            d.ColumnHeadersDefaultCellStyle.BackColor = colorMain;
            d.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            d.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            d.ColumnHeadersHeight = 40;
            d.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 245, 245);
            d.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            d.DefaultCellStyle.SelectionBackColor = colorSoft;
            d.DefaultCellStyle.SelectionForeColor = Color.Black;
        }
        private void ForceRedraw(Control ctl)
        {
            ctl.SuspendLayout();
            ctl.ResumeLayout(true);
            ctl.Invalidate(true);
            ctl.Update();
        }
        private void pnlLeft_Resize(object sender, EventArgs e)
        {
            Panel p = sender as Panel;
            if (p == null) return;

            using (GraphicsPath path = new GraphicsPath())
            {
                int radius = 25;
                path.AddArc(0, 0, radius, radius, 180, 90);
                path.AddArc(p.Width - radius, 0, radius, radius, 270, 90);
                path.AddArc(p.Width - radius, p.Height - radius, radius, radius, 0, 90);
                path.AddArc(0, p.Height - radius, radius, radius, 90, 90);
                path.CloseAllFigures();
                p.Region = new Region(path);
            }
        }

        private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }
        #endregion
    }
}
