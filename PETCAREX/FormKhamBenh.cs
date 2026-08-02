using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.IO;

namespace PETCAREX
{
    public partial class FormKhamBenh : Form

    {
        public Action OnBackToMain { get; set; }
        private bool IsValidPhone(string phone)
        {
            // Kiểm tra: Phải là số, không chứa chữ và đúng 10 ký tự
            return System.Text.RegularExpressions.Regex.IsMatch(phone, @"^[0-9]{10}$");
        }
        // --- CÁC BIẾN LOGIC GIỮ NGUYÊN ---
        string strCon = @"Data Source=.;Initial Catalog=QLTC;Integrated Security=True";
        string selectedMaCN = "";
        string selectedService = "Khám bệnh"; // Mặc định vì đây là form khám
        int currentMaTC = -1;
        int selectedMaBS = -1;
        bool isAddingNewPet = false;
        dynamic tempPetData = null;

        // --- CONTROLS GIAO DIỆN ---
        Panel panelPetSelection, panelLeft, panelRight;
        FlowLayoutPanel flowPetList, flowDoctors;
        TextBox txtPhone;
        Button btnSearchPhone, btnAddPetBottom;
        DateTimePicker dtpDate;
        ComboBox cboHour; // Thay cho dtpTime để khớp logic UpdateAvailableHours

        // CONSTRUCTOR: Nhận mã chi nhánh từ Form trước
        public FormKhamBenh(string maCN)
        {
            InitializeComponent();
            this.selectedMaCN = maCN;
            this.DoubleBuffered = true;
            InitUI();
            ShowPetSelection(); // Tự động chạy logic bước 2
        }

        private void InitUI()
        {
            this.Text = "ĐĂNG KÝ KHÁM BỆNH";
            this.Size = new Size(1150, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(255, 242, 242);
        }

        // ================= LOGIC HIỂN THỊ (GIỮ NGUYÊN TỪ CODE BẠN) =================
        void ShowPetSelection()
        {
            int sidebarWidth = 30;
            int topBarHeight = 20;
            int availableWidth = this.ClientSize.Width - sidebarWidth;
            int availableHeight = this.ClientSize.Height - topBarHeight;

            panelPetSelection = new Panel()
            {
                Size = new Size(availableWidth - 50, availableHeight - 50),
                BackColor = Color.Transparent,
                Location = new Point(sidebarWidth + 15, topBarHeight + 10)
            };
            this.Controls.Add(panelPetSelection);

            // --- CỘT TRÁI ---
            panelLeft = new Panel() { Size = new Size(400, 420), Location = new Point(0, 0), BackColor = Color.White };
            panelLeft.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = GetRoundedPath(new Rectangle(0, 0, panelLeft.Width, panelLeft.Height), 40))
                    panelLeft.Region = new Region(path);
            };

            Label lblTitleLeft = new Label()
            {
                Text = "ĐĂNG KÝ " + selectedService.ToUpper(),
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 90, 90),
                Location = new Point(35, 30),
                AutoSize = true
            };

