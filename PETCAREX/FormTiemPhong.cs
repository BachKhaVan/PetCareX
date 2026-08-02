using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;


namespace PETCAREX
{
    public partial class FormTiemPhong : Form
    {
        DataTable tempPetTable = new DataTable();
        DataRow tempPetData = null;
        void InitTempPetTable()
        {
            tempPetTable.Columns.Add("TENTC", typeof(string));
            tempPetTable.Columns.Add("LOAI", typeof(string));
            tempPetTable.Columns.Add("GIONG", typeof(string));
            tempPetTable.Columns.Add("NGAYSINH", typeof(DateTime));
            tempPetTable.Columns.Add("GIOITINH", typeof(string));
            tempPetTable.Columns.Add("MOTA", typeof(string));
        }
        RadioButton selectedDoctorRB = null;
        Panel selectedDoctorPanel = null;

        public Action OnBackToMain { get; set; }

        string strCon = @"Data Source=.;Initial Catalog=QLTC;Integrated Security=True";
        string selectedMaCN;

        // ===== LOGIC =====
        int currentMaTC = -1;
        int selectedMaBS = -1;
        int selectedMaTP = -1;

        bool isAddingNewPet = false;


        string loaiTiem = ""; // LE | GOI
        int soThangGoi = 0;

        // ===== UI =====
        Panel panelMain, panelLeft, panelRight;
        FlowLayoutPanel flowPetList, flowDoctors, flowVaccines;

        TextBox txtPhone;
        Button btnSearchPhone, btnAddPetBottom;

        DateTimePicker dtpDate;
        ComboBox cboHour;

        public FormTiemPhong(string maCN)
        {
            InitializeComponent();
            InitTempPetTable();
            selectedMaCN = maCN;
            InitUI();
            ShowPetSelection();
        }

        bool IsValidPhone(string phone)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(phone, @"^[0-9]{10}$");
        }

        void InitUI()
        {
            Text = "ĐĂNG KÝ TIÊM PHÒNG";
            Size = new Size(1150, 650);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(255, 242, 242);
        }

        // ================= BƯỚC 1: SĐT + THÚ CƯNG (Y CHANG KHÁM BỆNH) =================
        void ShowPetSelection()
        {
            panelMain = new Panel()
            {
                Size = new Size(1050, 550),
                Location = new Point(50, 30)
            };
            Controls.Add(panelMain);

            panelLeft = new Panel()
            {
                Size = new Size(400, 450),
                BackColor = Color.White
            };

            panelRight = new Panel()
            {
                Size = new Size(480, 500),
                Location = new Point(420, 0),
                Visible = false
            };

            Label lblTitle = new Label()
            {
                Text = "ĐĂNG KÝ TIÊM PHÒNG",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 90, 90),
                Location = new Point(30, 20),
                AutoSize = true
            };

