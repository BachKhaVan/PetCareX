namespace PETCAREX
{
    partial class MainFormAdmin
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelHeader = new System.Windows.Forms.Panel();
            this.iconLogOut = new FontAwesome.Sharp.IconButton();
            this.lblLogo = new System.Windows.Forms.Label();
            this.panelContent = new System.Windows.Forms.Panel();
            this.btnMoney = new FontAwesome.Sharp.IconButton();
            this.btnTransfer = new FontAwesome.Sharp.IconButton();
            this.panelSidebar = new System.Windows.Forms.Panel();
            this.panelHeader.SuspendLayout();
            this.panelContent.SuspendLayout();
            this.panelSidebar.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.panelHeader.Controls.Add(this.iconLogOut);
            this.panelHeader.Controls.Add(this.lblLogo);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Margin = new System.Windows.Forms.Padding(4);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(1444, 98);
            this.panelHeader.TabIndex = 0;
            // 
            // iconLogOut
            // 
            this.iconLogOut.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.iconLogOut.FlatAppearance.BorderSize = 0;
            this.iconLogOut.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.iconLogOut.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.iconLogOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconLogOut.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.iconLogOut.IconChar = FontAwesome.Sharp.IconChar.SignOut;
            this.iconLogOut.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.iconLogOut.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconLogOut.IconSize = 30;
            this.iconLogOut.Location = new System.Drawing.Point(1377, 2);
            this.iconLogOut.Margin = new System.Windows.Forms.Padding(4);
            this.iconLogOut.Name = "iconLogOut";
            this.iconLogOut.Size = new System.Drawing.Size(53, 98);
            this.iconLogOut.TabIndex = 3;
            this.iconLogOut.UseVisualStyleBackColor = false;
            this.iconLogOut.Click += new System.EventHandler(this.iconLogOut_Click);
            // 
            // lblLogo
            // 
            this.lblLogo.AutoSize = true;
            this.lblLogo.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblLogo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.lblLogo.Location = new System.Drawing.Point(22, 20);
            this.lblLogo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLogo.Name = "lblLogo";
            this.lblLogo.Size = new System.Drawing.Size(204, 50);
            this.lblLogo.TabIndex = 0;
            this.lblLogo.Text = "PETCAREX";
            // 
            // panelContent
            // 
            this.panelContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.panelContent.Controls.Add(this.panelSidebar);
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(0, 98);
            this.panelContent.Margin = new System.Windows.Forms.Padding(4);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(1444, 764);
            this.panelContent.TabIndex = 2;
            // 
            // btnMoney
            // 
            this.btnMoney.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.btnMoney.FlatAppearance.BorderSize = 0;
            this.btnMoney.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnMoney.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.btnMoney.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMoney.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMoney.IconChar = FontAwesome.Sharp.IconChar.MoneyBill;
            this.btnMoney.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnMoney.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnMoney.IconSize = 50;
            this.btnMoney.Location = new System.Drawing.Point(0, 117);
            this.btnMoney.Margin = new System.Windows.Forms.Padding(4);
            this.btnMoney.Name = "btnMoney";
            this.btnMoney.Size = new System.Drawing.Size(120, 120);
            this.btnMoney.TabIndex = 2;
            this.btnMoney.UseVisualStyleBackColor = false;
            // 
            // btnTransfer
            // 
            this.btnTransfer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.btnTransfer.FlatAppearance.BorderSize = 0;
            this.btnTransfer.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnTransfer.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.btnTransfer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTransfer.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTransfer.IconChar = FontAwesome.Sharp.IconChar.PeopleArrowsLeftRight;
            this.btnTransfer.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnTransfer.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnTransfer.IconSize = 50;
            this.btnTransfer.Location = new System.Drawing.Point(0, 0);
            this.btnTransfer.Margin = new System.Windows.Forms.Padding(4);
            this.btnTransfer.Name = "btnTransfer";
            this.btnTransfer.Size = new System.Drawing.Size(120, 120);
            this.btnTransfer.TabIndex = 1;
            this.btnTransfer.UseVisualStyleBackColor = false;
            // 
            // panelSidebar
            // 
            this.panelSidebar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.panelSidebar.Controls.Add(this.btnMoney);
            this.panelSidebar.Controls.Add(this.btnTransfer);
            this.panelSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelSidebar.Location = new System.Drawing.Point(0, 0);
            this.panelSidebar.Margin = new System.Windows.Forms.Padding(4);
            this.panelSidebar.Name = "panelSidebar";
            this.panelSidebar.Size = new System.Drawing.Size(120, 764);
            this.panelSidebar.TabIndex = 2;
            // 
            // MainFormAdmin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.ClientSize = new System.Drawing.Size(1444, 862);
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "MainFormAdmin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PETCAREX";
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.panelContent.ResumeLayout(false);
            this.panelSidebar.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblLogo;
        private System.Windows.Forms.Panel panelContent;
        private FontAwesome.Sharp.IconButton iconLogOut;
        private System.Windows.Forms.Panel panelSidebar;
        private FontAwesome.Sharp.IconButton btnMoney;
        private FontAwesome.Sharp.IconButton btnTransfer;
    }
}