            txtPhone = new TextBox()
            {
                Text = "Nhập SĐT khách hàng...",
                Font = new Font("Segoe UI", 13),
                Location = new Point(35, 90),
                Width = 230,
                ForeColor = Color.Gray
            };
            txtPhone.Enter += (s, e) => { if (txtPhone.Text == "Nhập SĐT khách hàng...") { txtPhone.Text = ""; txtPhone.ForeColor = Color.Black; } };
            txtPhone.Leave += (s, e) => {
                if (string.IsNullOrWhiteSpace(txtPhone.Text)) { txtPhone.Text = "Nhập SĐT khách hàng..."; txtPhone.ForeColor = Color.Gray; }
            };
            btnSearchPhone = new Button()
            {
                Text = "Tìm kiếm",
                Location = new Point(275, 88),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(255, 107, 107),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSearchPhone = new Button()
            {
                Text = "Tìm kiếm",
                Location = new Point(275, 88),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(255, 107, 107),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSearchPhone.Click += (s, e) => {
                string phone = txtPhone.Text.Trim();

                // 1. Kiểm tra rỗng hoặc chưa nhập
                if (phone == "Nhập SĐT khách hàng..." || string.IsNullOrEmpty(phone))
                {
                    MessageBox.Show("Vui lòng nhập số điện thoại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPhone.Focus();
                    return;
                }

                // 2. Kiểm tra định dạng (Chỉ số và đủ 10 số)
                if (!IsValidPhone(phone))
                {
                    MessageBox.Show("Số điện thoại không hợp lệ!\n(Phải là 10 chữ số và không chứa ký tự chữ)",
                                    "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPhone.Text = ""; // Xóa để nhập lại
                    txtPhone.Focus();
                    return;
                }
                // 3. KIỂM TRA SĐT TRONG CƠ SỞ DỮ LIỆU
                using (SqlConnection conn = new SqlConnection(strCon))
                {
                    try
                    {
                        conn.Open();
                        // Giả sử bảng khách hàng của bạn tên là KHACH_HANG và cột là SDT
                        string sqlCheck = "SELECT COUNT(*) FROM KHACH_HANG WHERE SDT = @sdt";
                        SqlCommand cmdCheck = new SqlCommand(sqlCheck, conn);
                        cmdCheck.Parameters.AddWithValue("@sdt", phone);

                        int count = (int)cmdCheck.ExecuteScalar();

                        if (count == 0)
                        {
                            // THÔNG BÁO NẾU KHÔNG CÓ DỮ LIỆU
                            MessageBox.Show("Không có dữ liệu khách hàng! Vui lòng kiểm tra lại hoặc đăng ký khách hàng mới.",
                                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            txtPhone.Text = ""; // Xóa SĐT sai
                            txtPhone.Focus();   // Bắt nhập lại
                            flowPetList.Controls.Clear(); // Xóa danh sách cũ nếu có
                            btnAddPetBottom.Visible = false; // Ẩn nút thêm bé
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi kiểm tra khách hàng: " + ex.Message);
                        return;
                    }
                }
                // 3. Nếu đúng hết thì mới gọi hàm Load
                LoadPetsByPhone(phone);
            };

            Panel pnlPetContainer = new Panel() { Location = new Point(40, 140), Size = new Size(340, 270), BackColor = Color.FromArgb(250, 250, 250) };

            flowPetList = new FlowLayoutPanel()
            {
                Location = new Point(5, 5),
                Size = new Size(330, 205),
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };

            btnAddPetBottom = new Button()
            {
                Text = "➕ Thêm bé mới",
                Location = new Point(5, 220),
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

            pnlPetContainer.Controls.Add(flowPetList);
            pnlPetContainer.Controls.Add(btnAddPetBottom);
            panelLeft.Controls.AddRange(new Control[] { lblTitleLeft, txtPhone, btnSearchPhone, pnlPetContainer });

            // --- CỘT PHẢI ---
            panelRight = new Panel() { Size = new Size(420, 520), Location = new Point(440, 0), BackColor = Color.Transparent, Visible = false };

            Label lblTimeTitle = new Label() { Text = "Thời gian", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Color.FromArgb(255, 90, 90), Location = new Point(10, 0), AutoSize = true };
            dtpDate = new DateTimePicker() { Location = new Point(15, 45), Width = 180, Font = new Font("Segoe UI", 12), MinDate = DateTime.Now };

            cboHour = new ComboBox() { Location = new Point(230, 45), Width = 110, Font = new Font("Segoe UI", 12), DropDownStyle = ComboBoxStyle.DropDownList };
            UpdateAvailableHours();
            Label lblDocTitle = new Label() { Text = "Chọn bác sĩ", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Color.FromArgb(255, 90, 90), Location = new Point(10, 100), AutoSize = true };
            flowDoctors = new FlowLayoutPanel() { Name = "flowDoctors", Location = new Point(10, 140), Size = new Size(400, 220), AutoScroll = true };

            // 1. Khi đổi ngày: Phải cập nhật lại danh sách GIỜ trước, sau đó mới load BÁC SĨ
            dtpDate.ValueChanged += (s, e) => {
                UpdateAvailableHours(); // <--- Thiếu cái này là nó không bao giờ hiện giờ
                LoadDoctorSelection(flowDoctors);
            };

            // 2. Khi đổi giờ: Load lại bác sĩ theo khung giờ mới
            cboHour.SelectedIndexChanged += (s, e) => {
                LoadDoctorSelection(flowDoctors);
            };



            Button btnConfirm = new Button()
            {
                Text = "XÁC NHẬN ĐẶT LỊCH",
                Size = new Size(350, 55),
                Location = new Point(30, 380),
                BackColor = Color.FromArgb(255, 107, 107),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnConfirm.Click += (s, e) => FinalizeBooking();

            panelRight.Controls.AddRange(new Control[] { lblTimeTitle, dtpDate, cboHour, lblDocTitle, flowDoctors, btnConfirm });
            panelPetSelection.Controls.AddRange(new Control[] { panelLeft, panelRight });
        }

        // ================= CÁC HÀM XỬ LÝ DỮ LIỆU (COPY NGUYÊN BẢN) =================

        void LoadPetsByPhone(string sdt)
        {
            flowPetList.Controls.Clear();
            panelRight.Visible = false;
            isAddingNewPet = false;
            using (SqlConnection conn = new SqlConnection(strCon))
            {
                try
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
                        Button btnPet = new Button()
                        {
                            Text = r["TENTC"].ToString(),
                            Tag = r["MATC"].ToString(),
                            Size = new Size(310, 45),
                            BackColor = Color.White,
                            FlatStyle = FlatStyle.Flat
                        };
                        btnPet.Click += (s, e) => {
                            currentMaTC = int.Parse(btnPet.Tag.ToString());
                            foreach (Control c in flowPetList.Controls) c.BackColor = Color.White;
                            btnPet.BackColor = Color.FromArgb(255, 230, 230);
                            panelRight.Visible = true;
                            LoadDoctorSelection(flowDoctors);
                        };
                        flowPetList.Controls.Add(btnPet);
                    }
                    btnAddPetBottom.Visible = hasPet;
                    if (!hasPet) ShowNoPetNotification(sdt);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }
        RadioButton selectedDoctorRB = null;
        Panel selectedDoctorPanel = null;
        void LoadDoctorSelection(FlowLayoutPanel flow)
        {
            if (cboHour.SelectedItem == null) return;

            flow.Controls.Clear();
            selectedMaBS = -1;
            selectedDoctorRB = null;
            selectedDoctorPanel = null;

            using (SqlConnection conn = new SqlConnection(strCon))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("sp_LAYBACSI", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@MaCN", SqlDbType.Char, 4).Value = selectedMaCN;
                    cmd.Parameters.Add("@NgayHen", SqlDbType.Date).Value = dtpDate.Value.Date;
                    cmd.Parameters.Add("@GioHen", SqlDbType.Time)
                       .Value = TimeSpan.Parse(cboHour.SelectedItem.ToString());

                    SqlDataReader r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        Panel pDoc = new Panel()
                        {
                            Size = new Size(380, 45),
                            Margin = new Padding(0, 5, 0, 5),
                            BackColor = Color.White
                        };

                        RadioButton rb = new RadioButton()
                        {
                            Text = "BS. " + r["HOTEN"].ToString(),
                            Tag = r["MANS"],
                            AutoSize = true,
                            Location = new Point(10, 10)
                        };

                        rb.Click += (s, e) =>
                        {
                            // 🔹 Bỏ chọn bác sĩ cũ
                            if (selectedDoctorRB != null)
                                selectedDoctorRB.Checked = false;

                            if (selectedDoctorPanel != null)
                                selectedDoctorPanel.BackColor = Color.White;

                            // 🔹 Chọn bác sĩ mới
                            rb.Checked = true;
                            selectedDoctorRB = rb;
                            selectedDoctorPanel = pDoc;
                            selectedMaBS = Convert.ToInt32(rb.Tag);

                            pDoc.BackColor = Color.FromArgb(255, 230, 230);
                        };

                        pDoc.Controls.Add(rb);
                        flow.Controls.Add(pDoc);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        void FinalizeBooking()
        {
            if (selectedMaBS == -1) { MessageBox.Show("Vui lòng chọn bác sĩ!"); return; }
            if (!isAddingNewPet && currentMaTC <= 0)
            {
                MessageBox.Show("Vui lòng chọn thú cưng từ danh sách!");
                return;
            }
            if (isAddingNewPet && tempPetData == null)
            {
                MessageBox.Show("Dữ liệu thú cưng mới chưa được lưu!"); return;
            }
            TimeSpan gioChon = TimeSpan.Parse(cboHour.SelectedItem.ToString());
            DateTime thoiGianHen = dtpDate.Value.Date + gioChon;
            DateTime now = DateTime.Now;
            if (cboHour.SelectedItem == null) return;
            // KIỂM TRA RÀNG BUỘC 15 PHÚT
            if (thoiGianHen.Date == now.Date)
            {
                // Nếu giờ hẹn đúng bằng giờ hiện tại
                if (thoiGianHen.Hour == now.Hour)
                {
                    if (now.Minute > 15)
                    {
                        MessageBox.Show("Giờ này đã quá thời gian cho phép đặt (quá 15 phút đầu giờ). Vui lòng chọn giờ khác!",
                                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        UpdateAvailableHours(); // Load lại danh sách giờ mới nhất
                        return;
                    }
                }
                // Nếu giờ hẹn nhỏ hơn giờ hiện tại (ví dụ 10h mà bây giờ đã 11h)
                else if (thoiGianHen.Hour < now.Hour)
                {
                    MessageBox.Show("Thời gian đã trôi qua, vui lòng chọn lại!",
                                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    UpdateAvailableHours();
                    return;
                }
            }
            if (MessageBox.Show("Xác nhận đăng ký khám?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(strCon))
                {
                    conn.Open();
                    SqlTransaction trans = conn.BeginTransaction();
                    try
                    {
                        int maTCCuoiCung = isAddingNewPet ? ExecuteAddPet(conn, trans) : currentMaTC;

                        SqlCommand cmd = new SqlCommand("SP_DATLICH", conn, trans);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@MaTC", maTCCuoiCung);
                        cmd.Parameters.AddWithValue("@MaDV", "KB");
                        cmd.Parameters.AddWithValue("@MaBS", selectedMaBS);
                        cmd.Parameters.AddWithValue("@ThoiGianHen", dtpDate.Value.Date + TimeSpan.Parse(cboHour.SelectedItem.ToString()));
                        cmd.Parameters.AddWithValue("@MaTP", DBNull.Value);

                        cmd.ExecuteNonQuery();
                        trans.Commit();
                        MessageBox.Show("Đăng ký thành công!");
                        OnBackToMain?.Invoke(); // Kích hoạt lệnh quay về ở Form chính
                        this.Close();

                    }
                    catch (Exception ex) { trans.Rollback(); MessageBox.Show("Lỗi: " + ex.Message); }
                }
            }
        }

        int ExecuteAddPet(SqlConnection conn, SqlTransaction trans)
        {
            SqlCommand cmdAdd = new SqlCommand("SP_THEMTHUCUNG", conn, trans);
            cmdAdd.CommandType = CommandType.StoredProcedure;
            cmdAdd.Parameters.AddWithValue("@TenTC", tempPetData.Ten);
            cmdAdd.Parameters.AddWithValue("@LoaiTC", tempPetData.Loai);
            cmdAdd.Parameters.AddWithValue("@GiongTC", tempPetData.Giong);
            cmdAdd.Parameters.AddWithValue("@NgaySinh", tempPetData.NgaySinh);
            cmdAdd.Parameters.AddWithValue("@GioiTinh", tempPetData.GioiTinh);
            cmdAdd.Parameters.AddWithValue("@TinhTrangSK", tempPetData.MoTa);
            cmdAdd.Parameters.AddWithValue("@SDT", txtPhone.Text.Trim());
            SqlParameter outParam = new SqlParameter("@MaTC_Moi", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmdAdd.Parameters.Add(outParam);
            cmdAdd.ExecuteNonQuery();
            return (int)outParam.Value;
        }

        // --- HÀM VẼ GIAO DIỆN PHỤ ---
        private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }

        void ShowNoPetNotification(string sdt)
        {
            Label lbl = new Label() { Text = "Chưa có thú cưng!", AutoSize = true, ForeColor = Color.Red };
            Button btn = new Button() { Text = "➕ Thêm mới", Size = new Size(310, 45), BackColor = Color.LightCoral };
            btn.Click += (s, e) => ShowAddPetForm(sdt);
            flowPetList.Controls.Add(lbl);
            flowPetList.Controls.Add(btn);
        }

        // 1. CẬP NHẬT GIỜ RẢNH (Logic 15 phút)
        private void UpdateAvailableHours()
        {
            cboHour.Items.Clear();
            DateTime now = DateTime.Now;
            DateTime selectedDate = dtpDate.Value.Date;

            // Vòng lặp chuẩn từ 8h đến 22h
            for (int hour = 8; hour <= 22; hour++)
            {
                // Nếu chọn ngày lớn hơn hôm nay (Tương lai) -> Luôn thêm
                if (selectedDate > now.Date)
                {
                    cboHour.Items.Add($"{hour:D2}:00");
                }
                // Nếu chọn đúng ngày hôm nay -> Chỉ thêm giờ chưa trôi qua (quá 15p)
                else if (selectedDate == now.Date)
                {
                    if (hour > now.Hour || (hour == now.Hour && now.Minute <= 15))
                    {
                        cboHour.Items.Add($"{hour:D2}:00");
                    }
                }
            }

            // Kiểm tra xem có giờ nào được add không
            if (cboHour.Items.Count > 0)
            {
                cboHour.SelectedIndex = 0;
                cboHour.Enabled = true; // Đảm bảo control không bị khóa
            }
            else
            {
                cboHour.Enabled = false;
                MessageBox.Show($"Ngày {selectedDate:dd/MM/yyyy} đã hết khung giờ khả dụng!", "Thông báo");
            }
        }
        // 2. HIỂN THỊ FORM THÊM THÚ CƯNG (Bên cột phải)
        void ShowAddPetForm(string sdt)
        {
            isAddingNewPet = true;
            panelRight.Controls.Clear();
            panelRight.Visible = true;
            panelRight.BackColor = Color.White;

            Label lblTitle = new Label()
            {
                Text = "THÊM THÔNG TIN THÚ CƯNG MỚI",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 53, 69),
                Location = new Point(10, 10),
                AutoSize = true
            };
            panelRight.Controls.Add(lblTitle);

            // Cố định tên biến để dễ truy xuất
            string[] labels = { "Tên bé:", "Loài:", "Giống:", "Ngày sinh:", "Giới tính:", "Mô tả:" };
            for (int i = 0; i < labels.Length; i++)
            {
                Label lbl = new Label() { Text = labels[i], Location = new Point(20, 70 + (i * 45)), Font = new Font("Segoe UI", 10), AutoSize = true };
                panelRight.Controls.Add(lbl);

                if (labels[i] == "Giới tính:")
                {
                    FlowLayoutPanel pnlGender = new FlowLayoutPanel() { Name = "pnlGender", Location = new Point(100, 70 + (i * 45) - 5), Size = new Size(200, 40) };
                    pnlGender.Controls.Add(new RadioButton() { Name = "rbDuc", Text = "Đực", Checked = true, Width = 70 });
                    pnlGender.Controls.Add(new RadioButton() { Name = "rbCai", Text = "Cái", Width = 70 });
                    panelRight.Controls.Add(pnlGender);
                }
                else if (labels[i] == "Ngày sinh:")
                {
                    DateTimePicker dtpNS = new DateTimePicker() { Name = "dtpNgaySinh", Location = new Point(100, 70 + (i * 45)), Width = 200, Format = DateTimePickerFormat.Short, MaxDate = DateTime.Now };
                    panelRight.Controls.Add(dtpNS);
                }
                else
                {
                    // Gán MaxLength trực tiếp để chặn người dùng gõ quá ký tự ngay từ đầu
                    TextBox txt = new TextBox() { Name = "txt" + i, Location = new Point(100, 70 + (i * 45)), Width = 200, Font = new Font("Segoe UI", 10) };
                    if (i == 0 || i == 1) txt.MaxLength = 20; // Tên và Loài
                    if (i == 2) txt.MaxLength = 50;           // Giống
                    if (i == 5) txt.MaxLength = 100;          // Mô tả

                    panelRight.Controls.Add(txt);
                }
            }

            Button btnSave = new Button()
            {
                Text = "LƯU THÔNG TIN",
                Size = new Size(200, 45),
                Location = new Point(100, 350),
                BackColor = Color.FromArgb(255, 90, 90),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            btnSave.Click += (s, e) => {
                // --- 1. LẤY CONTROL ---
                var txtTen = panelRight.Controls["txt0"] as TextBox;
                var txtLoai = panelRight.Controls["txt1"] as TextBox;
                var txtGiong = panelRight.Controls["txt2"] as TextBox;
                var dtpNS = panelRight.Controls["dtpNgaySinh"] as DateTimePicker;
                var pnlGender = panelRight.Controls["pnlGender"] as FlowLayoutPanel;
                var txtMoTa = panelRight.Controls["txt5"] as TextBox;

                // --- 2. VALIDATION LOGIC ---

                // Kiểm tra Tên bé
                if (string.IsNullOrWhiteSpace(txtTen.Text)) { MessageBox.Show("Tên bé không được để trống!"); return; }
                if (txtTen.Text.Trim().Length > 20) { MessageBox.Show("Tên bé không quá 20 ký tự!"); return; }
                if (!System.Text.RegularExpressions.Regex.IsMatch(txtTen.Text, @"^[\p{L}\s0-9]+$"))
                {
                    MessageBox.Show("Tên bé không được chứa ký tự đặc biệt!"); return;
                }

                // Kiểm tra Loài
                if (string.IsNullOrWhiteSpace(txtLoai.Text)) { MessageBox.Show("Loài không được để trống!"); return; }
                if (txtLoai.Text.Trim().Length > 20) { MessageBox.Show("Loài không quá 20 ký tự!"); return; }

                // Kiểm tra Giống
                if (txtGiong.Text.Trim().Length > 50) { MessageBox.Show("Giống không quá 50 ký tự!"); return; }

                // Kiểm tra Ngày sinh (Không quá khứ quá 30 năm, không tương lai)
                if (dtpNS.Value > DateTime.Now) { MessageBox.Show("Ngày sinh không hợp lệ!"); return; }
                if (dtpNS.Value < DateTime.Now.AddYears(-30)) { MessageBox.Show("Ngày sinh quá xa (thú cưng không thể quá 30 tuổi)!"); return; }

                // Kiểm tra Mô tả
                if (txtMoTa.Text.Trim().Length > 100) { MessageBox.Show("Mô tả không quá 100 ký tự!"); return; }

                // --- 3. GÁN DỮ LIỆU ---
                var textInfo = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo;
                tempPetData = new
                {
                    Ten = textInfo.ToTitleCase(txtTen.Text.Trim().ToLower()),
                    Loai = textInfo.ToTitleCase(txtLoai.Text.Trim().ToLower()),
                    Giong = txtGiong.Text.Trim(),
                    NgaySinh = dtpNS.Value,
                    GioiTinh = (pnlGender.Controls[1] as RadioButton).Checked ? "Cái" : "Đực",
                    MoTa = txtMoTa.Text.Trim()
                };

                DisplayTempPetInList(tempPetData.Ten);
                MessageBox.Show($"Đã ghi nhận bé {tempPetData.Ten}! Vui lòng chọn thời gian.");
                ShowDoctorSelectionLayout();
            };
            panelRight.Controls.Add(btnSave);
        }
        // 3. HIỂN THỊ BÉ MỚI LÊN LIST BÊN TRÁI
        void DisplayTempPetInList(string petName)
        {
            flowPetList.Controls.Clear();
            btnAddPetBottom.Visible = false;
            Button btnNewPet = new Button()
            {
                Text = "⭐ " + petName.ToUpper() + " (MỚI)",
                Size = new Size(310, 50),
                BackColor = Color.FromArgb(255, 192, 72),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.No
            };
            flowPetList.Controls.Add(btnNewPet);
            txtPhone.Enabled = false;
            btnSearchPhone.Enabled = false;
        }

        // 4. GIAO DIỆN CHỌN BÁC SĨ (Sau khi đã chọn thú cưng)
        void ShowDoctorSelectionLayout()
        {
            panelRight.Controls.Clear();
            panelRight.Visible = true;

            Label lblTime = new Label() { Text = "Thời gian", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.FromArgb(255, 90, 90), Location = new Point(10, 10), AutoSize = true };
            dtpDate = new DateTimePicker() { Location = new Point(15, 45), Width = 180, Font = new Font("Segoe UI", 11), MinDate = DateTime.Now };

            FlowLayoutPanel flowDocs = new FlowLayoutPanel() { Name = "flowDoctors", Location = new Point(10, 140), Size = new Size(400, 200), AutoScroll = true };

            dtpDate.ValueChanged += (s, e) => { UpdateAvailableHours(); LoadDoctorSelection(flowDocs); };
            cboHour.SelectedIndexChanged += (s, e) => LoadDoctorSelection(flowDocs);

            Button btnConfirm = new Button()
            {
                Text = "XÁC NHẬN ĐẶT LỊCH",
                Size = new Size(350, 50),
                Location = new Point(25, 360),
                BackColor = Color.FromArgb(255, 107, 107),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            btnConfirm.Click += (s, e) => FinalizeBooking();

            panelRight.Controls.AddRange(new Control[] { lblTime, dtpDate, cboHour, flowDocs, btnConfirm });

            UpdateAvailableHours(); // Chạy ngay để nạp giờ
            LoadDoctorSelection(flowDocs);
        }
    }
}