using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Threading;

using System.Runtime.InteropServices;

namespace ProxyServer
{
    public partial class Form1 : Form
    {
        Thread waitForMsg;
        String Msg;
        byte[] buf;

        public Form1()
        {
            InitializeComponent();
        }


        private void checkForMsg()
        {
            while (true)
            {
                if (Server.x == true)
                {
                    buf = clientManager.Clients[0].buffer;
                    Msg = buf.ToString();
                    textBox1.Invoke((MethodInvoker)(() => textBox1.Text = Server.data));
                }
            }
        }

        private void StartServerBtn_Click(object sender, EventArgs e)
        {
            Server.initServer();
            waitForMsg = new Thread(checkForMsg);
            waitForMsg.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
