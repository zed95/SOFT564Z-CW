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
        static Thread RequestHandlerThread = new Thread(RequestHandler.HandleRequest);



        static public void initServer()
        {

            try
            {
                ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                ipAddress = ipHostInfo.AddressList[1].MapToIPv4();
                //ipAddress = ipHostInfo.AddressList[0].MapToIPv4();
                localEndPoint = new IPEndPoint(ipAddress, 11000);

                if (!RequestHandlerThread.IsAlive)
                {
                    RequestHandlerThread.Start();
                }

                s = ipAddress.ToString();
                listenerSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listenerSocket.Bind(localEndPoint);
                listenerSocket.Listen(100);
                listenerSocket.BeginAccept(AcceptConnectionCallback, listenerSocket);


            }
            catch (Exception e)
            {
                Console.WriteLine("Error");
                Console.WriteLine(e);
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


        //Sends data to all connected clients.
        public static void sendAllClients(Byte requestType, Client client)
        {
            Byte[] ClientInfo;
            Byte[] NewClientUpdateInfo;

            ClientInfo = TransmissionConverter(requestType, client);  //Conver client data to send to all other clients.
            foreach (Client clients in clientManager.Clients)
            {
                try
                {
                    if (client.clientID != clients.clientID)
                    {
                        if (requestType == 1)
                        {
                            //send data of all clients (apart from the data of the actual client itself) to the newly connected client.
                            NewClientUpdateInfo = TransmissionConverter(requestType, clients);
                            client.clientSocket.BeginSend(NewClientUpdateInfo, 0, NewClientUpdateInfo.Length, SocketFlags.None, client.SendCallback, client.clientSocket);
                        }
                        clients.clientSocket.BeginSend(ClientInfo, 0, ClientInfo.Length, SocketFlags.None, clients.SendCallback, clients.clientSocket); //send data to the other clients
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine("Exception in sendAllClients");
                }
            }
        }

        //Converts client data and request types into a byte array to be sent over the network
        private static Byte[] TransmissionConverter(Byte requestType, Client client)
        {
            Byte[] returnByte = null;
            Byte[] ipAddress;
            Byte[] port;
            Byte[] id;
            Byte[] ByterequestType;
            String StrIPAddr;
            IPEndPoint endPoint;

            switch (requestType)
            {
                case 0:

                    break;
                case 1:
                    endPoint = (IPEndPoint)client.clientSocket.RemoteEndPoint;
                    IPAddress addr = endPoint.Address;
                    StrIPAddr = addr.ToString();
                    ipAddress = BitConverter.GetBytes(IPtoLong(StrIPAddr));
                    port = BitConverter.GetBytes(endPoint.Port);
                    id = BitConverter.GetBytes(client.clientID);

                    ByterequestType = BitConverter.GetBytes(requestType);
                    returnByte = new byte[ipAddress.Length + port.Length + id.Length + ByterequestType.Length];
                    Buffer.BlockCopy(ByterequestType, 0, returnByte, 0, ByterequestType.Length);
                    Buffer.BlockCopy(ipAddress, 0, returnByte, ByterequestType.Length, ipAddress.Length);
                    Buffer.BlockCopy(port, 0, returnByte, (ipAddress.Length + ByterequestType.Length), port.Length);
                    Buffer.BlockCopy(id, 0, returnByte, (ipAddress.Length + ByterequestType.Length + port.Length), id.Length);
                    break;
                case 2:
                    id = BitConverter.GetBytes(client.clientID);
                    ByterequestType = BitConverter.GetBytes(requestType);
                    returnByte = new byte[id.Length + ByterequestType.Length];
                    Buffer.BlockCopy(ByterequestType, 0, returnByte, 0, ByterequestType.Length);
                    Buffer.BlockCopy(id, 0, returnByte, ByterequestType.Length, id.Length);
                    break;
                default:
                    break;
            }

            return returnByte;
        }

        //Converts IP address from string format to long;
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
            Clients.Add(new Client(socket, AssignID()));                            //Add newly connected client to the list of clients
            Server.sendAllClients((Byte)1, Clients[Clients.Count - 1]);             //Send data about the newly connected client to all other connected clients.

        }

        public static void RemoveClient(int id)
        {
            Client removedClient;
            if (Clients.Exists(ClientToRemove => ClientToRemove.clientID == id))                        //Prevent Multiple exceptions from calling for removal of the client by checking if client of such id exists
            {
                removedClient = Clients.Find(DisconnectedClient => DisconnectedClient.clientID == id);  //extract info of the client to remove in order to tell other clients to remove the client from their list.
                Clients.RemoveAt(Clients.FindIndex(x => x.clientID == id));                             //Remove the client from the list
                Server.sendAllClients((Byte)2, removedClient);                                          //Update all the clients connected that the client has been removoed.
                //change the request type numbers into meaningful constants.
            }
        }

        //Generates a unique client id for newly connected clients.
        private static int AssignID()    
        {
            int newID = 0;

            foreach (Client client in Clients)
            {
                newID += client.clientID;
            }

            newID += Clients.Count;

            return newID;
        }

    }

    class Client
    {
        public Socket clientSocket;
        public int clientID;
        public byte[] callbackBuffer;
        private Byte[] unqueuedBytesBuffer = new byte[0];

        public Client(Socket socket, int id)
        {
            clientSocket = socket;
            clientID = id;
            callbackBuffer = new byte[1024];

            clientReceive();
        }

        //calls the BeginReceive function to start receiving incoming data to the socket.
        public void clientReceive()
        {
            clientSocket.BeginReceive(callbackBuffer, 0, callbackBuffer.Length, SocketFlags.None, ReceiveCallback, null);
        }

        //function called in response to data being received.
        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            int bytesReceived;
            int bytesLeft; 
            int copyOffset = unqueuedBytesBuffer.Length;        //new bytes to be copy at an offset of the of the old array before its size increases.
            Queue<Byte[]> tempQueue = new Queue<byte[]>();

            try
            {
                bytesReceived = clientSocket.EndReceive(asyncResult);    //how many bytes we got
                bytesLeft = bytesReceived + unqueuedBytesBuffer.Length; //the number of bytes is equal to the number of bytes that have not yet been processed + the number of new bytes that arrived.

                if (bytesReceived > 0)    //do something if we received any bytes
                {
                    Array.Resize(ref unqueuedBytesBuffer, bytesLeft);   //new size of array = previous bytes in the array + bytes that arrived.
                    Buffer.BlockCopy(callbackBuffer, 0, unqueuedBytesBuffer, copyOffset, bytesReceived); //copy new bytes from callback buffer into the unqued bytes are for processing.
                    Console.WriteLine("Number of bytes available: " + bytesLeft);
                    clientReceive();

                    //---

                    while (bytesLeft > 0)
                    {
                        switch (unqueuedBytesBuffer[0])
                        {
                            case RequestTypes.ListAddClient:
                                if (bytesLeft >= 18)
                                {
                                    tempQueue.Enqueue(ExtractRequest(unqueuedBytesBuffer, 18));
                                    bytesLeft -= 18;
                                    Buffer.BlockCopy(unqueuedBytesBuffer, 18, unqueuedBytesBuffer, 0, bytesLeft);
                                    Array.Resize(ref unqueuedBytesBuffer, bytesLeft);
                                }
                                else
                                {
                                    //if the command is recognised but not all required bytes arrived then jump 'breakout' to continue with the code
                                    goto breakout;
                                }
                                break;
                            case RequestTypes.ListRemoveClient:
                                if (bytesLeft >= 6)
                                {
                                    tempQueue.Enqueue(ExtractRequest(unqueuedBytesBuffer, 6));
                                    bytesLeft -= 6;
                                    Buffer.BlockCopy(unqueuedBytesBuffer, 6, unqueuedBytesBuffer, 0, bytesLeft);
                                    Array.Resize(ref unqueuedBytesBuffer, bytesLeft);
                                }
                                else
                                {
                                    //if the command is recognised but not all required bytes arrived then jump 'breakout' to continue with the code
                                    goto breakout;
                                }
                                break;
                            default:
                                break;
                        }

                    }

                     //if the command is recognised but not all required bytes arrived then jump to here to continue with the code
                    breakout:


                    RequestHandler.RequestQueueMutex.WaitOne();                 //Wait for signal that it's okay to enter
                    while (tempQueue.Count > 0) //Keep adding to request queue until the temporary queue is empty.
                    {
                        RequestHandler.RequestQueue.Enqueue(tempQueue.Dequeue());               //Add request data to the queue
                    }
                    RequestHandler.RequestQueueMutex.ReleaseMutex();            //Release the mutex


                }
            }
            catch(Exception e)  //In the case of a disconnection, remove the disconnected client and update controller clients
            {
                Console.WriteLine("ReceiveCallback");
                clientManager.RemoveClient(clientID);
            }
        }

        public void clientSend(Socket clientSocket, String Message)
        {
            byte[] msgByte;
            int msgLen;

            //Adapt this function to send data to a speciffied client.
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

        //callback function when data was sent.
        public void SendCallback(IAsyncResult asyncResult)
        {
            int sent = clientSocket.EndSend(asyncResult);
            //if(sent > 0)
            //{
            //    Console.WriteLine("Client Sent Bytes");
            //}
        }


        private Byte[] ExtractRequest(Byte[] bytes, int size)
        {
            Byte[] requestBytes = new byte[size];

            Buffer.BlockCopy(bytes, 0, requestBytes, 0, size);
            return requestBytes;
        }

    }
}
