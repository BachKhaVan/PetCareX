namespace PETCAREX
{
    partial class MainFormNhanVien
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
            this.lblDoctor = new System.Windows.Forms.Label();
            this.lblLogo = new System.Windows.Forms.Label();
            this.panelSidebar = new System.Windows.Forms.Panel();
            this.btnFileCirclePlus = new FontAwesome.Sharp.IconButton();
            this.btnScroll = new FontAwesome.Sharp.IconButton();
            this.panelContent = new System.Windows.Forms.Panel();
            this.btnCheckIn = new FontAwesome.Sharp.IconButton();
            this.panelHeader.SuspendLayout();
            this.panelSidebar.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.panelHeader.Controls.Add(this.iconLogOut);
            this.panelHeader.Controls.Add(this.lblDoctor);
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
            // lblDoctor
            // 
            this.lblDoctor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDoctor.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblDoctor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.lblDoctor.Location = new System.Drawing.Point(701, 28);
            this.lblDoctor.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDoctor.Name = "lblDoctor";
            this.lblDoctor.Size = new System.Drawing.Size(657, 42);
            this.lblDoctor.TabIndex = 2;
            this.lblDoctor.Text = "...";
            this.lblDoctor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            // panelSidebar
            // 
            this.panelSidebar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.panelSidebar.Controls.Add(this.btnCheckIn);
            this.panelSidebar.Controls.Add(this.btnFileCirclePlus);
            this.panelSidebar.Controls.Add(this.btnScroll);
            this.panelSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelSidebar.Location = new System.Drawing.Point(0, 98);
            this.panelSidebar.Margin = new System.Windows.Forms.Padding(4);
            this.panelSidebar.Name = "panelSidebar";
            this.panelSidebar.Size = new System.Drawing.Size(120, 764);
            this.panelSidebar.TabIndex = 1;
            // 
            // btnFileCirclePlus
            // 
            this.btnFileCirclePlus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.btnFileCirclePlus.FlatAppearance.BorderSize = 0;
            this.btnFileCirclePlus.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnFileCirclePlus.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.btnFileCirclePlus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFileCirclePlus.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFileCirclePlus.IconChar = FontAwesome.Sharp.IconChar.FileCirclePlus;
            this.btnFileCirclePlus.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnFileCirclePlus.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnFileCirclePlus.IconSize = 50;
            this.btnFileCirclePlus.Location = new System.Drawing.Point(0, 117);
            this.btnFileCirclePlus.Margin = new System.Windows.Forms.Padding(4);
            this.btnFileCirclePlus.Name = "btnFileCirclePlus";
            this.btnFileCirclePlus.Size = new System.Drawing.Size(120, 120);
            this.btnFileCirclePlus.TabIndex = 2;
            this.btnFileCirclePlus.UseVisualStyleBackColor = false;
            // 
            // btnScroll
            // 
            this.btnScroll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.btnScroll.FlatAppearance.BorderSize = 0;
            this.btnScroll.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnScroll.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.btnScroll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnScroll.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnScroll.IconChar = FontAwesome.Sharp.IconChar.Scroll;
            this.btnScroll.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnScroll.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnScroll.IconSize = 50;
            this.btnScroll.Location = new System.Drawing.Point(0, 0);
            this.btnScroll.Margin = new System.Windows.Forms.Padding(4);
            this.btnScroll.Name = "btnScroll";
            this.btnScroll.Size = new System.Drawing.Size(120, 120);
            this.btnScroll.TabIndex = 1;
            this.btnScroll.UseVisualStyleBackColor = false;
            // 
            // panelContent
            // 
            this.panelContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(120, 98);
            this.panelContent.Margin = new System.Windows.Forms.Padding(4);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(1324, 764);
            this.panelContent.TabIndex = 2;
            // 
            // btnCheckIn
            // 
            this.btnCheckIn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.btnCheckIn.FlatAppearance.BorderSize = 0;
            this.btnCheckIn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnCheckIn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.btnCheckIn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCheckIn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCheckIn.IconChar = FontAwesome.Sharp.IconChar.Check;
            this.btnCheckIn.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnCheckIn.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnCheckIn.IconSize = 50;
            this.btnCheckIn.Location = new System.Drawing.Point(0, 231);
            this.btnCheckIn.Margin = new System.Windows.Forms.Padding(4);
            this.btnCheckIn.Name = "btnCheckIn";
            this.btnCheckIn.Size = new System.Drawing.Size(120, 120);
            this.btnCheckIn.TabIndex = 3;
            this.btnCheckIn.UseVisualStyleBackColor = false;
            // 
            // MainFormNhanVien
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.ClientSize = new System.Drawing.Size(1444, 862);
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelSidebar);
            this.Controls.Add(this.panelHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "MainFormNhanVien";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PETCAREX";
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.panelSidebar.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblLogo;
        private System.Windows.Forms.Label lblDoctor;
        private System.Windows.Forms.Panel panelSidebar;
        private FontAwesome.Sharp.IconButton btnScroll;
        private FontAwesome.Sharp.IconButton btnFileCirclePlus;
        private System.Windows.Forms.Panel panelContent;
        private FontAwesome.Sharp.IconButton iconLogOut;
        private FontAwesome.Sharp.IconButton btnCheckIn;
    }
}
