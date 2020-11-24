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

namespace ProxyServer
{
    class Server
    {
        static private IPHostEntry ipHostInfo;
        static private IPAddress ipAddress;
        static private IPEndPoint localEndPoint;
        static private Socket listenerSocket, handler;
        static public String s;
        public static string data = null;
        static public bool x = false;
        static public int i = 0;
        


        static public void initServer()
        {

            try
            {
                ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                ipAddress = ipHostInfo.AddressList[1];
                localEndPoint = new IPEndPoint(ipAddress, 11000);


                s = ipAddress.ToString();
                listenerSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listenerSocket.Bind(localEndPoint);
                listenerSocket.Listen(100);
                listenerSocket.BeginAccept(AcceptConnectionCallback, listenerSocket);


            }
            catch (Exception e)
            {
                Console.WriteLine("Error");
            }

        }

        static public void listen()
        {
            byte[] bytes = new Byte[1024];
            int byteCount;
            byte[] sendData;

            while (true)
            {
                //handler = socketListener.Accept();
                data = null;

                // An incoming connection needs to be processed.  
                while (true)
                {
                    int bytesRec = handler.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    data = handler.RemoteEndPoint.ToString();
                    byteCount = Encoding.ASCII.GetByteCount(data + 1);
                    sendData = Encoding.ASCII.GetBytes(data);
                    handler.Send(sendData, sendData.Length, 0);

                    if (data.IndexOf("<EOF>") > -1)
                    {
                        break;
                    }

                }
            }

        }

        public static void AcceptConnectionCallback(IAsyncResult asyncResult)
        {
            Socket newClientSocket = listenerSocket.EndAccept(asyncResult);   //accept communication and create a new socket to handle communication with the new client
            Console.WriteLine("New client connected: " + newClientSocket.RemoteEndPoint);
            clientManager.AddClient(newClientSocket);   //create a client object the handle transcieving of data for the newly connected client.

            int count = clientManager.Clients.Count;
            Console.WriteLine(count);
            clientManager.Clients[count - 1].NewClientSend();

            listenerSocket.BeginAccept(AcceptConnectionCallback, listenerSocket);   //start accepting new incoming connections again.

        }

    }

    static class clientManager
    {
        public static List<Client> Clients = new List<Client>();

        public static void AddClient(Socket socket)
        {
            Clients.Add(new Client(socket, Clients.Count));
        }

        public static void RemoveClient(int id)
        {
            Clients.RemoveAt(Clients.FindIndex(x => x.clientID == id));
        }
    }

    class Client
    {
        public Socket clientSocket;
        public int clientID;
        public byte[] buffer;

        public Client(Socket socket, int id)
        {
            clientSocket = socket;
            clientID = id;
            buffer = new byte[1024];

            clientReceive();

        }

        public void clientReceive()
        {
            clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, null);
        }

        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            int received = clientSocket.EndReceive(asyncResult);
            if (received > 0)    //if there is 
            {
                Server.data = Encoding.Default.GetString(buffer);
                Server.x = true;
                Console.WriteLine(Server.data.Trim());
                Array.Clear(buffer, 0, buffer.Length);
                clientReceive();
                clientSend(Server.data);
            }
        }

        public void clientSend(String Message)
        {
            String msg = "Data Received";
            byte[] msgByte;
            int msgLen;

            msgLen = Encoding.ASCII.GetByteCount(Message + 1);
            msgByte = Encoding.ASCII.GetBytes(Message);
            //clientSocket.BeginSend(msgByte, 0, msgByte.Length, SocketFlags.None, SendCallback, clientManager.Clients[1].clientSocket);
            if (msgByte[0] == '0')
            {
                clientManager.Clients[0].clientSocket.BeginSend(msgByte, 0, msgByte.Length, SocketFlags.None, SendCallback, clientSocket); //send data to the other client
            }
            else
            {
                Console.WriteLine("It's not 0");
            }

        }

        public void NewClientSend()
        {
            byte[] msgByte;
            int ID;

            ID = clientManager.Clients[0].clientID;
            msgByte = BitConverter.GetBytes(ID);
            clientSocket.BeginSend(msgByte, 0, msgByte.Length, SocketFlags.None, SendCallback, clientSocket);

        }


        private void SendCallback(IAsyncResult asyncResult)
        {
            //int sent = clientSocket.EndSend(asyncResult);
            //if(sent > 0)
            //{
            //    Console.WriteLine("Client Sent Bytes");
            //}
        }

    }
}
