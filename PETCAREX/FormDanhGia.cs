using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

namespace PETCAREX
{
    public partial class FormDanhGia : Form
    {
        string connStr = "Data Source=.;Initial Catalog=QLTC;Integrated Security=True";


        int _maHD;
        string _maDV;
        int _maKH;

        // ===== CONTROLS =====
        Label lblTitle, lblMaHD, lblLoaiDV, lblKH;
        Label lblDiem, lblThaiDo, lblMucDo, lblBinhLuan;
        NumericUpDown numDiem;
        ComboBox cboThaiDo, cboMucDo;
        TextBox txtBinhLuan;
        Button btnGui, btnHuy;

        public FormDanhGia(int maHD, string maDV, int maKH)
        {
            _maHD = maHD;
            _maDV = maDV;
            _maKH = maKH;

            InitForm();
            InitControls();
            LoadData();
        }


        // ================= FORM =================
        void InitForm()
        {
            this.Text = "Đánh giá dịch vụ";
            this.Size = new Size(500, 520);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(255, 240, 240);
        }

        // ================= UI =================
        void InitControls()
        {
            lblTitle = new Label()
            {
                Text = "📝 ĐÁNH GIÁ DỊCH VỤ",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.DarkRed,
                Location = new Point(20, 20),
                AutoSize = true
            };

            lblMaHD = CreateLabel("", 20, 70);
            lblLoaiDV = CreateLabel("", 20, 100);
            lblKH = CreateLabel("", 20, 130);

            lblDiem = CreateLabel("Điểm chất lượng (1–5):", 20, 170);
            numDiem = new NumericUpDown()
            {
                Location = new Point(220, 168),
                Width = 60,
                Minimum = 1,
                Maximum = 5,
                Value = 5
            };

            lblThaiDo = CreateLabel("Thái độ nhân viên:", 20, 210);
            cboThaiDo = new ComboBox()
            {
                Location = new Point(220, 208),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            lblMucDo = CreateLabel("Mức độ hài lòng:", 20, 250);
            cboMucDo = new ComboBox()
            {
                Location = new Point(220, 248),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            lblBinhLuan = CreateLabel("Bình luận:", 20, 290);
            txtBinhLuan = new TextBox()
            {
                Location = new Point(20, 320),
                Width = 440,
                Height = 80,
                Multiline = true,
                MaxLength = 100
            };

            btnGui = new Button()
            {
                Text = "Gửi đánh giá",
                Location = new Point(220, 420),
                Width = 120,
                BackColor = Color.IndianRed,
                ForeColor = Color.White
            };
            btnGui.Click += BtnGui_Click;

            btnHuy = new Button()
            {
                Text = "Hủy",
                Location = new Point(350, 420),
                Width = 80
            };
            btnHuy.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[]
            {
                lblTitle, lblMaHD, lblLoaiDV, lblKH,
                lblDiem, numDiem,
                lblThaiDo, cboThaiDo,
                lblMucDo, cboMucDo,
                lblBinhLuan, txtBinhLuan,
                btnGui, btnHuy
            });
        }

        // ================= DATA GIẢ =================
        void LoadData()
        {
            lblMaHD.Text = $"Hóa đơn: {_maHD}";
            lblLoaiDV.Text = $"Loại dịch vụ: {(_maDV == "KB" ? "Khám bệnh" : "Tiêm phòng")}";
            lblKH.Text = $"Khách hàng: {_maKH}";

            //cboThaiDo.DropDownStyle = ComboBoxStyle.DropDown; 
            cboThaiDo.Items.AddRange(new object[] 
            { "Rất thân thiện", "Bình thường", "Thiếu nhiệt tình" });

            cboMucDo.Items.AddRange(new object[]
            {
        "Tốt",
        "Trung Bình",
        "Tệ"
            });

            cboThaiDo.SelectedIndex = 0;
            cboMucDo.SelectedIndex = 0;
        }

        // ================= EVENT =================
        private void BtnGui_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand(
     @"INSERT INTO DANH_GIA
      (MAHD, MADV, MAKH, DIEMCLDV, THAIDONV, MUCDOHL, BINHLUAN)
      VALUES
      (@MAHD, @MADV, @MAKH, @DIEMCLDV, @THAIDONV, @MUCDOHL, @BINHLUAN)", conn))
                {
                    cmd.Parameters.AddWithValue("@MAHD", _maHD);
                    cmd.Parameters.AddWithValue("@MADV", _maDV);
                    cmd.Parameters.AddWithValue("@MAKH", _maKH);
                    cmd.Parameters.AddWithValue("@DIEMCLDV", (int)numDiem.Value);
                    cmd.Parameters.AddWithValue("@THAIDONV", cboThaiDo.Text);
                    cmd.Parameters.AddWithValue("@MUCDOHL", cboMucDo.Text);
                    cmd.Parameters.AddWithValue("@BINHLUAN", txtBinhLuan.Text);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show(
                    "Đánh giá thành công!",
                    "Thành công",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                this.Close();
            }
            catch (SqlException ex)
            {
                // ❌ ĐÃ ĐÁNH GIÁ RỒI
                MessageBox.Show(
                    ex.Message,
                    "Không thể đánh giá",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi hệ thống: " + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }


        // ================= HELPER =================
        Label CreateLabel(string text, int x, int y)
        {
            return new Label()
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            };
        }
    }
}
