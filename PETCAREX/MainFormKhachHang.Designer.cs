namespace PETCAREX
{
    partial class MainFormKhachHang
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
            this.btnShop = new FontAwesome.Sharp.IconButton();
            this.btnDoc = new FontAwesome.Sharp.IconButton();
            this.btnHistory = new FontAwesome.Sharp.IconButton();
            this.btnCreate = new FontAwesome.Sharp.IconButton();
            this.btnFind = new FontAwesome.Sharp.IconButton();
            this.panelContent = new System.Windows.Forms.Panel();
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
            this.panelSidebar.Controls.Add(this.btnShop);
            this.panelSidebar.Controls.Add(this.btnDoc);
            this.panelSidebar.Controls.Add(this.btnHistory);
            this.panelSidebar.Controls.Add(this.btnCreate);
            this.panelSidebar.Controls.Add(this.btnFind);
            this.panelSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelSidebar.Location = new System.Drawing.Point(0, 98);
            this.panelSidebar.Margin = new System.Windows.Forms.Padding(4);
            this.panelSidebar.Name = "panelSidebar";
            this.panelSidebar.Size = new System.Drawing.Size(120, 764);
            this.panelSidebar.TabIndex = 1;
            // 
            // btnShop
            // 
            this.btnShop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.btnShop.FlatAppearance.BorderSize = 0;
            this.btnShop.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnShop.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.btnShop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShop.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShop.IconChar = FontAwesome.Sharp.IconChar.CartPlus;
            this.btnShop.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnShop.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnShop.IconSize = 50;
            this.btnShop.Location = new System.Drawing.Point(0, 472);
            this.btnShop.Margin = new System.Windows.Forms.Padding(4);
            this.btnShop.Name = "btnShop";
            this.btnShop.Size = new System.Drawing.Size(120, 120);
            this.btnShop.TabIndex = 3;
            this.btnShop.UseVisualStyleBackColor = false;
            // 
            // btnDoc
            // 
            this.btnDoc.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.btnDoc.FlatAppearance.BorderSize = 0;
            this.btnDoc.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnDoc.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.btnDoc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDoc.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDoc.IconChar = FontAwesome.Sharp.IconChar.UserMd;
            this.btnDoc.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnDoc.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnDoc.IconSize = 50;
            this.btnDoc.Location = new System.Drawing.Point(0, 119);
            this.btnDoc.Margin = new System.Windows.Forms.Padding(4);
            this.btnDoc.Name = "btnDoc";
            this.btnDoc.Size = new System.Drawing.Size(120, 120);
            this.btnDoc.TabIndex = 2;
            this.btnDoc.UseVisualStyleBackColor = false;
            // 
            // btnHistory
            // 
            this.btnHistory.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.btnHistory.FlatAppearance.BorderSize = 0;
            this.btnHistory.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnHistory.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.btnHistory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHistory.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHistory.IconChar = FontAwesome.Sharp.IconChar.ClockFour;
            this.btnHistory.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnHistory.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnHistory.IconSize = 50;
            this.btnHistory.Location = new System.Drawing.Point(0, 355);
            this.btnHistory.Margin = new System.Windows.Forms.Padding(4);
            this.btnHistory.Name = "btnHistory";
            this.btnHistory.Size = new System.Drawing.Size(120, 120);
            this.btnHistory.TabIndex = 3;
            this.btnHistory.UseVisualStyleBackColor = false;
            // 
            // btnCreate
            // 
            this.btnCreate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.btnCreate.FlatAppearance.BorderSize = 0;
            this.btnCreate.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnCreate.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.btnCreate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreate.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCreate.IconChar = FontAwesome.Sharp.IconChar.FileCirclePlus;
            this.btnCreate.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnCreate.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnCreate.IconSize = 50;
            this.btnCreate.Location = new System.Drawing.Point(0, 236);
            this.btnCreate.Margin = new System.Windows.Forms.Padding(4);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(120, 120);
            this.btnCreate.TabIndex = 4;
            this.btnCreate.UseVisualStyleBackColor = false;
            // 
            // btnFind
            // 
            this.btnFind.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.btnFind.FlatAppearance.BorderSize = 0;
            this.btnFind.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnFind.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.btnFind.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFind.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFind.IconChar = FontAwesome.Sharp.IconChar.MagnifyingGlass;
            this.btnFind.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnFind.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnFind.IconSize = 50;
            this.btnFind.Location = new System.Drawing.Point(0, 0);
            this.btnFind.Margin = new System.Windows.Forms.Padding(4);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(120, 120);
            this.btnFind.TabIndex = 1;
            this.btnFind.UseVisualStyleBackColor = false;
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
            // MainFormKhachHang
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
            this.Name = "MainFormKhachHang";
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
        private FontAwesome.Sharp.IconButton btnFind;
        private FontAwesome.Sharp.IconButton btnDoc;
        private System.Windows.Forms.Panel panelContent;
        private FontAwesome.Sharp.IconButton iconLogOut;
        private FontAwesome.Sharp.IconButton btnShop;
        private FontAwesome.Sharp.IconButton btnHistory;
        private FontAwesome.Sharp.IconButton btnCreate;
    }
}