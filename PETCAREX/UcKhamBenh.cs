using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PETCAREX
{
    public partial class UcKhamBenh : UserControl
    {
        int _maKB, _maTC;
        string connectionString = "Data Source=.;Initial Catalog=QLTC;Integrated Security=True";
        DataGridView dgvThuCung, dgvThuoc;
        TextBox txtTrieuChung, txtChuanDoan;
        Button btnThemThuoc, btnLuuDon;
        Label lblTCCount, lblCDCount;

        public UcKhamBenh(int maKB, int maTC)
        {
            _maKB = maKB;
            _maTC = maTC;

            InitializeComponent();
            BuildUI();
            LoadThuCung();
        }

        // ================= UI =================
        private void BuildUI()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.FromArgb(245, 245, 245);

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

            // ===== TOP =====
            dgvThuCung = CreateGrid();
            dgvThuCung.Dock = DockStyle.Fill;
            dgvThuCung.CellContentClick += dgvThuCung_CellContentClick;

            dgvThuCung.Columns.Add("Ten", "Tên");
            dgvThuCung.Columns.Add("Loai", "Loại");
            dgvThuCung.Columns.Add("Giong", "Giống");
            dgvThuCung.Columns.Add("GioiTinh", "Giới tính");
            dgvThuCung.Columns.Add("NgaySinh", "Ngày sinh");
            dgvThuCung.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Tình trạng",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            dgvThuCung.Columns.Add(new DataGridViewButtonColumn
            {
                HeaderText = "Hành động",
                Text = "Xem LS",
                UseColumnTextForButtonValue = true,
                Width = 110
            });

            main.Controls.Add(dgvThuCung, 0, 0);

            // ===== CONTENT =====
            TableLayoutPanel content = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2
            };
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 450));
            content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            main.Controls.Add(content, 0, 1);

            // ===== LEFT =====
            FlowLayoutPanel left = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(0, 0, 20, 0)
            };
            content.Controls.Add(left, 0, 0);

            Label lblTC = CreateLabel("Triệu chứng");
            txtTrieuChung = CreateTextBox();
            txtTrieuChung.MaxLength = 200;
            lblTCCount = CreateCounterLabel();
            txtTrieuChung.TextChanged += (s, e) =>
                lblTCCount.Text = txtTrieuChung.Text.Length + " / 200";

            left.Controls.Add(lblTC);
            left.Controls.Add(txtTrieuChung);
            left.Controls.Add(lblTCCount);

            Label lblCD = CreateLabel("Chuẩn đoán");
            txtChuanDoan = CreateTextBox();
            txtChuanDoan.MaxLength = 200;
            lblCDCount = CreateCounterLabel();
            txtChuanDoan.TextChanged += (s, e) =>
                lblCDCount.Text = txtChuanDoan.Text.Length + " / 200";

            left.Controls.Add(lblCD);
            left.Controls.Add(txtChuanDoan);
            left.Controls.Add(lblCDCount);

            // ===== RIGHT =====
            Panel right = new Panel { Dock = DockStyle.Fill };
            content.Controls.Add(right, 1, 0);

            dgvThuoc = CreateGrid();
            dgvThuoc.Dock = DockStyle.Fill;
            dgvThuoc.AutoGenerateColumns = false;

            dgvThuoc.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MASP",
                Visible = false
            });
            dgvThuoc.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Tên thuốc",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            dgvThuoc.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Số lượng",
                Width = 60,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            btnThemThuoc = new Button
            {
                Text = "+ Thêm thuốc",
                Dock = DockStyle.Bottom,
                Height = 45,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(255, 210, 210),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnThemThuoc.FlatAppearance.BorderSize = 0;
            btnThemThuoc.Click += BtnThemThuoc_Click;

            right.Controls.Add(dgvThuoc);
            right.Controls.Add(btnThemThuoc);

            // ===== BOTTOM =====
            Panel bottom = new Panel { Dock = DockStyle.Fill };

            btnLuuDon = new Button
            {
                Text = "Lưu đơn thuốc",
                Dock = DockStyle.Right,
                Width = 220,
                Height = 45,
                BackColor = Color.FromArgb(220, 80, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnLuuDon.FlatAppearance.BorderSize = 0;
            btnLuuDon.Click += BtnLuuDon_Click;

            bottom.Controls.Add(btnLuuDon);
            main.Controls.Add(bottom, 0, 2);
        }

        // ================= EVENTS =================
        private void BtnThemThuoc_Click(object sender, EventArgs e)
        {
            FrmChonThuoc frm = new FrmChonThuoc();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                dgvThuoc.Rows.Add(frm.MaSP, frm.TenSP, frm.SoLuong);
                dgvThuoc.ClearSelection();
            }
        }

        private void dgvThuCung_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvThuCung.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
            {
                new FrmLichSuKham(_maTC).ShowDialog();
            }
        }

        // ================= LƯU KHÁM (PROC) =================
        private void BtnLuuDon_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTrieuChung.Text) ||
                string.IsNullOrWhiteSpace(txtChuanDoan.Text))
            {
                MessageBox.Show("Vui lòng nhập Triệu chứng và Chuẩn đoán");
                return;
            }

            if (dgvThuoc.Rows.Count == 0)
            {
                MessageBox.Show("Chưa có thuốc trong toa");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SP_KHAMBENH", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@MADV", "KB");
                cmd.Parameters.AddWithValue("@STT", _maKB);
                cmd.Parameters.AddWithValue("@MATC", _maTC);
                cmd.Parameters.AddWithValue("@TRIEUCHUNG", txtTrieuChung.Text);
                cmd.Parameters.AddWithValue("@CHUANDOAN", txtChuanDoan.Text);

                SqlParameter pThuoc = cmd.Parameters.AddWithValue(
                    "@DS_THUOC", TaoBangThuoc());
                pThuoc.SqlDbType = SqlDbType.Structured;
                pThuoc.TypeName = "THUOC";

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Lưu khám bệnh & toa thuốc thành công");

                    Form f = FindForm();
                    Panel p = f.Controls.Find("panelContent", true)
                                        .FirstOrDefault() as Panel;
                    if (p != null)
                    {
                        p.Controls.Clear();
                        p.Controls.Add(new UcCalendarBacSi { Dock = DockStyle.Fill });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        // ================= HELPERS =================
        private DataTable TaoBangThuoc()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MATHUOC", typeof(int));
            dt.Columns.Add("SOLUONG", typeof(int));


            foreach (DataGridViewRow r in dgvThuoc.Rows)
            {
                if (r.IsNewRow) continue;

                dt.Rows.Add(
                    Convert.ToInt32(r.Cells["MASP"].Value),
                    Convert.ToInt32(r.Cells[2].Value)

                );
            }
            return dt;
        }

        private Label CreateLabel(string text) => new Label
        {
            Text = text,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.FromArgb(120, 50, 50),
            AutoSize = true
        };

        private Label CreateCounterLabel() => new Label
        {
            Text = "0 / 200",
            Font = new Font("Segoe UI", 8),
            ForeColor = Color.Gray,
            AutoSize = true
        };

        private TextBox CreateTextBox() => new TextBox
        {
            Width = 420,
            Height = 100,
            Multiline = true,
            Font = new Font("Segoe UI", 10)
        };

        private DataGridView CreateGrid()
        {
            DataGridView dgv = new DataGridView
            {
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            dgv.ColumnHeadersHeight = 44;
            dgv.RowTemplate.Height = 38;

            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 210, 210);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(120, 50, 50);
            dgv.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 11, FontStyle.Bold);

            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgv.DefaultCellStyle.SelectionBackColor = dgv.DefaultCellStyle.BackColor;
            dgv.DefaultCellStyle.SelectionForeColor = dgv.DefaultCellStyle.ForeColor;

            return dgv;
        }

        private void LoadThuCung()
        {
            dgvThuCung.Rows.Clear();

            string sql =
                @"SELECT TENTC, LOAITC, GIONGTC, GIOITINH,
                         CONVERT(VARCHAR, NGAYSINH,103), TINHTRANGSK
                  FROM THU_CUNG WHERE MATC=@MaTC";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaTC", _maTC);
                conn.Open();
                SqlDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    dgvThuCung.Rows.Add(rd[0], rd[1], rd[2], rd[3], rd[4], rd[5]);
                }
            }

            dgvThuCung.ClearSelection();
        }
    }
}
