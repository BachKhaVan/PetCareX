using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace PETCAREX
{
    public partial class FormMuaHang : Form
    {
        string connectionString =
            "Data Source=.;Initial Catalog=QLTC;Integrated Security=True";

        Panel panelHeader, panelSearch, panelResult, panelAction;
        Label lblTitle;
        TextBox txtTenSP;
        ComboBox cboLoaiSP;
        Button btnTimKiem, btnThemGio;
        DataGridView dgvSanPham;
        NumericUpDown nudSoLuong;
        Button btnXemGio;

        // (MASP, MACN) → TỒN KHO
        Dictionary<Tuple<string, string>, int> TonKhoTheoChiNhanh =
            new Dictionary<Tuple<string, string>, int>();

        public FormMuaHang()
        {
            InitUI();              // ❗ UI GIỮ NGUYÊN
            LoadFakeData();        // ❗ GIỜ LOAD DB
            HookChiNhanhChange();  // ❗ ĐỔI CN → ĐỔI TỒN
        }

        // ================= LOAD DATA FROM DB =================
        void LoadFakeData()
        {
            dgvSanPham.Rows.Clear();
            TonKhoTheoChiNhanh.Clear();

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_MUAHANG", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    Dictionary<string, DataGridViewRow> rowMap =
                        new Dictionary<string, DataGridViewRow>();

                    while (rd.Read())
                    {
                        string masp = rd["MASP"].ToString();
                        string tensp = rd["TENSP"].ToString();
                        string loai = rd["LOAISP"].ToString();
                        decimal gia = Convert.ToDecimal(rd["GIABAN"]);
                        string macn = rd["MACN"].ToString();
                        string tencn = rd["TENCN"].ToString();
                        int ton = Convert.ToInt32(rd["SLTONKHO"]);

                        // map tồn kho
                        TonKhoTheoChiNhanh[
                            new Tuple<string, string>(masp, macn)
                        ] = ton;

                        // mỗi MASP chỉ tạo 1 row
                        if (!rowMap.ContainsKey(masp))
                        {
                            int idx = dgvSanPham.Rows.Add(
                                masp, tensp, loai, gia, null, 0
                            );
                            rowMap[masp] = dgvSanPham.Rows[idx];
                        }

                        DataGridViewComboBoxCell cb =
                            rowMap[masp].Cells["CHINHANH"]
                            as DataGridViewComboBoxCell;

                        string displayCN = tencn + " (" + macn + ")";

                        if (!cb.Items.Contains(displayCN))
                            cb.Items.Add(displayCN);

                        // set chi nhánh đầu tiên
                        if (cb.Value == null)
                        {
                            cb.Value = displayCN;
                            rowMap[masp].Cells["TON"].Value = ton;
                        }
                    }
                }
            }
        }

        // ================= UI (GIỮ NGUYÊN) =================
        void InitUI()
        {
            this.Text = "Mua hàng";
            this.Size = new Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(255, 240, 240);
            this.TopLevel = false;
            this.Dock = DockStyle.Fill;

            // ===== TITLE =====
            Label lblTitle = new Label()
            {
                Text = "Mua hàng",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 70, 70),
                AutoSize = true,
                Location = new Point(
                    (this.ClientSize.Width - 300) / 2 + 15,
                    20
                )
            };
            this.Controls.Add(lblTitle);
            //panelHeader = new Panel()
            //{
            //    Dock = DockStyle.Top,
            //    Height = 80,
            //    BackColor = Color.FromArgb(255, 210, 210)
            //};
            //this.Controls.Add(panelHeader);

            //lblTitle = new Label()
            //{
            //    Text = "CỬA HÀNG",
            //    Font = new Font("Segoe UI", 20, FontStyle.Bold),
            //    ForeColor = Color.FromArgb(210, 50, 50),
            //    AutoSize = true,
            //    Location = new Point(
            //        (this.ClientSize.Width - 200) / 2,
            //        panelHeader.Bottom + 10)
            //};
            //this.Controls.Add(lblTitle);

            // ===== SEARCH =====
            panelSearch = new Panel()
            {
                // ✅ FIX Ở ĐÂY – KHÔNG ĐÈ TITLE
                Location = new Point(30, lblTitle.Bottom + 15),
                Size = new Size(920, 80),
                BackColor = Color.White
            };
            panelSearch.Paint += DrawBorder;
            this.Controls.Add(panelSearch);

            txtTenSP = CreateTextBox("Tên sản phẩm", 20, 22, 220);

            cboLoaiSP = new ComboBox()
            {
                Location = new Point(260, 22),
                Size = new Size(220, 36),
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboLoaiSP.Items.AddRange(new string[]
            {
                "Tất cả", "Thuốc", "Thức ăn", "Phụ kiện"
            });
            cboLoaiSP.SelectedIndex = 0;

            btnTimKiem = new Button()
            {
                Text = "🔍 Tìm kiếm",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(150, 36),
                Location = new Point(740, 22),
                BackColor = Color.FromArgb(220, 70, 70),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnTimKiem.FlatAppearance.BorderSize = 0;

            btnTimKiem.Click += (s, e) =>
            {
                string ten = txtTenSP.Text.Trim().ToLower();
                string loai = cboLoaiSP.Text.Trim();

                foreach (DataGridViewRow row in dgvSanPham.Rows)
                {
                    if (row.IsNewRow) continue;

                    string tenSP = row.Cells["TENSP"].Value.ToString().ToLower();
                    string loaiSP = row.Cells["LOAI"].Value.ToString().Trim();

                    bool matchTen =
                        ten == "" || ten == "tên sản phẩm" || tenSP.Contains(ten);

                    bool matchLoai =
                        loai == "Tất cả" || loaiSP.Equals(loai, StringComparison.OrdinalIgnoreCase);

                    row.Visible = matchTen && matchLoai;
                }
            };


            panelSearch.Controls.Add(txtTenSP);
            panelSearch.Controls.Add(cboLoaiSP);
            panelSearch.Controls.Add(btnTimKiem);

            // ===== RESULT =====
            panelResult = new Panel()
            {
                Location = new Point(30, panelSearch.Bottom + 15),
                Size = new Size(920, 270),
                BackColor = Color.White
            };
            panelResult.Paint += DrawBorder;
            this.Controls.Add(panelResult);

            dgvSanPham = new DataGridView()
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                //ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false
            };

            dgvSanPham.Columns.Add("MASP", "Mã SP");
            dgvSanPham.Columns.Add("TENSP", "Tên sản phẩm");
            dgvSanPham.Columns.Add("LOAI", "Loại");
            dgvSanPham.Columns.Add("GIA", "Giá bán");

            DataGridViewComboBoxColumn colCN = new DataGridViewComboBoxColumn();
            colCN.Name = "CHINHANH";
            colCN.HeaderText = "Chi nhánh";
            dgvSanPham.Columns.Add(colCN);

            dgvSanPham.Columns.Add("TON", "Tồn kho");

            dgvSanPham.Columns["MASP"].Visible = false;
            dgvSanPham.Columns["GIA"].DefaultCellStyle.Format = "N0";
            dgvSanPham.Columns["MASP"].ReadOnly = true;
            dgvSanPham.Columns["TENSP"].ReadOnly = true;
            dgvSanPham.Columns["LOAI"].ReadOnly = true;
            dgvSanPham.Columns["GIA"].ReadOnly = true;
            dgvSanPham.Columns["TON"].ReadOnly = true;

            // ✅ CHỈ CHO EDIT CHI NHÁNH
            dgvSanPham.Columns["CHINHANH"].ReadOnly = false;

            panelResult.Controls.Add(dgvSanPham);

            // ===== ACTION =====
            panelAction = new Panel()
            {
                Location = new Point(30, panelResult.Bottom + 15),
                Size = new Size(920, 80),
                BackColor = Color.White
            };
            panelAction.Paint += DrawBorder;
            this.Controls.Add(panelAction);

            Label lblSL = new Label()
            {
                Text = "Số lượng:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(30, 28),
                AutoSize = true
            };

            nudSoLuong = new NumericUpDown()
            {
                Location = new Point(130, 30),
                Size = new Size(80, 30),
                Minimum = 1,
                Maximum = 100,
                Font = new Font("Segoe UI", 11)
            };

            btnThemGio = new Button()
            {
                Text = "🛒 Thêm vào giỏ",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(180, 40),
                Location = new Point(710, 20),
                BackColor = Color.FromArgb(220, 70, 70),
                ForeColor = Color.White
            };
            btnThemGio.Click += (s, e) =>
            {
                if (dgvSanPham.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn sản phẩm");
                    return;
                }

                DataGridViewRow row = dgvSanPham.SelectedRows[0];

                int maSP = Convert.ToInt32(row.Cells["MASP"].Value);
                string tenSP = row.Cells["TENSP"].Value.ToString();
                decimal gia = Convert.ToDecimal(row.Cells["GIA"].Value);
                int soLuongThem = (int)nudSoLuong.Value;

                // ===== LẤY CHI NHÁNH =====
                if (row.Cells["CHINHANH"].Value == null)
                {
                    MessageBox.Show("Vui lòng chọn chi nhánh");
                    return;
                }

                string displayCN = row.Cells["CHINHANH"].Value.ToString();
                // format: "Chi nhánh Q1 (CN01)"
                int idx = displayCN.LastIndexOf("(");
                if (idx < 0)
                {
                    MessageBox.Show("Chi nhánh không hợp lệ");
                    return;
                }

                string tenCN = displayCN;
                string maCN = displayCN.Substring(idx + 1)
                                        .Replace(")", "")
                                        .Trim();

                // ===== KIỂM TRA TỒN =====
                int ton = Convert.ToInt32(row.Cells["TON"].Value);
                if (soLuongThem > ton)
                {
                    MessageBox.Show(
                        $"Sản phẩm chỉ còn {ton} đơn vị",
                        "Không đủ tồn kho",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // ===== TÌM TRONG GIỎ (MASP + MACN) =====
                GioHangItem item = GioHangData.Items
                    .Find(x => x.MaSP == maSP && x.MaCN == maCN);

                if (item != null)
                {
                    // cộng dồn
                    if (item.SoLuong + soLuongThem > ton)
                    {
                        MessageBox.Show(
                            $"Tổng số lượng vượt quá tồn kho ({ton})",
                            "Không đủ tồn kho",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }

                    item.SoLuong += soLuongThem;
                }
                else
                {
                    GioHangData.Items.Add(new GioHangItem
                    {
                        MaSP = maSP,
                        TenSP = tenSP,
                        MaCN = maCN,
                        TenCN = tenCN,
                        Gia = gia,
                        SoLuong = soLuongThem
                    });
                }

                MessageBox.Show("Đã thêm vào giỏ hàng ✔️");
            };
            btnXemGio = new Button()
            {
                Text = "🧺 Xem giỏ hàng",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(180, 40),
                Location = new Point(520, 20),
                BackColor = Color.FromArgb(200, 200, 200),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat
            };
            btnXemGio.FlatAppearance.BorderSize = 0;

            btnXemGio.Click += (s, e) =>
            {
                if (GioHangData.Items.Count == 0)
                {
                    MessageBox.Show("Giỏ hàng đang trống");
                    return;
                }
                new FormGioHang().ShowDialog();
            };

            panelAction.Controls.Add(btnXemGio);
            panelAction.Controls.Add(lblSL);
            panelAction.Controls.Add(nudSoLuong);
            panelAction.Controls.Add(btnThemGio);
        }

        // ================= ĐỔI CHI NHÁNH =================
        void HookChiNhanhChange()
        {
            dgvSanPham.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (dgvSanPham.IsCurrentCellDirty)
                    dgvSanPham.CommitEdit(
                        DataGridViewDataErrorContexts.Commit);
            };

            dgvSanPham.CellValueChanged += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                if (dgvSanPham.Columns[e.ColumnIndex].Name != "CHINHANH")
                    return;

                DataGridViewRow row = dgvSanPham.Rows[e.RowIndex];

                string masp = row.Cells["MASP"].Value.ToString();
                string display = row.Cells["CHINHANH"].Value.ToString();
                string macn = display.Substring(
                    display.LastIndexOf("(") + 1, 4);

                Tuple<string, string> key =
                    new Tuple<string, string>(masp, macn);

                row.Cells["TON"].Value =
                    TonKhoTheoChiNhanh.ContainsKey(key)
                    ? TonKhoTheoChiNhanh[key]
                    : 0;
            };
        }

        TextBox CreateTextBox(string placeholder, int x, int y, int w)
        {
            TextBox t = new TextBox()
            {
                Text = placeholder,
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 11),
                Location = new Point(x, y),
                Size = new Size(w, 36)
            };

            t.Enter += (s, e) =>
            {
                if (t.Text == placeholder)
                {
                    t.Text = "";
                    t.ForeColor = Color.Black;
                }
            };

            t.Leave += (s, e) =>
            {
                if (t.Text == "")
                {
                    t.Text = placeholder;
                    t.ForeColor = Color.Gray;
                }
            };
            return t;
        }

        void DrawBorder(object sender, PaintEventArgs e)
        {
            using (Pen pen = new Pen(Color.FromArgb(220, 70, 70), 2))
            {
                e.Graphics.DrawRectangle(
                    pen, 0, 0,
                    ((Control)sender).Width - 1,
                    ((Control)sender).Height - 1);
            }
        }
    }
}
