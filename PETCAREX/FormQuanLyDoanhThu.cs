using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace PETCAREX
{
    public partial class FormQuanLyDoanhThu : Form
    {
        // ================= DB =================
        string connStr = "Data Source=.;Initial Catalog=QLTC;Integrated Security=True";
        string _maCN = UserSession.MaCN;

        // ===== COLORS =====
        Color COLOR_BG = Color.FromArgb(255, 235, 235);
        Color COLOR_PRIMARY = Color.FromArgb(220, 70, 70);
        Color COLOR_BTN = Color.FromArgb(255, 160, 160);
        Color COLOR_CARD = Color.FromArgb(255, 250, 245);
        Color COLOR_TEXT = Color.FromArgb(120, 60, 60);

        // ===== INFO =====
        Label lblTitle, lblBranch, lblAddress, lblTime;

        // ===== FILTER BUTTONS =====
        Button btnNgay, btnThang, btnQuy, btnNam, btnTimKiem;

        // ===== FILTER INPUT =====
        DateTimePicker dtNgay;
        ComboBox cboThang, cboQuy;
        NumericUpDown numNam, numNam1;

        // ===== CARDS =====
        Label lblSP, lblDV, lblLuot, lblTong;

        public FormQuanLyDoanhThu()
        {
            InitializeComponent();
            InitForm();
            InitUI();
            LoadThongTinChiNhanh();
            ResetThongKe();
        }

        // ================= FORM =================
        void InitForm()
        {
            this.TopLevel = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.Fill;
            this.BackColor = COLOR_BG;
        }

        void ResetThongKe()
        {
            lblSP.Text = "--- VND";
            lblDV.Text = "--- VND";
            lblTong.Text = "--- VND";
            lblLuot.Text = "--- lượt";
        }

        // ================= LOAD CHI NHÁNH =================
        void LoadThongTinChiNhanh()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(
                "SELECT TENCN, DIACHI, TGMO, TGDONG FROM CHI_NHANH WHERE MACN=@MACN", conn))
            {
                cmd.Parameters.AddWithValue("@MACN", _maCN);
                conn.Open();

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        lblBranch.Text = "Chi nhánh: " + rd["TENCN"];
                        lblAddress.Text = "Địa chỉ: " + rd["DIACHI"];
                        lblTime.Text =
                            $"Giờ làm việc: {((TimeSpan)rd["TGMO"]):hh\\:mm} – {((TimeSpan)rd["TGDONG"]):hh\\:mm}";
                    }
                }
            }
        }

        // ================= UI =================
        void InitUI()
        {
            lblTitle = new Label()
            {
                Text = "QUẢN LÝ DOANH THU",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = COLOR_PRIMARY,
                AutoSize = true,
                Location = new Point(340, 30)
            };
            Controls.Add(lblTitle);

            lblBranch = CreateInfoLabel("Chi nhánh:", 50, 90, true);
            lblAddress = CreateInfoLabel("Địa chỉ:", 50, 120, false);
            lblTime = CreateInfoLabel("Giờ làm việc:", 50, 145, false);

            dtNgay = new DateTimePicker()
            {
                Location = new Point(50, 180),
                Width = 180,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                Visible = false
            };

            cboThang = new ComboBox()
            {
                Location = new Point(50, 180),
                Width = 80,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Visible = false
            };
            for (int i = 1; i <= 12; i++) cboThang.Items.Add(i);
            cboThang.SelectedIndex = 0;

            cboQuy = new ComboBox()
            {
                Location = new Point(50, 180),
                Width = 80,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Visible = false
            };
            cboQuy.Items.AddRange(new object[] { "1", "2", "3", "4" });
            cboQuy.SelectedIndex = 0;

            numNam = new NumericUpDown()
            {
                Location = new Point(140, 180),
                Width = 90,
                Minimum = 2023,
                Maximum = DateTime.Now.Year,
                Value = DateTime.Now.Year,
                Visible = false
            };

            numNam1 = new NumericUpDown()
            {
                Location = new Point(50, 180),
                Width = 90,
                Minimum = 2023,
                Maximum = DateTime.Now.Year,
                Value = DateTime.Now.Year,
                Visible = false
            };

            btnTimKiem = new Button()
            {
                Text = "Tìm kiếm",
                Location = new Point(250, 174),
                Size = new Size(120, 36),
                BackColor = COLOR_PRIMARY,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Visible = false
            };
            btnTimKiem.FlatAppearance.BorderSize = 0;
            btnTimKiem.Paint += (s, e) => ApplyRounded(btnTimKiem, 14);
            btnTimKiem.Click += BtnTimKiem_Click;

            Controls.AddRange(new Control[]
            {
                dtNgay, cboThang, cboQuy, numNam, numNam1, btnTimKiem
            });

            btnNgay = CreateFilterButton("Doanh thu theo ngày", 500, 95);
            btnThang = CreateFilterButton("Doanh thu theo tháng", 750, 95);
            btnQuy = CreateFilterButton("Doanh thu theo quý", 500, 155);
            btnNam = CreateFilterButton("Doanh thu theo năm", 750, 155);

            Controls.AddRange(new Control[] { btnNgay, btnThang, btnQuy, btnNam });

            // ===== CARD SP (CLICK ĐƯỢC) =====
            Panel cardSP, cardLuot;

            CreateCard("DOANH THU SẢN PHẨM", 40, 260, out lblSP, out cardSP);
            cardSP.Click += CardSP_Click;

            CreateCard("DOANH THU DỊCH VỤ", 550, 260, out lblDV);

            CreateCard("SỐ LƯỢT KHÁM / TIÊM", 40, 420, out lblLuot, out cardLuot);
            cardLuot.Click += CardLuot_Click;

            CreateCard("TỔNG DOANH THU", 550, 420, out lblTong);

        }

        // ================= CLICK CARD SP =================
        void CardSP_Click(object sender, EventArgs e)
        {
            if (!btnTimKiem.Visible)
            {
                MessageBox.Show("Vui lòng tra cứu doanh thu trước");
                return;
            }

            DateTime from, to;

            if (dtNgay.Visible)
            {
                from = dtNgay.Value.Date;
                to = from.AddDays(1).AddSeconds(-1);
            }
            else if (cboThang.Visible)
            {
                from = new DateTime((int)numNam.Value, (int)cboThang.SelectedItem, 1);
                to = from.AddMonths(1).AddSeconds(-1);
            }
            else if (cboQuy.Visible)
            {
                int startMonth = (int.Parse(cboQuy.Text) - 1) * 3 + 1;
                from = new DateTime((int)numNam.Value, startMonth, 1);
                to = from.AddMonths(3).AddSeconds(-1);
            }
            else
            {
                from = new DateTime((int)numNam1.Value, 1, 1);
                to = new DateTime((int)numNam1.Value, 12, 31, 23, 59, 59);
            }

            using (var f = new FormTopSanPham(connStr, _maCN, from, to))
            {
                f.ShowDialog();
            }
            return;

        }

        // ================= SEARCH =================
        void BtnTimKiem_Click(object sender, EventArgs e)
        {
            DateTime from, to;

            if (dtNgay.Visible)
            {
                from = dtNgay.Value.Date;
                to = from.AddDays(1).AddSeconds(-1);
            }
            else if (cboThang.Visible)
            {
                from = new DateTime((int)numNam.Value, (int)cboThang.SelectedItem, 1);
                to = from.AddMonths(1).AddSeconds(-1);
            }
            else if (cboQuy.Visible)
            {
                int startMonth = (int.Parse(cboQuy.Text) - 1) * 3 + 1;
                from = new DateTime((int)numNam.Value, startMonth, 1);
                to = from.AddMonths(3).AddSeconds(-1);
            }
            else
            {
                from = new DateTime((int)numNam1.Value, 1, 1);
                to = new DateTime((int)numNam1.Value, 12, 31, 23, 59, 59);
            }

            LoadDoanhThu(from, to);
        }

        // ================= DB =================
        void LoadDoanhThu(DateTime from, DateTime to)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("SP_Admin_ThongKeDoanhThu", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Phạm vi: chi nhánh đang đăng nhập
                cmd.Parameters.AddWithValue("@PhamVi", "Chi nhánh");
                cmd.Parameters.AddWithValue("@MaCN_CuThe", _maCN);

                // Xác định loại thống kê theo UI đang bật
                if (dtNgay.Visible)
                {
                    cmd.Parameters.AddWithValue("@LoaiThongKe", "Ngay");
                    cmd.Parameters.AddWithValue("@Ngay", from.Day);
                    cmd.Parameters.AddWithValue("@Thang", from.Month);
                    cmd.Parameters.AddWithValue("@Quy", DBNull.Value);
                }
                else if (cboThang.Visible)
                {
                    cmd.Parameters.AddWithValue("@LoaiThongKe", "Thang");
                    cmd.Parameters.AddWithValue("@Ngay", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Thang", from.Month);
                    cmd.Parameters.AddWithValue("@Quy", DBNull.Value);
                }
                else if (cboQuy.Visible)
                {
                    cmd.Parameters.AddWithValue("@LoaiThongKe", "Quy");
                    cmd.Parameters.AddWithValue("@Ngay", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Thang", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Quy", (int.Parse(cboQuy.Text)));
                }
                else
                {
                    cmd.Parameters.AddWithValue("@LoaiThongKe", "Nam");
                    cmd.Parameters.AddWithValue("@Ngay", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Thang", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Quy", DBNull.Value);
                }

                cmd.Parameters.AddWithValue("@Nam", from.Year);

                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        lblSP.Text = $"{Convert.ToDecimal(rd["DoanhThuSanPham"]):N0} VND";
                        lblDV.Text = $"{Convert.ToDecimal(rd["DoanhThuDichVu"]):N0} VND";
                        lblTong.Text = $"{Convert.ToDecimal(rd["TongDoanhThu"]):N0} VND";
                        lblLuot.Text = $"{Convert.ToInt32(rd["TongSoHoaDon"])} lượt";
                    }
                }
            }
        }


        // ================= FILTER BUTTON =================
        Button CreateFilterButton(string text, int x, int y)
        {
            Button btn = new Button()
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(230, 46),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = COLOR_BTN,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Paint += (s, e) => ApplyRounded(btn, 18);

            btn.Click += (s, e) =>
            {
                dtNgay.Visible = cboThang.Visible = cboQuy.Visible =
                numNam.Visible = numNam1.Visible = false;

                btnTimKiem.Visible = true;
                ResetThongKe();

                if (text.Contains("ngày")) dtNgay.Visible = true;
                else if (text.Contains("tháng")) { cboThang.Visible = true; numNam.Visible = true; }
                else if (text.Contains("quý")) { cboQuy.Visible = true; numNam.Visible = true; }
                else numNam1.Visible = true;
            };

            return btn;
        }

        // ================= CARD =================
        void CreateCard(string title, int x, int y, out Label valueLabel, out Panel panel)
        {
            panel = new Panel()
            {
                Location = new Point(x, y),
                Size = new Size(420, 130),
                BackColor = COLOR_CARD,
                Cursor = Cursors.Hand
            };

            // ✅ SET REGION KHI RESIZE (AN TOÀN 100%)
            panel.Resize += (s, e) =>
            {
                Panel p = (Panel)s;
                if (p.Width > 0 && p.Height > 0)
                    p.Region = new Region(RoundedPath(p.ClientRectangle, 18));
            };

            // ✅ PAINT CHỈ ĐỂ VẼ VIỀN
            panel.Paint += (s, e) =>
            {
                Panel p = (Panel)s;
                using (Pen pen = new Pen(COLOR_PRIMARY, 2))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.DrawPath(pen, RoundedPath(p.ClientRectangle, 18));
                }
            };

            Label lbl = new Label()
            {
                Text = title,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = COLOR_PRIMARY,
                Location = new Point(20, 15),
                AutoSize = true,
                Cursor = Cursors.Hand
            };

            valueLabel = new Label()
            {
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(150, 50, 50),
                Location = new Point(20, 60),
                AutoSize = true,
                Cursor = Cursors.Hand
            };

            panel.Controls.Add(lbl);
            panel.Controls.Add(valueLabel);
            Controls.Add(panel);
        }

           
        void CardLuot_Click(object sender, EventArgs e)
        {
            if (!btnTimKiem.Visible)
            {
                MessageBox.Show("Vui lòng tra cứu doanh thu trước");
                return;
            }

            DateTime from, to;

            if (dtNgay.Visible)
            {
                from = dtNgay.Value.Date;
                to = from.AddDays(1).AddSeconds(-1);
            }
            else if (cboThang.Visible)
            {
                from = new DateTime((int)numNam.Value, (int)cboThang.SelectedItem, 1);
                to = from.AddMonths(1).AddSeconds(-1);
            }
            else if (cboQuy.Visible)
            {
                int startMonth = (int.Parse(cboQuy.Text) - 1) * 3 + 1;
                from = new DateTime((int)numNam.Value, startMonth, 1);
                to = from.AddMonths(3).AddSeconds(-1);
            }
            else
            {
                from = new DateTime((int)numNam1.Value, 1, 1);
                to = new DateTime((int)numNam1.Value, 12, 31, 23, 59, 59);
            }

            using (var f = new FormTopBacSi(connStr, _maCN, from, to))
            {
                f.ShowDialog();
            }
        }

        void CreateCard(string title, int x, int y, out Label valueLabel)
        {
            Panel _;
            CreateCard(title, x, y, out valueLabel, out _);
        }

        Label CreateInfoLabel(string text, int x, int y, bool bold)
        {
            Label lbl = new Label()
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Segoe UI", bold ? 12 : 10, bold ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = bold ? COLOR_PRIMARY : COLOR_TEXT
            };
            Controls.Add(lbl);
            return lbl;
        }

        // ================= ROUNDED =================
        void ApplyRounded(Control c, int r)
        {
            c.Region = new Region(RoundedPath(c.ClientRectangle, r));
        }

        GraphicsPath RoundedPath(Rectangle r, int radius)
        {
            int d = radius * 2;
            GraphicsPath path = new GraphicsPath();
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