            txtPhone = new TextBox()
            {
                Text = "Nhập SĐT khách hàng...",
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 13),
                Location = new Point(30, 80),
                Width = 230
            };

            txtPhone.Enter += (s, e) =>
            {
                if (txtPhone.Text.Contains("Nhập"))
                {
                    txtPhone.Text = "";
                    txtPhone.ForeColor = Color.Black;
                }
            };

            txtPhone.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtPhone.Text))
                {
                    txtPhone.Text = "Nhập SĐT khách hàng...";
                    txtPhone.ForeColor = Color.Gray;
                }
            };

            btnSearchPhone = new Button()
            {
                Text = "Tìm kiếm",
                Location = new Point(270, 78),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(255, 107, 107),
                ForeColor = Color.White
            };

            btnSearchPhone.Click += (s, e) =>
            {
                string phone = txtPhone.Text.Trim();

                if (!IsValidPhone(phone))
                {
                    MessageBox.Show("Số điện thoại không hợp lệ!\n(Phải là 10 chữ số và không chứa ký tự chữ)",
                                    "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPhone.Text = ""; // Xóa để nhập lại
                    txtPhone.Focus();
                    return;
                }

                using (SqlConnection conn = new SqlConnection(strCon))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(
                        "SELECT COUNT(*) FROM KHACH_HANG WHERE SDT=@sdt", conn);
                    cmd.Parameters.AddWithValue("@sdt", phone);
                    if ((int)cmd.ExecuteScalar() == 0)
                    {
                        MessageBox.Show("Không có dữ liệu khách hàng khách hàng!");
                        return;
                    }
                }

                LoadPetsByPhone(phone);
            };

            flowPetList = new FlowLayoutPanel()
            {
                Location = new Point(30, 120),
                Size = new Size(340, 250),

                FlowDirection = FlowDirection.TopDown, // ⬅️ xếp dọc
                WrapContents = false,                  // ⬅️ CẤM KÉO NGANG
                AutoScroll = true,

                //BorderStyle = BorderStyle.FixedSingle
            };


            btnAddPetBottom = new Button()
            {
                Text = "➕ Thêm bé mới",
                Location = new Point(25, 380),
                Size = new Size(330, 45),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(255, 90, 90),
                Font = new Font("Segoe UI", 11, FontStyle.Italic),
                Visible = false
            };

            btnAddPetBottom.Click += (s, e) => {
                string phone = txtPhone.Text.Trim();
                if (phone != "Nhập SĐT khách hàng...") ShowAddPetForm(phone);
            };

            panelLeft.Controls.AddRange(new Control[]
            {
                lblTitle, txtPhone, btnSearchPhone, flowPetList, btnAddPetBottom
            });

            panelMain.Controls.Add(panelLeft);
            panelMain.Controls.Add(panelRight);
        }

        void LoadPetsByPhone(string sdt)
        {
            flowPetList.Controls.Clear();
            panelRight.Visible = false;

            using (SqlConnection conn = new SqlConnection(strCon))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SP_LAYTHUCUNG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SDT", sdt);

                SqlDataReader r = cmd.ExecuteReader();
                bool hasPet = false;

                while (r.Read())
                {
                    hasPet = true;
                    Button btn = new Button()
                    {
                        Text = r["TENTC"].ToString(),
                        Tag = r["MATC"],
                        Size = new Size(320, 45),
                        BackColor = Color.White
                    };

                    btn.Click += (s, e) =>
                    {
                        currentMaTC = Convert.ToInt32(btn.Tag);
                        foreach (Control c in flowPetList.Controls)
                            c.BackColor = Color.White;
                        btn.BackColor = Color.MistyRose;
                        ShowLoaiTiem();
                    };

                    flowPetList.Controls.Add(btn);
                }
                // Chỉ hiện nút thêm thú cưng khi KHÔNG ở chế độ thêm mới
                btnAddPetBottom.Visible = !isAddingNewPet;

                if (!hasPet && !isAddingNewPet)
                    ShowAddPetForm(sdt);
            }
        }

        // ================= BƯỚC 2: LOẠI TIÊM =================
        void ShowLoaiTiem()
        {
            panelRight.Controls.Clear();
            panelRight.Visible = true;
            panelRight.BringToFront();
            btnAddPetBottom.Visible = !isAddingNewPet;
            Label lbl = new Label()
            {
                Text = "CHỌN LOẠI TIÊM",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 107, 107), // Màu hồng chủ đạo
                Location = new Point(20, 20),
                AutoSize = true
            };
            Button btnLe = new Button()
            {
                Text = "TIÊM LẺ",
                Location = new Point(20, 80),
                Size = new Size(160, 60),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(255, 107, 107),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            btnLe.Click += (s, e) =>
            {
                loaiTiem = "LE";
                soThangGoi = 0;
                ShowVaccineBookingScreen();
            };

            // Thiết kế nút Tiêm Gói chuyên nghiệp hơn
            Button btnGoi = new Button()
            {
                Text = "TIÊM GÓI",
                Location = new Point(190, 80),
                Size = new Size(160, 60),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(255, 107, 107),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnGoi.Click += (s, e) => ShowChonGoi();

            panelRight.Controls.AddRange(new Control[] { lbl, btnLe, btnGoi });
        }

        void ShowChonGoi()
        {
            panelRight.Controls.Clear();

            // Thêm tiêu đề để không bị trống trải
            Label lbl = new Label()
            {
                Text = "CHỌN GÓI TIÊM",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 70, 70),         // đỏ tươi nổi bật
                Location = new Point(20, 20),
                AutoSize = true
            };
            panelRight.Controls.Add(lbl);
            int[] goi = { 3, 6, 12 };
            int[] giam = { 5, 10, 15 }; // giảm 5%, 10%, 15%

            for (int i = 0; i < goi.Length; i++)
            {
                int thang = goi[i];
                int phanTramGiam = giam[i];
                Button b = new Button()
                {
                    Text = $"{thang} tháng - giảm {phanTramGiam}%",

                    // Đổi i * 180 ở trục X thành i * 60 ở trục Y để xếp dọc
                    Location = new Point(30, 80 + i * 60),
                    Size = new Size(250, 45), // Tăng chiều rộng nút cho đẹp
                    BackColor = Color.FromArgb(255, 180, 180),
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    ForeColor = Color.FromArgb(220, 70, 70)
                };

                b.MouseEnter += (s, e) =>
                {
                    b.BackColor = Color.FromArgb(220, 70, 70);
                    b.ForeColor = Color.White;
                };
                b.MouseLeave += (s, e) =>
                {
                    b.BackColor = Color.FromArgb(255, 180, 180);
                    b.ForeColor = Color.FromArgb(220, 70, 70);
                };
                b.Click += (s, e) =>
                {
                    loaiTiem = "GOI";
                    soThangGoi = thang;
                    ShowVaccineBookingScreen();
                };
                panelRight.Controls.Add(b);
            }
        }

        // ================= BƯỚC 3: VACCINE + THỜI GIAN + BS =================




        // ================= BƯỚC 4: GIỜ + BÁC SĨ (Y CHANG KHÁM BỆNH) ================

        void LoadDoctors()
        {
            if (cboHour.SelectedItem == null) return;
            flowDoctors.Controls.Clear();
            selectedMaBS = -1;

            using (SqlConnection conn = new SqlConnection(strCon))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("sp_LAYBACSI", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MaCN", selectedMaCN);
                cmd.Parameters.AddWithValue("@NgayHen", dtpDate.Value.Date);
                cmd.Parameters.AddWithValue("@GioHen", TimeSpan.Parse(cboHour.Text));

                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    Panel p = new Panel()
                    {
                        Height = 45,
                        Width = flowDoctors.ClientSize.Width - 25, // ⬅️ trừ scrollbar
                        BackColor = Color.White,
                        Margin = new Padding(3),
                        Cursor = Cursors.Hand
                    };

                    RadioButton rb = new RadioButton()
                    {
                        Text = "BS. " + r["HOTEN"],
                        Tag = r["MANS"],
                        Font = new Font("Segoe UI", 11), // Thêm font cho đẹp
                        Location = new Point(10, 5),    // Chỉnh lại tọa Y cho cân đối giữa Panel
                        AutoSize = true,                // QUAN TRỌNG: Để nó tự giãn theo tên dài ngắn
                        Cursor = Cursors.Hand
                    };

                    rb.CheckedChanged += (s, e) =>
                    {
                        if (!rb.Checked) return;

                        // ❌ bỏ chọn bác sĩ cũ
                        if (selectedDoctorRB != null)
                            selectedDoctorRB.Checked = false;

                        if (selectedDoctorPanel != null)
                            selectedDoctorPanel.BackColor = Color.White;

                        // ✅ chọn bác sĩ mới
                        rb.Checked = true;
                        selectedDoctorRB = rb;
                        selectedDoctorPanel = p;
                        selectedMaBS = Convert.ToInt32(rb.Tag);

                        p.BackColor = Color.MistyRose;
                    };

                    p.Controls.Add(rb);
                    flowDoctors.Controls.Add(p);
                }
            }
        }
        RadioButton selectedVaccineRB = null;
        Panel selectedVaccinePanel = null;
        void UpdateAvailableHours()
        {
            cboHour.Items.Clear();
            DateTime now = DateTime.Now;
            DateTime d = dtpDate.Value.Date;

            for (int h = 8; h <= 22; h++)
            {
                if (d > now.Date || (d == now.Date && (h > now.Hour || (h == now.Hour && now.Minute <= 15))))
                    cboHour.Items.Add($"{h:D2}:00");
            }

            if (cboHour.Items.Count > 0)
                cboHour.SelectedIndex = 0;
        }

        // ================= FINAL =================
        void FinalizeBooking()
        {
            if (selectedMaBS == -1 || selectedMaTP == -1)
            {
                MessageBox.Show("Thiếu thông tin!");
                return;
            }

            if (MessageBox.Show("Xác nhận đặt lịch tiêm?", "Xác nhận",
                MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            using (SqlConnection conn = new SqlConnection(strCon))
            {
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    int maTC = currentMaTC;
                    if (isAddingNewPet)
                        maTC = ExecuteAddPet(conn, tran);

                    SqlCommand cmd = new SqlCommand("SP_DATLICH", conn, tran);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MaTC", maTC);
                    cmd.Parameters.AddWithValue("@MaDV", "TP");
                    cmd.Parameters.AddWithValue("@MaBS", selectedMaBS);
                    cmd.Parameters.AddWithValue("@ThoiGianHen",
                        dtpDate.Value.Date + TimeSpan.Parse(cboHour.Text));
                    cmd.Parameters.AddWithValue("@MaTP", selectedMaTP);

                    cmd.ExecuteNonQuery();
                    tran.Commit();

                    MessageBox.Show("Đặt lịch tiêm thành công!");
                    OnBackToMain?.Invoke();
                    Close();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    MessageBox.Show(ex.Message);
                }
            }
        }
        void SetPlaceholder(TextBox txt, string text)
        {
            txt.Text = text;
            txt.ForeColor = Color.Gray;

            txt.Enter += (s, e) =>
            {
                if (txt.ForeColor == Color.Gray)
                {
                    txt.Text = "";
                    txt.ForeColor = Color.Black;
                }
            };

            txt.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txt.Text))
                {
                    txt.Text = text;
                    txt.ForeColor = Color.Gray;
                }
            };
        }

        int ExecuteAddPet(SqlConnection conn, SqlTransaction tran)
        {
            SqlCommand cmd = new SqlCommand("SP_THEMTHUCUNG", conn, tran);
            cmd.CommandType = CommandType.StoredProcedure; // 🔥 BẮT BUỘC

            cmd.Parameters.AddWithValue("@TenTC", tempPetData["TENTC"]);
            cmd.Parameters.AddWithValue("@LoaiTC", tempPetData["LOAI"]);
            cmd.Parameters.AddWithValue("@GiongTC", tempPetData["GIONG"]);
            cmd.Parameters.AddWithValue("@NgaySinh", tempPetData["NGAYSINH"]);
            cmd.Parameters.AddWithValue("@GioiTinh", tempPetData["GIOITINH"]);
            cmd.Parameters.AddWithValue("@TinhTrangSK", tempPetData["MOTA"]);

            cmd.Parameters.AddWithValue("@SDT", txtPhone.Text.Trim());

            SqlParameter outParam = new SqlParameter("@MaTC_Moi", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(outParam);
            cmd.ExecuteNonQuery();

            return (int)outParam.Value;
        }

        void ShowAddPetForm(string sdt)
        {
            isAddingNewPet = true;
            currentMaTC = -1;

            btnAddPetBottom.Visible = false;

            btnAddPetBottom.Visible = false;
            // THÊM DÒNG NÀY: Khóa danh sách bên trái
            flowPetList.Enabled = false;
            panelRight.Controls.Clear();
            panelRight.Visible = true;
            panelRight.BackColor = Color.White;


            Label lblTitle = new Label()
            {
                Text = "THÊM THÚ CƯNG MỚI",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 53, 69),
                Location = new Point(0, 15),
                AutoSize = true
            };
            panelRight.Controls.Add(lblTitle);

            // ===== TEXTBOX =====
            TextBox txtTen = new TextBox() { Name = "txtTen", Location = new Point(120, 70), Width = 200 };
            TextBox txtLoai = new TextBox() { Name = "txtLoai", Location = new Point(120, 115), Width = 200 };
            TextBox txtGiong = new TextBox() { Name = "txtGiong", Location = new Point(120, 160), Width = 200 };
            TextBox txtMoTa = new TextBox() { Name = "txtMoTa", Location = new Point(120, 295), Width = 200 };

            txtTen.MaxLength = 20;
            txtLoai.MaxLength = 20;
            txtGiong.MaxLength = 50;
            txtMoTa.MaxLength = 100;

            SetPlaceholder(txtTen, "Tên thú cưng");
            SetPlaceholder(txtLoai, "Loài");
            SetPlaceholder(txtGiong, "Giống");
            SetPlaceholder(txtMoTa, "Mô tả");

            panelRight.Controls.AddRange(new Control[] { txtTen, txtLoai, txtGiong, txtMoTa });

            // ===== LABEL =====
            string[] lbls = { "Tên:", "Loài:", "Giống:", "Ngày sinh:", "Giới tính:", "Mô tả:" };
            int[] ys = { 70, 115, 160, 205, 250, 295 };

            for (int i = 0; i < lbls.Length; i++)
            {
                panelRight.Controls.Add(new Label()
                {
                    Text = lbls[i],
                    Location = new Point(40, ys[i] + 5),
                    AutoSize = true
                });
            }

            // ===== NGÀY SINH =====
            DateTimePicker dtpNS = new DateTimePicker()
            {
                Name = "dtpNgaySinh",
                Location = new Point(120, 205),
                Format = DateTimePickerFormat.Short,
                MaxDate = DateTime.Now
            };
            panelRight.Controls.Add(dtpNS);

            // ===== GIỚI TÍNH =====
            FlowLayoutPanel pnlGender = new FlowLayoutPanel()
            {
                Name = "pnlGender",
                Location = new Point(150, 245),
                Size = new Size(250, 35)
            };
            pnlGender.Controls.Add(new RadioButton() { Text = "Đực", Checked = true });
            pnlGender.Controls.Add(new RadioButton() { Text = "Cái" });
            panelRight.Controls.Add(pnlGender);

            // ===== SAVE =====
            Button btnSave = new Button()
            {
                Text = "LƯU THÚ CƯNG",
                Size = new Size(250, 45),
                Location = new Point(100, 360),
                BackColor = Color.FromArgb(255, 90, 90),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnSave.Click += (s, e) =>
            {
                if (txtTen.ForeColor == Color.Gray || txtLoai.ForeColor == Color.Gray)
                {
                    MessageBox.Show("Tên và loài không được bỏ trống");
                    return;
                }

                tempPetTable.Rows.Clear();
                DataRow r = tempPetTable.NewRow();

                r["TENTC"] = txtTen.Text.Trim();
                r["LOAI"] = txtLoai.Text.Trim();
                r["GIONG"] = txtGiong.ForeColor == Color.Gray ? "" : txtGiong.Text.Trim();
                r["NGAYSINH"] = dtpNS.Value;
                r["GIOITINH"] = (pnlGender.Controls[1] as RadioButton).Checked ? "Cái" : "Đực";
                r["MOTA"] = txtMoTa.ForeColor == Color.Gray ? "" : txtMoTa.Text.Trim();

                tempPetTable.Rows.Add(r);
                tempPetData = r;

                DisplayTempPetInList(r["TENTC"].ToString());
                btnAddPetBottom.Visible = true;
                // THÊM DÒNG NÀY: Mở khóa lại danh sách khi đã lưu xong
                flowPetList.Enabled = true;

                ShowLoaiTiem();
            };

            panelRight.Controls.Add(btnSave);
        }
        void DisplayTempPetInList(string tenTC)
        {
            flowPetList.Controls.Clear();
            // Ẩn nút thêm bé mới để người dùng tập trung chọn loại tiêm cho bé hiện tại
            btnAddPetBottom.Visible = false;
            btnAddPetBottom.Update();

            Button btn = new Button()
            {
                Text = "⭐ " + tenTC + " (MỚI)",
                Size = new Size(310, 50),
                BackColor = Color.FromArgb(255, 192, 72),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.No
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += (s, e) =>
            {
                currentMaTC = -1;   // thú mới
                isAddingNewPet = true; // Vẫn đang trong luồng bé mới
                ShowLoaiTiem();
            };

            flowPetList.Controls.Add(btn);
        }
        void ShowVaccineBookingScreen()
        {

            panelLeft.Controls.Clear();
            panelRight.Controls.Clear();

            panelRight.Visible = true;

            ShowVaccineList(panelLeft);
            ShowTimeDoctorPanel(panelRight);
            panelRight.Visible = true;

        }
        class VaccineDTO
        {
            public int MaTP;
            public string Ten;
            public int Gia;
        }

        void ShowVaccineList(Panel panel)
        {
            Label lbl = new Label()
            {
                Text = "CHỌN VẮC XIN",
                Font = new Font("Segoe UI", 18, FontStyle.Bold), // Chỉnh cỡ chữ to hơn xíu
                ForeColor = Color.FromArgb(255, 107, 107),      // Màu hồng đỏ đặc trưng
                Location = new Point(20, 10),                   // Đẩy sát lên trên 1 chút
                AutoSize = true
            };

            FlowLayoutPanel flow = new FlowLayoutPanel()
            {
                Location = new Point(20, 60),
                Size = new Size(panel.Width - 40, panel.Height - 80),

                FlowDirection = FlowDirection.TopDown,   // ⬅️ XẾP DỌC
                WrapContents = false,                    // ⬅️ CẤM KÉO NGANG
                AutoScroll = true,

                BorderStyle = BorderStyle.FixedSingle
            };

            panel.Controls.Add(lbl);
            lbl.BringToFront();
            panel.Controls.Add(flow);

            // 🚀 LOAD ASYNC
            Task.Run(() =>
            {
                List<VaccineDTO> list = new List<VaccineDTO>();

                using (SqlConnection conn = new SqlConnection(strCon))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SP_LAYVACXIN", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MATC", currentMaTC);
                    cmd.Parameters.AddWithValue("@LoaiChon", loaiTiem == "LE" ? 0 : 1);

                    if (loaiTiem == "GOI")
                        cmd.Parameters.AddWithValue("@SoThangGoi", soThangGoi);

                    SqlDataReader r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        list.Add(new VaccineDTO
                        {
                            MaTP = Convert.ToInt32(r["MATP"]),
                            Ten = r["LOAIVX"].ToString(),
                            Gia = Convert.ToInt32(r["GIATIEN"])
                        });
                    }
                }

                // 🔥 VẼ UI SAU
                this.Invoke(new Action(() =>
                {
                    flow.SuspendLayout();
                    flow.Controls.Clear();

                    foreach (var vx in list)
                    {
                        Panel row = new Panel()
                        {
                            Height = 42,
                            Width = flow.ClientSize.Width - 25,   // ⬅️ TRỪ CHỖ SCROLLBAR
                            BackColor = Color.White,
                            Margin = new Padding(3)
                        };


                        RadioButton rb = new RadioButton()
                        {
                            Text = vx.Ten,
                            Tag = vx.MaTP,
                            Location = new Point(10, 10),
                            AutoSize = true
                        };


                        // chọn = lưu MaTP + đổi màu dòng
                        rb.CheckedChanged += (s, e) =>
                        {
                            if (!rb.Checked) return;

                            // ❌ bỏ chọn vắc-xin cũ
                            if (selectedVaccineRB != null)
                                selectedVaccineRB.Checked = false;

                            if (selectedVaccinePanel != null)
                                selectedVaccinePanel.BackColor = Color.White;

                            // ✅ chọn vắc-xin mới
                            rb.Checked = true;
                            selectedVaccineRB = rb;
                            selectedVaccinePanel = row;
                            selectedMaTP = (int)rb.Tag;

                            row.BackColor = Color.MistyRose;
                        };

                        // click panel cũng chọn radio
                        row.Click += (s, e) => rb.Checked = true;

                        row.Controls.Add(rb);
                        flow.Controls.Add(row);
                    }

                    flow.ResumeLayout();
                }));
            });
        }


        void ShowTimeDoctorPanel(Panel panel)
        {
            panel.Controls.Clear();

            Label lbl = new Label()
            {
                Text = "THỜI GIAN",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 107, 107),      // Màu hồng đỏ
                Location = new Point(20, 10),
                AutoSize = true
            };

            panel.Controls.Add(lbl);
            lbl.BringToFront();
            dtpDate = new DateTimePicker()
            {
                Location = new Point(20, 60),
                MinDate = DateTime.Now
            };

            cboHour = new ComboBox()
            {
                Location = new Point(220, 60),
                Width = 120,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            UpdateAvailableHours();

            flowDoctors = new FlowLayoutPanel()
            {
                Location = new Point(20, 110),
                Size = new Size(panel.Width - 40, 230),

                FlowDirection = FlowDirection.TopDown, // ⬅️ xếp dọc
                WrapContents = false,                  // ⬅️ cấm kéo ngang
                AutoScroll = true,

                BorderStyle = BorderStyle.FixedSingle
            };


            dtpDate.ValueChanged += (s, e) =>
            {
                UpdateAvailableHours();
                LoadDoctors();
            };

            cboHour.SelectedIndexChanged += (s, e) => LoadDoctors();

            Button btnConfirm = new Button()
            {
                Text = "XÁC NHẬN ĐẶT LỊCH",
                Size = new Size(200, 50),
                Location = new Point(120, 380),
                BackColor = Color.FromArgb(255, 107, 107),
                ForeColor = Color.White
            };

            btnConfirm.Click += (s, e) => FinalizeBooking();

            panel.Controls.AddRange(new Control[]
{
   dtpDate, cboHour, flowDoctors, btnConfirm
});

            LoadDoctors();
        }

    }
}
