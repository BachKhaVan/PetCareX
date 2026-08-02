using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PETCAREX
{
    public partial class UcTiemPhong : UserControl
    {
        int _maTC;
        int _maTP;
        string connectionString = "Data Source=.;Initial Catalog=QLTC;Integrated Security=True";
        DataGridView dgvThuCung, dgvLichSu;
        Label lblGoiTiem;
        Button btnXacNhan;

        public UcTiemPhong(int maTP, int maTC)
        {
            _maTC = maTC;
            _maTP = maTP;
            InitializeComponent();
            BuildUI();
            LoadThuCung();
            LoadLichSuTiem();
            LoadThongTinGoiTiem();
        }

        // ================= UI =================
        private void BuildUI()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.FromArgb(255, 245, 245);

            TableLayoutPanel main = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                RowCount = 3
            };
            main.RowStyles.Add(new RowStyle(SizeType.Absolute, 90));
            main.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            main.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            Controls.Add(main);

            // ===== THÚ CƯNG =====
            dgvThuCung = CreateGrid();
            dgvThuCung.Dock = DockStyle.Fill;
            dgvThuCung.ColumnHeadersHeight = 44;

            dgvThuCung.Columns.Add("Ten", "Tên");
            dgvThuCung.Columns.Add("Loai", "Loại");
            dgvThuCung.Columns.Add("Giong", "Giống");
            dgvThuCung.Columns.Add("GioiTinh", "Giới tính");
            dgvThuCung.Columns.Add("NgaySinh", "Ngày sinh");
            dgvThuCung.Columns.Add("TinhTrang", "Tình trạng");

            dgvThuCung.ClearSelection();
            dgvThuCung.CellEnter += (s, e) => dgvThuCung.ClearSelection();

            main.Controls.Add(dgvThuCung, 0, 0);

            // ===== CONTENT =====
            TableLayoutPanel content = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2
            };
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 350));
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            main.Controls.Add(content, 0, 1);

            // ===== LỊCH SỬ TIÊM =====
            Panel left = new Panel { Dock = DockStyle.Fill };
            Label lblLS = CreateLabel("Lịch sử tiêm");

            dgvLichSu = CreateGrid();
            dgvLichSu.Dock = DockStyle.Fill;
            dgvLichSu.ColumnHeadersHeight = 40;
            dgvLichSu.Columns.Add("STT", "STT");
            dgvLichSu.Columns.Add("TG", "Thời gian tiêm");

            dgvLichSu.ClearSelection();
            dgvLichSu.CellEnter += (s, e) => dgvLichSu.ClearSelection();


            left.Controls.Add(dgvLichSu);
            left.Controls.Add(lblLS);
            content.Controls.Add(left, 0, 0);

            // ===== GÓI TIÊM =====
            Panel right = new Panel { Dock = DockStyle.Fill };

            lblGoiTiem = new Label
            {
                Dock = DockStyle.Top,
                Height = 160,
                Padding = new Padding(15),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 11),
                BackColor = Color.White
            };

            Label lblGT = CreateLabel("Thông tin gói tiêm");

            right.Controls.Add(lblGoiTiem);
            right.Controls.Add(lblGT);
            content.Controls.Add(right, 1, 0);

            // ===== BOTTOM =====
            btnXacNhan = new Button
            {
                Text = "Xác nhận tiêm",
                Dock = DockStyle.Right,
                Width = 220,
                Height = 45,
                BackColor = Color.FromArgb(220, 80, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnXacNhan.FlatAppearance.BorderSize = 0;
            btnXacNhan.Click += BtnXacNhan_Click;

            Panel bottom = new Panel { Dock = DockStyle.Fill };
            bottom.Controls.Add(btnXacNhan);
            main.Controls.Add(bottom, 0, 2);
        }
        // ================= DATA =================
        private void LoadThuCung()
        {
            dgvThuCung.Rows.Clear();
            string sql =
                @"SELECT TENTC, LOAITC, GIONGTC, GIOITINH,
                         CONVERT(VARCHAR, NGAYSINH, 103), TINHTRANGSK
                  FROM THU_CUNG WHERE MATC = @MaTC";

            using (SqlConnection c = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, c);
                cmd.Parameters.AddWithValue("@MaTC", _maTC);
                c.Open();
                SqlDataReader r = cmd.ExecuteReader();
                if (r.Read())
                    dgvThuCung.Rows.Add(r[0], r[1], r[2], r[3], r[4], r[5]);
            }
            dgvThuCung.ClearSelection();
        }

        private void LoadLichSuTiem()
        {
            dgvLichSu.Rows.Clear();

            string sql =
                @"SELECT STT, CONVERT(VARCHAR, TGTIEM, 103)
          FROM TC_TP 
          WHERE MATC = @MaTC
            AND MATP = @MaTP
          ORDER BY STT";

            using (SqlConnection c = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, c);
                cmd.Parameters.AddWithValue("@MaTC", _maTC);
                cmd.Parameters.AddWithValue("@MaTP", _maTP); // ✅ QUAN TRỌNG

                c.Open();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                    dgvLichSu.Rows.Add(r[0], r[1]);
            }

            dgvLichSu.ClearSelection();
        }


        private void LoadThongTinGoiTiem()
        {
            string sql =
                @"SELECT LOAIVX, LIEULUONG , GIATIEN,
                  CASE 
                    WHEN TPGOI = 0 THEN 1
                    ELSE GOITIEM
                  END AS GOITIEM
                  FROM TIEM_PHONG
                  WHERE MATP = @MaTP AND MADV = 'TP'";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaTP", _maTP);
                conn.Open();
                SqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    lblGoiTiem.Text =
                        $"{rd[0]}\n" +
                        $"Liều lượng: {rd[1]} ml\n" +
                        $"Giá tiêm: {rd[2]:#,0}\n" +
                        $"Gói tiêm: {rd[3]}\n";
                }
            }
        }

        // ================= HELPERS =================
        private Label CreateLabel(string text) => new Label
        {
            Text = text,
            Dock = DockStyle.Top,
            Height = 26,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.FromArgb(120, 50, 50)
        };

        private DataGridView CreateGrid()
        {
            DataGridView dgv = new DataGridView
            {
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                BorderStyle = BorderStyle.None,
                BackgroundColor = Color.White,
                EnableHeadersVisualStyles = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            dgv.DefaultCellStyle.SelectionBackColor = Color.White;
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;

            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 210, 210);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(120, 50, 50);
            dgv.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 11, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor =
                dgv.ColumnHeadersDefaultCellStyle.BackColor;
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor =
                dgv.ColumnHeadersDefaultCellStyle.ForeColor;

            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgv.RowTemplate.Height = 36;

            return dgv;
        }
        private void BtnXacNhan_Click(object sender, EventArgs e)
        {

            // ===== QUAY LẠI CALENDAR BÁC SĨ =====
            Form frm = this.FindForm();
            if (frm == null) return;

            Panel panelContent = frm.Controls
                .Find("panelContent", true)
                .FirstOrDefault() as Panel;

            if (panelContent != null)
            {
                panelContent.Controls.Clear();
                panelContent.Controls.Add(new UcCalendarBacSi());
            }
        }

    }
}
