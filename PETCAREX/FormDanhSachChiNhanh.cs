using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace PETCAREX
{
    public partial class FormDanhSachChiNhanh : Form
    {
        Panel panelHeader, panelLeft, panelRight;
        FlowLayoutPanel flowChiNhanh;
        TextBox txtSearch;

        Label lblTenCN, lblDiaChi, lblGio;
        Label lblKham, lblTiem, lblSanPham;

        string connectionString = "Data Source=.;Initial Catalog=QLTC;Integrated Security=True";

        // 🔥 PHẢI LƯU MACN
        List<(string MaCN, string Ten, string DiaChi, string Gio)> allChiNhanh
            = new List<(string, string, string, string)>();
        
        public FormDanhSachChiNhanh()
        {
            InitUI();
            LoadChiNhanhFromDB();
        }

        // ================= LOAD CHI NHÁNH =================
        void LoadChiNhanhFromDB()
        {
            flowChiNhanh.Controls.Clear();
            allChiNhanh.Clear();

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_LAY_DS_CHI_NHANH", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        string maCN = rd["MACN"].ToString();
                        string ten = rd["TENCN"].ToString();
                        string diaChi = rd["DIACHI"].ToString();

                        TimeSpan tgMo = (TimeSpan)rd["TGMO"];
                        TimeSpan tgDong = (TimeSpan)rd["TGDONG"];
                        string gio = $"{tgMo:hh\\:mm} - {tgDong:hh\\:mm}";

                        // 🔥 LƯU VÀO LIST GỐC
                        allChiNhanh.Add((maCN, ten, diaChi, gio));

                        // 🔥 VẼ UI
                        AddChiNhanh(maCN, ten, diaChi, gio);
                    }

                }
            }
        }


        // ================= LOAD DỊCH VỤ =================
        void LoadDichVuTheoChiNhanh(string maCN)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_LAY_DV_CHI_NHANH", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MACN", maCN);

                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        bool coKham = (int)rd["CO_KHAM"] == 1;
                        bool coTiem = (int)rd["CO_TIEM"] == 1;
                        bool coSP = (int)rd["CO_SANPHAM"] == 1;

                        lblKham.Text = (coKham ? "🐾" : "❌") + " Khám bệnh";
                        lblTiem.Text = (coTiem ? "🐾" : "❌") + " Tiêm phòng";
                        lblSanPham.Text = (coSP ? "🐾" : "❌") + " Sản phẩm";
                    }
                }
            }
        }

        // ================= UI =================
        void InitUI()
        {
            int contentTop = 70; // khoảng cách từ title xuống panel


            //this.Text = "Danh sách chi nhánh";
            //this.TopLevel = false;
            //this.Dock = DockStyle.Fill;
            //this.BackColor = Color.FromArgb(255, 240, 240);


            //Label lblTitle = new Label()
            //{
            //    Text = "Danh sách chi nhánh",
            //    Font = new Font("Segoe UI", 20, FontStyle.Bold),
            //    ForeColor = Color.FromArgb(220, 70, 70),
            //    AutoSize = true,
            //    Location = new Point(
            //    (this.ClientSize.Width - 300) / 2 + 31,20),
            //    BackColor = Color.Transparent
            //};
            //this.Controls.Add(lblTitle);
            this.Text = "Danh sách chi nhánh";
            this.Size = new Size(1100, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(255, 240, 240);
            this.TopLevel = false;
            this.Dock = DockStyle.Fill;

            Label lblTitle = new Label()
            {
                Text = "Danh sách chi nhánh",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 70, 70),
                AutoSize = true,
                Location = new Point((this.ClientSize.Width - 300) / 2 -70, 20)
            };
            this.Controls.Add(lblTitle);
            panelLeft = new Panel()
            {
                Location = new Point(20, contentTop),
                Size = new Size(350, this.Height - contentTop - 20),
                BackColor = Color.FromArgb(255, 245, 245),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left
            };
            this.Controls.Add(panelLeft);

            // ===== SEARCH =====
            Panel searchBox = new Panel()
            {
                Location = new Point(15, 15),
                Size = new Size(300, 40),
                BackColor = Color.White
            };
            searchBox.Paint += (s, e) =>
            {
                using (Pen pen = new Pen(Color.FromArgb(220, 70, 70), 2))
                    e.Graphics.DrawRectangle(pen, 0, 0, searchBox.Width - 1, searchBox.Height - 1);
            };

            txtSearch = new TextBox()
            {
                Text = "Chi nhánh...",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.Gray,
                BorderStyle = BorderStyle.None,
                Location = new Point(10, 10),
                Width = 230
            };
            txtSearch.TextChanged += (s, e) =>
            {
                if (txtSearch.Text == "Chi nhánh...") return;

                string keyword = txtSearch.Text.Trim().ToLower();
                flowChiNhanh.Controls.Clear();

                foreach (var cn in allChiNhanh)
                {
                    if (cn.Ten.ToLower().Contains(keyword) ||
                        cn.DiaChi.ToLower().Contains(keyword))
                    {
                        AddChiNhanh(cn.MaCN, cn.Ten, cn.DiaChi, cn.Gio);
                    }
                }
            };

            txtSearch.Enter += (s, e) =>
            {
                if (txtSearch.Text == "Chi nhánh...")
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = Color.Black;
                }
            };
            txtSearch.Leave += (s, e) =>
            {
                if (txtSearch.Text == "")
                {
                    txtSearch.Text = "Chi nhánh...";
                    txtSearch.ForeColor = Color.Gray;
                }
            };

            searchBox.Controls.Add(txtSearch);
            panelLeft.Controls.Add(searchBox);

            flowChiNhanh = new FlowLayoutPanel()
            {
                Location = new Point(10, 70),
                Size = new Size(330, 350),
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };
            panelLeft.Controls.Add(flowChiNhanh);

            panelRight = new Panel()
            {
                Location = new Point(390, contentTop),
                Size = new Size(this.Width - 410, this.Height - contentTop - 20),
                BackColor = Color.FromArgb(255, 240, 240),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(panelRight);

            GroupBox gbInfo = new GroupBox()
            {
                Text = "Thông tin chi nhánh",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 70, 70),
                Size = new Size(560, 180),
                Location = new Point(20, 20)
            };

            lblTenCN = CreateLabel("Tên:", 40);
            lblDiaChi = CreateLabel("Địa chỉ:", 80);
            lblGio = CreateLabel("Hoạt động:", 120);

            gbInfo.Controls.Add(lblTenCN);
            gbInfo.Controls.Add(lblDiaChi);
            gbInfo.Controls.Add(lblGio);
            panelRight.Controls.Add(gbInfo);

            GroupBox gbService = new GroupBox()
            {
                Text = "Thông tin dịch vụ",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 70, 70),
                Size = new Size(560, 180),
                Location = new Point(20, 220)
            };

            lblKham = CreateLabel("🐾 Khám bệnh", 40);
            lblTiem = CreateLabel("❌ Tiêm phòng", 80);
            lblSanPham = CreateLabel("🐾 Sản phẩm", 120);

            gbService.Controls.Add(lblKham);
            gbService.Controls.Add(lblTiem);
            gbService.Controls.Add(lblSanPham);
            panelRight.Controls.Add(gbService);
        }

        // ================= CARD =================
        void AddChiNhanh(string maCN, string ten, string diaChi, string gio)
        {
            Panel card = new Panel()
            {
                Size = new Size(300, 75),
                BackColor = Color.White,
                Margin = new Padding(6),
                Cursor = Cursors.Hand
            };

            Label lblTen = new Label()
            {
                Text = ten,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };

            Label lblDC = new Label()
            {
                Text = diaChi,
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.Gray,
                Location = new Point(10, 35),
                AutoSize = true
            };

            void SelectCN(object s, EventArgs e)
            {
                lblTenCN.Text = "Tên: " + ten;
                lblDiaChi.Text = "Địa chỉ: " + diaChi;
                lblGio.Text = "Hoạt động: " + gio;
                LoadDichVuTheoChiNhanh(maCN);
            }

            card.Click += SelectCN;
            lblTen.Click += SelectCN;
            lblDC.Click += SelectCN;

            card.Controls.Add(lblTen);
            card.Controls.Add(lblDC);
            flowChiNhanh.Controls.Add(card);
        }

        Label CreateLabel(string text, int y)
        {
            return new Label()
            {
                Text = text,
                Location = new Point(30, y),
                Font = new Font("Segoe UI", 13),
                ForeColor = Color.FromArgb(150, 60, 60),
                AutoSize = true
            };
        }
    }
}
