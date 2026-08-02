using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PETCAREX
{
    public partial class FormCheckin : Form
    {
        private Panel pnlHeader;
        private TextBox txtSDT;
        private Button btnSearch;
        private Label lblKH;
        private DataGridView dgvLich;
        private Button btnConfirm;

        // Biến lưu trữ Mã khách hàng hiện tại sau khi tìm kiếm thành công
        private int currentMaKH = -1;
   
        private string strConn = @"Data Source=.;Initial Catalog=QLTC;Integrated Security=True";
        private Color pinkPastel = Color.FromArgb(255, 235, 235);
        private Color pinkDeep = Color.FromArgb(230, 150, 150);

        public FormCheckin()
        {
            InitMyUI();
        }

        private void InitMyUI()
        {
            this.Text = "Check-in Dịch Vụ";
            this.Size = new Size(1000, 650);
            this.BackColor = Color.White;
            // --- CẤU HÌNH THANH TRƯỢT CHO FORM ---
            this.AutoScroll = true;
            this.AutoScrollMinSize = new Size(950, 600); // Hiện thanh trượt nếu Form bị thu quá nhỏ


            pnlHeader = new Panel { Dock = DockStyle.Top, Height = 120, BackColor = pinkPastel };
            Label title = new Label { Text = "TIẾP NHẬN THÚ CƯNG", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = pinkDeep, Location = new Point(20, 15), AutoSize = true };
            txtSDT = new TextBox { Location = new Point(25, 75), Width = 220, Font = new Font("Segoe UI", 12) };

            btnSearch = new Button { Text = "TÌM KHÁCH", Location = new Point(255, 73), Size = new Size(100, 32), BackColor = pinkDeep, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Click += (s, e) => TimKiemBangSDT();

            lblKH = new Label { Text = "Vui lòng nhập số điện thoại để tìm mã khách hàng...", Location = new Point(380, 78), Font = new Font("Segoe UI", 11, FontStyle.Italic), ForeColor = Color.DimGray, AutoSize = true };
            pnlHeader.Controls.AddRange(new Control[] { title, txtSDT, btnSearch, lblKH });

            // 2. Bảng dịch vụ (Đặt trong một Panel để quản lý cuộn tốt hơn nếu cần)
            dgvLich = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                EnableHeadersVisualStyles = false,

                // CẤU HÌNH THANH TRƯỢT CHO BẢNG
                ScrollBars = ScrollBars.Both, // Hiện cả dọc và ngang cho bảng nếu cần
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells // Để các cột không bị nén quá nhỏ
            };
            dgvLich.ColumnHeadersDefaultCellStyle.BackColor = pinkDeep;
            dgvLich.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            btnConfirm = new Button { Text = "XÁC NHẬN CHECK-IN (DÙNG MÃ KH)", Dock = DockStyle.Bottom, Height = 55, BackColor = pinkDeep, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold) };
            btnConfirm.FlatAppearance.BorderSize = 0;
            btnConfirm.Click += (s, e) => ThucHienCheckin();

            this.Controls.Add(dgvLich);
            this.Controls.Add(pnlHeader);
            this.Controls.Add(btnConfirm);
        }

        private void TimKiemBangSDT()
        {
            string sdt = txtSDT.Text.Trim();
            if (!Regex.IsMatch(sdt, @"^0\d{9}$")) { MessageBox.Show("SĐT không hợp lệ!"); return; }

            try
            {
                using (SqlConnection conn = new SqlConnection(strConn))
                {
                    conn.Open();
                    // Bước 1: Lấy thông tin khách hàng bằng SDT (Lấy được MAKH)
                    SqlCommand cmdKH = new SqlCommand("SP_LAYKHACHHANG", conn);
                    cmdKH.CommandType = CommandType.StoredProcedure;
                    cmdKH.Parameters.AddWithValue("@SDT", sdt);
                    SqlDataAdapter da = new SqlDataAdapter(cmdKH);
                    DataTable dtKH = new DataTable();
                    da.Fill(dtKH);

                    if (dtKH.Rows.Count > 0)
                    {
                        currentMaKH = Convert.ToInt32(dtKH.Rows[0]["MAKH"]);
                        lblKH.Text = $"🌸 KH: {dtKH.Rows[0]["HOTEN"]} (Mã: {currentMaKH})";
                       
                        LoadLichBangMaKH(currentMaKH, conn);
                    }
                    else
                    {
                        currentMaKH = -1;
                        MessageBox.Show("Khách hàng không tồn tại!");
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void LoadLichBangMaKH(int maKH, SqlConnection conn)
        {
            SqlCommand cmd = new SqlCommand("SP_GET_LICH_HEN_CHECKIN", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@MAKH", maKH);
            cmd.Parameters.AddWithValue("@MANV_CHECKIN", UserSession.MaUser);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgvLich.DataSource = dt;

            if (dgvLich.Columns["colCheck"] == null)
            {
                dgvLich.Columns.Insert(0, new DataGridViewCheckBoxColumn { Name = "colCheck", HeaderText = "Chọn", Width = 50 });
            }
        }
        private void ResetForm()
        {
            txtSDT.Clear();
            lblKH.Text = "Vui lòng nhập số điện thoại để tìm mã khách hàng...";
            lblKH.ForeColor = Color.DimGray;
            dgvLich.DataSource = null; // Xóa dữ liệu trên bảng
            currentMaKH = -1;
            txtSDT.Focus(); // Đưa con trỏ chuột về ô nhập liệu
        }
        private void ThucHienCheckin()
        {
            if (currentMaKH == -1) { MessageBox.Show("Vui lòng tìm khách hàng!"); return; }

            DateTime thoiGianCheckinChung = DateTime.Now;

            // Chặn nếu quá 30 phút như bạn yêu cầu
            if (thoiGianCheckinChung.Minute > 60)
            {
                MessageBox.Show($"Hiện tại là {thoiGianCheckinChung:HH:mm}. Đã hết thời gian check-in!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int success = 0;
            using (SqlConnection conn = new SqlConnection(strConn))
            {
                conn.Open();
                foreach (DataGridViewRow row in dgvLich.Rows)
                {
                    if (row.Cells["colCheck"].Value != null && Convert.ToBoolean(row.Cells["colCheck"].Value))
                    {
                        try
                        {
                            SqlCommand cmd = new SqlCommand("SP_UPDATE_CHECKIN", conn);
                            cmd.CommandType = CommandType.StoredProcedure;

                            // Các tham số chung
                            cmd.Parameters.AddWithValue("@LOAI", row.Cells["LOAI"].Value);
                            cmd.Parameters.AddWithValue("@MATC", row.Cells["MATC"].Value);
                            cmd.Parameters.AddWithValue("@MADV", row.Cells["MADV"].Value);
                            cmd.Parameters.AddWithValue("@STT", row.Cells["STT"].Value);
                            cmd.Parameters.AddWithValue("@THOIGIAN", thoiGianCheckinChung);

                            // XỬ LÝ MATP: Nếu là TP thì lấy giá trị, nếu là KB thì truyền DBNull
                            if (row.Cells["LOAI"].Value.ToString() == "TP")
                            {
                                cmd.Parameters.AddWithValue("@MATP", row.Cells["MATP"].Value);
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@MATP", DBNull.Value);
                            }

                            cmd.ExecuteNonQuery();
                            success++;
                        }
                        catch (SqlException ex)
                        {
                            MessageBox.Show($"Lỗi dịch vụ {row.Cells["TENDV"].Value}: {ex.Message}");
                        }
                    }
                }
            }

            if (success > 0)
            {
                MessageBox.Show($"Thành công {success} dịch vụ vào lúc {thoiGianCheckinChung:HH:mm:ss}");
                ResetForm();
            }
        }

    }

}