using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading;
using System.Security.Cryptography.X509Certificates;

namespace SOFT564DSUI
{
    public partial class ClientGUI : Form
    {
        static int x = 1;
        Thread grabData;
        public ClientGUI()
        {
            InitializeComponent();
            buggyConnectBtn.Enabled = false;
            buggyDisconnectBtn.Enabled = false;
            textBoxBuggyConnectStatus.Enabled = false;
            comboBoxIntMode.Enabled = false;
            comboBoxConfig.Enabled = false;
            buttonForward.Enabled = false;
            buttonReverse.Enabled = false;
            buttonRight.Enabled = false;
            buttonLeft.Enabled = false;
            textBoxConfigStatus.Enabled = false;
            textBoxCurrConfig.Enabled = false;
            textBoxConfigStatus.Enabled = false;
            buttonConfigUpdate.Enabled = false;
            comboBoxNewConfig.Enabled = false;
            TempTextBox.Enabled = false;
            HumTextBox.Enabled = false;
            LIntTextBox.Enabled = false;
            buttonReqData.Enabled = false;

            comboBoxIntMode.Items.Add("Manual");
            comboBoxIntMode.Items.Add("Autonomous");
            comboBoxIntMode.Items.Add("Configuration");

            comboBoxConfig.Items.Add("Autonomous Data T (ms)");          //sets the period data requests in autonomous mode.
            comboBoxConfig.Items.Add("Max Object Distance (cm)");        //Sets the closest distance that the buggy can be from object before it stops.
            comboBoxConfig.Items.Add("Buggy Speed");                //Changes Buggy speed.
            comboBoxConfig.Items.Add("Light Intensity Delta");      //used to set the difference in light levels which determines wheter the buggy should turn in autonomous mode.



            

            //stream.Close();
            //client.Close();
            //Console.ReadKey();

        }

