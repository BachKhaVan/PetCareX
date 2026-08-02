using System;
using System.Drawing;
using System.Windows.Forms;
using FontAwesome.Sharp;

namespace PETCAREX
{
    public partial class MainFormAdmin : Form
    {
        private Color defaultColor = Color.FromArgb(255, 235, 235);
        private Color activeColor = Color.FromArgb(230, 150, 150);

        private Form currentChildForm = null;

        public MainFormAdmin()
        {
            InitializeComponent();

            // Gán sự kiện bằng code (không cần designer)
            btnTransfer.Click += BtnTransfer_Click;
            btnMoney.Click += BtnMoney_Click;

            // Vừa vào là mở Form Điều Chuyển
            OpenChildForm(new FormDieuChuyen(), btnTransfer);
        }

        // ================== CORE ==================
        private void OpenChildForm(Form childForm, IconButton activeBtn)
        {
            // Đóng form cũ
            if (currentChildForm != null)
            {
                currentChildForm.Close();
                currentChildForm = null;
            }

            // Reset màu nút
            ResetSidebarButtons();

            // Active nút được chọn
            if (activeBtn != null)
            {
                activeBtn.BackColor = activeColor;
                activeBtn.IconColor = Color.White;
            }

            // Clear panelContent NHƯNG GIỮ SIDEBAR
            panelContent.Controls.Clear();
            panelContent.Controls.Add(panelSidebar);

            // Nhúng form con
            currentChildForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;

            panelContent.Controls.Add(childForm);
            childForm.BringToFront();
            childForm.Show();
        }

        // ================== SIDEBAR ==================
        private void ResetSidebarButtons()
        {
            btnTransfer.BackColor = defaultColor;
            btnMoney.BackColor = defaultColor;

            btnTransfer.IconColor = Color.FromArgb(220, 80, 80);
            btnMoney.IconColor = Color.FromArgb(220, 80, 80);
        }

        private void BtnTransfer_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FormDieuChuyen(), btnTransfer);
        }

        private void BtnMoney_Click(object sender, EventArgs e)
        {
          
            // ĐỔI FormQLDT_ALL thành form thứ 2 của bạn nếu tên khác
            OpenChildForm(new FormQLDT_ALL(), btnMoney);
        }

        // ================== LOGOUT ==================
        private void iconLogOut_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn đăng xuất?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

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
