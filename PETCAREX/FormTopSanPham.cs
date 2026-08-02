using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace PETCAREX
{
    public partial class FormTopSanPham : Form
    {
        string connStr;
        string maCN;
        DateTime fromDate, toDate;

        DataGridView dgv;

        // ===== COLOR THEME =====
        Color COLOR_BG = Color.FromArgb(255, 235, 235);
        Color COLOR_PRIMARY = Color.FromArgb(220, 70, 70);
        Color COLOR_CARD = Color.FromArgb(255, 250, 245);
        Color COLOR_TEXT = Color.FromArgb(120, 60, 60);

        void InitUI()
        {
            Text = "Top 5 sản phẩm bán chạy";
            Size = new Size(760, 420);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = COLOR_BG;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // ===== NÚT QUAY LẠI =====
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
            btnBack.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            Controls.Add(btnBack);

            // ===== TITLE =====
            Label lblTitle = new Label()
            {
                Text = "TOP 5 SẢN PHẨM BÁN CHẠY",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = COLOR_PRIMARY,
                AutoSize = true,
                Location = new Point(220, 20)
            };
            Controls.Add(lblTitle);

            // ===== CARD =====
            Panel card = new Panel()
            {
                Location = new Point(30, 70),
                Size = new Size(680, 290),
                BackColor = COLOR_CARD
            };

            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (Pen pen = new Pen(COLOR_PRIMARY, 2))
                {
                    e.Graphics.DrawRectangle(
                        pen,
                        1, 1,
                        card.Width - 3,
                        card.Height - 3
                    );
                }
            };

            Controls.Add(card);

            // ===== DATAGRID =====
            dgv = new DataGridView()
            {
                Location = new Point(15, 15),
                Size = new Size(650, 260),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            dgv.AllowUserToOrderColumns = false;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = COLOR_PRIMARY;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 40;

            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgv.DefaultCellStyle.ForeColor = COLOR_TEXT;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 200, 200);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;

            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 245, 245);

            card.Controls.Add(dgv);
        }
        public FormTopSanPham(string _connStr, string _maCN, DateTime _from, DateTime _to)
        {
            connStr = _connStr;
            maCN = _maCN;
            fromDate = _from;
            toDate = _to;

            InitUI();
            LoadData();

            // FIX: ÉP FOCUS FORM + BUTTON NGAY KHI HIỆN
            this.Shown += (s, e) =>
            {
                this.Activate();
                this.Focus();

                // ép focus nút quay lại
                foreach (Control c in this.Controls)
                {
                    if (c is Button btn && btn.Text.Contains("Quay lại"))
                    {
                        this.ActiveControl = btn;
                        btn.Focus();
                        break;
                    }
                }
            };
        }


        void LoadData()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("SP_TOP5_SANPHAM_BANCHAY_CN", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MACN", maCN);
                cmd.Parameters.AddWithValue("@FROMDATE", fromDate);
                cmd.Parameters.AddWithValue("@TODATE", toDate);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgv.DataSource = dt;
                // KHÓA THỨ TỰ CỘT
                dgv.Columns["TENSP"].DisplayIndex = 0;
                dgv.Columns["GIABAN"].DisplayIndex = 1;
                dgv.Columns["TONG_SOLUONG"].DisplayIndex = 2;
                dgv.Columns["TONG_TIEN"].DisplayIndex = 3;

                // KHÔNG CHO ĐỔI VỊ TRÍ
                foreach (DataGridViewColumn col in dgv.Columns)
                {
                    col.SortMode = DataGridViewColumnSortMode.NotSortable;
                }

                if (dt.Columns.Contains("TENSP"))
                    dgv.Columns["TENSP"].HeaderText = "Tên sản phẩm";

                if (dt.Columns.Contains("GIABAN"))
                {
                    dgv.Columns["GIABAN"].HeaderText = "Giá bán (VND)";
                    dgv.Columns["GIABAN"].DefaultCellStyle.Format = "N0";
                }

                if (dt.Columns.Contains("TONG_SOLUONG"))
                    dgv.Columns["TONG_SOLUONG"].HeaderText = "Số lượng bán";

                if (dt.Columns.Contains("TONG_TIEN"))
                {
                    dgv.Columns["TONG_TIEN"].HeaderText = "Tổng tiền (VND)";
                    dgv.Columns["TONG_TIEN"].DefaultCellStyle.Format = "N0";
                }
            }
        }
    }
}
