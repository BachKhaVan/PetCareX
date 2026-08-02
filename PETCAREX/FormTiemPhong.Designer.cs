using System;
using System.Windows.Forms;

namespace PETCAREX
{
    partial class FormTiemPhong : Form
    {
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.SuspendLayout();
            // 
            // FormTiemPhong
            // 
            this.ClientSize = new System.Drawing.Size(1150, 650);
            this.Name = "FormTiemPhong";
            this.Text = "Đăng ký tiêm phòng PETCAREX";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
        }

        #endregion
    }
}
