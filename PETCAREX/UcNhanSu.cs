using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PETCAREX
{
    public partial class UcNhanSu : UserControl
    {
        string connectionString = "Data Source=.;Initial Catalog=QLTC;Integrated Security=True";

        DataGridView dgvNhanSu;
        Button btnThem;
        TextBox txtTimMa;

        ContextMenuStrip menuHanhDong;
        int _maNSDangChon = -1;

        public UcNhanSu()
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
            BackColor = Color.FromArgb(255, 245, 245);

            BuildUI();
            LoadNhanSu();
        }

        // ================= UI =================
        private void BuildUI()
        {
            TableLayoutPanel main = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                RowCount = 2
            };
            main.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            main.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(main);

            // ===== TOP =====
            Panel top = new Panel { Dock = DockStyle.Fill };

            txtTimMa = new TextBox
            {
                Width = 220,
                Height = 36,
                Font = new Font("Segoe UI", 10),
                Location = new Point(0, 18)
            };
            txtTimMa.TextChanged += (s, e) => LocTheoMa();

            btnThem = new Button
            {
                Text = "+ Thêm nhân viên",
                Dock = DockStyle.Right,
                Width = 200,
                Height = 40,
                BackColor = Color.FromArgb(220, 80, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnThem.FlatAppearance.BorderSize = 0;
            btnThem.Click += BtnThem_Click;
            top.Controls.Add(txtTimMa);
            top.Controls.Add(btnThem);
            main.Controls.Add(top, 0, 0);

            // ===== GRID =====
            dgvNhanSu = CreateGrid_NhanSu();
            dgvNhanSu.CellContentClick += dgvNhanSu_CellContentClick;

            dgvNhanSu.Columns.Add("MANS", "Mã");
            dgvNhanSu.Columns.Add("HOTEN", "Họ tên");
            dgvNhanSu.Columns.Add("NGAYSINH", "Ngày sinh");
            dgvNhanSu.Columns.Add("GIOITINH", "Giới tính");
            dgvNhanSu.Columns.Add("NGAYVAOLAM", "Ngày vào làm");
            dgvNhanSu.Columns.Add("CALAM", "Ca làm");
            dgvNhanSu.Columns.Add("LUONG", "Lương CB");
            dgvNhanSu.Columns.Add("SDT", "SĐT");
            dgvNhanSu.Columns.Add("LOAI", "Loại NV");

            dgvNhanSu.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "HANHDONG",
                HeaderText = "Hành động",
                Text = "⋮",
                UseColumnTextForButtonValue = true,
                Width = 80
            });

            // ===== GIỮ CANH SIZE CỘT =====
            dgvNhanSu.Columns["MANS"].FillWeight = 60;
            dgvNhanSu.Columns["HOTEN"].FillWeight = 160;
            dgvNhanSu.Columns["NGAYSINH"].FillWeight = 90;
            dgvNhanSu.Columns["GIOITINH"].FillWeight = 80;
            dgvNhanSu.Columns["NGAYVAOLAM"].FillWeight = 100;
            dgvNhanSu.Columns["CALAM"].FillWeight = 80;
            dgvNhanSu.Columns["LUONG"].FillWeight = 90;
            dgvNhanSu.Columns["SDT"].FillWeight = 120;
            dgvNhanSu.Columns["LOAI"].FillWeight = 70;
            dgvNhanSu.Columns["HANHDONG"].FillWeight = 60;

            foreach (DataGridViewColumn c in dgvNhanSu.Columns)
                c.SortMode = DataGridViewColumnSortMode.NotSortable;

            main.Controls.Add(dgvNhanSu, 0, 1);

            // ===== MENU HÀNH ĐỘNG =====
            menuHanhDong = new ContextMenuStrip();
            menuHanhDong.Items.Add("✏️ Sửa thông tin", null, (s, e) =>
            {
                DataGridViewRow row = dgvNhanSu
                    .Rows
                    .Cast<DataGridViewRow>()
                    .First(r => Convert.ToInt32(r.Cells["MANS"].Value) == _maNSDangChon);

                int luong = Convert.ToInt32(row.Cells["LUONG"].Value);
                string ca = row.Cells["CALAM"].Value.ToString();

                FrmSuaNhanSu frm = new FrmSuaNhanSu(
                    _maNSDangChon,
                    luong,
                    ca
                );

                if (frm.ShowDialog() == DialogResult.OK)
                    LoadNhanSu();
            });

            menuHanhDong.Items.Add("🗑️ Xóa nhân viên", null, (s, e) =>
            {
                if (MessageBox.Show("Xóa nhân viên này?", "Xác nhận",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    XoaNhanSu(_maNSDangChon);
                    LoadNhanSu();
                }
            });
        }

        // ================= DATA =================
        private void LoadNhanSu()
        {
            dgvNhanSu.Rows.Clear();

            using (SqlConnection c = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SP_THONGTIN_NV", c);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MaQL", UserSession.MaUser);
                cmd.Parameters.AddWithValue(
                    "@MaNS",
                    string.IsNullOrWhiteSpace(txtTimMa.Text)
                        ? (object)DBNull.Value
                        : txtTimMa.Text
                );
                c.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    int maNS = Convert.ToInt32(r[0]);

                    if (maNS == UserSession.MaUser)
                        continue;

                    dgvNhanSu.Rows.Add(
                        r[0], r[1], r[2], r[3],
                        r[4], r[5], r[6], r[7], r[8]
                    );
                }
            }

            dgvNhanSu.ClearSelection();
        }

        // ================= EVENTS =================
        private void dgvNhanSu_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dgvNhanSu.Columns[e.ColumnIndex].Name == "HANHDONG")
            {
                _maNSDangChon = Convert.ToInt32(
                    dgvNhanSu.Rows[e.RowIndex].Cells["MANS"].Value);

                Rectangle rect = dgvNhanSu.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                Point location = dgvNhanSu.PointToScreen(
                    new Point(rect.Left, rect.Bottom));

                menuHanhDong.Show(location);
            }
        }

        private void XoaNhanSu(int maNS)
        {
            using (SqlConnection c = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SP_XOA_NV", c);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MaNS", maNS);
                c.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // ================= TÌM KIẾM Ở UI =================
        private void LocTheoMa()
        {
            foreach (DataGridViewRow row in dgvNhanSu.Rows)
            {
                if (row.Cells["MANS"].Value == null) continue;

                row.Visible = string.IsNullOrWhiteSpace(txtTimMa.Text)
                    || row.Cells["MANS"].Value.ToString().Contains(txtTimMa.Text);
            }
        }

        // ================= HELPER =================
        private DataGridView CreateGrid_NhanSu()
        {
            DataGridView dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                EnableHeadersVisualStyles = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            // ===== FIX MẤT CHỮ HEADER =====
            dgv.ColumnHeadersHeight = 52;
            dgv.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.RowTemplate.Height = 44;

            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgv.DefaultCellStyle.SelectionBackColor = Color.White;
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;

            dgv.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(255, 210, 210);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.FromArgb(120, 50, 50);
            dgv.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 11, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor =
                dgv.ColumnHeadersDefaultCellStyle.BackColor;
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor =
                dgv.ColumnHeadersDefaultCellStyle.ForeColor;

            return dgv;
        }
        private void BtnThem_Click(object sender, EventArgs e)
        {
            FrmThemNhanSu frm = new FrmThemNhanSu();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                LoadNhanSu(); // load lại danh sách sau khi thêm
            }
        }

    }
}
