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
            int x = BitConverter.ToInt32(TCPClient.buffer, 0);
            while(true) {
                if (TCPClient.dataAvailable)
                {
                    Message = Encoding.UTF8.GetString(TCPClient.buffer, 0, TCPClient.buffer.Length);
                    if(MessageHandler.newClient == true)
                    {
                        listBox1.Invoke((MethodInvoker)(() => listBox1.Items.Add(clientManager.Clients[clientManager.Clients.Count - 1].ipAddress)));
                        MessageHandler.newClient = false;
                    }
                 
                    textBox2.Invoke((MethodInvoker)(() => textBox2.Text = Message));
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
            TCPClient.asyncSend(textBox1.Text, id);
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
