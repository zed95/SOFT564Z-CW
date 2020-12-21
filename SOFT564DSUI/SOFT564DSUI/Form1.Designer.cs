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
            this.buggyConnectBtn = new System.Windows.Forms.Button();
            this.BuggyControlsLbl = new System.Windows.Forms.Label();
            this.buggyDisconnectBtn = new System.Windows.Forms.Button();
            this.textBoxBuggyConnectStatus = new System.Windows.Forms.TextBox();
            this.BuggyConnectStatusLbl = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.comboBoxIntMode = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.buttonForward = new System.Windows.Forms.Button();
            this.buttonLeft = new System.Windows.Forms.Button();
            this.buttonRight = new System.Windows.Forms.Button();
            this.buttonReverse = new System.Windows.Forms.Button();
            this.buttonReqData = new System.Windows.Forms.Button();
            this.comboBoxConfig = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxCurrConfig = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textBoxNewConfig = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.buttonConfigUpdate = new System.Windows.Forms.Button();
            this.textBoxConfigStatus = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // SendButton
            // 
            this.SendButton.Location = new System.Drawing.Point(200, 72);
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
            this.ipAddressTB.Text = "192.168.0.95";
            // 
            // portTB
            // 
            this.portTB.Location = new System.Drawing.Point(230, 46);
            this.portTB.Name = "portTB";
            this.portTB.Size = new System.Drawing.Size(71, 20);
            this.portTB.TabIndex = 5;
            this.portTB.Text = "11000";
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
            this.label3.Location = new System.Drawing.Point(7, 22);
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
            // buggyConnectBtn
            // 
            this.buggyConnectBtn.Location = new System.Drawing.Point(12, 133);
            this.buggyConnectBtn.Name = "buggyConnectBtn";
            this.buggyConnectBtn.Size = new System.Drawing.Size(75, 23);
            this.buggyConnectBtn.TabIndex = 23;
            this.buggyConnectBtn.Text = "Connect";
            this.buggyConnectBtn.UseVisualStyleBackColor = true;
            this.buggyConnectBtn.Click += new System.EventHandler(this.buggyConnectBtn_Click);
            // 
            // BuggyControlsLbl
            // 
            this.BuggyControlsLbl.AutoSize = true;
            this.BuggyControlsLbl.Location = new System.Drawing.Point(372, 93);
            this.BuggyControlsLbl.Name = "BuggyControlsLbl";
            this.BuggyControlsLbl.Size = new System.Drawing.Size(78, 13);
            this.BuggyControlsLbl.TabIndex = 24;
            this.BuggyControlsLbl.Text = "Buggy Controls";
            // 
            // buggyDisconnectBtn
            // 
            this.buggyDisconnectBtn.Location = new System.Drawing.Point(93, 133);
            this.buggyDisconnectBtn.Name = "buggyDisconnectBtn";
            this.buggyDisconnectBtn.Size = new System.Drawing.Size(75, 23);
            this.buggyDisconnectBtn.TabIndex = 25;
            this.buggyDisconnectBtn.Text = "Disconnect";
            this.buggyDisconnectBtn.UseVisualStyleBackColor = true;
            this.buggyDisconnectBtn.Click += new System.EventHandler(this.BuggyDisconnectBtn_Click);
            // 
            // textBoxBuggyConnectStatus
            // 
            this.textBoxBuggyConnectStatus.Location = new System.Drawing.Point(174, 133);
            this.textBoxBuggyConnectStatus.Name = "textBoxBuggyConnectStatus";
            this.textBoxBuggyConnectStatus.ReadOnly = true;
            this.textBoxBuggyConnectStatus.Size = new System.Drawing.Size(260, 20);
            this.textBoxBuggyConnectStatus.TabIndex = 26;
            // 
            // BuggyConnectStatusLbl
            // 
            this.BuggyConnectStatusLbl.AutoSize = true;
            this.BuggyConnectStatusLbl.Location = new System.Drawing.Point(171, 117);
            this.BuggyConnectStatusLbl.Name = "BuggyConnectStatusLbl";
            this.BuggyConnectStatusLbl.Size = new System.Drawing.Size(37, 13);
            this.BuggyConnectStatusLbl.TabIndex = 27;
            this.BuggyConnectStatusLbl.Text = "Status";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(42, 117);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(94, 13);
            this.label9.TabIndex = 28;
            this.label9.Text = "Buggy Connection";
            // 
            // comboBoxIntMode
            // 
            this.comboBoxIntMode.FormattingEnabled = true;
            this.comboBoxIntMode.Location = new System.Drawing.Point(15, 183);
            this.comboBoxIntMode.Name = "comboBoxIntMode";
            this.comboBoxIntMode.Size = new System.Drawing.Size(153, 21);
            this.comboBoxIntMode.TabIndex = 29;
            this.comboBoxIntMode.SelectedIndexChanged += new System.EventHandler(this.comboBoxIntMode_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 167);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(87, 13);
            this.label10.TabIndex = 30;
            this.label10.Text = "Interaction Mode";
            // 
            // buttonForward
            // 
            this.buttonForward.Location = new System.Drawing.Point(50, 241);
            this.buttonForward.Name = "buttonForward";
            this.buttonForward.Size = new System.Drawing.Size(75, 23);
            this.buttonForward.TabIndex = 31;
            this.buttonForward.Text = "Forward";
            this.buttonForward.UseVisualStyleBackColor = true;
            this.buttonForward.Click += new System.EventHandler(this.buttonForward_Click);
            // 
            // buttonLeft
            // 
            this.buttonLeft.Location = new System.Drawing.Point(12, 270);
            this.buttonLeft.Name = "buttonLeft";
            this.buttonLeft.Size = new System.Drawing.Size(75, 23);
            this.buttonLeft.TabIndex = 32;
            this.buttonLeft.Text = "Left";
            this.buttonLeft.UseVisualStyleBackColor = true;
            this.buttonLeft.Click += new System.EventHandler(this.buttonLeft_Click);
            // 
            // buttonRight
            // 
            this.buttonRight.Location = new System.Drawing.Point(93, 270);
            this.buttonRight.Name = "buttonRight";
            this.buttonRight.Size = new System.Drawing.Size(75, 23);
            this.buttonRight.TabIndex = 33;
            this.buttonRight.Text = "Right";
            this.buttonRight.UseVisualStyleBackColor = true;
            this.buttonRight.Click += new System.EventHandler(this.buttonRight_Click);
            // 
            // buttonReverse
            // 
            this.buttonReverse.Location = new System.Drawing.Point(50, 299);
            this.buttonReverse.Name = "buttonReverse";
            this.buttonReverse.Size = new System.Drawing.Size(75, 23);
            this.buttonReverse.TabIndex = 34;
            this.buttonReverse.Text = "Reverse";
            this.buttonReverse.UseVisualStyleBackColor = true;
            this.buttonReverse.Click += new System.EventHandler(this.buttonReverse_Click);
            // 
            // buttonReqData
            // 
            this.buttonReqData.Location = new System.Drawing.Point(572, 296);
            this.buttonReqData.Name = "buttonReqData";
            this.buttonReqData.Size = new System.Drawing.Size(91, 23);
            this.buttonReqData.TabIndex = 35;
            this.buttonReqData.Text = "Request Data";
            this.buttonReqData.UseVisualStyleBackColor = true;
            this.buttonReqData.Click += new System.EventHandler(this.buttonReqData_Click);
            // 
            // comboBoxConfig
            // 
            this.comboBoxConfig.FormattingEnabled = true;
            this.comboBoxConfig.Location = new System.Drawing.Point(258, 218);
            this.comboBoxConfig.Name = "comboBoxConfig";
            this.comboBoxConfig.Size = new System.Drawing.Size(153, 21);
            this.comboBoxConfig.TabIndex = 36;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(211, 193);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(69, 13);
            this.label11.TabIndex = 37;
            this.label11.Text = "Configuration";
            // 
            // textBoxCurrConfig
            // 
            this.textBoxCurrConfig.Location = new System.Drawing.Point(258, 245);
            this.textBoxCurrConfig.Name = "textBoxCurrConfig";
            this.textBoxCurrConfig.Size = new System.Drawing.Size(153, 20);
            this.textBoxCurrConfig.TabIndex = 38;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(211, 248);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(41, 13);
            this.label12.TabIndex = 39;
            this.label12.Text = "Current";
            // 
            // textBoxNewConfig
            // 
            this.textBoxNewConfig.Location = new System.Drawing.Point(258, 271);
            this.textBoxNewConfig.Name = "textBoxNewConfig";
            this.textBoxNewConfig.Size = new System.Drawing.Size(153, 20);
            this.textBoxNewConfig.TabIndex = 40;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(223, 276);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(29, 13);
            this.label13.TabIndex = 41;
            this.label13.Text = "New";
            // 
            // buttonConfigUpdate
            // 
            this.buttonConfigUpdate.Location = new System.Drawing.Point(336, 299);
            this.buttonConfigUpdate.Name = "buttonConfigUpdate";
            this.buttonConfigUpdate.Size = new System.Drawing.Size(75, 23);
            this.buttonConfigUpdate.TabIndex = 42;
            this.buttonConfigUpdate.Text = "Update";
            this.buttonConfigUpdate.UseVisualStyleBackColor = true;
            // 
            // textBoxConfigStatus
            // 
            this.textBoxConfigStatus.Location = new System.Drawing.Point(258, 301);
            this.textBoxConfigStatus.Name = "textBoxConfigStatus";
            this.textBoxConfigStatus.Size = new System.Drawing.Size(72, 20);
            this.textBoxConfigStatus.TabIndex = 43;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(215, 304);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(37, 13);
            this.label14.TabIndex = 44;
            this.label14.Text = "Status";
            // 
            // label15
            // 
            this.label15.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label15.Location = new System.Drawing.Point(7, 212);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(177, 3);
            this.label15.TabIndex = 45;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(9, 216);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(78, 13);
            this.label16.TabIndex = 46;
            this.label16.Text = "Manual Control";
            // 
            // label17
            // 
            this.label17.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label17.Location = new System.Drawing.Point(211, 190);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(223, 3);
            this.label17.TabIndex = 47;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(214, 221);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(38, 13);
            this.label18.TabIndex = 48;
            this.label18.Text = "Modify";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(564, 9);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(122, 13);
            this.label19.TabIndex = 49;
            this.label19.Text = "Server Connect Controls";
            // 
            // ClientGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(692, 331);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.textBoxConfigStatus);
            this.Controls.Add(this.buttonConfigUpdate);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.textBoxNewConfig);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.textBoxCurrConfig);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.comboBoxConfig);
            this.Controls.Add(this.buttonReqData);
            this.Controls.Add(this.buttonReverse);
            this.Controls.Add(this.buttonRight);
            this.Controls.Add(this.buttonLeft);
            this.Controls.Add(this.buttonForward);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.comboBoxIntMode);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.BuggyConnectStatusLbl);
            this.Controls.Add(this.textBoxBuggyConnectStatus);
            this.Controls.Add(this.buggyDisconnectBtn);
            this.Controls.Add(this.BuggyControlsLbl);
            this.Controls.Add(this.buggyConnectBtn);
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
            this.KeyPreview = true;
            this.Name = "ClientGUI";
            this.Text = "Client GUI SOFT564Z";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
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
        private System.Windows.Forms.Button buggyConnectBtn;
        private System.Windows.Forms.Label BuggyControlsLbl;
        private System.Windows.Forms.Button buggyDisconnectBtn;
        private System.Windows.Forms.TextBox textBoxBuggyConnectStatus;
        private System.Windows.Forms.Label BuggyConnectStatusLbl;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox comboBoxIntMode;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button buttonForward;
        private System.Windows.Forms.Button buttonLeft;
        private System.Windows.Forms.Button buttonRight;
        private System.Windows.Forms.Button buttonReverse;
        private System.Windows.Forms.Button buttonReqData;
        private System.Windows.Forms.ComboBox comboBoxConfig;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBoxCurrConfig;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBoxNewConfig;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button buttonConfigUpdate;
        private System.Windows.Forms.TextBox textBoxConfigStatus;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
    }
}

