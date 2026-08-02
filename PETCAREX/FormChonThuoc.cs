using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace PETCAREX
{
    public partial class FrmChonThuoc : Form
    {
        string connectionString = "Data Source=.;Initial Catalog=QLTC;Integrated Security=True";
        // === DATA TRẢ VỀ ===
        public int MaSP { get; private set; }
        public string TenSP { get; private set; }
        public int SoLuong { get; private set; }

        TextBox txtTenThuoc, txtGia, txtSoLuong;
        Button btnLuu;

        DataTable dtThuoc = new DataTable();

        public FrmChonThuoc()
        {
            InitializeComponent();
            BuildUI();
            LoadThuocAutoComplete();
        }

        // ================= UI =================
        private void BuildUI()
        {
            Text = "Thêm thuốc";
            Size = new Size(450, 300);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;

            Label lblTen = CreateLabel("Tên thuốc", 20);
            txtTenThuoc = CreateTextBox(45);
            txtTenThuoc.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtTenThuoc.AutoCompleteSource = AutoCompleteSource.CustomSource;
            txtTenThuoc.Leave += TxtTenThuoc_Leave;

            Label lblGia = CreateLabel("Giá bán", 85);
            txtGia = CreateTextBox(110);
            txtGia.ReadOnly = true;
            txtGia.BackColor = Color.FromArgb(245, 245, 245);

            Label lblSL = CreateLabel("Số lượng", 150);
            txtSoLuong = CreateTextBox(175);

            btnLuu = new Button
            {
                Text = "Lưu",
                Dock = DockStyle.Bottom,
                Height = 45,
                BackColor = Color.FromArgb(220, 80, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnLuu.FlatAppearance.BorderSize = 0;
            btnLuu.Click += BtnLuu_Click;

            Controls.Add(btnLuu);
            Controls.Add(txtSoLuong);
            Controls.Add(lblSL);
            Controls.Add(txtGia);
            Controls.Add(lblGia);
            Controls.Add(txtTenThuoc);
            Controls.Add(lblTen);
        }

        // ================= AUTOCOMPLETE =================
        private void LoadThuocAutoComplete()
        {
            string sql =
                @"SELECT MASP, TENSP, GIABAN
                  FROM SAN_PHAM
                  WHERE LOAISP = N'Thuốc'";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                da.Fill(dtThuoc);
            }

            AutoCompleteStringCollection source = new AutoCompleteStringCollection();
            foreach (DataRow row in dtThuoc.Rows)
                source.Add(row["TENSP"].ToString());

            txtTenThuoc.AutoCompleteCustomSource = source;
        }

        // Khi chọn xong tên → map MASP + GIABAN
        private void TxtTenThuoc_Leave(object sender, EventArgs e)
        {
            DataRow[] rows = dtThuoc.Select(
                "TENSP = '" + txtTenThuoc.Text.Replace("'", "''") + "'");

            if (rows.Length > 0)
            {
                MaSP = Convert.ToInt32(rows[0]["MASP"]);
                txtGia.Text = rows[0]["GIABAN"].ToString();
            }
            else
            {
                MaSP = 0;
                txtGia.Clear();
            }
        }

        // ================= SAVE =================
        private void BtnLuu_Click(object sender, EventArgs e)
        {
            if (MaSP == 0)
            {
                MessageBox.Show("Vui lòng chọn thuốc hợp lệ");
                return;
            }

            if (!int.TryParse(txtSoLuong.Text, out int sl) || sl <= 0)
            {
                MessageBox.Show("Số lượng không hợp lệ");
                return;
            }

            TenSP = txtTenThuoc.Text;
            SoLuong = sl;

            DialogResult = DialogResult.OK;
        }

        // ================= HELPERS =================
        private Label CreateLabel(string text, int top)
        {
            return new Label
            {
                Text = text,
                Left = 20,
                Top = top,
                Width = 120,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
        }

        private TextBox CreateTextBox(int top)
        {
            return new TextBox
            {
                Left = 20,
                Top = top,
                Width = 380,
                Height = 30,
                Font = new Font("Segoe UI", 10)
            };
        }
    }
}
