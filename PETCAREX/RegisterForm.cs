using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;


namespace PETCAREX
{
    public partial class RegisterForm : Form
    {
        Panel panelMain;
        TextBox txtPassword, txtSDT, txtCCCD, txtNgaySinh;
        TextBox txtHoTen, txtEmail;
        ComboBox cboGioiTinh;
        bool isPasswordVisible = false;
        string connectionString = "Data Source=.;Initial Catalog=QLTC;Integrated Security=True";
        public RegisterForm()
        {
            // ================= FORM =================
            this.Text = "PetCareX - Đăng ký";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(255, 230, 230);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // ================= HEADER =================
            Panel panelHeader = new Panel()
            {
                Size = new Size(this.Width, 80),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(255, 210, 210)
            };

            panelHeader.Controls.Add(new Label()
            {
                Text = "PETCAREX",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.FromArgb(180, 40, 40),
                Location = new Point(30, 20),
                AutoSize = true
            });

            panelHeader.Controls.Add(new Label()
            {
                Text = "Yêu thương thú cưng của bạn theo cách chuyên nghiệp nhất.",
                Font = new Font("Segoe UI", 11, FontStyle.Italic),
                ForeColor = Color.FromArgb(180, 40, 40),
                Location = new Point(195, 35),
                AutoSize = true
            });

            this.Controls.Add(panelHeader);

            // ================= PANEL MAIN =================
            panelMain = new Panel()
            {
                Size = new Size(900, 420),
                Location = new Point((this.ClientSize.Width - 900) / 2, 130),
                BackColor = Color.FromArgb(255, 230, 230)
            };
            panelMain.Paint += PanelMain_Paint;
            this.Controls.Add(panelMain);

            panelMain.Controls.Add(new Label()
            {
                Text = "Đăng Ký Tài Khoản",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 90, 90),
                Location = new Point(480, 20),
                AutoSize = true
            });

            int x = 420, y = 90;

            txtHoTen = CreateTextBox("Họ và tên", x, y, 420);
            txtEmail = CreateTextBox("Email", x, y + 60, 260);

            panelMain.Controls.Add(txtHoTen);
            panelMain.Controls.Add(txtEmail);


            cboGioiTinh = new ComboBox()
            {
                Location = new Point(x + 290, y + 60),
                Size = new Size(130, 36),
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboGioiTinh.Items.AddRange(new string[] { "Nam", "Nữ" });
            cboGioiTinh.SelectedIndex = 0;
            panelMain.Controls.Add(cboGioiTinh);

            txtSDT = CreateTextBox("SĐT", x, y + 120, 130);
            txtCCCD = CreateTextBox("CCCD", x + 150, y + 120, 160);
            txtNgaySinh = CreateTextBox("dd/MM/yyyy", x + 330, y + 120, 160);

            panelMain.Controls.Add(txtSDT);
            panelMain.Controls.Add(txtCCCD);
            panelMain.Controls.Add(txtNgaySinh);
            // ================= ICON (BÊN TRÁI) =================
            PictureBox pic = new PictureBox()
            { Size = new Size(260, 260), Location = new Point(40, 60), SizeMode = PictureBoxSizeMode.Zoom };
            // 👉 bạn có thể đổi hình khác nếu muốn
            pic.Image = Image.FromFile("RegisterForm_Image.png"); // để file paw.png cùng exe
            panelMain.Controls.Add(pic);
            // ================= PASSWORD =================
            txtPassword = new TextBox()
            {
                Text = "Mật khẩu",
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 11),
                Size = new Size(420, 36),
                Location = new Point(x, y + 180),
                UseSystemPasswordChar = false,
                Padding = new Padding(0, 0, 32, 0)
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
                    txtPassword.Text = "Mật khẩu";
                    txtPassword.ForeColor = Color.Gray;
                    txtPassword.UseSystemPasswordChar = false;
                    isPasswordVisible = false;
                }
            };

            panelMain.Controls.Add(txtPassword);

            // ================= EYE BUTTON =================
            Button btnEye = new Button()
            {
                Text = "👁",
                Font = new Font("Segoe UI Emoji", 8),
                Size = new Size(26, 22),
                Location = new Point(
                    txtPassword.Right - 30,
                    txtPassword.Top + (txtPassword.Height - 22) / 2
                ),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                Cursor = Cursors.Hand,
                TabStop = false
            };

            btnEye.FlatAppearance.BorderSize = 0;
            btnEye.FlatAppearance.MouseOverBackColor = Color.White;
            btnEye.FlatAppearance.MouseDownBackColor = Color.White;

            btnEye.Click += (s, e) =>
            {
                if (txtPassword.Text == "Mật khẩu") return;
                isPasswordVisible = !isPasswordVisible;
                txtPassword.UseSystemPasswordChar = !isPasswordVisible;
            };

            panelMain.Controls.Add(btnEye);
            btnEye.BringToFront();

