using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FontAwesome.Sharp;

namespace PETCAREX
{
    public partial class MainFormQuanLy : Form
    {
        private Color defaultColor = Color.FromArgb(255, 235, 235);
        private Color activeColor = Color.FromArgb(230, 150, 150);

        public MainFormQuanLy()
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(UserSession.FullName))
            {
                lblDoctor.Text = "👤 QL. " + UserSession.FullName.ToUpper();
            }
            SetupSidebarEvents();
        }

        private void SetupSidebarEvents()
        {
            // Gán sự kiện Click cho các nút IconButton
            btnChart.Click += Sidebar_Click;
            btnUser.Click += Sidebar_Click;

            // Mặc định chọn nút Home
            SetActiveButton(btnChart);
        }

        private void Sidebar_Click(object sender, EventArgs e)
        {
            IconButton clickedButton = sender as IconButton;
            SetActiveButton(clickedButton);

            if (clickedButton == btnChart)
            {
                OpenChildForm(new FormQuanLyDoanhThu()); // hoặc form dashboard QL
            }
            else if (clickedButton == btnUser)
            {
                  LoadContent(new UcNhanSu());
            }
        }

        // Thay đổi tham số nhận vào là IconButton
        private void SetActiveButton(IconButton activeBtn)
        {
            // 1. Reset tất cả các nút về trạng thái bình thường
            DisableButton(btnChart);
            DisableButton(btnUser);

            // 2. Kích hoạt nút được chọn
            if (activeBtn != null)
            {
                activeBtn.BackColor = activeColor;
                activeBtn.IconColor = Color.White; // Đổi màu icon sang trắng cho nổi bật
                activeBtn.ForeColor = Color.White; // Đổi màu chữ sang trắng
            }
        }

        // Hàm phụ trợ để reset nút về mặc định
        private void DisableButton(IconButton btn)
        {
            if (btn != null)
            {
                btn.BackColor = defaultColor;
                btn.IconColor = Color.FromArgb(220, 80, 80);
                btn.ForeColor = Color.FromArgb(100, 60, 60);
            }
        }
        private void OpenChildForm(Form childForm)
        {
            panelContent.Controls.Clear();

            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;

            panelContent.Controls.Add(childForm);
            childForm.Show();
        }

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

                LoginForm loginForm = new LoginForm();
                loginForm.Show();

                this.Hide();
            }
        }
        private void LoadContent(UserControl uc)
        {
            panelContent.Controls.Clear();
            uc.Dock = DockStyle.Fill;
            panelContent.Controls.Add(uc);
        }
    }
}
