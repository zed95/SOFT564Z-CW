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
            //clientManager.Clients[count - 1].NewClientSend();

            listenerSocket.BeginAccept(AcceptConnectionCallback, listenerSocket);   //start accepting new incoming connections again.

        }


        //public void NewClientSend()
        //{
        //    byte[] msgByte;
        //    int ID;

        //    ID = clientManager.Clients[0].clientID;
        //    msgByte = BitConverter.GetBytes(ID);
        //    clientSocket.BeginSend(msgByte, 0, msgByte.Length, SocketFlags.None, SendCallback, clientSocket);

        //}


        public void clientSend(Socket clientSocket, String Message)
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

        public static void sendAllClients(Client newClient)
        {
            Byte[] ipAddress;
            Byte[] port;
            Byte[] id;
            Byte[] requestType;
            String StrIPAddr;
            IPEndPoint endPoint;
            Byte[] msgByte;
            int msgLen;

            endPoint = (IPEndPoint)newClient.clientSocket.RemoteEndPoint;
            IPAddress addr = endPoint.Address;
            StrIPAddr = addr.ToString();
            Console.WriteLine(StrIPAddr);
            Console.WriteLine(endPoint.Port);
            Console.WriteLine(newClient.clientID);
            Console.WriteLine(IPtoLong(StrIPAddr));
            ipAddress = BitConverter.GetBytes(IPtoLong(StrIPAddr));
            port = BitConverter.GetBytes(endPoint.Port);
            id = BitConverter.GetBytes(newClient.clientID);

            requestType = BitConverter.GetBytes((Byte)1);

            msgByte = new byte[ipAddress.Length + port.Length + id.Length + requestType.Length];
            Buffer.BlockCopy(requestType, 0, msgByte, 0, requestType.Length);
            Buffer.BlockCopy(ipAddress, 0, msgByte, requestType.Length, ipAddress.Length);
            Buffer.BlockCopy(port, 0, msgByte, (ipAddress.Length + requestType.Length), port.Length);
            Buffer.BlockCopy(id, 0, msgByte, (ipAddress.Length + requestType.Length + port.Length), id.Length);

            //use foreach method
            foreach (Client client in clientManager.Clients)
            {
                client.clientSocket.BeginSend(msgByte, 0, msgByte.Length, SocketFlags.None, SendCallback, client.clientSocket); //send data to the other client


            }
        }

        private static void SendCallback(IAsyncResult asyncResult)
        {
            //int sent = clientSocket.EndSend(asyncResult);
            //if(sent > 0)
            //{
            //    Console.WriteLine("Client Sent Bytes");
            //}
        }

        static private long IPtoLong(String addressIP)
        {
            String number = "";
            long longIpAddress = 0;
            int i = 0;
            int j = 0;

            for (int x = 0; x < addressIP.Length; x++)
            {
                if (addressIP[x] != 46) //if dot not encountered keep concatenating numbers if ip address for long parsing
                {
                    number = number + addressIP[x].ToString();
                }

                if (addressIP[x] == 46 || x == (addressIP.Length - 1))   //if dot encountered in string or end of ip string
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

    static class clientManager
    {
        public static List<Client> Clients = new List<Client>();

        public static void AddClient(Socket socket)
        {
            Clients.Add(new Client(socket, Clients.Count));
            Server.sendAllClients(Clients[Clients.Count - 1]); //Send an update to all clients that new connection has been Made;

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
            }
        }

    }
}
