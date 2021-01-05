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
        Thread updateGUI;
        public ClientGUI()
        {
            InitializeComponent();

            //At the beginning keep these GUI objects disabled
            buggyConnectBtn.Enabled = false;
            buggyDisconnectBtn.Enabled = false;
            textBoxBuggyConnectStatus.Enabled = false;
            comboBoxIntMode.Enabled = false;
            comboBoxConfig.Enabled = false;
            ManualControlFocusBtn.Enabled = false;
            textBoxConfigStatus.Enabled = false;
            textBoxCurrConfig.Enabled = false;
            textBoxConfigStatus.Enabled = false;
            buttonConfigUpdate.Enabled = false;
            comboBoxNewConfig.Enabled = false;
            TempTextBox.Enabled = false;
            HumTextBox.Enabled = false;
            LIntTextBox.Enabled = false;
            buttonReqData.Enabled = false;

            //Add interaction mode options to the combo box
            comboBoxIntMode.Items.Add("Manual");
            comboBoxIntMode.Items.Add("Autonomous");
            comboBoxIntMode.Items.Add("Configuration");

            //Add configuration options to the combo box
            comboBoxConfig.Items.Add("Autonomous Data T (ms)");     //sets the period data requests in autonomous mode.
            comboBoxConfig.Items.Add("Max Object Distance (cm)");   //Sets the closest distance that the buggy can be from object before it stops.
            comboBoxConfig.Items.Add("Buggy Speed");                //Changes Buggy speed.
            comboBoxConfig.Items.Add("Light Intensity Delta");      //used to set the difference in light levels which determines wheter the buggy should turn in autonomous mode.

            MessageHandler.StartRequestHandlerThread();

        }

        public void UpdateGUI()
        {
            while(true) {

                //Adds the new client to the GUI listbox list if there are less clients in the gui list box than there are in the connectedclient list.
                if (listBox1.Items.Count < clientManager.Clients.Count)
                {
                    foreach (ConnectedClients client in clientManager.Clients)  //iterate through is client in the list
                    {
                        if (listBox1.FindStringExact(client.clientID.ToString()) == -1) //if there is any client in the connectedclient list that isn't on the list then add it.
                        {
                            listBox1.Invoke((MethodInvoker)(() => listBox1.Items.Add(client.clientID)));    //Add client to the list of available clients.
                        }
                    }
                }


                //Removes a specific client from the GUI listbox if there are more clients in the GUI listbox than there are in the connectedclient list
                if (listBox1.Items.Count > clientManager.Clients.Count)
                {
                    for (int xx = 0; xx < listBox1.Items.Count; xx++)   //iterate through every client in the GUI listbox
                    {
                        if (!clientManager.Clients.Exists(connectedClient => connectedClient.clientID == Convert.ToInt32(listBox1.Items[xx])))  //If client id on the GUI list does not belong to any clients in the connected client list remove it from the list.
                        {
                            listBox1.Invoke((MethodInvoker)(() => listBox1.Items.RemoveAt(xx)));    //Remove the client from the available client list.
                            xx--;   //push the counter back once so that all items on the list are compared against the connected clients list. The list is rearranged after removal of an item.
                        }

                    }
                }

                //If any of the values that were received from the buggy differ from the old readings displayed then update the readings.
                if((float.Parse(TempTextBox.Text) != EnvData.ftemperature) || (Int32.Parse(HumTextBox.Text) != EnvData.humidity) || (Int32.Parse(LIntTextBox.Text) != EnvData.lIntensity))
                {
                    TempTextBox.Invoke((MethodInvoker)(() => TempTextBox.Text = EnvData.ftemperature.ToString()));
                    HumTextBox.Invoke((MethodInvoker)(() => HumTextBox.Text = EnvData.humidity.ToString()));
                    LIntTextBox.Invoke((MethodInvoker)(() => LIntTextBox.Text = EnvData.lIntensity.ToString()));
                }


                
                if(!ConnectionManager.ConnectionLost)
                {
                    connectToServerBtn.Invoke((MethodInvoker)(() => connectToServerBtn.Enabled = false));
                }

                //If connection with the server has been lost at any point then enable the server connect button to allow the controller client to reconnect.
                //Also update the messaged to notify the user what happened witht he server connection.
                if ((ConnectionManager.ConnectionLost) && (connectToServerBtn.IsHandleCreated))
                {
                    //Enable and Disable appropriate units
                    connectToServerBtn.Invoke((MethodInvoker)(() => connectToServerBtn.Enabled = true));
                    buggyConnectBtn.Invoke((MethodInvoker)(() => buggyConnectBtn.Enabled = false));
                    buggyDisconnectBtn.Invoke((MethodInvoker)(() => buggyDisconnectBtn.Enabled = false));
                    textBoxBuggyConnectStatus.Invoke((MethodInvoker)(() => textBoxBuggyConnectStatus.Enabled = false));
                    comboBoxIntMode.Invoke((MethodInvoker)(() => comboBoxIntMode.Enabled = false));
                    comboBoxConfig.Invoke((MethodInvoker)(() => comboBoxConfig.Enabled = false));
                    ManualControlFocusBtn.Invoke((MethodInvoker)(() => ManualControlFocusBtn.Enabled = false));
                    textBoxConfigStatus.Invoke((MethodInvoker)(() => textBoxConfigStatus.Enabled = false));
                    textBoxCurrConfig.Invoke((MethodInvoker)(() => textBoxCurrConfig.Enabled = false));
                    textBoxConfigStatus.Invoke((MethodInvoker)(() => textBoxConfigStatus.Enabled = false));
                    buttonConfigUpdate.Invoke((MethodInvoker)(() => buttonConfigUpdate.Enabled = false));
                    comboBoxNewConfig.Invoke((MethodInvoker)(() => comboBoxNewConfig.Enabled = false));
                    TempTextBox.Invoke((MethodInvoker)(() => TempTextBox.Enabled = false));
                    HumTextBox.Invoke((MethodInvoker)(() => HumTextBox.Enabled = false));
                    LIntTextBox.Invoke((MethodInvoker)(() => LIntTextBox.Enabled = false));
                    buttonReqData.Invoke((MethodInvoker)(() => buttonReqData.Enabled = false));

                    listBox1.Invoke((MethodInvoker)(() => listBox1.Items.Clear()));

                    //clear the connected client list on disconnection
                    statusTB.Invoke((MethodInvoker)(() => statusTB.Text = "Disconnected"));

                    Thread.CurrentThread.Abort();
                }

                if (ConnectionManager.BuggyConnectionResult)
                {
                    if (!ConnectionManager.buggyConnected)
                    {
                        buggyConnectBtn.Invoke((MethodInvoker)(() => buggyConnectBtn.Enabled = true));
                        buggyDisconnectBtn.Invoke((MethodInvoker)(() => buggyDisconnectBtn.Enabled = false));
                        textBoxBuggyConnectStatus.Invoke((MethodInvoker)(() => textBoxBuggyConnectStatus.Enabled = false));
                        comboBoxIntMode.Invoke((MethodInvoker)(() => comboBoxIntMode.Enabled = false));
                        comboBoxConfig.Invoke((MethodInvoker)(() => comboBoxConfig.Enabled = false));
                        ManualControlFocusBtn.Invoke((MethodInvoker)(() => ManualControlFocusBtn.Enabled = false));
                        textBoxConfigStatus.Invoke((MethodInvoker)(() => textBoxConfigStatus.Enabled = false));
                        textBoxCurrConfig.Invoke((MethodInvoker)(() => textBoxCurrConfig.Enabled = false));
                        textBoxConfigStatus.Invoke((MethodInvoker)(() => textBoxConfigStatus.Enabled = false));
                        buttonConfigUpdate.Invoke((MethodInvoker)(() => buttonConfigUpdate.Enabled = false));
                        comboBoxNewConfig.Invoke((MethodInvoker)(() => comboBoxNewConfig.Enabled = false));
                        TempTextBox.Invoke((MethodInvoker)(() => TempTextBox.Enabled = false));
                        HumTextBox.Invoke((MethodInvoker)(() => HumTextBox.Enabled = false));
                        LIntTextBox.Invoke((MethodInvoker)(() => LIntTextBox.Enabled = false));
                        buttonReqData.Invoke((MethodInvoker)(() => buttonReqData.Enabled = false));

                        textBoxBuggyConnectStatus.Invoke((MethodInvoker)(() => textBoxBuggyConnectStatus.Text = "Disconnected"));
                    }
                    ConnectionManager.BuggyConnectionResult = false;
                }

                //Update the current configuration parameter value in the textbox
                textBoxCurrConfig.Invoke((MethodInvoker)(() => textBoxCurrConfig.Text = BuggyConfigurationData.currConfigParam.ToString()));

                //If the buggy has been configured, display "OK" message in the configuration status textbox
                if(MessageHandler.configStatusUpdate)
                {
                    if (BuggyConfigurationData.configUpdateStatus == 1)
                    {
                        textBoxConfigStatus.Invoke((MethodInvoker)(() => textBoxConfigStatus.Text = "OK"));
                    }
                    MessageHandler.configStatusUpdate = false;
                }
            }
        }

        private bool CheckIPInput(String ipAddress)
        {
            String number = "";
            long addressInt = 0;
            int nOfDots = 0;
            int nOfints = 0;
            int nOfNumberChars = 0;
            char previousChar = (char)0;
            int i = 0;
            int j = 0;

            for (int x = 0; x < ipAddress.Length; x++)
            {

                //check if first and last chars are numbers
                if ((ipAddress[0] < 48) || (ipAddress[0] > 57)) 
                {
                    //error has been detected therefore no need to carry on checking. Jump to IpaddressError label.
                    goto IpaddressError;
                }
                else if ((ipAddress[ipAddress.Length -1] < 48) || (ipAddress[ipAddress.Length - 1] > 57)) {
                    goto IpaddressError;
                }

                //If the value is either a . or number between 0 and 9 then continue
                if ((ipAddress[x] == 46) || (ipAddress[x] >= 48 && ipAddress[x] <= 57)) {

                    //Check the address contains more than one dot in a row
                    if (previousChar == 46 && ipAddress[x] == 46)
                    {
                        goto IpaddressError;
                    }
                    else
                    {
                        previousChar = ipAddress[x];
                    }

                    if (ipAddress[x] != 46) //if dot not encountered keep concatenating numbers if ip address for long parsing
                    {
                        //extract ip number from the ip string
                        number = number + ipAddress[x].ToString();
                        nOfNumberChars++;

                        //check if a number created for the ip address is too big
                        if(nOfNumberChars > 3)
                        {
                            goto IpaddressError;
                        }
                    } 
                    else
                    {
                        nOfDots++;
                    }


                    if (ipAddress[x] == 46 || x == (ipAddress.Length - 1))   //if dot encountered in string or end of ip string
                    {
                        //convert the string to a number and shift to the left to create correct size and add the converted numbers together 
                        addressInt = int.Parse(number);
                        if(addressInt < 0 || addressInt > 255)
                        {
                            goto IpaddressError;
                        }

                        nOfints++;

                        //clear the number string for next string to be converted
                        number = "";
                        nOfNumberChars = 0;
                    }
                }
                else
                {
                    goto IpaddressError;
                }

            }

            //if more than 4 ints converted or more than 3 dots detected: error
            if(nOfints > 4 || nOfDots > 3)
            {
                goto IpaddressError;
            }

            //No error detected
            return true;


        //Error was detected
        IpaddressError:
            return false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        //function that runs when a key has been pressed
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //if in manual interaction mode, record states of the following key presses.
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

        //function that runs when a key has been released
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //if in manual interaction mode, record states of the following key releases.
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
            //Disable connect to server button to prevent multiple connection requets.
            connectToServerBtn.Enabled = false;

            //Connection lost status flag set to disabled as connection request begins.
            ConnectionManager.ConnectionLost = false;

            //Attempt to connect to server with the ip address and port from the textboxes ip address and port textboxes
            if (CheckIPInput(ipAddressTB.Text))
            {
                TCPClient.ConnectToServer(ipAddressTB.Text, Int32.Parse(portTB.Text));

                while(!ConnectionManager.ServerConnectionResult) { };

                ConnectionManager.ServerConnectionResult = false;

                //If failed to establish connection then enable connect button and display status otherwise:
                if (!ConnectionManager.ConnectionEstablished)
                {
                    connectToServerBtn.Enabled = true;
                    statusTB.Text = "Failed To Connect";
                }
                else if (ConnectionManager.ConnectionEstablished)
                {
                    //Keep connect button disabled and display connection status
                    connectToServerBtn.Enabled = false;
                    statusTB.Text = "Connected";

                    //enable appropriate GUI objects
                    buggyConnectBtn.Enabled = true;
                    textBoxBuggyConnectStatus.Enabled = true;

                    //Start thread that will keep the GUI updated with information
                    updateGUI = new Thread(UpdateGUI);
                    updateGUI.Start();
                }
            }
            else
            {
                statusTB.Text = "Invalid IP address";
                connectToServerBtn.Enabled = true;
            }

        }

        private void portTB_TextChanged(object sender, EventArgs e)
        {
            //if input box has or is any of these, disable connection button
            if (Regex.IsMatch(portTB.Text, @"[a-zA-Z!@#$%^&£*(),.?:{ }|<> _ +=[\]\""\;\'\~\¬\-`\\/]") || (portTB.Text.Length == 0) || (Int32.Parse(portTB.Text) > 65535)) 
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
            //Disable button to prevent multiple disconnection requests.
            buggyDisconnectBtn.Enabled = false;

            //Send message to server to notify that the controller client is disconnecting from the buggy and that it is available for connections again.
            MessageHandler.BuggyDisconnect();

            //Wait until disconnection finishes before continuing with enabling/disabling buggy controls.
            while (ConnectionManager.buggyConnected) { }

            //Enable buggy connect button
            buggyConnectBtn.Enabled = true;

            //Disable buggy controls
            comboBoxIntMode.Enabled = false;
            comboBoxConfig.Enabled = false;
            ManualControlFocusBtn.Enabled = false;
            textBoxConfigStatus.Enabled = false;
            textBoxCurrConfig.Enabled = false;
            buttonConfigUpdate.Enabled = false;
            comboBoxNewConfig.Enabled = false;
            TempTextBox.Enabled = false;
            HumTextBox.Enabled = false;
            LIntTextBox.Enabled = false;
            buttonReqData.Enabled = false;

            //Don't record motor control inputs and don't send them to the buggy as requests
            BuggyMotorControl.PauseMotorControl();

            //Display disconnection status.
            textBoxBuggyConnectStatus.Text = "Disconnected";
        }

        private void buggyConnectBtn_Click(object sender, EventArgs e)
        {
            int index = 0;

            if(listBox1.SelectedIndex == -1)        //if no client was selected from the listbox
            {
                textBoxBuggyConnectStatus.Text = "Select a client to connect to.";
            }
            else                                    //client was selected
            {
                //Disable button to prevent multiple requests from being made.
                buggyConnectBtn.Enabled = false;

                //Send request to server to ask for permission to connect to the selected buggy.
                MessageHandler.BuggyConnect(Convert.ToInt32(listBox1.SelectedItem));

                //Wait for the response from the server or until connection with server is lost
                while (BuggyConnectResponse.response == 0 && (ConnectionManager.ConnectionLost == false)) { }

                if (ConnectionManager.ConnectionLost == false)  //Display response if connection with server was not lost
                {
                    switch (BuggyConnectResponse.response)
                    {
                        case BuggyConnectResponse.ConnectPermitted:
                            //find the buggy connection infromation from the list by searching the list by client id and make a connection to the buggy.
                            index = clientManager.Clients.FindIndex(x => x.clientID == Convert.ToInt32(listBox1.SelectedItem));
                            ConnectionManager.AddClient(clientManager.Clients[index].ipAddress, 80);

                            //display connection status of the buggy
                            textBoxBuggyConnectStatus.Text = "Connected to buggy";

                            /*
                             * Disable appropriate controls so that the user cannot connect to other buggies while connected to one.
                             * Enable appropriate buggy controls to allow the user to use the buggy.
                             */
                            buggyConnectBtn.Enabled = false;
                            buggyDisconnectBtn.Enabled = true;
                            comboBoxIntMode.Enabled = true;

                            //Start thread that will monitor motor control inputs if manual mode is on.
                            BuggyMotorControl.StartMotorControl();
                            break;
                        case BuggyConnectResponse.BuggyInUse:
                            textBoxBuggyConnectStatus.Text = "Buggy is used by another client";
                            buggyConnectBtn.Enabled = true;
                            break;
                        default:
                            textBoxBuggyConnectStatus.Text = "Buggy is used by another client";
                            buggyConnectBtn.Enabled = true;
                            break;
                    }
                }

                //Reset The response indicator
                BuggyConnectResponse.response = 0;
            }
        }

        private void comboBoxIntMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*
             * Enable or disable buggy controls based on the interaction mode selected.
             * Set boolean that enables motor control values to be recorded in manual mode based on interaction mode.
             * Send interaction mode request to the buggy.
             */
            switch(comboBoxIntMode.SelectedItem.ToString())
            {
                case "Manual":
                    ManualControlFocusBtn.Enabled = true;
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
                    ManualControlFocusBtn.Enabled = false;
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
                    ManualControlFocusBtn.Enabled = false;
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
                    ManualControlFocusBtn.Enabled = false;
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
            //if the button is clicked, send request to the buggy to send back sensor data
            MessageHandler.SendEnvData();
        }

        private void comboBoxNewConfig_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBoxConfig_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxNewConfig.Items.Clear();                    //new config option selected, clear out old options from the New combo box.
            
            //Add configuration options into the new ComboBox based on the selected option in the Config ComboBox.
            //Also sends a request to get the current data for the slected configuration option from the buggy.
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

        //Sends a request to update the buggy with the new parameter for the selected option. 
        private void buttonConfigUpdate_Click(object sender, EventArgs e)
        {
            textBoxConfigStatus.Clear();    //Clears the config status textbox to get it ready to display new update status

            //Send a request based on the selected configuration option
            switch (comboBoxConfig.SelectedItem.ToString())
            {
                case "Autonomous Data T (ms)":
                    MessageHandler.UpdateConfigOption(ConfigurationOptions.AutonomousDataT, Int32.Parse(comboBoxNewConfig.SelectedItem.ToString()));
                    break;
                case "Max Object Distance (cm)":
                    MessageHandler.UpdateConfigOption(ConfigurationOptions.MaxObjectDistance, Int32.Parse(comboBoxNewConfig.SelectedItem.ToString()));
                    break;
                case "Buggy Speed":
                    MessageHandler.UpdateConfigOption(ConfigurationOptions.BuggySpeed, Int32.Parse(comboBoxNewConfig.SelectedItem.ToString()));
                    break;
                case "Light Intensity Delta":
                    MessageHandler.UpdateConfigOption(ConfigurationOptions.LightIntensityDelta, Int32.Parse(comboBoxNewConfig.SelectedItem.ToString()));
                    break;
                default:

                    break;
            }
        }
    }
}
