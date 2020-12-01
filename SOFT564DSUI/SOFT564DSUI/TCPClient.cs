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
        static public byte[] buffer = new byte[1460];

        static public void initClient(String serverIP, Int32 port)
        {
            try
            {
                long longServerIP = IPtoLong(serverIP);
                //The massive number represents the ip address
                serverSocket = new IPEndPoint(longServerIP, port);
                socket = new Socket(serverSocket.AddressFamily, SocketType.Stream, ProtocolType.Tcp); //setup a new socket to connect to the server.
                socket.BeginConnect(serverSocket, ConnectionCallback, null);    //Start attempting to connect to the remote host.

                ConnectionEstablished = true;
            }   
              catch(Exception e)
             {
                ConnectionEstablished = false;
             }
}


        public static void ConnectionCallback(IAsyncResult asyncResult)
        {
            socket.EndConnect(asyncResult);     //stop requesting connection when connected;
            //Start listening for incoming bytes.
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, null); //start listening for incoming data from the server.
        }


        public static void ReceiveCallback(IAsyncResult asyncResult)
        {
            //Get the number of received bytes
            int bytesReceived = socket.EndReceive(asyncResult);

            //if bytes available then do something
            if(bytesReceived > 0)
            {
                Byte[] Message = new byte[bytesReceived];                   //Create a message buffer of the same size as bytes received.
                Buffer.BlockCopy(buffer, 0, Message, 0, bytesReceived);     //Copy the contents of the buffer to the other buffer. the buffer used to receive data from the TCP stream is overwritten on every callback.
                MessageHandler.HandleRequest(Message);                      //Process the request
                dataAvailable = true;
            }

            //start waiting for bytes again. 
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, null);
        }

        static public void asyncSend(String message, String ID)
        {
            int byteCount = Encoding.ASCII.GetByteCount(ID + message + 1);      //get the number of bytes in the message that I want to send
            byte[] sendData = Encoding.ASCII.GetBytes(ID + message);            //convert data in an array of bytes.
            Console.WriteLine("Sending");
            socket.BeginSend(sendData, 0, sendData.Length, SocketFlags.None, SendCallback, socket);     //start the transmission process
        }

        //asynchronous sending callback witht the result of the transmission attempt.
        public static void SendCallback(IAsyncResult asyncResult)
        {

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
