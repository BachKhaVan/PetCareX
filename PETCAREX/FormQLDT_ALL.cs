using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace PETCAREX
{
    public partial class FormQLDT_ALL : Form
    {
        // ================= DB =================
        string connStr = "Data Source=.;Initial Catalog=QLTC;Integrated Security=True";

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

        public FormQLDT_ALL()
        {
            InitializeComponent();
            InitForm();
            InitUI();
            ResetThongKe();

            lblBranch.Text = "Phạm vi: TOÀN BỘ CHI NHÁNH";
            lblAddress.Text = "";
            lblTime.Text = "";
        }

        // ================= FORM =================
        void InitForm()
        {
            // ⚠️ BẮT BUỘC KHI LÀ CHILD FORM
            TopLevel = false;
            FormBorderStyle = FormBorderStyle.None;
            Dock = DockStyle.Fill;

            // ❌ KHÔNG DÙNG
            // StartPosition
            // WindowState

            BackColor = COLOR_BG;
            this.AutoScroll = true;
        }


        void ResetThongKe()
        {
            lblSP.Text = "--- VND";
            lblDV.Text = "--- VND";
            lblTong.Text = "--- VND";
            lblLuot.Text = "--- lượt";
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

            lblBranch = CreateInfoLabel("", 50, 90, true);
            lblAddress = CreateInfoLabel("", 50, 120, false);
            lblTime = CreateInfoLabel("", 50, 145, false);

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

            Panel cardSP, cardDV, cardLuot;

            CreateCard("DOANH THU SẢN PHẨM", 40, 260, out lblSP, out cardSP);
            cardSP.Click += CardSP_Click;

            CreateCard("DOANH THU DỊCH VỤ", 550, 260, out lblDV, out cardDV);
            cardDV.Click += CardDV_Click;

            CreateCard("SỐ LƯỢT KHÁM / TIÊM", 40, 420, out lblLuot, out cardLuot);
            cardLuot.Click += CardLuot_Click;

            CreateCard("TỔNG DOANH THU", 550, 420, out lblTong);
        }

        // ================= SEARCH =================
        void BtnTimKiem_Click(object sender, EventArgs e)
        {
            GetFromTo(out DateTime from, out DateTime to);
            LoadDoanhThu(from, to);
        }

        // ================= LOAD ALL =================
        void LoadDoanhThu(DateTime from, DateTime to)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("SP_Admin_ThongKeDoanhThu", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@PhamVi", "Toàn hệ thống");
                cmd.Parameters.AddWithValue("@MaCN_CuThe", DBNull.Value);

                string loai;
                object ngay = DBNull.Value, thang = DBNull.Value, quy = DBNull.Value;
                int nam;

                if (dtNgay.Visible)
                {
                    loai = "Ngay";
                    ngay = from.Day;
                    thang = from.Month;
                    nam = from.Year;
                }
                else if (cboThang.Visible)
                {
                    loai = "Thang";
                    thang = from.Month;
                    nam = from.Year;
                }
                else if (cboQuy.Visible)
                {
                    loai = "Quy";
                    quy = int.Parse(cboQuy.Text);
                    nam = from.Year;
                }
                else
                {
                    loai = "Nam";
                    nam = from.Year;
                }

                cmd.Parameters.AddWithValue("@LoaiThongKe", loai);
                cmd.Parameters.AddWithValue("@Ngay", ngay);
                cmd.Parameters.AddWithValue("@Thang", thang);
                cmd.Parameters.AddWithValue("@Quy", quy);
                cmd.Parameters.AddWithValue("@Nam", nam);

                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        decimal dtSP = Convert.ToDecimal(rd["DoanhThuSanPham"]);
                        decimal dtDV = Convert.ToDecimal(rd["DoanhThuDichVu"]);
                        decimal dtTong = Convert.ToDecimal(rd["TongDoanhThu"]);
                        int soLuot = Convert.ToInt32(rd["TongSoHoaDon"]);

                        lblSP.Text = $"{dtSP:N0} VND";
                        lblDV.Text = $"{dtDV:N0} VND";
                        lblTong.Text = $"{dtTong:N0} VND";
                        lblLuot.Text = $"{soLuot} lượt";
                    }
                }
            }
        }




        // ================= CARD CLICK =================
        void CardSP_Click(object sender, EventArgs e)
        {
            if (!btnTimKiem.Visible)
            {
                MessageBox.Show("Vui lòng tra cứu doanh thu trước");
                return;
            }

            GetFromTo(out DateTime from, out DateTime to);

            new FormBieuDoDoanhThuCN(
            connStr,
            "SP_DT_SANPHAM_ALL_CN",   // ✅ PROC ĐÚNG
            from,
            to,
            "TOP CHI NHÁNH – DOANH THU SẢN PHẨM"
            ).ShowDialog();
        }

        void CardDV_Click(object sender, EventArgs e)
        {
            if (!btnTimKiem.Visible) return;
            GetFromTo(out DateTime from, out DateTime to);

            new FormBieuDoDoanhThuCN(
                connStr,
                "SP_DT_DICHVU_ALL_CN",
                from,
                to,
                "TOP 10 CHI NHÁNH – DOANH THU DỊCH VỤ"
            ).ShowDialog();
        }

        void CardLuot_Click(object sender, EventArgs e)
        {
            if (!btnTimKiem.Visible) return;
            GetFromTo(out DateTime from, out DateTime to);

            new FormTopBacSi_ALL(connStr, from, to).ShowDialog();
        }

        // ================= UTIL =================
        void GetFromTo(out DateTime from, out DateTime to)
        {
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
                int m = (int.Parse(cboQuy.Text) - 1) * 3 + 1;
                from = new DateTime((int)numNam.Value, m, 1);
                to = from.AddMonths(3).AddSeconds(-1);
            }
            else
            {
                from = new DateTime((int)numNam1.Value, 1, 1);
                to = new DateTime((int)numNam1.Value, 12, 31, 23, 59, 59);
            }
        }

        // ===== UI HELPERS =====
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

        void CreateCard(string title, int x, int y, out Label valueLabel, out Panel panel)
        {
            panel = new Panel()
            {
                Location = new Point(x, y),
                Size = new Size(420, 130),
                BackColor = COLOR_CARD,
                Cursor = Cursors.Hand
            };

            // ✅ SET REGION KHI RESIZE (KHÔNG GDI+, KHÔNG LỖI)
            panel.Resize += (s, e) =>
            {
                Panel p = (Panel)s;
                if (p.Width > 0 && p.Height > 0)
                    p.Region = new Region(RoundedPath(p.ClientRectangle, 18));
            };

            // ✅ PAINT CHỈ VẼ VIỀN – KHÔNG ĐỤNG REGION
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

        void ApplyRounded(Control c, int r)
        {
            if (c.Width <= 0 || c.Height <= 0) return;
            Rectangle rect = c.ClientRectangle;
            if (rect.Width <= 0 || rect.Height <= 0) return;
            c.Region = new Region(RoundedPath(rect, r));
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
