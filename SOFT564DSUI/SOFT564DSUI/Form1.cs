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

                if((Int32.Parse(TempTextBox.Text) != EnvData.temperature) || (Int32.Parse(HumTextBox.Text) != EnvData.humidity) || (Int32.Parse(LIntTextBox.Text) != EnvData.lIntensity))
                {
                    TempTextBox.Invoke((MethodInvoker)(() => TempTextBox.Text = EnvData.temperature.ToString()));
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
            ConnectionManager.Connections[0].asyncSend(textBox1.Text, id);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                SendButton.PerformClick();
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


    }
}
