using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace PETCAREX
{
    public partial class UcCalendarBacSi : UserControl
    {
        string connectionString = "Data Source=.;Initial Catalog=QLTC;Integrated Security=True";
        private Panel pnlContainer;
        private Panel pnlBottom;
        private DataGridView dgvCalendar;
        private Button btnPrev, btnToday, btnNext;
        private Label lblDate;
        private Label lblEmpty; // 🔥 LABEL EMPTY STATE

        private DateTime currentDate = DateTime.Today;

        public UcCalendarBacSi()
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
            BuildUI();
            LoadData();
        }

        // ================= UI =================
        private void BuildUI()
        {
            BackColor = Color.FromArgb(245, 245, 245);

            pnlContainer = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20, 15, 20, 10),
                BackColor = Color.FromArgb(245, 245, 245)
            };

            // ===== GRID =====
            dgvCalendar = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                BorderStyle = BorderStyle.None,
                BackgroundColor = Color.White,
                MultiSelect = false,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
                EnableHeadersVisualStyles = false
            };

            // HEADER
            dgvCalendar.ColumnHeadersHeight = 48;
            dgvCalendar.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(255, 210, 210);
            dgvCalendar.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.FromArgb(120, 50, 50);
            dgvCalendar.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 12, FontStyle.Bold);

            // CELL
            dgvCalendar.DefaultCellStyle.Font = new Font("Segoe UI", 11);
            dgvCalendar.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(255, 230, 230);
            dgvCalendar.DefaultCellStyle.SelectionForeColor = Color.Black;

            // COLUMNS
            dgvCalendar.Columns.Add(CreateTextCol("LoaiDV", "Loại DV", 120));
            dgvCalendar.Columns.Add(CreateTextCol("ThoiGian", "Thời gian", 140));
            dgvCalendar.Columns.Add(CreateTextCol("CongViec", "Công việc", 300));

            var thuCungCol = CreateTextCol("ThuCung", "Thú cưng", 0);
            thuCungCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            thuCungCol.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvCalendar.Columns.Add(thuCungCol);

            dgvCalendar.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "HanhDong",
                HeaderText = "Hành động",
                Text = "Thực hiện",
                UseColumnTextForButtonValue = true,
                Width = 140
            });

            dgvCalendar.CellClick += dgvCalendar_CellClick;

            // ===== EMPTY LABEL =====
            lblEmpty = new Label
            {
                Text = "Bạn không có lịch hôm nay",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 14, FontStyle.Italic),
                ForeColor = Color.Gray,
                BackColor = Color.White,
                Visible = false
            };

            // ===== BOTTOM =====
            pnlBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 55
            };

            btnPrev = CreateNavButton("◀ Hôm qua", 20);
            btnPrev.Click += (s, e) =>
            {
                currentDate = currentDate.AddDays(-1);
                LoadData();
            };

            btnToday = CreateNavButton("Hôm nay", 160);
            btnToday.Click += (s, e) =>
            {
                currentDate = DateTime.Today;
                LoadData();
            };

            btnNext = CreateNavButton("Ngày mai ▶", 300);
            btnNext.Click += (s, e) =>
            {
                currentDate = currentDate.AddDays(1);
                LoadData();
            };

            lblDate = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(120, 50, 50),
                Left = 460,
                Top = 17
            };

            pnlBottom.Controls.AddRange(new Control[]
            {
                btnPrev, btnToday, btnNext, lblDate
            });

            pnlContainer.Controls.Add(dgvCalendar);
            pnlContainer.Controls.Add(lblEmpty); // đè lên grid
            pnlContainer.Controls.Add(pnlBottom);

            Controls.Add(pnlContainer);

            lblEmpty.BringToFront();
        }

        private Button CreateNavButton(string text, int left)
        {
            return new Button
            {
                Text = text,
                Width = 120,
                Height = 32,
                Left = left,
                Top = 12,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(255, 210, 210),
                ForeColor = Color.FromArgb(120, 50, 50),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
        }

        private DataGridViewTextBoxColumn CreateTextCol(string name, string header, int width)
        {
            return new DataGridViewTextBoxColumn
            {
                Name = name,
                DataPropertyName = name,
                HeaderText = header,
                Width = width
            };
        }

        // ================= DATA =================
        private void LoadData()
        {
            lblDate.Text = currentDate.ToString("dd/MM/yyyy");

            var data = GetLichTheoNgay(UserSession.MaUser, currentDate);
            dgvCalendar.DataSource = data;

            bool isToday = currentDate.Date == DateTime.Today;
            dgvCalendar.Columns["HanhDong"].ReadOnly = !isToday;

            // ===== EMPTY STATE =====
            if (isToday && data.Count == 0)
            {
                dgvCalendar.Visible = false;
                lblEmpty.Visible = true;
            }
            else
            {
                dgvCalendar.Visible = true;
                lblEmpty.Visible = false;
            }
        }

        private List<LichBacSiDTO> GetLichTheoNgay(int maBS, DateTime date)
        {
            List<LichBacSiDTO> list = new List<LichBacSiDTO>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_LICHBACSI", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MaBS", maBS);
                cmd.Parameters.AddWithValue("@Ngay", date.Date);

                conn.Open();
                SqlDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    list.Add(new LichBacSiDTO
                    {
                        LoaiDV = rd["LoaiDV"].ToString(),
                        ThoiGian = rd["ThoiGian"].ToString(),
                        CongViec = rd["CongViec"].ToString(),
                        ThuCung = rd["ThuCung"].ToString(),
                        Ma = Convert.ToInt32(rd["MA"]),
                        MaTC = Convert.ToInt32(rd["MaTC"])
                    });
                }
            }
            return list;
        }

        // ================= EVENT =================
        private void dgvCalendar_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (currentDate.Date != DateTime.Today) return;

            if (dgvCalendar.Columns[e.ColumnIndex].Name != "HanhDong")
                return;

            var dto = (LichBacSiDTO)dgvCalendar.Rows[e.RowIndex].DataBoundItem;

            Form frm = FindForm();
            if (frm == null) return;

            Panel panelContent = frm.Controls["panelContent"] as Panel;
            if (panelContent == null) return;

            panelContent.Controls.Clear();

            if (dto.LoaiDV == "TP")
                panelContent.Controls.Add(new UcTiemPhong(dto.Ma, dto.MaTC));
            else if (dto.LoaiDV == "KB")
                panelContent.Controls.Add(new UcKhamBenh(dto.Ma, dto.MaTC));
        }
    }

    public class LichBacSiDTO
    {
        public int Ma { get; set; }
        public int MaTC { get; set; }
        public string LoaiDV { get; set; }
        public string ThoiGian { get; set; }
        public string CongViec { get; set; }
        public string ThuCung { get; set; }
    }
}
