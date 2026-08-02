
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PETCAREX
{
    public partial class FormTraCuuBS : Form
    {
        ComboBox cboDichVu, cboChiNhanh, cboGio;
        DateTimePicker dtpNgay;
        FlowLayoutPanel flowBacSi;
        Panel panelFilter;
        Panel panelScroll;
        DataTable _dtChiNhanh;

        string connectionString =
            "Data Source=.;Initial Catalog=QLTC;Integrated Security=True";

        public FormTraCuuBS()
        {
            InitializeComponent();
            InitUI();
            LoadChiNhanh(); // 🔥 LOAD CHI NHÁNH TỪ DB
        }

        // ================= INIT UI =================
        void InitUI()
        {
            //this.BackColor = Color.FromArgb(255, 245, 245);
            //this.ClientSize = new Size(1000, 650);
            //this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(255, 245, 245);
            this.Padding = new Padding(10);

            panelFilter = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 150,
                Padding = new Padding(10),
                BackColor = Color.Transparent
            };
            this.Controls.Add(panelFilter);

            Label lblTitle = new Label()
            {
                Text = "Tra cứu lịch bác sĩ",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 70, 70),
                AutoSize = true,
                //Location = new Point((this.ClientSize.Width - 300) / 2 - 70, 20)
                Location = new Point(330, 0)
            };
            panelFilter.Controls.Add(lblTitle);

            int y = 60;

            panelFilter.Controls.Add(CreateLabel("Dịch vụ", 10, y -15));
            cboDichVu = CreateCombo(Array.Empty<string>(), 10, y + 10);
            cboDichVu.Width = 120;
            cboDichVu.Items.Add("Chọn dịch vụ");
            cboDichVu.Items.Add("Khám bệnh");
            cboDichVu.Items.Add("Tiêm phòng");
            cboDichVu.SelectedIndex = 0;

            cboDichVu.SelectedIndexChanged += (s, e) =>
            {
                if (cboDichVu.SelectedIndex > 0)
                    FilterChiNhanhTheoDichVu();
            };


            panelFilter.Controls.Add(cboDichVu);

            panelFilter.Controls.Add(CreateLabel("Chi nhánh", 160, y - 15));
            cboChiNhanh = CreateCombo(Array.Empty<string>(), 160, y + 10);
            cboChiNhanh.Width = 295;
            cboChiNhanh.DropDownStyle = ComboBoxStyle.DropDownList;
            panelFilter.Controls.Add(cboChiNhanh);

            panelFilter.Controls.Add(CreateLabel("Ngày", 488, y - 15));
            dtpNgay = new DateTimePicker()
            {
                Location = new Point(490, y + 10),
                Size = new Size(150, 32),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy",
                Font = new Font("Segoe UI", 10.5f),
                MinDate = DateTime.Today,
                Value = DateTime.Today
            };
            // ===== AUTO CHUYỂN SANG NGÀY MAI NẾU ĐÃ QUA 22H =====
            TimeSpan nowTime = DateTime.Now.TimeOfDay;
            TimeSpan endTime = new TimeSpan(22, 0, 0);

            if (nowTime > endTime)
            {
                dtpNgay.MinDate = DateTime.Today.AddDays(1);
                dtpNgay.Value = DateTime.Today.AddDays(1);
            }
            else
            {
                dtpNgay.MinDate = DateTime.Today;
                dtpNgay.Value = DateTime.Today;
            }
            panelFilter.Controls.Add(dtpNgay);

            panelFilter.Controls.Add(CreateLabel("Giờ", 670, y - 15));
            cboGio = CreateCombo(Array.Empty<string>(), 670, y + 10);
            cboGio.Items.Add("Chọn giờ");

            foreach (int h in Enumerable.Range(8, 15))
            {
                cboGio.Items.Add($"{h:00}:00");
            }

            cboGio.SelectedIndex = 0;
            panelFilter.Controls.Add(cboGio);

            Button btnTraCuu = new Button()
            {
                Text = "🔍 TRA CỨU",
                Location = new Point(10, y + 45),
                Size = new Size(120, 38),
                BackColor = Color.FromArgb(255, 90, 90),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnTraCuu.FlatAppearance.BorderSize = 0;
            btnTraCuu.Click += (s, e) => LoadBacSiFromDB();
            panelFilter.Controls.Add(btnTraCuu);

            //Panel divider = new Panel()
            //{
            //    Location = new Point(20, panelFilter.Bottom + 3),
            //    Size = new Size(this.ClientSize.Width - 40, 1),
            //    BackColor = Color.FromArgb(235, 200, 200),
            //    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            //};
            //this.Controls.Add(divider);

            //panelScroll = new Panel()
            //{
            //    Dock = DockStyle.Fill,
            //    AutoScroll = true,
            //    Padding = new Padding(0),
            //    BackColor = Color.Transparent
            //};
            //this.Controls.Add(panelScroll);

            //flowBacSi = new FlowLayoutPanel()
            //{
            //    Dock = DockStyle.Top,
            //    AutoSize = true,
            //    FlowDirection = FlowDirection.TopDown,
            //    WrapContents = false,
            //    Padding = new Padding(10),
            //    Margin = new Padding(0)
            //};


            //panelScroll.Resize += (s, e) =>
            //    flowBacSi.Width = panelScroll.ClientSize.Width;

            //panelScroll.Controls.Add(flowBacSi);
            // ===== DIVIDER =====
            Panel divider = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 1,
                BackColor = Color.FromArgb(235, 200, 200)
            };
            //this.Controls.Add(divider);

            // ===== PANEL SCROLL =====
            panelScroll = new Panel()
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(0),
                BackColor = Color.Transparent
            };
            //this.Controls.Add(panelScroll);

            // ===== FLOW BÁC SĨ =====
            flowBacSi = new FlowLayoutPanel()
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(10),
                Margin = new Padding(0)
            };
            this.Controls.Add(panelScroll);   // add trước
            this.Controls.Add(divider);       // add sau
            this.Controls.Add(panelFilter);   // add cuối (trên cùng)

            panelScroll.Resize += (s, e) =>
                flowBacSi.Width = panelScroll.ClientSize.Width;

            panelScroll.Controls.Add(flowBacSi);

        }

        // ================= LOAD CHI NHÁNH =================
        void LoadChiNhanh()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_LAY_DS_CHI_NHANH", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                _dtChiNhanh = new DataTable();

                conn.Open();
                _dtChiNhanh.Load(cmd.ExecuteReader());

                // 🔥 THÊM CỘT HIỂN THỊ
                if (!_dtChiNhanh.Columns.Contains("TEN_HIENTHI"))
                    _dtChiNhanh.Columns.Add("TEN_HIENTHI", typeof(string));

                foreach (DataRow row in _dtChiNhanh.Rows)
                {
                    row["TEN_HIENTHI"] =
                        $"{row["TENCN"]} – {row["DIACHI"]}";
                }

                // 🔥 THÊM DÒNG PLACEHOLDER "CHỌN CHI NHÁNH"
                DataRow r = _dtChiNhanh.NewRow();

                r["MACN"] = "";          // value rỗng để biết là placeholder
                r["TENCN"] = "";
                r["DIACHI"] = "";

                // ⚠️ BẮT BUỘC GÁN GIÁ TRỊ HỢP LỆ (KHÔNG ĐƯỢC NULL)
                r["TGMO"] = new TimeSpan(0, 0, 0);
                r["TGDONG"] = new TimeSpan(0, 0, 0);

                // 👀 CỘT DÙNG ĐỂ HIỂN THỊ
                r["TEN_HIENTHI"] = "Chọn chi nhánh";

                // 👉 INSERT 1 LẦN DUY NHẤT
                _dtChiNhanh.Rows.InsertAt(r, 0);

                // ===== BIND COMBOBOX =====
                cboChiNhanh.DataSource = _dtChiNhanh;
                cboChiNhanh.DisplayMember = "TEN_HIENTHI"; // 👈 HIỂN THỊ
                cboChiNhanh.ValueMember = "MACN";        // 👈 DÙNG SQL
                cboChiNhanh.SelectedIndex = 0;

            }
        }





        // ================= LOAD BÁC SĨ =================

        void LoadBacSiFromDB()
        {
            if (cboDichVu.SelectedIndex == 0 ||
                cboChiNhanh.SelectedValue == null ||
                cboGio.SelectedIndex == 0)
            {
                MessageBox.Show(
                    "Vui lòng chọn đầy đủ dịch vụ, chi nhánh và giờ.",
                    "Thiếu thông tin",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            flowBacSi.Controls.Clear();
            panelScroll.AutoScrollPosition = new Point(0, 0);

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_LAYBACSI", conn))   // 👈 GỌI ĐÚNG PROC
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@MaCN", cboChiNhanh.SelectedValue.ToString());
                cmd.Parameters.AddWithValue("@NgayHen", dtpNgay.Value.Date);
                cmd.Parameters.AddWithValue("@GioHen", TimeSpan.Parse(cboGio.Text));

                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    if (!rd.HasRows)
                    {
                        ShowEmpty();
                        return;
                    }

                    while (rd.Read())
                    {
                        AddBacSiCard(
                            rd["HOTEN"].ToString(),
                            rd["CALAMVIEC"].ToString()
                        );
                    }

                    flowBacSi.PerformLayout();
                    panelScroll.AutoScrollMinSize = new Size(0, flowBacSi.PreferredSize.Height + 20);

                }
            }
        }

        // ================= UI CARD =================
        void AddBacSiCard(string tenBS, string ca)
        {
            Panel card = new Panel()
            {
                Size = new Size(panelScroll.ClientSize.Width - 30, 60),
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 12)
            };

            card.Paint += (s, e) =>
            {
                using (Pen pen = new Pen(Color.FromArgb(230, 190, 190)))
                    e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
            };

            card.Controls.Add(new Label()
            {
                Text = "🩺 " + tenBS,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(180, 60, 60),
                Location = new Point(16, 10),
                AutoSize = true
            });

            card.Controls.Add(new Label()
            {
                Text = "Ca làm việc: " + ca,
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = Color.Gray,
                Location = new Point(16, 32),
                AutoSize = true
            });

            flowBacSi.Controls.Add(card);
        }

        void ShowEmpty()
        {
            flowBacSi.Controls.Add(new Label()
            {
                Text = "❌ Không có bác sĩ rảnh khung giờ này",
                Font = new Font("Segoe UI", 11, FontStyle.Italic),
                ForeColor = Color.Gray,
                AutoSize = true,
                Padding = new Padding(10)
            });
        }

        // ================= UTIL =================
        ComboBox CreateCombo(string[] data, int x, int y)
        {
            ComboBox c = new ComboBox()
            {
                Location = new Point(x, y),
                Size = new Size(200, 32),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            c.Items.AddRange(data);
            return c;
        }


        Label CreateLabel(string text, int x, int y)
        {
            return new Label()
            {
                Text = text,
                Location = new Point(x, y),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(150, 60, 60),
                AutoSize = true
            };
        }
        bool ChiNhanhCoDichVu(string macn)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand("SP_LAY_DV_CHI_NHANH", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MACN", macn);

                conn.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    if (!rd.Read()) return false;

                    if (cboDichVu.Text == "Khám bệnh")
                        return rd.GetInt32(rd.GetOrdinal("CO_KHAM")) == 1;

                    if (cboDichVu.Text == "Tiêm phòng")
                        return rd.GetInt32(rd.GetOrdinal("CO_TIEM")) == 1;
                }
            }
            return false;
        }

        void FilterChiNhanhTheoDichVu()
        {
            if (_dtChiNhanh == null) return;

            DataTable filtered = _dtChiNhanh.Clone();

            foreach (DataRow row in _dtChiNhanh.Rows)
            {
                string macn = row["MACN"].ToString();

                if (ChiNhanhCoDichVu(macn))
                    filtered.ImportRow(row);
            }

            cboChiNhanh.DataSource = filtered;
        }

    }
}
