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
            this.TempTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.HumTextBox = new System.Windows.Forms.TextBox();
            this.LIntTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
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
            this.SendButton.Location = new System.Drawing.Point(12, 170);
            this.SendButton.Name = "SendButton";
            this.SendButton.Size = new System.Drawing.Size(75, 23);
            this.SendButton.TabIndex = 3;
            this.SendButton.Text = "Send";
            this.SendButton.UseVisualStyleBackColor = true;
            this.SendButton.Click += new System.EventHandler(this.SendButton_Click);
            // 
            // ipAddressTB
            // 
            this.ipAddressTB.Location = new System.Drawing.Point(12, 46);
            this.ipAddressTB.Name = "ipAddressTB";
            this.ipAddressTB.Size = new System.Drawing.Size(212, 20);
            this.ipAddressTB.TabIndex = 4;
            this.ipAddressTB.Text = "192.168.0.54";
            // 
            // portTB
            // 
            this.portTB.Location = new System.Drawing.Point(230, 46);
            this.portTB.Name = "portTB";
            this.portTB.Size = new System.Drawing.Size(71, 20);
            this.portTB.TabIndex = 5;
            this.portTB.Text = "80";
            this.portTB.TextChanged += new System.EventHandler(this.portTB_TextChanged);
            // 
            // ipAddressLb
            // 
            this.ipAddressLb.AutoSize = true;
            this.ipAddressLb.Location = new System.Drawing.Point(9, 30);
            this.ipAddressLb.Name = "ipAddressLb";
            this.ipAddressLb.Size = new System.Drawing.Size(58, 13);
            this.ipAddressLb.TabIndex = 6;
            this.ipAddressLb.Text = "IP Address";
            // 
            // portLb
            // 
            this.portLb.AutoSize = true;
            this.portLb.Location = new System.Drawing.Point(227, 30);
            this.portLb.Name = "portLb";
            this.portLb.Size = new System.Drawing.Size(26, 13);
            this.portLb.TabIndex = 7;
            this.portLb.Text = "Port";
            // 
            // connectToServerBtn
            // 
            this.connectToServerBtn.Location = new System.Drawing.Point(12, 72);
            this.connectToServerBtn.Name = "connectToServerBtn";
            this.connectToServerBtn.Size = new System.Drawing.Size(75, 23);
            this.connectToServerBtn.TabIndex = 8;
            this.connectToServerBtn.Text = "Connect";
            this.connectToServerBtn.UseVisualStyleBackColor = true;
            this.connectToServerBtn.Click += new System.EventHandler(this.connectToServerBtn_Click);
            // 
            // statusTB
            // 
            this.statusTB.Location = new System.Drawing.Point(307, 46);
            this.statusTB.Name = "statusTB";
            this.statusTB.ReadOnly = true;
            this.statusTB.Size = new System.Drawing.Size(127, 20);
            this.statusTB.TabIndex = 9;
            this.statusTB.Text = "Disconnected";
            // 
            // statusLb
            // 
            this.statusLb.AutoSize = true;
            this.statusLb.Location = new System.Drawing.Point(304, 30);
            this.statusLb.Name = "statusLb";
            this.statusLb.Size = new System.Drawing.Size(37, 13);
            this.statusLb.TabIndex = 10;
            this.statusLb.Text = "Status";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(456, 46);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(224, 134);
            this.listBox1.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(453, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Available Clients";
            // 
            // TempTextBox
            // 
            this.TempTextBox.Location = new System.Drawing.Point(555, 216);
            this.TempTextBox.Name = "TempTextBox";
            this.TempTextBox.Size = new System.Drawing.Size(125, 20);
            this.TempTextBox.TabIndex = 13;
            this.TempTextBox.Text = "0";
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Location = new System.Drawing.Point(7, 108);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(443, 3);
            this.label2.TabIndex = 14;
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label3.Location = new System.Drawing.Point(7, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(679, 3);
            this.label3.TabIndex = 15;
            // 
            // label4
            // 
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label4.Location = new System.Drawing.Point(457, 190);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(223, 3);
            this.label4.TabIndex = 16;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(458, 193);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(91, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "Greenhouse Data";
            // 
            // HumTextBox
            // 
            this.HumTextBox.Location = new System.Drawing.Point(555, 242);
            this.HumTextBox.Name = "HumTextBox";
            this.HumTextBox.Size = new System.Drawing.Size(125, 20);
            this.HumTextBox.TabIndex = 18;
            this.HumTextBox.Text = "0";
            // 
            // LIntTextBox
            // 
            this.LIntTextBox.Location = new System.Drawing.Point(555, 268);
            this.LIntTextBox.Name = "LIntTextBox";
            this.LIntTextBox.Size = new System.Drawing.Size(125, 20);
            this.LIntTextBox.TabIndex = 19;
            this.LIntTextBox.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(466, 219);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(83, 13);
            this.label6.TabIndex = 20;
            this.label6.Text = "Temperature (C)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(485, 245);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(64, 13);
            this.label7.TabIndex = 21;
            this.label7.Text = "Humidity (%)";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(477, 273);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(72, 13);
            this.label8.TabIndex = 22;
            this.label8.Text = "Light Intensity";
            // 
            // ClientGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(692, 308);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.LIntTextBox);
            this.Controls.Add(this.HumTextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TempTextBox);
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
        private System.Windows.Forms.TextBox TempTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox HumTextBox;
        private System.Windows.Forms.TextBox LIntTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
    }
}

