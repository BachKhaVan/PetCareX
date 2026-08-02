using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace PETCAREX
{
    public partial class FormTopBacSi : Form
    {
        string connStr;
        string maCN;
        DateTime fromDate, toDate;

        DataGridView dgv;

        Color COLOR_BG = Color.FromArgb(255, 235, 235);
        Color COLOR_PRIMARY = Color.FromArgb(220, 70, 70);
        Color COLOR_CARD = Color.FromArgb(255, 250, 245);
        Color COLOR_TEXT = Color.FromArgb(120, 60, 60);

        public FormTopBacSi(string _connStr, string _maCN, DateTime _from, DateTime _to)
        {
            connStr = _connStr;
            maCN = _maCN;
            fromDate = _from;
            toDate = _to;

            InitUI();
            LoadData();
        }

        void InitUI()
        {
            Text = "Bác sĩ có nhiều lượt khám / tiêm nhất";
            Size = new Size(720, 420);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = COLOR_BG;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // ===== BACK =====
            Button btnBack = new Button()
            {
                Text = "← Quay lại",
                Location = new Point(20, 15),
                Size = new Size(110, 36),
                BackColor = COLOR_PRIMARY,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.Click += (s, e) => this.Close();
            Controls.Add(btnBack);

            // ===== TITLE =====
            Label lblTitle = new Label()
            {
                Text = "TOP 5 BÁC SĨ KHÁM / TIÊM",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = COLOR_PRIMARY,
                AutoSize = true,
                Location = new Point(190, 20)
            };
            Controls.Add(lblTitle);

            // ===== CARD =====
            Panel card = new Panel()
            {
                Location = new Point(30, 70),
                Size = new Size(650, 290),
                BackColor = COLOR_CARD
            };
            card.Paint += (s, e) =>
            {
                using (Pen pen = new Pen(COLOR_PRIMARY, 2))
                {
                    e.Graphics.DrawRectangle(pen, 1, 1, card.Width - 3, card.Height - 3);
                }
            };
            Controls.Add(card);

            // ===== GRID =====
            dgv = new DataGridView()
            {
                Location = new Point(15, 15),
                Size = new Size(620, 260),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToOrderColumns = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None
            };

            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = COLOR_PRIMARY;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 40;

            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgv.DefaultCellStyle.ForeColor = COLOR_TEXT;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 200, 200);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 245, 245);

            card.Controls.Add(dgv);
        }

        void LoadData()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("SP_TOP5_BACSI_KHAM_TIEM_CN", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MACN", maCN);
                cmd.Parameters.AddWithValue("@FROMDATE", fromDate);
                cmd.Parameters.AddWithValue("@TODATE", toDate);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgv.DataSource = dt;

                dgv.Columns["MABS"].Visible = false;

                dgv.Columns["TEN_BAC_SI"].HeaderText = "Bác sĩ";
                dgv.Columns["TONG_LUOT"].HeaderText = "Tổng lượt khám / tiêm";

                dgv.Columns["TEN_BAC_SI"].DisplayIndex = 0;
                dgv.Columns["TONG_LUOT"].DisplayIndex = 1;

                foreach (DataGridViewColumn col in dgv.Columns)
                    col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }
    }
}
