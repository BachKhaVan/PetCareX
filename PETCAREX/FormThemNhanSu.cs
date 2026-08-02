using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace PETCAREX
{
    public partial class FrmThemNhanSu : Form
    {
        //string connectionString = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;
        string connectionString = "Data Source=.;Initial Catalog=QLTC;Integrated Security=True";

        TextBox txtHoTen, txtLuong, txtSDT, txtMatKhau;
        ComboBox cboGioiTinh, cboCaLam, cboLoaiNV;
        DateTimePicker dtpNgaySinh, dtpNgayVaoLam;
        Button btnLuu, btnHuy;

        public FrmThemNhanSu()
        {
            Text = "Thêm nhân sự";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(540, 560);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            BackColor = Color.FromArgb(255, 245, 245);

            BuildUI();
        }

        // ================= UI =================
        private void BuildUI()
        {
            TableLayoutPanel main = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(30),
                ColumnCount = 2,
                RowCount = 10
            };

            main.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));
            main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            for (int i = 0; i < 9; i++)
                main.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
            main.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));

            Controls.Add(main);

            main.Controls.Add(CreateLabel("Họ tên"), 0, 0);
            txtHoTen = CreateTextBox();
            main.Controls.Add(txtHoTen, 1, 0);

            main.Controls.Add(CreateLabel("Ngày sinh"), 0, 1);
            dtpNgaySinh = CreateDatePicker();
            dtpNgaySinh.MaxDate = DateTime.Today;
            main.Controls.Add(dtpNgaySinh, 1, 1);

            main.Controls.Add(CreateLabel("Giới tính"), 0, 2);
            cboGioiTinh = CreateComboBox("Nam", "Nữ");
            main.Controls.Add(cboGioiTinh, 1, 2);

            main.Controls.Add(CreateLabel("Ngày vào làm"), 0, 3);
            dtpNgayVaoLam = CreateDatePicker();
            main.Controls.Add(dtpNgayVaoLam, 1, 3);

            main.Controls.Add(CreateLabel("Ca làm việc"), 0, 4);
            cboCaLam = CreateComboBox("Sáng", "Trưa", "Chiều");
            main.Controls.Add(cboCaLam, 1, 4);

            main.Controls.Add(CreateLabel("Lương cơ bản"), 0, 5);
            txtLuong = CreateTextBox();
            main.Controls.Add(txtLuong, 1, 5);

            main.Controls.Add(CreateLabel("Số điện thoại"), 0, 6);
            txtSDT = CreateTextBox();
            main.Controls.Add(txtSDT, 1, 6);

            main.Controls.Add(CreateLabel("Mật khẩu"), 0, 7);
            txtMatKhau = CreateTextBox();
            txtMatKhau.UseSystemPasswordChar = true;
            main.Controls.Add(txtMatKhau, 1, 7);

            main.Controls.Add(CreateLabel("Loại nhân viên"), 0, 8);
            cboLoaiNV = CreateComboBox("NV", "BS");
            main.Controls.Add(cboLoaiNV, 1, 8);

            // ===== BUTTON =====
            FlowLayoutPanel bottom = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0, 20, 0, 0)
            };

            btnLuu = new Button
            {
                Text = "Lưu",
                Width = 100,
                Height = 40,
                BackColor = Color.FromArgb(220, 80, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnLuu.FlatAppearance.BorderSize = 0;
            btnLuu.Click += BtnLuu_Click;

            btnHuy = new Button
            {
                Text = "Hủy",
                Width = 100,
                Height = 40,
                BackColor = Color.FromArgb(230, 150, 150),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnHuy.Click += (s, e) => Close();

            bottom.Controls.Add(btnLuu);
            bottom.Controls.Add(btnHuy);

            main.SetColumnSpan(bottom, 2);
            main.Controls.Add(bottom, 0, 9);
        }

        // ================= SAVE =================
        private void BtnLuu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHoTen.Text) ||
                string.IsNullOrWhiteSpace(txtLuong.Text) ||
                string.IsNullOrWhiteSpace(txtSDT.Text) ||
                string.IsNullOrWhiteSpace(txtMatKhau.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin");
                return;
            }

            if (!int.TryParse(txtLuong.Text, out int luong) || luong <= 0)
            {
                MessageBox.Show("Lương không hợp lệ");
                return;
            }

            if (txtSDT.Text.Length != 10 || !long.TryParse(txtSDT.Text, out _))
            {
                MessageBox.Show("Số điện thoại phải đủ 10 chữ số");
                return;
            }

            if (dtpNgayVaoLam.Value.Date < dtpNgaySinh.Value.Date)
            {
                MessageBox.Show("Ngày vào làm không hợp lệ");
                return;
            }

            try
            {
                using (SqlConnection c = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("SP_THEM_NV", c);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@MaQL", UserSession.MaUser);
                    cmd.Parameters.AddWithValue("@HoTen", txtHoTen.Text.Trim());
                    cmd.Parameters.AddWithValue("@NgaySinh", dtpNgaySinh.Value.Date);
                    cmd.Parameters.AddWithValue("@GioiTinh", cboGioiTinh.Text);
                    cmd.Parameters.AddWithValue("@NgayVaoLam", dtpNgayVaoLam.Value.Date);
                    cmd.Parameters.AddWithValue("@CaLamViec", cboCaLam.Text);
                    cmd.Parameters.AddWithValue("@LuongCB", luong);
                    cmd.Parameters.AddWithValue("@SDT", txtSDT.Text);
                    cmd.Parameters.AddWithValue("@MatKhau", txtMatKhau.Text);
                    cmd.Parameters.AddWithValue("@LoaiNV", cboLoaiNV.Text);

                    c.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Thêm nhân sự thành công");
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // ================= HELPER =================
        private Label CreateLabel(string text) => new Label
        {
            Text = text,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.FromArgb(120, 50, 50)
        };

        private TextBox CreateTextBox() => new TextBox
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10)
        };

        private ComboBox CreateComboBox(params string[] items)
        {
            ComboBox cbo = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cbo.Items.AddRange(items);
            cbo.SelectedIndex = 0;
            return cbo;
        }

        private DateTimePicker CreateDatePicker() => new DateTimePicker
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10),
            Format = DateTimePickerFormat.Custom,
            CustomFormat = "dd/MM/yyyy"
        };
    }
}
