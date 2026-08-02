using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PETCAREX
{
    public partial class FormGioHang : Form
    {
        Panel panelHeader, panelFooter;
        DataGridView dgv;
        Label lblTitle, lblTongTien;
        Button btnXoa, btnDong;
        Button btnThanhToan;
        
        public FormGioHang()
        {
            InitUI();
            LoadData();
        }

        void InitUI()
        {
            // ================= FORM =================
            this.Text = "Giỏ hàng";
            this.Size = new Size(780, 480);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(255, 240, 240);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // ================= HEADER =================
            panelHeader = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(255, 210, 210)
            };
            this.Controls.Add(panelHeader);

            lblTitle = new Label()
            {
                Text = "🧺 GIỎ HÀNG",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(210, 50, 50),
                AutoSize = true,
                Location = new Point(20, 18)
            };
            panelHeader.Controls.Add(lblTitle);

            // ================= TABLE =================
            dgv = new DataGridView()
            {
                Location = new Point(20, 90),
                Size = new Size(740, 260),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None
            };

            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(255, 220, 220);
            dgv.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 10, FontStyle.Bold);

            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);

            // ===== COLUMNS (ĐÚNG THỨ TỰ) =====
            dgv.Columns.Add("TEN", "Tên sản phẩm");
            dgv.Columns.Add("CN", "Chi nhánh");
            dgv.Columns.Add("GIA", "Giá");
            dgv.Columns.Add("SL", "Số lượng");
            dgv.Columns.Add("TT", "Thành tiền");

            this.Controls.Add(dgv);

            // ================= FOOTER =================
            panelFooter = new Panel()
            {
                Location = new Point(20, 355),
                Size = new Size(740, 70),
                BackColor = Color.White
            };
            panelFooter.Paint += DrawBorder;
            this.Controls.Add(panelFooter);

            lblTongTien = new Label()
            {
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(180, 0, 0),
                Location = new Point(20, 22),
                AutoSize = true
            };
            panelFooter.Controls.Add(lblTongTien);

            btnXoa = new Button()
            {
                Text = "❌ Xóa",
                Size = new Size(120, 36),
                Location = new Point(470, 18),
                BackColor = Color.FromArgb(220, 220, 220),
                FlatStyle = FlatStyle.Flat
            };
            btnXoa.FlatAppearance.BorderSize = 0;
            btnXoa.Click += XoaSanPham;
            panelFooter.Controls.Add(btnXoa);

            btnDong = new Button()
            {
                Text = "Đóng",
                Size = new Size(120, 36),
                Location = new Point(600, 18),
                BackColor = Color.FromArgb(220, 70, 70),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnDong.FlatAppearance.BorderSize = 0;
            btnDong.Click += (s, e) => this.Close();
            panelFooter.Controls.Add(btnDong);

            btnThanhToan = new Button()
            {
                Text = "💳 Thanh toán",
                Size = new Size(120, 36),
                Location = new Point(340, 18),
                BackColor = Color.FromArgb(70, 160, 70),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnThanhToan.FlatAppearance.BorderSize = 0;
            btnThanhToan.Click += ThanhToan;
            panelFooter.Controls.Add(btnThanhToan);

        }

        // ================= LOAD DATA =================
        void LoadData()
        {
            dgv.Rows.Clear();
            decimal tong = 0;

            foreach (var item in GioHangData.Items)
            {
                decimal thanhTien = item.Gia * item.SoLuong;
                tong += thanhTien;

                dgv.Rows.Add(
                    item.TenSP,
                    item.TenCN,
                    item.Gia.ToString("N0"),
                    item.SoLuong,
                    thanhTien.ToString("N0")
                );
            }

            lblTongTien.Text = "Tổng tiền: " + tong.ToString("N0") + " đ";
        }
        void ThanhToan(object sender, EventArgs e)
        {
            if (GioHangData.Items.Count == 0)
            {
                MessageBox.Show("Giỏ hàng đang trống");
                return;
            }

            if (UserSession.MaUser <= 0)
            {
                MessageBox.Show("Vui lòng đăng nhập trước khi thanh toán");
                return;
            }

            int maKH = UserSession.MaUser;
            string hinhThucTT = "QR"; // hoặc CHUYENKHOAN

            using (SqlConnection conn =
                new SqlConnection("Data Source=.;Initial Catalog=QLTC;Integrated Security=True"))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    foreach (var item in GioHangData.Items)
                    {
                        SqlCommand cmd = new SqlCommand(
                            "SP_MUA_SAN_PHAM", conn, tran);

                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@MAKH", maKH);
                        cmd.Parameters.AddWithValue("@MACN", item.MaCN);
                        cmd.Parameters.AddWithValue("@MASP", item.MaSP);
                        cmd.Parameters.AddWithValue("@SOLUONG", item.SoLuong);
                        cmd.Parameters.AddWithValue("@HTTHANHTOAN", hinhThucTT);

                        cmd.ExecuteNonQuery();
                    }

                    tran.Commit();

                    GioHangData.Items.Clear();
                    LoadData();

                    MessageBox.Show("Thanh toán thành công ✔️");
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    MessageBox.Show(
                        "Thanh toán thất bại:\n" + ex.Message,
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }


        // ================= XÓA =================
        void XoaSanPham(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) return;

            int index = dgv.SelectedRows[0].Index;
            GioHangData.Items.RemoveAt(index);
            LoadData();
        }

        // ================= BORDER =================
        void DrawBorder(object sender, PaintEventArgs e)
        {
            using (Pen pen = new Pen(Color.FromArgb(220, 70, 70), 2))
            {
                e.Graphics.DrawRectangle(
                    pen,
                    0,
                    0,
                    ((Control)sender).Width - 1,
                    ((Control)sender).Height - 1);
            }
        }
    }
}
