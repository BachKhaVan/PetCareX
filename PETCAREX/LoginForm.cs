using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Configuration;
namespace PETCAREX
{
    public partial class LoginForm : Form
    {
        Panel panelLogin;
        TextBox txtSDT, txtPassword;
        Button btnLogin;
        string connectionString = "Data Source=.;Initial Catalog=QLTC;Integrated Security=True";

        public LoginForm()
        {
            // ================= FORM =================
            this.Text = "PetCareX - Đăng nhập";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(255, 230, 230);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            // ================= HEADER PANEL =================
            Panel panelHeader = new Panel()
            {
                Size = new Size(this.Width, 80),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(255, 210, 210) 
            };
            // ================= HEADER =================
            Label lblLogo = new Label()
            {
                Text = "PETCAREX",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 70, 70),
                Location = new Point(30, 20),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            Label lblSlogan = new Label()
            {
                Text = "Yêu thương thú cưng của bạn theo cách chuyên nghiệp nhất.",
                Font = new Font("Segoe UI", 11, FontStyle.Italic),
                ForeColor = Color.FromArgb(220, 70, 70),
                Location = new Point(195, 35),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            // ================= PANEL LOGIN =================
            panelLogin = new Panel()
            {
                Size = new Size(380, 360),
                Location = new Point(
                    (this.ClientSize.Width - 380) / 2,
                    (this.ClientSize.Height - 300) / 2),
                BackColor = Color.White
            };
            panelLogin.Paint += PanelLogin_Paint;

            // ================= TITLE =================
            Label lblTitle = new Label()
            {
                Text = "Đăng Nhập",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 90, 90),
                Location = new Point(110, 25),
                AutoSize = true
            };

            // ================= SDT =================
            txtSDT = new TextBox()
            {
                Text = "SĐT",
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 12),
                Size = new Size(260, 40),
                Location = new Point(60, 90)
            };
            txtSDT.Enter += (s, e) =>
            {
                if (txtSDT.Text == "SĐT")
                {
                    txtSDT.Text = "";
                    txtSDT.ForeColor = Color.Black;
                }
            };
            txtSDT.Leave += (s, e) =>
            {
                if (txtSDT.Text == "")
                {
                    txtSDT.Text = "SĐT";
                    txtSDT.ForeColor = Color.Gray;
                }
            };

            // ================= PASSWORD =================
            txtPassword = new TextBox()
            {
                Text = "Mật khẩu",
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 12),
                Size = new Size(260, 40),
                Location = new Point(60, 150),
                UseSystemPasswordChar = false
            };
            txtPassword.Enter += (s, e) =>
            {
                if (txtPassword.Text == "Mật khẩu")
                {
                    txtPassword.Text = "";
                    txtPassword.ForeColor = Color.Black;
                    txtPassword.UseSystemPasswordChar = true;
                }
            };
            txtPassword.Leave += (s, e) =>
            {
                if (txtPassword.Text == "")
                {
                    txtPassword.UseSystemPasswordChar = false;
                    txtPassword.Text = "Mật khẩu";
                    txtPassword.ForeColor = Color.Gray;
                }
            };

