
namespace Dashboard
{
    partial class Main
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.TbSourceDir = new System.Windows.Forms.TextBox();
            this.BtPathSourceDir = new System.Windows.Forms.Button();
            this.BtPathNewDir = new System.Windows.Forms.Button();
            this.RtbLogs = new System.Windows.Forms.RichTextBox();
            this.TbKeysDir = new System.Windows.Forms.TextBox();
            this.BtPathKeys = new System.Windows.Forms.Button();
            this.TbNewDir = new System.Windows.Forms.TextBox();
            this.ChbSourceDir = new System.Windows.Forms.CheckBox();
            this.ChbNewDir = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.BtEncrypt = new System.Windows.Forms.Button();
            this.BtDecrypt = new System.Windows.Forms.Button();
            this.BtClearConsole = new System.Windows.Forms.Button();
            this.ChbCustomKeysName = new System.Windows.Forms.CheckBox();
            this.LbEnterName = new System.Windows.Forms.Label();
            this.TbCustomKeysName = new System.Windows.Forms.TextBox();
            this.formStyleComponent1 = new Dashboard.FormStyleComponent(this.components);
            this.SuspendLayout();
            // 
            // TbSourceDir
            // 
            this.TbSourceDir.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(79)))), ((int)(((byte)(99)))));
            this.TbSourceDir.Font = new System.Drawing.Font("Corbel", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TbSourceDir.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.TbSourceDir.Location = new System.Drawing.Point(318, 14);
            this.TbSourceDir.Multiline = true;
            this.TbSourceDir.Name = "TbSourceDir";
            this.TbSourceDir.ReadOnly = true;
            this.TbSourceDir.Size = new System.Drawing.Size(292, 25);
            this.TbSourceDir.TabIndex = 2;
            this.TbSourceDir.Text = "Path to source directory";
            // 
            // BtPathSourceDir
            // 
            this.BtPathSourceDir.Location = new System.Drawing.Point(630, 14);
            this.BtPathSourceDir.Name = "BtPathSourceDir";
            this.BtPathSourceDir.Size = new System.Drawing.Size(25, 25);
            this.BtPathSourceDir.TabIndex = 3;
            this.BtPathSourceDir.Text = "...";
            this.BtPathSourceDir.UseVisualStyleBackColor = true;
            this.BtPathSourceDir.Click += new System.EventHandler(this.BtPathSourceDir_Click);
            // 
            // BtPathNewDir
            // 
            this.BtPathNewDir.Location = new System.Drawing.Point(630, 57);
            this.BtPathNewDir.Name = "BtPathNewDir";
            this.BtPathNewDir.Size = new System.Drawing.Size(25, 25);
            this.BtPathNewDir.TabIndex = 5;
            this.BtPathNewDir.Text = "...";
            this.BtPathNewDir.UseVisualStyleBackColor = true;
            this.BtPathNewDir.Click += new System.EventHandler(this.BtPathNewDir_Click);
            // 
            // RtbLogs
            // 
            this.RtbLogs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(42)))), ((int)(((byte)(64)))));
            this.RtbLogs.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.RtbLogs.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RtbLogs.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(126)))), ((int)(((byte)(249)))));
            this.RtbLogs.Location = new System.Drawing.Point(12, 12);
            this.RtbLogs.Name = "RtbLogs";
            this.RtbLogs.ReadOnly = true;
            this.RtbLogs.Size = new System.Drawing.Size(286, 200);
            this.RtbLogs.TabIndex = 7;
            this.RtbLogs.Text = "";
            // 
            // TbKeysDir
            // 
            this.TbKeysDir.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(79)))), ((int)(((byte)(99)))));
            this.TbKeysDir.Font = new System.Drawing.Font("Corbel", 12F, System.Drawing.FontStyle.Bold);
            this.TbKeysDir.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.TbKeysDir.Location = new System.Drawing.Point(318, 101);
            this.TbKeysDir.Multiline = true;
            this.TbKeysDir.Name = "TbKeysDir";
            this.TbKeysDir.ReadOnly = true;
            this.TbKeysDir.Size = new System.Drawing.Size(292, 25);
            this.TbKeysDir.TabIndex = 11;
            this.TbKeysDir.Text = "Path for saving keys (will in  new dir)";
            // 
            // BtPathKeys
            // 
            this.BtPathKeys.Location = new System.Drawing.Point(630, 101);
            this.BtPathKeys.Name = "BtPathKeys";
            this.BtPathKeys.Size = new System.Drawing.Size(25, 25);
            this.BtPathKeys.TabIndex = 12;
            this.BtPathKeys.Text = "...";
            this.BtPathKeys.UseVisualStyleBackColor = true;
            this.BtPathKeys.Click += new System.EventHandler(this.BtPathKeys_Click);
            // 
            // TbNewDir
            // 
            this.TbNewDir.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(79)))), ((int)(((byte)(99)))));
            this.TbNewDir.Font = new System.Drawing.Font("Corbel", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TbNewDir.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.TbNewDir.Location = new System.Drawing.Point(318, 57);
            this.TbNewDir.Multiline = true;
            this.TbNewDir.Name = "TbNewDir";
            this.TbNewDir.ReadOnly = true;
            this.TbNewDir.Size = new System.Drawing.Size(292, 25);
            this.TbNewDir.TabIndex = 2;
            this.TbNewDir.Text = "Path to new directory";
            // 
            // ChbSourceDir
            // 
            this.ChbSourceDir.AutoCheck = false;
            this.ChbSourceDir.AutoSize = true;
            this.ChbSourceDir.Location = new System.Drawing.Point(303, 20);
            this.ChbSourceDir.Name = "ChbSourceDir";
            this.ChbSourceDir.Size = new System.Drawing.Size(15, 14);
            this.ChbSourceDir.TabIndex = 14;
            this.ChbSourceDir.UseVisualStyleBackColor = true;
            // 
            // ChbNewDir
            // 
            this.ChbNewDir.AutoCheck = false;
            this.ChbNewDir.AutoSize = true;
            this.ChbNewDir.Location = new System.Drawing.Point(303, 63);
            this.ChbNewDir.Name = "ChbNewDir";
            this.ChbNewDir.Size = new System.Drawing.Size(15, 14);
            this.ChbNewDir.TabIndex = 15;
            this.ChbNewDir.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(317, 139);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 17;
            // 
            // BtEncrypt
            // 
            this.BtEncrypt.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtEncrypt.Font = new System.Drawing.Font("Nirmala UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtEncrypt.ForeColor = System.Drawing.Color.MediumPurple;
            this.BtEncrypt.Image = ((System.Drawing.Image)(resources.GetObject("BtEncrypt.Image")));
            this.BtEncrypt.Location = new System.Drawing.Point(12, 217);
            this.BtEncrypt.Name = "BtEncrypt";
            this.BtEncrypt.Size = new System.Drawing.Size(143, 56);
            this.BtEncrypt.TabIndex = 21;
            this.BtEncrypt.Text = "Start encrypting";
            this.BtEncrypt.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.BtEncrypt.UseVisualStyleBackColor = true;
            this.BtEncrypt.Click += new System.EventHandler(this.BtEncrypt_Click);
            // 
            // BtDecrypt
            // 
            this.BtDecrypt.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtDecrypt.Font = new System.Drawing.Font("Nirmala UI", 9F, System.Drawing.FontStyle.Bold);
            this.BtDecrypt.ForeColor = System.Drawing.Color.MediumPurple;
            this.BtDecrypt.Image = ((System.Drawing.Image)(resources.GetObject("BtDecrypt.Image")));
            this.BtDecrypt.Location = new System.Drawing.Point(155, 217);
            this.BtDecrypt.Name = "BtDecrypt";
            this.BtDecrypt.Size = new System.Drawing.Size(143, 56);
            this.BtDecrypt.TabIndex = 22;
            this.BtDecrypt.Text = "Start decrypting";
            this.BtDecrypt.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.BtDecrypt.UseVisualStyleBackColor = true;
            this.BtDecrypt.Click += new System.EventHandler(this.BtDecrypt_Click);
            // 
            // BtClearConsole
            // 
            this.BtClearConsole.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(51)))), ((int)(((byte)(79)))));
            this.BtClearConsole.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtClearConsole.Font = new System.Drawing.Font("Roboto", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtClearConsole.ForeColor = System.Drawing.Color.MediumPurple;
            this.BtClearConsole.Location = new System.Drawing.Point(12, 279);
            this.BtClearConsole.Name = "BtClearConsole";
            this.BtClearConsole.Size = new System.Drawing.Size(286, 35);
            this.BtClearConsole.TabIndex = 24;
            this.BtClearConsole.Text = "Clear console";
            this.BtClearConsole.UseVisualStyleBackColor = false;
            this.BtClearConsole.Click += new System.EventHandler(this.BtClearConsole_Click);
            // 
            // ChbCustomKeysName
            // 
            this.ChbCustomKeysName.AutoSize = true;
            this.ChbCustomKeysName.Font = new System.Drawing.Font("Corbel", 12F, System.Drawing.FontStyle.Bold);
            this.ChbCustomKeysName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.ChbCustomKeysName.Location = new System.Drawing.Point(303, 155);
            this.ChbCustomKeysName.Name = "ChbCustomKeysName";
            this.ChbCustomKeysName.Size = new System.Drawing.Size(158, 23);
            this.ChbCustomKeysName.TabIndex = 25;
            this.ChbCustomKeysName.Text = "Custom keys name";
            this.ChbCustomKeysName.UseVisualStyleBackColor = true;
            this.ChbCustomKeysName.CheckedChanged += new System.EventHandler(this.ChbCustomKeysName_CheckedChanged);
            // 
            // LbEnterName
            // 
            this.LbEnterName.AutoSize = true;
            this.LbEnterName.Font = new System.Drawing.Font("Corbel", 12F, System.Drawing.FontStyle.Bold);
            this.LbEnterName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.LbEnterName.Location = new System.Drawing.Point(316, 181);
            this.LbEnterName.Name = "LbEnterName";
            this.LbEnterName.Size = new System.Drawing.Size(123, 19);
            this.LbEnterName.TabIndex = 27;
            this.LbEnterName.Text = "Enter name here";
            this.LbEnterName.Visible = false;
            // 
            // TbCustomKeysName
            // 
            this.TbCustomKeysName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(79)))), ((int)(((byte)(99)))));
            this.TbCustomKeysName.Font = new System.Drawing.Font("Corbel", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TbCustomKeysName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.TbCustomKeysName.Location = new System.Drawing.Point(320, 217);
            this.TbCustomKeysName.Name = "TbCustomKeysName";
            this.TbCustomKeysName.Size = new System.Drawing.Size(292, 27);
            this.TbCustomKeysName.TabIndex = 28;
            this.TbCustomKeysName.Visible = false;
            this.TbCustomKeysName.TextChanged += new System.EventHandler(this.TbCustomKeysName_TextChanged);
            // 
            // formStyleComponent1
            // 
            this.formStyleComponent1.Form = this;
            this.formStyleComponent1.FormStyle = Dashboard.FormStyleComponent.FStyle.None;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(51)))), ((int)(((byte)(73)))));
            this.ClientSize = new System.Drawing.Size(667, 385);
            this.Controls.Add(this.TbCustomKeysName);
            this.Controls.Add(this.LbEnterName);
            this.Controls.Add(this.ChbCustomKeysName);
            this.Controls.Add(this.BtClearConsole);
            this.Controls.Add(this.BtDecrypt);
            this.Controls.Add(this.BtEncrypt);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ChbNewDir);
            this.Controls.Add(this.ChbSourceDir);
            this.Controls.Add(this.BtPathKeys);
            this.Controls.Add(this.TbKeysDir);
            this.Controls.Add(this.RtbLogs);
            this.Controls.Add(this.BtPathNewDir);
            this.Controls.Add(this.BtPathSourceDir);
            this.Controls.Add(this.TbNewDir);
            this.Controls.Add(this.TbSourceDir);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox TbSourceDir;
        private System.Windows.Forms.Button BtPathSourceDir;
        private System.Windows.Forms.Button BtPathNewDir;
        private System.Windows.Forms.RichTextBox RtbLogs;
        private System.Windows.Forms.TextBox TbKeysDir;
        private System.Windows.Forms.Button BtPathKeys;
        private System.Windows.Forms.TextBox TbNewDir;
        private System.Windows.Forms.CheckBox ChbSourceDir;
        private System.Windows.Forms.CheckBox ChbNewDir;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BtDecrypt;
        private System.Windows.Forms.Button BtEncrypt;
        private FormStyleComponent formStyleComponent1;
        private System.Windows.Forms.Button BtClearConsole;
        private System.Windows.Forms.CheckBox ChbCustomKeysName;
        private System.Windows.Forms.Label LbEnterName;
        private System.Windows.Forms.TextBox TbCustomKeysName;
    }
}