            // ================= REGISTER BUTTON =================
            Button btnRegister = new Button()
            {
                Text = "Đăng ký",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(160, 45),
                Location = new Point(x + 130, y + 240),
                BackColor = Color.FromArgb(255, 90, 90),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRegister.FlatAppearance.BorderSize = 0;

            btnRegister.Click += (s, e) =>
            {
                // 1️⃣ KIỂM THIẾU THÔNG TIN 
                bool isMissing =
                    txtSDT.Text == "" || txtSDT.Text == "SĐT" ||
                    txtCCCD.Text == "" || txtCCCD.Text == "CCCD" ||
                    txtNgaySinh.Text == "" || txtNgaySinh.Text == "dd/MM/yyyy" ||
                    txtPassword.Text == "" || txtPassword.Text == "Mật khẩu";

                if (isMissing)
                {
                    MessageBox.Show(
                        "Vui lòng nhập đầy đủ thông tin",
                        "Thiếu thông tin",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                // 2️⃣ KIỂM ĐỊNH DẠNG
                if (!IsValidPhone(txtSDT.Text.Trim()))
                {
                    MessageBox.Show("SĐT không hợp lệ (10 số, bắt đầu bằng 0)");
                    txtSDT.Focus();
                    return;
                }

                if (!IsValidCCCD(txtCCCD.Text.Trim()))
                {
                    MessageBox.Show("CCCD phải đúng 12 số");
                    txtCCCD.Focus();
                    return;
                }

                if (!IsValidBirthDate(txtNgaySinh.Text.Trim()))
                {
                    MessageBox.Show("Ngày sinh không hợp lệ (dd/MM/yyyy)");
                    txtNgaySinh.Focus();
                    return;
                }

                if (!IsValidPassword(txtPassword.Text.Trim()))
                {
                    MessageBox.Show("Mật khẩu 8–20 ký tự, chỉ chữ và số");
                    txtPassword.Focus();
                    return;
                }

                if (txtEmail.Text != "Email" && !IsValidEmail(txtEmail.Text.Trim()))
                {
                    MessageBox.Show(
                        "Email không hợp lệ hoặc có dấu tiếng Việt",
                        "Lỗi Email",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    txtEmail.Focus();
                    return;
                }
                try
                {
                    DateTime ngaySinh = DateTime.ParseExact(
                        txtNgaySinh.Text.Trim(),
                        "dd/MM/yyyy",
                        CultureInfo.InvariantCulture
                    );

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    using (SqlCommand cmd = new SqlCommand("SP_DANGKY_KHACHHANG", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@HOTEN", txtHoTen.Text.Trim());
                        cmd.Parameters.AddWithValue("@SDT", txtSDT.Text.Trim());
                        cmd.Parameters.AddWithValue("@EMAIL", txtEmail.Text == "Email" ? (object)DBNull.Value : txtEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@CCCD", txtCCCD.Text.Trim());
                        cmd.Parameters.AddWithValue("@GIOITINH", cboGioiTinh.Text);
                        cmd.Parameters.AddWithValue("@NGAYSINH", ngaySinh);
                        cmd.Parameters.AddWithValue("@MATKHAU", txtPassword.Text.Trim());

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Đăng ký thành công ✔️");
                    this.Hide();
                    new LoginForm().Show();
                }
                catch (SqlException ex)
                {
                    // lỗi từ RAISERROR bên SQL
                    MessageBox.Show(ex.Message, "Lỗi đăng ký");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi hệ thống: " + ex.Message);
                }


            };


            panelMain.Controls.Add(btnRegister);
            // ================= LOGIN LINK =================
            Label lblHaveAccount = new Label()
            {
                Text = "Đã có tài khoản?",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Red,
                AutoSize = true,
                Location = new Point(x + 100, y + 300)
            };

            Label lblLoginNow = new Label()
            {
                Text = "Đăng nhập ngay",
                Font = new Font("Segoe UI", 10, FontStyle.Underline),
                ForeColor = Color.Red,
                AutoSize = true,
                Cursor = Cursors.Hand,
                Location = new Point(lblHaveAccount.Right + 7, y + 300)
            };

            // CLICK → CHUYỂN SANG LOGIN FORM
            lblLoginNow.Click += (s, e) =>
            {
                this.Hide();
                new LoginForm().Show();
            };

            panelMain.Controls.Add(lblHaveAccount);
            panelMain.Controls.Add(lblLoginNow);

        }

        // ================= UTIL =================
        private void PanelMain_Paint(object sender, PaintEventArgs e)
        {
            int r = 25;
            GraphicsPath p = new GraphicsPath();
            p.AddArc(0, 0, r, r, 180, 90);
            p.AddArc(panelMain.Width - r, 0, r, r, 270, 90);
            p.AddArc(panelMain.Width - r, panelMain.Height - r, r, r, 0, 90);
            p.AddArc(0, panelMain.Height - r, r, r, 90, 90);
            p.CloseAllFigures();
            panelMain.Region = new Region(p);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        }

        TextBox CreateTextBox(string ph, int x, int y, int w)
        {
            TextBox t = new TextBox()
            {
                Text = ph,
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 11),
                Size = new Size(w, 36),
                Location = new Point(x, y)
            };
            t.Enter += (s, e) => { if (t.Text == ph) { t.Text = ""; t.ForeColor = Color.Black; } };
            t.Leave += (s, e) => { if (t.Text == "") { t.Text = ph; t.ForeColor = Color.Gray; } };
            return t;
        }

        bool IsValidPhone(string s) => Regex.IsMatch(s, @"^0\d{9}$");
        bool IsValidCCCD(string s) => Regex.IsMatch(s, @"^\d{12}$");
        bool IsValidPassword(string s) => Regex.IsMatch(s, @"^[a-zA-Z0-9]{8,20}$");
        bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return true; // cho phép bỏ trống

            // ❌ Có ký tự tiếng Việt
            if (Regex.IsMatch(email, @"[àáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễ
                                ìíịỉĩ
                                òóọỏõôồốộổỗơờớợởỡ
                                ùúụủũưừứựửữ
                                ỳýỵỷỹđ]", RegexOptions.IgnoreCase))
                return false;

            // ✔ Email format cơ bản
            return Regex.IsMatch(
                email,
                @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"
            );
        }

        bool IsValidBirthDate(string d)
        {
            return DateTime.TryParseExact(d, "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime dob) && dob < DateTime.Now;
        }
    }
}
