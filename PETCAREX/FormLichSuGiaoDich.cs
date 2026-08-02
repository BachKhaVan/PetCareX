using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace PETCAREX
{
    public partial class FormLichSuGiaoDich : Form
    {
        string connectionString =
            "Data Source=.;Initial Catalog=QLTC;Integrated Security=True";
        HoaDon _currentHoaDon;
        string _currentDanhGiaDV; // "KB" hoặc "TP"

        // ================= MODEL =================
        class HoaDon
        {
            public int MAHD;
            public DateTime TGLAP;

            public int TONGTIEN_GOC;
            public decimal KHUYENMAI;
            public int TIEN_KHUYENMAI;
            public int TONGTIEN_SAU_KM;

            public string HTTHANHTOAN;
            public int? NVLAP;
            public int KHMUA;
            public bool LOAIHD;
        }


        class ChiTietDichVu
        {
            public string LoaiDV;     // Khám / Tiêm
            public string TenThuCung;
            public string TenBacSi;
            public DateTime ThoiGian;
        }

        List<HoaDon> danhSachHoaDon = new List<HoaDon>();

        Panel panelLeft, panelRight;
        FlowLayoutPanel flpHoaDon;

        Label lblHD, lblLoai, lblTG, lblKM, lblHTTT, lblTong, lblDanhGia;

        DataGridView dgvMuaHang;
        DataGridView dgvKham;
        DataGridView dgvTiem;

        public FormLichSuGiaoDich()
        {
            InitUI();
            LoadHoaDonFromDB();
        }

        // ================= LOAD HOA DON =================
        void LoadHoaDonFromDB()
        {
            danhSachHoaDon.Clear();
            flpHoaDon.Controls.Clear();

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_LS_HOADON", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MAKH", UserSession.MaUser);

                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        danhSachHoaDon.Add(new HoaDon
                        {
                            MAHD = (int)rd["MAHD"],
                            TGLAP = (DateTime)rd["TGLAP"],

                            TONGTIEN_GOC = (int)rd["TONGTIEN_GOC"],
                            KHUYENMAI = (decimal)rd["KHUYENMAI"],
                            TIEN_KHUYENMAI = (int)rd["TIEN_KHUYENMAI"],
                            TONGTIEN_SAU_KM = (int)rd["TONGTIEN_SAU_KM"],

                            HTTHANHTOAN = rd["HTTHANHTOAN"].ToString(),
                            NVLAP = rd["NVLAP"] == DBNull.Value ? null : (int?)rd["NVLAP"],
                            KHMUA = (int)rd["KHMUA"],
                            LOAIHD = (bool)rd["LOAIHD"]
                        });
                    }
                }
            }

            foreach (var hd in danhSachHoaDon)
                AddHoaDonItem(hd);

            if (danhSachHoaDon.Count > 0)
                ShowChiTiet(danhSachHoaDon[0]);
        }

        // ================= ADD ITEM =================
        void AddHoaDonItem(HoaDon hd)
        {
            Panel p = new Panel()
            {
                Size = new Size(300, 70),
                BackColor = Color.FromArgb(255, 245, 245),
                Margin = new Padding(5),
                Cursor = Cursors.Hand
            };
            p.Paint += DrawBorder;
            p.Click += (s, e) => ShowChiTiet(hd);

            Label l1 = new Label()
            {
                Text = $"HÓA ĐƠN {hd.MAHD}",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(10, 8),
                AutoSize = true
            };

            // ⭐ DÙNG TỔNG SAU KHUYẾN MÃI
            Label l2 = new Label()
            {
                Text = hd.TONGTIEN_SAU_KM.ToString("N0") + " VND",
                ForeColor = Color.Red,
                Location = new Point(10, 38),
                AutoSize = true
            };
          
            l1.Click += (s, e) => ShowChiTiet(hd);
            l2.Click += (s, e) => ShowChiTiet(hd);
            p.Controls.Add(l1);
            p.Controls.Add(l2);
            flpHoaDon.Controls.Add(p);
        }


        // ================= DETAIL =================
        void ShowChiTiet(HoaDon hd)
        {
            lblHD.Text = $"HÓA ĐƠN {hd.MAHD}";
            lblLoai.Text = "Loại HD: " + (hd.LOAIHD ? "Dịch vụ" : "Mua sản phẩm");
            lblTG.Text = "Thời gian: " + hd.TGLAP.ToString("HH:mm dd/MM/yyyy");
            lblHTTT.Text = "Thanh toán: " + hd.HTTHANHTOAN;

            // ===== KHUYẾN MÃI =====
            if (hd.KHUYENMAI > 0)
            {
                lblKM.Visible = true;
                lblKM.ForeColor = Color.Red;
                lblKM.Text = $"Khuyến mãi: {hd.KHUYENMAI * 100:0.#}% (-{hd.TIEN_KHUYENMAI:N0} VND)";
            }
            else
            {
                lblKM.Visible = true;
                lblKM.ForeColor = Color.Gray;
                lblKM.Text = "Khuyến mãi: 0%";
            }


            // ===== PANEL TỔNG TIỀN =====
            var lblTongGocValue = panelRight.Controls
                .Find("lblTongGocValue", true)[0] as Label;

            var lblThanhToanValue = panelRight.Controls
                .Find("lblThanhToanValue", true)[0] as Label;

            lblTongGocValue.Text = $"{hd.TONGTIEN_GOC:N0} VND";
            lblThanhToanValue.Text = $"{hd.TONGTIEN_SAU_KM:N0} VND";

            //lblDanhGia.Visible = true;

            dgvMuaHang.Visible = false;
            dgvKham.Visible = false;
            dgvTiem.Visible = false;

            dgvMuaHang.Rows.Clear();
            dgvKham.Rows.Clear();
            dgvTiem.Rows.Clear();

            if (hd.LOAIHD)
            {
                LoadKhamToGrid(hd);
                dgvKham.Visible = dgvKham.Rows.Count > 0;

                LoadTiemPhongToGrid(hd);
                dgvTiem.Visible = dgvTiem.Rows.Count > 0;
            }
            else
            {
                dgvMuaHang.Visible = true;
                LoadMuaHangToGrid(hd);
            }

            // ===== ĐÁNH GIÁ =====
            // ===== ĐÁNH GIÁ =====
            _currentHoaDon = hd;
            _currentDanhGiaDV = null;

            // reset
            lblDanhGia.Visible = false;
            lblDanhGia.Click -= LblDanhGia_Click;

            // xác định dịch vụ
            bool coKB = dgvKham.Rows.Count > 0;
            bool coTP = dgvTiem.Rows.Count > 0;

            if (coKB)
            {
                _currentDanhGiaDV = "KB";
                lblDanhGia.Text = "⭐ Đánh giá dịch vụ khám bệnh";
            }
            else if (coTP)
            {
                _currentDanhGiaDV = "TP";
                lblDanhGia.Text = "⭐ Đánh giá dịch vụ tiêm phòng";
            }

            if (_currentDanhGiaDV != null)
            {
                lblDanhGia.ForeColor = Color.Red;
                lblDanhGia.Cursor = Cursors.Hand;
                lblDanhGia.Visible = true;
                lblDanhGia.Click += LblDanhGia_Click;
            }
        }


        void InitUI()
        {
            this.Text = "Lịch sử giao dịch";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(255, 235, 235);
            //int HEADER_HEIGHT = 80;
            Panel panelScroll = new Panel()
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(255, 235, 235)
            };
            this.Controls.Add(panelScroll);

            // ================= LEFT =================
            panelLeft = new Panel()
            {
                Location = new Point(20, 20 ),
                Size = new Size(360, 540),
                BackColor = Color.White
            };
            panelScroll.Controls.Add(panelLeft);
            //this.Controls.Add(panelLeft);

            Label lblLeftTitle = new Label()
            {
                Text = "📜 Lịch sử giao dịch",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 60, 60),
                Location = new Point(15, 15),
                AutoSize = true
            };
            panelLeft.Controls.Add(lblLeftTitle);

            flpHoaDon = new FlowLayoutPanel()
            {
                Location = new Point(15, 55),
                Size = new Size(330, 470),
                AutoScroll = true,
                BackColor = Color.FromArgb(255, 245, 245)
            };
            panelLeft.Controls.Add(flpHoaDon);

            // ================= RIGHT =================
            panelRight = new Panel()
            {
                Location = new Point(400, 20),
                Size = new Size(660, 540),
                BackColor = Color.White
            };
            panelScroll.Controls.Add(panelRight);

            Label lblRightTitle = new Label()
            {
                Text = "🧾 Chi tiết giao dịch",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 60, 60),
                Location = new Point(20, 15),
                AutoSize = true
            };
            panelRight.Controls.Add(lblRightTitle);

            // ===== CỘT TRÁI: THÔNG TIN =====
            lblHD = CreateLabel("", 20, 55, true);
            lblLoai = CreateLabel("", 20, 90);
            lblTG = CreateLabel("", 20, 120);
            lblKM = CreateLabel("", 20, 150);
            lblHTTT = CreateLabel("", 20, 180);

            panelRight.Controls.AddRange(new Control[]
            {
        lblHD, lblLoai, lblTG, lblKM, lblHTTT
            });

            // ================= PANEL TỔNG TIỀN (BÊN PHẢI) =================
            Panel panelTongTien = new Panel()
            {
                Name = "panelTongTien",
                Location = new Point(360, 30),
                Size = new Size(260, 150),
                BackColor = Color.FromArgb(255, 245, 245)
            };
            panelTongTien.Paint += DrawBorder;
            panelRight.Controls.Add(panelTongTien);

            Label lblTitleTong = new Label()
            {
                Text = "💰 Tổng thanh toán",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(180, 40, 40),
                Location = new Point(15, 10),
                AutoSize = true
            };

            Label lblTongGocTitle = new Label()
            {
                Text = "Tổng tiền gốc",
                Location = new Point(15, 45),
                AutoSize = true
            };

            Label lblTongGocValue = new Label()
            {
                Name = "lblTongGocValue",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(15, 65),
                AutoSize = true
            };

            Label lblThanhToanTitle = new Label()
            {
                Text = "Thanh toán",
                Location = new Point(15, 95),
                AutoSize = true
            };

            Label lblThanhToanValue = new Label()
            {
                Name = "lblThanhToanValue",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(180, 40, 40),
                Location = new Point(15, 115),
                AutoSize = true
            };

            panelTongTien.Controls.AddRange(new Control[]
            {
        lblTitleTong,
        lblTongGocTitle, lblTongGocValue,
        lblThanhToanTitle, lblThanhToanValue
            });

            // ===== DIVIDER =====
            Panel divider = new Panel()
            {
                Location = new Point(20, 220),
                Size = new Size(620, 2),
                BackColor = Color.FromArgb(220, 100, 100)
            };
            panelRight.Controls.Add(divider);

            // ===== LABEL ĐÁNH GIÁ =====
            lblDanhGia = new Label()
            {
                Text = "⭐ Thực hiện đánh giá ngay!",
                Font = new Font("Segoe UI", 11, FontStyle.Italic | FontStyle.Underline),
                ForeColor = Color.Red,
                Location = new Point(20, 230),
                AutoSize = true
            };
            panelRight.Controls.Add(lblDanhGia);

            // ================= GRID =================
            dgvMuaHang = CreateGrid(260);
            dgvMuaHang.Columns.Add("TENSP", "Tên sản phẩm");
            dgvMuaHang.Columns.Add("CN", "Chi nhánh");
            dgvMuaHang.Columns.Add("SL", "Số lượng");
            dgvMuaHang.Columns.Add("GIA", "Giá");
            dgvMuaHang.Columns.Add("TT", "Thành tiền");
            panelRight.Controls.Add(dgvMuaHang);

            dgvKham = CreateGrid(260);
            dgvKham.Columns.Add("TC", "Thú cưng");
            dgvKham.Columns.Add("BS", "Bác sĩ");
            dgvKham.Columns.Add("THUOC", "Thuốc");
            dgvKham.Columns.Add("SL", "Số lượng");
            dgvKham.Columns.Add("GIA", "Giá");
            dgvKham.Columns.Add("TT", "Thành tiền");
            panelRight.Controls.Add(dgvKham);

            dgvTiem = CreateGrid(260);
            dgvTiem.Columns.Add("TC", "Thú cưng");
            dgvTiem.Columns.Add("BS", "Bác sĩ");
            dgvTiem.Columns.Add("VX", "Loại vắc xin");
            dgvTiem.Columns.Add("GOC", "Giá gốc");
            dgvTiem.Columns.Add("UD", "Ưu đãi");
            dgvTiem.Columns.Add("TT", "Thành tiền");
            panelRight.Controls.Add(dgvTiem);
        }

        DataGridView CreateGrid(int y)
        {
            return new DataGridView()
            {
                Location = new Point(20, y),
                Size = new Size(620, 250),
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };
        }

        void LoadMuaHangToGrid(HoaDon hd)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_CHITIET_MUAHANG", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MAKH", hd.KHMUA);
                cmd.Parameters.AddWithValue("@TGLAP", hd.TGLAP);

                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        dgvMuaHang.Rows.Add(
                            rd["TENSP"],
                            rd["TENCN"],
                            rd["SOLUONG"],
                            ((int)rd["GIABAN"]).ToString("N0"),
                            ((int)rd["THANHTIEN"]).ToString("N0")
                        );
                    }
                }
            }
        }
        void LoadKhamToGrid(HoaDon hd)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_CHITIET_KB", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MAKH", hd.KHMUA);
                cmd.Parameters.AddWithValue("@TGLAP", hd.TGLAP);

                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        dgvKham.Rows.Add(
                            rd["TENTC"],
                            rd["TENBS"],
                            rd["TENSP"],
                            rd["SOLUONG"],
                            ((int)rd["GIA"]).ToString("N0"),
                            ((int)rd["THANHTIEN"]).ToString("N0")
                        );
                    }
                }
            }
        }
        void LoadTiemPhongToGrid(HoaDon hd)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_CHITIET_TP", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MAKH", hd.KHMUA);
                cmd.Parameters.AddWithValue("@TGLAP", hd.TGLAP);

                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        decimal uuDai = rd["UUDAI"] == DBNull.Value
                                        ? 0
                                        : Convert.ToDecimal(rd["UUDAI"]);

                        dgvTiem.Rows.Add(
                            rd["TENTC"],                          // Thú cưng
                            rd["TENBS"],                          // Bác sĩ
                            rd["TENVACCINE"],                     // ⭐ LOẠI VẮC XIN
                            ((int)rd["GIAGOC"]).ToString("N0"),   // Giá gốc
                            (uuDai * 100) + "%",                  // Ưu đãi
                            ((int)rd["THANHTIEN"]).ToString("N0") // Thành tiền
                        );
                    }
                }
            }
        }

        Label CreateLabel(string text, int x, int y, bool bold = false)
        {
            return new Label()
            {
                Text = text,
                Font = new Font("Segoe UI", bold ? 14 : 11,
                    bold ? FontStyle.Bold : FontStyle.Regular),
                Location = new Point(x, y),
                AutoSize = true
            };
        }

        void DrawBorder(object sender, PaintEventArgs e)
        {
            using (Pen pen = new Pen(Color.FromArgb(220, 100, 100), 2))
            {
                e.Graphics.DrawRectangle(
                    pen, 0, 0,
                    ((Control)sender).Width - 1,
                    ((Control)sender).Height - 1);
            }
        }
        void LblDanhGia_Click(object sender, EventArgs e)
        {
            if (_currentHoaDon == null || string.IsNullOrEmpty(_currentDanhGiaDV))
                return;

            using (var f = new FormDanhGia(
                _currentHoaDon.MAHD,
                _currentDanhGiaDV,
                UserSession.MaUser))
            {
                f.ShowDialog();
            }

            // refresh lại chi tiết sau khi đánh giá
            ShowChiTiet(_currentHoaDon);
        }


    }
}