            // ================= LOGIN BUTTON =================
            btnLogin = new Button()
            {
                Text = "Đăng nhập",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(200, 45),
                Location = new Point(90, 220),
                BackColor = Color.FromArgb(255, 90, 90),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;


            // ================= REGISTER =================
            Label lblNoAccount = new Label()
            {
                Text = "Chưa có tài khoản?",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Red,
                Location = new Point(81, 280),
                AutoSize = true
            };
            Label lblRegister = new Label()
            {
                Text = "Đăng ký ngay",
                Font = new Font("Segoe UI", 10, FontStyle.Underline),
                ForeColor = Color.Red,
                Location = new Point(lblNoAccount.Right + 22, 280),
                AutoSize = true,
                Cursor = Cursors.Hand
            };


            // ================= ADD =================
            panelLogin.Controls.Add(lblTitle);
            panelLogin.Controls.Add(txtSDT);
            panelLogin.Controls.Add(txtPassword);
            panelLogin.Controls.Add(btnLogin);
            panelLogin.Controls.Add(lblNoAccount);
            panelLogin.Controls.Add(lblRegister);

            this.Controls.Add(panelLogin);

            // ================= ADD CHO HEADER =================
            this.Controls.Add(lblLogo);
            this.Controls.Add(lblSlogan);
            this.Controls.Add(panelHeader);
            lblLogo.Parent = panelHeader;
            lblSlogan.Parent = panelHeader;

            lblRegister.MouseEnter += (s, e) =>
            {
                lblRegister.ForeColor = Color.DarkRed;
            };

            lblRegister.MouseLeave += (s, e) =>
            {
                lblRegister.ForeColor = Color.Red;
            };
            lblRegister.Click += LblRegister_Click;
        }

        // ================= BO TRÒN PANEL =================
        private void PanelLogin_Paint(object sender, PaintEventArgs e)
        {
            int radius = 25;
            GraphicsPath path = new GraphicsPath();

            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(panelLogin.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(panelLogin.Width - radius, panelLogin.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, panelLogin.Height - radius, radius, radius, 90, 90);
            path.CloseAllFigures();

            panelLogin.Region = new Region(path);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }

        // ================= LOGIN (KẾT NỐI DB) =================
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string sdt = txtSDT.Text.Trim();
            string matkhau = txtPassword.Text.Trim();

            if (sdt == "" || sdt == "SĐT" || matkhau == "" || matkhau == "Mật khẩu")
            {
                MessageBox.Show("Vui lòng nhập SĐT và mật khẩu!");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("SP_DANGNHAP_CHUNG", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SDT", sdt);
                    cmd.Parameters.AddWithValue("@MATKHAU", matkhau);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (!reader.Read())
                    {
                        MessageBox.Show("Sai số điện thoại hoặc mật khẩu!");
                        return;
                    }

                    int ma = Convert.ToInt32(reader["MA"]);
                    string hoten = reader["HOTEN"].ToString();
                    string loaiNV = reader["LOAINV"].ToString();
                    bool isQuanLyChiNhanh = Convert.ToBoolean(reader["IS_QL_CN"]);

                    // ĐÓNG reader trước
                    reader.Close();

                    // ===== LƯU SESSION =====
                    UserSession.MaUser = ma;
                    UserSession.FullName = hoten;
                    UserSession.Phone = sdt;
                    UserSession.IsQuanLyChiNhanh = isQuanLyChiNhanh;

                    // ===== LẤY MÃ CHI NHÁNH =====
                    using (SqlCommand cmdCN = new SqlCommand(
                        "SELECT TOP 1 MACN FROM CN_NV WHERE MANS=@MANS AND NGAYKTCU IS NULL", conn))
                    {
                        cmdCN.Parameters.AddWithValue("@MANS", ma);
                        object macn = cmdCN.ExecuteScalar();
                        UserSession.MaCN = macn == null ? null : macn.ToString();
                    }

                    // ===== ĐIỀU HƯỚNG =====
                    this.Hide();

                    if (loaiNV == "BS")
                        new MainFormBacSi().Show();
                    else if (loaiNV == "NV")
                    {
                        if (isQuanLyChiNhanh)
                            new MainFormQuanLy().Show();   // NV nhưng là QL CN
                        else
                            new MainFormNhanVien().Show();
                            
                    }
                    else if (loaiNV == "KH")
                        new MainFormKhachHang().Show();
                    else if (loaiNV == "AD")
                        new MainFormAdmin().Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi CSDL:\n" + ex.Message);
            }
        }

        private void LblRegister_Click(object sender, EventArgs e)
        {
            RegisterForm registerForm = new RegisterForm();
            registerForm.Show();

            this.Hide(); // ẩn form đăng nhập
        }
    }
}