        public void callDisplay()
        {
            String Message;
            bool remove = true;
            while(true) {


                if (listBox1.Items.Count < clientManager.Clients.Count)
                {
                    foreach (ConnectedClients client in clientManager.Clients)
                    {
                        if (listBox1.FindStringExact(client.clientID.ToString()) == -1) //if there any client in the connectedclient list that isn't on the list then add it.
                        {
                            listBox1.Invoke((MethodInvoker)(() => listBox1.Items.Add(client.clientID)));    //Add client to the list of available clients.
                        }
                    }
                }


                if (listBox1.Items.Count > clientManager.Clients.Count)
                {
                    for (int xx = 0; xx < listBox1.Items.Count; xx++)
                    {
                        Console.WriteLine("Listbox Counter: " + listBox1.Items.Count);
                        Console.WriteLine("Loop Increment : " + xx);
                        if (!clientManager.Clients.Exists(connectedClient => connectedClient.clientID == Convert.ToInt32(listBox1.Items[xx])))  //If client id on the list does not belong to any clients in the connected client list remove it from the list.
                        {
                            listBox1.Invoke((MethodInvoker)(() => listBox1.Items.RemoveAt(xx)));    //Remove the client from the available client list.
                            xx--;   //push the counter back once so that all items on the list are compared against the connected clients list. The list ios rearranged after removal of an item.
                            remove = false;
                        }

                    }
                }

                if((float.Parse(TempTextBox.Text) != EnvData.ftemperature) || (Int32.Parse(HumTextBox.Text) != EnvData.humidity) || (Int32.Parse(LIntTextBox.Text) != EnvData.lIntensity))
                {
                    TempTextBox.Invoke((MethodInvoker)(() => TempTextBox.Text = EnvData.ftemperature.ToString()));
                    HumTextBox.Invoke((MethodInvoker)(() => HumTextBox.Text = EnvData.humidity.ToString()));
                    LIntTextBox.Invoke((MethodInvoker)(() => LIntTextBox.Text = EnvData.lIntensity.ToString()));
                }


                if (TCPClient.dataAvailable)
                {
                    TCPClient.dataAvailable = false;
                }
                if(!TCPClient.ConnectionLost)
                {
                    connectToServerBtn.Invoke((MethodInvoker)(() => connectToServerBtn.Enabled = false));
                }
                if((TCPClient.ConnectionLost) && (connectToServerBtn.IsHandleCreated))
                {
                    connectToServerBtn.Invoke((MethodInvoker)(() => connectToServerBtn.Enabled = true));
                    statusTB.Invoke((MethodInvoker)(() => statusTB.Text = "Disconnected"));
                    Thread.CurrentThread.Abort();
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            String id = listBox1.GetItemText(listBox1.SelectedItem);
            //TCPClient.send(textBox1.Text, id);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!BuggyMotorControl.pauseMotorControl)
            {
                if (e.KeyCode == Keys.W)
                {
                    BuggyMotorControl.forward = true;
                }
                if (e.KeyCode == Keys.S)
                {
                    BuggyMotorControl.reverse = true;
                }
                if (e.KeyCode == Keys.D)
                {
                    BuggyMotorControl.right = true;
                }
                if (e.KeyCode == Keys.A)
                {
                    BuggyMotorControl.left = true;
                }
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {

            if (!BuggyMotorControl.pauseMotorControl)
            {
                if (e.KeyCode == Keys.W)
                {
                    BuggyMotorControl.forward = false;
                }
                if (e.KeyCode == Keys.S)
                {
                    BuggyMotorControl.reverse = false;
                }
                if (e.KeyCode == Keys.D)
                {
                    BuggyMotorControl.right = false;
                }
                if (e.KeyCode == Keys.A)
                {
                    BuggyMotorControl.left = false;
                }
            }

        }

        private void connectToServerBtn_Click(object sender, EventArgs e)
        {
            connectToServerBtn.Enabled = false;
            TCPClient.ConnectionLost = false;
            Int32 port = Int32.Parse(portTB.Text);
            TCPClient.initClient(ipAddressTB.Text, port);
            if (!TCPClient.ConnectionEstablished)
            {
                connectToServerBtn.Enabled = true;
                statusTB.Text = "Failed To Connect";
            }
            else if(TCPClient.ConnectionEstablished)
            {
                connectToServerBtn.Enabled = false;
                statusTB.Text = "Connected";

                //enable appropriate gui units
                buggyConnectBtn.Enabled = true;
                textBoxBuggyConnectStatus.Enabled = true;

                grabData = new Thread(callDisplay);
                grabData.Start();
            }

        }

        private void portTB_TextChanged(object sender, EventArgs e)
        {
            
             if(Regex.IsMatch(portTB.Text, @"[a-zA-Z!@#$%^&£*(),.?:{ }|<> _ +=[\]\""\;\'\~\¬\-`\\/]")) //if input box has any of these, disable connection button
            {
                connectToServerBtn.Enabled = false;
            }
            else
            {
                connectToServerBtn.Enabled = true;
            }
        }

        private void BuggyDisconnectBtn_Click(object sender, EventArgs e)
        {

        }

        private void buggyConnectBtn_Click(object sender, EventArgs e)
        {
            int index = 0;
            if(listBox1.SelectedIndex == -1)
            {
                textBoxBuggyConnectStatus.Text = "Select a client to connect to.";
            }
            else
            {
                MessageHandler.BuggyConnect(Convert.ToInt32(listBox1.SelectedItem));
                buggyConnectBtn.Enabled = false;
                while (BuggyConnectResponse.response == 0) { };     //Wait for the response from the server

                switch (BuggyConnectResponse.response)
                {
                    case BuggyConnectResponse.ConnectPermitted:
                        index = clientManager.Clients.FindIndex(x => x.clientID == Convert.ToInt32(listBox1.SelectedItem));
                        ConnectionManager.AddClient(clientManager.Clients[index].ipAddress, 80);
                        textBoxBuggyConnectStatus.Text = "Connected to buggy";
                        comboBoxIntMode.Enabled = true;
                        buggyConnectBtn.Enabled = false;
                        BuggyMotorControl.StartMotorControl();      //Start thread that will monitor motor control inputs if manual mode is on.
                        break;
                    case BuggyConnectResponse.BuggyInUse:
                        textBoxBuggyConnectStatus.Text = "Buggy is used by another client";
                        buggyConnectBtn.Enabled = true;
                        break;
                    default:

                        break;
                }

                BuggyConnectResponse.response = 0;  //Reset The response
            }
        }

        private void comboBoxIntMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(comboBoxIntMode.SelectedItem.ToString())
            {
                case "Manual":
                    buttonForward.Enabled = true;
                    buttonReverse.Enabled = true;
                    buttonRight.Enabled = true;
                    buttonLeft.Enabled = true;
                    TempTextBox.Enabled = true;
                    HumTextBox.Enabled = true;
                    LIntTextBox.Enabled = true;
                    buttonReqData.Enabled = true;
                    comboBoxConfig.Enabled = false;
                    textBoxCurrConfig.Enabled = false;
                    textBoxConfigStatus.Enabled = false;
                    buttonConfigUpdate.Enabled = false;
                    comboBoxNewConfig.Enabled = false;
                    BuggyMotorControl.RestartMotorControl();
                    MessageHandler.InteractionMode(InteractionMode.Manual);
                    break;
                case "Autonomous":
                    TempTextBox.Enabled = true;
                    HumTextBox.Enabled = true;
                    LIntTextBox.Enabled = true;
                    comboBoxConfig.Enabled = false;
                    buttonForward.Enabled = false;
                    buttonReverse.Enabled = false;
                    buttonRight.Enabled = false;
                    buttonLeft.Enabled = false;
                    textBoxCurrConfig.Enabled = false;
                    textBoxConfigStatus.Enabled = false;
                    buttonConfigUpdate.Enabled = false;
                    comboBoxNewConfig.Enabled = false;
                    buttonReqData.Enabled = false;
                    BuggyMotorControl.PauseMotorControl();
                    MessageHandler.InteractionMode(InteractionMode.Autonomous);
                    break;
                case "Configuration":
                    comboBoxConfig.Enabled = true;
                    buttonForward.Enabled = false;
                    buttonReverse.Enabled = false;
                    buttonRight.Enabled = false;
                    buttonLeft.Enabled = false;
                    textBoxCurrConfig.Enabled = true;
                    textBoxConfigStatus.Enabled = true;
                    buttonConfigUpdate.Enabled = true;
                    comboBoxNewConfig.Enabled = true;
                    TempTextBox.Enabled = false;
                    HumTextBox.Enabled = false;
                    LIntTextBox.Enabled = false;
                    buttonReqData.Enabled = false;
                    BuggyMotorControl.PauseMotorControl();
                    MessageHandler.InteractionMode(InteractionMode.Configuration);
                    break;
                default:
                    comboBoxConfig.Enabled = false;
                    buttonForward.Enabled = false;
                    buttonReverse.Enabled = false;
                    buttonRight.Enabled = false;
                    buttonLeft.Enabled = false;
                    textBoxConfigStatus.Enabled = false;
                    textBoxCurrConfig.Enabled = false;
                    buttonConfigUpdate.Enabled = false;
                    comboBoxNewConfig.Enabled = false;
                    TempTextBox.Enabled = false;
                    HumTextBox.Enabled = false;
                    LIntTextBox.Enabled = false;
                    buttonReqData.Enabled = false;
                    BuggyMotorControl.PauseMotorControl();
                    break;
            }
        }

        private void buttonReqData_Click(object sender, EventArgs e)
        {
            MessageHandler.SendEnvData();
        }

        private void buttonForward_Click(object sender, EventArgs e)
        {

        }

        private void buttonReverse_Click(object sender, EventArgs e)
        {

        }

        private void buttonRight_Click(object sender, EventArgs e)
        {

        }

        private void buttonLeft_Click(object sender, EventArgs e)
        {

        }

        private void comboBoxNewConfig_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBoxConfig_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxNewConfig.Items.Clear();                    //new config option selected, clear out old options from the New combo box.
                    
            switch (comboBoxConfig.SelectedItem.ToString())
            {
                case "Autonomous Data T (ms)":
                    comboBoxNewConfig.Items.Add(1000);
                    comboBoxNewConfig.Items.Add(500);
                    comboBoxNewConfig.Items.Add(200);
                    comboBoxNewConfig.Items.Add(100);
                    comboBoxNewConfig.Items.Add(50);
                    MessageHandler.CurrConfigParam(ConfigurationOptions.AutonomousDataT);
                    break;
                case "Max Object Distance (cm)":
                    comboBoxNewConfig.Items.Add(50);
                    comboBoxNewConfig.Items.Add(40);
                    comboBoxNewConfig.Items.Add(30);
                    comboBoxNewConfig.Items.Add(20);
                    comboBoxNewConfig.Items.Add(10);
                    MessageHandler.CurrConfigParam(ConfigurationOptions.MaxObjectDistance);
                    break;
                case "Buggy Speed":
                    comboBoxNewConfig.Items.Add(255);
                    comboBoxNewConfig.Items.Add(230);
                    comboBoxNewConfig.Items.Add(210);
                    comboBoxNewConfig.Items.Add(190);
                    comboBoxNewConfig.Items.Add(170);
                    MessageHandler.CurrConfigParam(ConfigurationOptions.BuggySpeed);
                    break;
                case "Light Intensity Delta":
                    comboBoxNewConfig.Items.Add(100);
                    comboBoxNewConfig.Items.Add(70);
                    comboBoxNewConfig.Items.Add(40);
                    comboBoxNewConfig.Items.Add(20);
                    comboBoxNewConfig.Items.Add(10);
                    MessageHandler.CurrConfigParam(ConfigurationOptions.LightIntensityDelta);
                    break;
                default:

                    break;
            }
        }

        private void buttonConfigUpdate_Click(object sender, EventArgs e)
        {

        }
    }
}
