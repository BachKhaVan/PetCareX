using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace PETCAREX
{
    public partial class UcLichSu : UserControl
    {
        string connectionString = "Data Source=.;Initial Catalog=QLTC;Integrated Security=True";
        DataGridView dgv;
        Button btnKham, btnTiem;

        public UcLichSu()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.FromArgb(255, 245, 245);
            BuildUI();

            SetActiveButton(btnKham, btnTiem);
            LoadKhamBenh();
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

            // ===== TOP BUTTON =====
            FlowLayoutPanel top = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight
            };

            btnKham = CreateButton("Lịch sử khám bệnh");
            btnTiem = CreateButton("Lịch sử tiêm phòng");

            btnKham.Click += (s, e) =>
            {
                SetActiveButton(btnKham, btnTiem);
                LoadKhamBenh();
            };

            btnTiem.Click += (s, e) =>
            {
                SetActiveButton(btnTiem, btnKham);
                LoadTiemPhong();
            };

            top.Controls.Add(btnKham);
            top.Controls.Add(btnTiem);
            main.Controls.Add(top, 0, 0);

            // ===== GRID =====
            dgv = CreateGrid();
            main.Controls.Add(dgv, 0, 1);
        }

        // ================= LOAD KHÁM =================
        private void LoadKhamBenh()
        {
            dgv.Columns.Clear();
            dgv.Rows.Clear();

            dgv.Columns.Add("TENTC", "Tên TC");
            dgv.Columns.Add("STT", "STT");
            dgv.Columns.Add("TG", "Thời gian");
            dgv.Columns.Add("TC", "Triệu chứng");
            dgv.Columns.Add("CD", "Chuẩn đoán");

            using (SqlConnection c = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_LICHSUKHAM_BS", c))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@MaBS", SqlDbType.Int).Value = UserSession.MaUser;

                c.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        dgv.Rows.Add(
                            r["TENTC"],
                            r["STT"],
                            Convert.ToDateTime(r["TGKHAM"]).ToString("dd/MM/yyyy"),
                            r["MTTRIEUCHUNG"],
                            r["MTCHUANDOAN"]
                        );
                    }
                }
            }

            // ===== CÂN CỘT =====
            dgv.Columns["TENTC"].FillWeight = 15;
            dgv.Columns["STT"].FillWeight = 8;
            dgv.Columns["TG"].FillWeight = 12;
            dgv.Columns["TC"].FillWeight = 32;
            dgv.Columns["CD"].FillWeight = 32;

            dgv.Columns["STT"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns["TG"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            dgv.Columns["TC"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgv.Columns["CD"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            LockGrid();
        }

        // ================= LOAD TIÊM =================
        private void LoadTiemPhong()
        {
            dgv.Columns.Clear();
            dgv.Rows.Clear();

            dgv.Columns.Add("TENTC", "Tên TC");
            dgv.Columns.Add("STT", "Lần tiêm");
            dgv.Columns.Add("LOAIVX", "Vaccine");
            dgv.Columns.Add("TG", "Thời gian tiêm");

            using (SqlConnection c = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_LICHSUTIEM_BS", c))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@MaBS", SqlDbType.Int).Value = UserSession.MaUser;

                c.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        dgv.Rows.Add(
                            r["TENTC"],
                            r["STT"],
                            r["LOAIVX"],
                            Convert.ToDateTime(r["TGTIEM"]).ToString("dd/MM/yyyy")
                        );
                    }
                }
            }

            dgv.Columns["TENTC"].FillWeight = 25;
            dgv.Columns["STT"].FillWeight = 10;
            dgv.Columns["LOAIVX"].FillWeight = 35;
            dgv.Columns["TG"].FillWeight = 20;

            dgv.Columns["STT"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns["TG"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            LockGrid();
        }

        // ================= GRID FIX =================
        private void LockGrid()
        {
            foreach (DataGridViewColumn c in dgv.Columns)
                c.SortMode = DataGridViewColumnSortMode.NotSortable;

            dgv.ClearSelection();
            dgv.CurrentCell = null;
        }

        // ================= HELPER =================
        private Button CreateButton(string text) => new Button
        {
            Text = text,
            Width = 200,
            Height = 40,
            Margin = new Padding(0, 0, 15, 0),
            BackColor = Color.FromArgb(255, 210, 210),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            Cursor = Cursors.Hand
        };

        private DataGridView CreateGrid()
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

            // HEADER
            dgv.ColumnHeadersHeight = 46;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 210, 210);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(120, 50, 50);
            dgv.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 11, FontStyle.Bold);

            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgv.RowTemplate.Height = 42;

            dgv.DefaultCellStyle.SelectionBackColor = Color.White;
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.RowsDefaultCellStyle.SelectionBackColor = Color.White;
            dgv.RowsDefaultCellStyle.SelectionForeColor = Color.Black;

            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = dgv.ColumnHeadersDefaultCellStyle.BackColor;

            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = dgv.ColumnHeadersDefaultCellStyle.ForeColor;

            dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            return dgv;
        }

        private void SetActiveButton(Button active, Button inactive)
        {
            active.BackColor = Color.FromArgb(220, 80, 80);
            active.ForeColor = Color.White;

            inactive.BackColor = Color.FromArgb(255, 210, 210);
            inactive.ForeColor = Color.White;
        }
    }
}
