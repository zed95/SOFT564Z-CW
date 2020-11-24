using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Threading;
using System.Data;

namespace SOFT564DSUI
{
    class TCPClient
    {
        public static bool dataAvailable = false;
        public static bool ConnectionEstablished = false;
        public static bool ConnectionLost = false;
        static private Socket socket;
        static private IPEndPoint serverSocket;
        static private Thread pollSocket = new Thread(pollSocketAvailable);
        static public byte[] buffer = new byte[1460];

        static public void initClient(String serverIP, Int32 port)
        {
            try
            {
                long longServerIP = IPtoLong(serverIP);
                //The massive number represents the ip address
                serverSocket = new IPEndPoint(longServerIP, port);
                socket = new Socket(serverSocket.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(serverSocket);
                if(!pollSocket.IsAlive)
                {
                    pollSocket.Start();
                }
                ConnectionEstablished = true;
            }   
              catch(Exception e)
             {
                ConnectionEstablished = false;
             }
}

        static public void send(String message, String ID)
        {
            try { 
                int byteCount = Encoding.ASCII.GetByteCount(ID + message + 1);
                byte[] sendData = Encoding.ASCII.GetBytes(ID + message);
                Console.WriteLine("Sending");
                socket.Send(sendData, sendData.Length, 0);
                ConnectionLost = false;
            }
            catch(Exception e)
            {
                Console.WriteLine("Failed to send");
                ConnectionLost = true;

            }
        }

        static public void receive()
        { 
            socket.Receive(buffer, 0, buffer.Length, 0);
            dataAvailable = true;
        }

        static private void pollSocketAvailable()
        {
            while (true)
            {
                while (socket.Available == 0) { }
                receive();
            }
        }


        static private long IPtoLong(String addressIP) {
            String number = "";
            long longIpAddress = 0;
            int i = 0;
            int j = 0;

            for(int x = 0; x < addressIP.Length; x++)
            {
                if (addressIP[x] != 46) //if dot not encountered keep concatenating numbers if ip address for long parsing
                {
                    number = number + addressIP[x].ToString();
                }

                if(addressIP[x] == 46 || x == (addressIP.Length - 1))   //if dot encountered in string or end of ip string
                {
                    longIpAddress = longIpAddress + (long.Parse(number) << i);
                    j++;
                    i = j * 8;
                    number = "";
                }
            }

            return longIpAddress;
        }

    }

}
