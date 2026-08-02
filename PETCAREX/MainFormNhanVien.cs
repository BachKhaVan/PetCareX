using System;
using System.Drawing;
using System.Windows.Forms;
using FontAwesome.Sharp;

namespace PETCAREX
{
    public partial class MainFormNhanVien : Form
    {
        private Color defaultColor = Color.FromArgb(255, 235, 235);
        private Color activeColor = Color.FromArgb(230, 150, 150);

        // 1. Biến để lưu trữ form đang hiển thị
        private Form activeForm = null;

        private Panel panelDesktop; // Thêm dòng này để máy biết panelDesktop là gì
        public MainFormNhanVien()
        {
            InitializeComponent();

            // 1. Tạo Panel làm nền
            panelDesktop = new Panel();
            panelDesktop.Dock = DockStyle.Fill;
            panelDesktop.BackColor = Color.White;
            this.Controls.Add(panelDesktop);

            // RẤT QUAN TRỌNG: Đẩy ra sau để thấy Menu và Header
            panelDesktop.SendToBack();

            //this.WindowState = FormWindowState.Maximized;

            // 2. Load tên User
            if (!string.IsNullOrEmpty(UserSession.FullName))
            {
                lblDoctor.Text = "👤 NV. " + UserSession.FullName.ToUpper();
            }

            LayoutCustomControls();
            SetupSidebarEvents();
            //them
            Sidebar_Click(btnScroll, EventArgs.Empty);
        }

        private void OpenChildForm(Form childForm)
        {
            if (activeForm != null)
            {
                activeForm.Close();
            }

            activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;

            // Xóa sạch rác trong panel trước khi bỏ form mới vào
            panelDesktop.Controls.Clear();
            panelDesktop.Controls.Add(childForm);
            panelDesktop.Tag = childForm;

            childForm.BringToFront();
            childForm.Show();

            // Đảm bảo cái Panel chứa form con phải hiện lên trên cùng của lớp NỀN
            panelDesktop.BringToFront();
            // Nhưng phải cho Sidebar và Header hiện lên trên nữa
            // Giả sử tên Sidebar của bạn là panelMenu và Header là panelHeader
            // panelMenu.BringToFront();
            // panelHeader.BringToFront();
        }

        private void LayoutCustomControls()
        {
            // Đặt nút thoát ở sát lề phải
            iconLogOut.Left = this.ClientSize.Width - iconLogOut.Width - 10;
            iconLogOut.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            // Đặt Label tên ở bên trái nút thoát
            lblDoctor.Left = iconLogOut.Left - lblDoctor.Width - 5;
            lblDoctor.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        }

        // 2. Hàm dùng để mở form con vào Panel chính


        private void SetupSidebarEvents()
        {
            btnScroll.Click += Sidebar_Click;
            btnFileCirclePlus.Click += Sidebar_Click;
            btnCheckIn.Click += Sidebar_Click;
            SetActiveButton(btnScroll);
        }

        private void Sidebar_Click(object sender, EventArgs e)
        {
            IconButton clickedButton = sender as IconButton;
            SetActiveButton(clickedButton);

            // 3. Logic mở form tương ứng khi bấm nút
            if (clickedButton == btnFileCirclePlus)
            {
                // Thay 'FormDatLich' bằng tên chính xác của Form đặt lịch trong đồ án
                OpenChildForm(new FormDatLich());
            }
            else if (clickedButton == btnScroll)
            {
                // Ví dụ mở form Trang Chủ
                // OpenChildForm(new FormTrangChu());
                OpenChildForm(new FormHoaDon());
            }
            else if (clickedButton == btnCheckIn)
            {
                OpenChildForm(new FormCheckin());
            }
        }

        private void SetActiveButton(IconButton activeBtn)
        {
            DisableButton(btnScroll);
            DisableButton(btnFileCirclePlus);
            DisableButton(btnCheckIn);
            if (activeBtn != null)
            {
                activeBtn.BackColor = activeColor;
                activeBtn.IconColor = Color.White;
                activeBtn.ForeColor = Color.White;
            }
        }

        private void DisableButton(IconButton btn)
        {
            if (btn != null)
            {
                btn.BackColor = defaultColor;
                btn.IconColor = Color.FromArgb(220, 80, 80);
                btn.ForeColor = Color.FromArgb(100, 60, 60);
            }
        }

        private void iconLogOut_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                UserSession.MaUser = 0;
                UserSession.FullName = null;
                new LoginForm().Show();
                this.Hide();
            }
        }
    }
}
