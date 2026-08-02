using System.Windows.Forms;
using System;
using System.Drawing;

namespace PETCAREX
{
    partial class FormKhamBenh
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }


        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // FormKhamBenh
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(914, 480);
            this.Name = "FormKhamBenh";
            this.Text = "FormKhamBenh";
            this.Load += new System.EventHandler(this.FormKhamBenh_Load);
            this.ResumeLayout(false);

        }
        private void FormKhamBenh_Load(object sender, EventArgs e)
        {
            //chưa có
        }
    }
}
