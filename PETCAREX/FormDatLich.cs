using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace PETCAREX
{
    public partial class FormDatLich : Form
    {
        // --- BIẾN TOÀN CỤC ---
        string strCon = @"Data Source=.;Initial Catalog=QLTC;Integrated Security=True";
        string selectedService = "";
        string selectedMaCN = "";
        Button lastSelectedBranch = null;
        Panel panelMain;
        Panel pnlStep1;

        // --- CONTROLS ---
        Panel cardService, panelBranch, scrollBranch;
        TextBox txtSearch;
        Button btnKham, btnTiem, btnNext;

        public FormDatLich()
        {
            InitializeComponent(); // nếu dùng designer, nếu không có có thể bỏ
            this.DoubleBuffered = true;
            InitUI();
        }

        // --- KHỞI TẠO GIAO DIỆN ---
        public void InitUI()
        {
            ResetFlow();

            // --- Panel chính ---
            if (panelMain == null)
            {
                panelMain = new Panel()
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.Transparent,
                    Name = "panelMain"
                };
                this.Controls.Add(panelMain);
                panelMain.SendToBack();
            }

            // --- Step1 panel ---
            if (pnlStep1 == null)
                pnlStep1 = new Panel() { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            pnlStep1.Controls.Clear();  // chỉ clear nội dung step1
            if (!panelMain.Controls.Contains(pnlStep1))
                panelMain.Controls.Add(pnlStep1);

            // --- Form setup ---
            this.Text = "PETCAREX - Đăng ký khám";
            this.Size = new Size(1150, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(255, 242, 242);

            // --- CARD DỊCH VỤ ---
            cardService = new Panel()
            {
                Location = new Point(20, 20),
                Size = new Size(400, 450),
                BackColor = Color.White
            };
            cardService.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = GetRoundedPath(new Rectangle(0, 0, cardService.Width, cardService.Height), 30))
                {
                    cardService.Region = new Region(path);
                    using (Pen pen = new Pen(Color.FromArgb(255, 180, 180), 2))
                    {
                        e.Graphics.DrawPath(pen, path);
                    }
                }
            };

            Label lblTitle = new Label()
            {
                Text = "Đăng ký Khám và Tiêm phòng",
                Font = new Font("Segoe UI", 19, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 90, 90),
                Location = new Point(13, 30),
                AutoSize = true
            };
            cardService.Controls.Add(lblTitle);

            string assetPath = Path.Combine(Application.StartupPath, "Assets");
            cardService.Controls.Add(CreateCircularPic(Path.Combine(assetPath, "cat.png"), 65, 100, 1.0f));
            cardService.Controls.Add(CreateCircularPic(Path.Combine(assetPath, "dog.png"), 236, 100, 1.4f));

            Label lblChoose = new Label()
            {
                Text = "Chọn dịch vụ",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 90, 90),
                Location = new Point(145, 290),
                AutoSize = true
            };
            cardService.Controls.Add(lblChoose);

            btnKham = CreateServiceButton("❤️ Khám bệnh", 35, 340);
            btnTiem = CreateServiceButton("💉 Tiêm phòng", 210, 340);

            btnKham.Click += (s, e) => SelectService(btnKham, "Khám bệnh");
            btnTiem.Click += (s, e) => SelectService(btnTiem, "Tiêm phòng");

            cardService.Controls.Add(btnKham);
            cardService.Controls.Add(btnTiem);

            // --- PANEL CHI NHÁNH ---
            panelBranch = new Panel()
            {
                Location = new Point(450, 20),
                Size = new Size(600, 500),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BackColor = Color.Transparent
            };

            Label lblCN = new Label()
            {
                Text = "🎀 Chi nhánh",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 70, 70),
                AutoSize = true
            };
            panelBranch.Controls.Add(lblCN);

            txtSearch = new TextBox()
            {
                Font = new Font("Segoe UI", 13),
                Location = new Point(0, 45),
                Size = new Size(500, 35),
                Text = " Chọn chi nhánh",
                ReadOnly = true,
                BackColor = Color.White
            };
            panelBranch.Controls.Add(txtSearch);

            scrollBranch = new Panel()
            {
                Location = new Point(0, 95),
                Size = new Size(500, 300),
                AutoScroll = true
            };
            panelBranch.Controls.Add(scrollBranch);

            // --- NÚT TIẾP TỤC ---
            if (btnNext == null)
            {
                btnNext = new Button()
                {
                    Text = "Tiếp tục ➔",
                    Size = new Size(160, 50),
                    Location = new Point(950, 540),
                    Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                    BackColor = Color.FromArgb(220, 70, 70),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                btnNext.Click += (s, e) => HandleNextStep();
                btnNext.Paint += (s, e) =>
                {
                    btnNext.Region = new Region(GetRoundedPath(new Rectangle(0, 0, btnNext.Width, btnNext.Height), 25));
                };
                this.Controls.Add(btnNext); // luôn nằm ngoài panelMain
            }
            btnNext.BringToFront();

            pnlStep1.Controls.Add(cardService);
            pnlStep1.Controls.Add(panelBranch);

            LoadChiNhanh();
        }

        // --- LOGIC NEXT STEP ---
        private void HandleNextStep()
        {
            if (string.IsNullOrEmpty(selectedService))
            {
                MessageBox.Show("Vui lòng chọn dịch vụ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(selectedMaCN))
            {
                MessageBox.Show("Vui lòng chọn chi nhánh!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Ẩn nút tiếp tục khi nhúng form con
            btnNext.Visible = false;

            pnlStep1.Controls.Clear(); // chỉ clear step1, không xóa btnNext

            if (selectedService == "Khám bệnh")
            {
                FormKhamBenh frmKham = new FormKhamBenh(selectedMaCN);
                frmKham.OnBackToMain = () =>
                {
                    this.Invoke(new Action(() =>
                    {
                        btnNext.Visible = true; // hiện lại nút khi quay về
                        InitUI();
                    }));
                };
                frmKham.TopLevel = false;
                frmKham.FormBorderStyle = FormBorderStyle.None;
                frmKham.Dock = DockStyle.Fill;
                pnlStep1.Controls.Add(frmKham);
                frmKham.Show();
            }
            else if (selectedService == "Tiêm phòng")
            {
                FormTiemPhong frmTiem = new FormTiemPhong(selectedMaCN);
                frmTiem.OnBackToMain = () =>
                {
                    this.Invoke(new Action(() =>
                    {
                        btnNext.Visible = true; // hiện lại nút khi quay về
                        InitUI();
                    }));
                };
                frmTiem.TopLevel = false;
                frmTiem.FormBorderStyle = FormBorderStyle.None;
                frmTiem.Dock = DockStyle.Fill;
                pnlStep1.Controls.Add(frmTiem);
                frmTiem.Show();
            }
        }


        // --- LOAD CHI NHÁNH ---
        void LoadChiNhanh(string filterService = "")
        {
            scrollBranch.Controls.Clear();
            int yPos = 5;

            using (SqlConnection conn = new SqlConnection(strCon))
            {
                try
                {
                    conn.Open();
                    string sql = string.IsNullOrEmpty(filterService)
                        ? "SELECT MACN, TENCN, DIACHI FROM CHI_NHANH"
                        : $@"SELECT DISTINCT CN.MACN, CN.TENCN, CN.DIACHI 
                             FROM CHI_NHANH CN
                             JOIN CN_DV CD ON CN.MACN = CD.MACN
                             JOIN DICH_VU DV ON CD.MADV = DV.MADV
                             WHERE DV.TENDV = N'{filterService}'";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataReader r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        Button b = new Button()
                        {
                            Text = $"{r["TENCN"]} ({r["DIACHI"]})",
                            Tag = r["MACN"].ToString(),
                            Font = new Font("Segoe UI", 9, FontStyle.Bold),
                            Size = new Size(580, 40),
                            Location = new Point(10, yPos),
                            BackColor = Color.White,
                            ForeColor = Color.FromArgb(220, 70, 70),
                            FlatStyle = FlatStyle.Flat,
                            TextAlign = ContentAlignment.MiddleLeft,
                            Padding = new Padding(10, 0, 0, 0)
                        };
                        b.Click += (s, e) =>
                        {
                            if (lastSelectedBranch != null)
                            {
                                lastSelectedBranch.BackColor = Color.White;
                                lastSelectedBranch.ForeColor = Color.FromArgb(220, 70, 70);
                            }
                            b.BackColor = Color.FromArgb(220, 70, 70);
                            b.ForeColor = Color.White;
                            txtSearch.Text = " " + b.Text;
                            lastSelectedBranch = b;
                            selectedMaCN = b.Tag.ToString();
                        };
                        scrollBranch.Controls.Add(b);
                        yPos += 50;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kết nối: " + ex.Message);
                }
            }
        }

        // --- CHỌN DỊCH VỤ ---
        void SelectService(Button btnSelected, string serviceName)
        {
            selectedService = serviceName;
            btnKham.BackColor = Color.FromArgb(255, 180, 180);
            btnTiem.BackColor = Color.FromArgb(255, 180, 180);
            btnSelected.BackColor = Color.FromArgb(220, 70, 70);

            selectedMaCN = ""; // reset chi nhánh khi đổi dịch vụ
            txtSearch.Text = " Chọn chi nhánh";
            LoadChiNhanh(serviceName);
        }

        // --- HỖ TRỢ ĐỒ HỌA ---
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

        PictureBox CreateCircularPic(string imagePath, int x, int y, float scale = 1.0f)
        {
            PictureBox pb = new PictureBox() { Size = new Size(120, 120), Location = new Point(x, y), BackColor = Color.Transparent };
            Image img = File.Exists(imagePath) ? Image.FromFile(imagePath) : null;

            pb.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                if (img != null)
                {
                    int nW = (int)(pb.Width * scale), nH = (int)(pb.Height * scale);
                    e.Graphics.DrawImage(img, (pb.Width - nW) / 2, (pb.Height - nH) / 2, nW, nH);
                }
                using (Pen p = new Pen(Color.FromArgb(255, 180, 180), 3))
                {
                    e.Graphics.DrawEllipse(p, 2, 2, pb.Width - 5, pb.Height - 5);
                }
            };
            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(0, 0, pb.Width, pb.Height);
            pb.Region = new Region(gp);
            return pb;
        }

        void ResetFlow()
        {
            selectedService = "";
            selectedMaCN = "";
            lastSelectedBranch = null;
        }

        Button CreateServiceButton(string text, int x, int y)
        {
            Button b = new Button()
            {
                Text = text,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(160, 45),
                Location = new Point(x, y),
                BackColor = Color.FromArgb(255, 180, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            b.Region = new Region(GetRoundedPath(new Rectangle(0, 0, b.Width, b.Height), 20));
            return b;
        }
    }
}
