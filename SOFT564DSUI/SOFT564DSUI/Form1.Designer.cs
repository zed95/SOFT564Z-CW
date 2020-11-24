namespace SOFT564DSUI
{
    partial class ClientGUI
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.SendButton = new System.Windows.Forms.Button();
            this.ipAddressTB = new System.Windows.Forms.TextBox();
            this.portTB = new System.Windows.Forms.TextBox();
            this.ipAddressLb = new System.Windows.Forms.Label();
            this.portLb = new System.Windows.Forms.Label();
            this.connectToServerBtn = new System.Windows.Forms.Button();
            this.statusTB = new System.Windows.Forms.TextBox();
            this.statusLb = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 114);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(422, 20);
            this.textBox1.TabIndex = 1;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(12, 140);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(422, 20);
            this.textBox2.TabIndex = 2;
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // SendButton
            // 
            this.SendButton.Location = new System.Drawing.Point(12, 166);
            this.SendButton.Name = "SendButton";
            this.SendButton.Size = new System.Drawing.Size(75, 23);
            this.SendButton.TabIndex = 3;
            this.SendButton.Text = "Send";
            this.SendButton.UseVisualStyleBackColor = true;
            this.SendButton.Click += new System.EventHandler(this.SendButton_Click);
            // 
            // ipAddressTB
            // 
            this.ipAddressTB.Location = new System.Drawing.Point(12, 29);
            this.ipAddressTB.Name = "ipAddressTB";
            this.ipAddressTB.Size = new System.Drawing.Size(212, 20);
            this.ipAddressTB.TabIndex = 4;
            // 
            // portTB
            // 
            this.portTB.Location = new System.Drawing.Point(230, 29);
            this.portTB.Name = "portTB";
            this.portTB.Size = new System.Drawing.Size(71, 20);
            this.portTB.TabIndex = 5;
            this.portTB.TextChanged += new System.EventHandler(this.portTB_TextChanged);
            // 
            // ipAddressLb
            // 
            this.ipAddressLb.AutoSize = true;
            this.ipAddressLb.Location = new System.Drawing.Point(9, 13);
            this.ipAddressLb.Name = "ipAddressLb";
            this.ipAddressLb.Size = new System.Drawing.Size(58, 13);
            this.ipAddressLb.TabIndex = 6;
            this.ipAddressLb.Text = "IP Address";
            // 
            // portLb
            // 
            this.portLb.AutoSize = true;
            this.portLb.Location = new System.Drawing.Point(227, 13);
            this.portLb.Name = "portLb";
            this.portLb.Size = new System.Drawing.Size(26, 13);
            this.portLb.TabIndex = 7;
            this.portLb.Text = "Port";
            // 
            // connectToServerBtn
            // 
            this.connectToServerBtn.Location = new System.Drawing.Point(12, 55);
            this.connectToServerBtn.Name = "connectToServerBtn";
            this.connectToServerBtn.Size = new System.Drawing.Size(75, 23);
            this.connectToServerBtn.TabIndex = 8;
            this.connectToServerBtn.Text = "Connect";
            this.connectToServerBtn.UseVisualStyleBackColor = true;
            this.connectToServerBtn.Click += new System.EventHandler(this.connectToServerBtn_Click);
            // 
            // statusTB
            // 
            this.statusTB.Location = new System.Drawing.Point(307, 29);
            this.statusTB.Name = "statusTB";
            this.statusTB.ReadOnly = true;
            this.statusTB.Size = new System.Drawing.Size(127, 20);
            this.statusTB.TabIndex = 9;
            this.statusTB.Text = "Disconnected";
            // 
            // statusLb
            // 
            this.statusLb.AutoSize = true;
            this.statusLb.Location = new System.Drawing.Point(304, 13);
            this.statusLb.Name = "statusLb";
            this.statusLb.Size = new System.Drawing.Size(37, 13);
            this.statusLb.TabIndex = 10;
            this.statusLb.Text = "Status";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(456, 29);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(224, 134);
            this.listBox1.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(453, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Available Clients";
            // 
            // ClientGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(692, 268);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.statusLb);
            this.Controls.Add(this.statusTB);
            this.Controls.Add(this.connectToServerBtn);
            this.Controls.Add(this.portLb);
            this.Controls.Add(this.ipAddressLb);
            this.Controls.Add(this.portTB);
            this.Controls.Add(this.ipAddressTB);
            this.Controls.Add(this.SendButton);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.KeyPreview = true;
            this.Name = "ClientGUI";
            this.Text = "Client GUI SOFT564Z";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button SendButton;
        private System.Windows.Forms.TextBox ipAddressTB;
        private System.Windows.Forms.TextBox portTB;
        private System.Windows.Forms.Label ipAddressLb;
        private System.Windows.Forms.Label portLb;
        private System.Windows.Forms.Button connectToServerBtn;
        private System.Windows.Forms.TextBox statusTB;
        private System.Windows.Forms.Label statusLb;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label label1;
    }
}

