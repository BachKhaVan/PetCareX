using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace PETCAREX
{
    public partial class FrmLichSuKham : Form
    {
        int _maTC;
        string connectionString = "Data Source=.;Initial Catalog=QLTC;Integrated Security=True";
        DataGridView dgvLichSu;
        Button btnDong;

        public FrmLichSuKham(int maTC)
        {
            _maTC = maTC;

            BuildUI();
            LoadLichSuKham();
        }

        private void BuildUI()
        {
            Text = "Lịch sử khám bệnh";
            Size = new Size(800, 450);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;

            dgvLichSu = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                EnableHeadersVisualStyles = false
            };

            dgvLichSu.ColumnHeadersHeight = 44;
            dgvLichSu.RowTemplate.Height = 38;

            dgvLichSu.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(255, 210, 210);
            dgvLichSu.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(120, 50, 50);
            dgvLichSu.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvLichSu.DefaultCellStyle.Font = new Font("Segoe UI", 10);

            dgvLichSu.Columns.Add("MaKB", "STT");
            dgvLichSu.Columns.Add("NgayKham", "Ngày khám");
            dgvLichSu.Columns.Add("TrieuChung", "Triệu chứng");
            dgvLichSu.Columns.Add("ChuanDoan", "Chuẩn đoán");

            btnDong = new Button
            {
                Text = "Đóng",
                Dock = DockStyle.Bottom,
                Height = 45,
                BackColor = Color.FromArgb(220, 80, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnDong.FlatAppearance.BorderSize = 0;
            btnDong.Click += (s, e) => this.Close();

            Controls.Add(dgvLichSu);
            Controls.Add(btnDong);
        }

        private void LoadLichSuKham()
        {
            dgvLichSu.Rows.Clear();

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_LICHSUKHAM_TC", conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@MaTC", System.Data.SqlDbType.Int).Value = _maTC;

                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        dgvLichSu.Rows.Add(
                            rd.GetInt32(0),
                            rd.GetString(1),     // Ngày khám 
                            rd.GetString(2),     // Triệu chứng
                            rd.GetString(3)      // Chuẩn đoán
                        );
                    }
                }
            }
        }
    }
}
