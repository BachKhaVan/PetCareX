using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace PETCAREX
{
    public partial class FrmSuaNhanSu : Form
    {
        int _maNS;
        string connectionString = "Data Source=.;Initial Catalog=QLTC;Integrated Security=True";
        TextBox txtLuong;
        ComboBox cboCa;
        Button btnLuu, btnHuy;

        public FrmSuaNhanSu(int maNS, int luong, string caLam)
        {
            _maNS = maNS;
            InitializeComponent();
            BuildUI();
            txtLuong.Text = luong.ToString();
            cboCa.SelectedItem = caLam;
        }

        // ================= UI =================
        private void BuildUI()
        {
            Text = "Cập nhật nhân sự";
            Size = new Size(420, 400);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.FromArgb(255, 245, 245);

            TableLayoutPanel main = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(25),
                RowCount = 4,
                ColumnCount = 1
            };
            main.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            main.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            main.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            main.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(main);

            Label lblTitle = new Label
            {
                Text = "SỬA THÔNG TIN NHÂN SỰ",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.FromArgb(180, 60, 60),
                TextAlign = ContentAlignment.MiddleCenter
            };
            main.Controls.Add(lblTitle);

            main.Controls.Add(CreateField("Lương cơ bản", out txtLuong));

            main.Controls.Add(CreateComboField(
                "Ca làm việc",
                out cboCa,
                new object[] { "Sáng", "Trưa", "Chiều" }
            ));

            FlowLayoutPanel bottom = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0, 15, 0, 0)
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
            main.Controls.Add(bottom);
        }

        // ================= SAVE =================
        private void BtnLuu_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtLuong.Text, out int luong) || luong <= 0)
            {
                MessageBox.Show("Lương không hợp lệ");
                return;
            }

            using (SqlConnection c = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SP_SUA_NV", c);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@MaNS", _maNS);
                cmd.Parameters.AddWithValue("@LUONG", luong);
                cmd.Parameters.AddWithValue("@CA", cboCa.SelectedItem.ToString());

                c.Open();
                cmd.ExecuteNonQuery();
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        // ==========  ======= UI HELPERS =================
        private Panel CreateField(string label, out TextBox txt)
        {
            Panel p = new Panel { Dock = DockStyle.Fill };

            Label lbl = new Label
            {
                Text = label,
                Dock = DockStyle.Top,
                Height = 22,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(120, 50, 50)
            };

            txt = new TextBox
            {
                Dock = DockStyle.Top,
                Height = 34,
                Font = new Font("Segoe UI", 10)
            };

            p.Controls.Add(txt);
            p.Controls.Add(lbl);
            return p;
        }

        private Panel CreateComboField(string label, out ComboBox cbo, object[] items)
        {
            Panel p = new Panel { Dock = DockStyle.Fill };

            Label lbl = new Label
            {
                Text = label,
                Dock = DockStyle.Top,
                Height = 22,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(120, 50, 50)
            };

            cbo = new ComboBox
            {
                Dock = DockStyle.Top,
                Height = 34,
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cbo.Items.AddRange(items);
            cbo.SelectedIndex = 0;

            p.Controls.Add(cbo);
            p.Controls.Add(lbl);
            return p;
        }
    }
}
