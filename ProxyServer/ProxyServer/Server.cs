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
        public static string data = null;
        static public bool x = false;
        static public int i = 0;
        public static Thread IsAliveThread = new Thread(RequestHandler.IsAlive);


        public static Queue<Socket> SocketQueue = new Queue<Socket>();
        public static Mutex SocketQueueMutex = new Mutex();

        static public void initServer()
        {
            RequestHandler.StartRequestHandlerThread();                 //start handling the incoming requests if there are any

            //starts a thread (if not already started) that periodically checks the connection status of each connected client
            if (!IsAliveThread.IsAlive)
            {
                IsAliveThread.Start();
            }

            try
            {
                ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());           //get host informatiom from which we get the local endpoint address
                ipAddress = ipHostInfo.AddressList[1].MapToIPv4();          //map the address to ipv4
                //ipAddress = ipHostInfo.AddressList[0].MapToIPv4();
                localEndPoint = new IPEndPoint(ipAddress, 11000);           //set the local endpoint to local ip address and port 11,000

                listenerSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);  //create a new socket
                listenerSocket.Bind(localEndPoint);                                                         //Associate a Socket with the created local endpoint
                listenerSocket.Listen(100);                                                                 //places the socket into listening state. Creates a connection queue of 100 before it starts rejecting incoming connections.
                listenerSocket.BeginAccept(AcceptConnectionCallback, listenerSocket);                       //Start asynchronously accepting connections


            }
            catch (Exception e)
            {
                Console.WriteLine("Error");
                Console.WriteLine(e);
            }

        }

        //Accepts the connection and starts the process to add the connected client to the connected client list
        public static void AcceptConnectionCallback(IAsyncResult asyncResult)
        {
            Socket newClientSocket = listenerSocket.EndAccept(asyncResult);   //accept communication and create a new socket to handle communication with the new client
            SocketQueueMutex.WaitOne();                                       //take mutex to queue the client for addition to the connected clients list by the request handlert
            SocketQueue.Enqueue(newClientSocket);                             //put the client in the queue
            SocketQueueMutex.ReleaseMutex();                                  //release the mutex

            RequestHandler.RequestQueueMutex.WaitOne();                                             //take mutex to add request to add a new client to the connected client list into the queue
            RequestHandler.RequestQueue.Enqueue(BitConverter.GetBytes(RequestTypes.AddNewClient));  //queue the request
            RequestHandler.RequestQueueMutex.ReleaseMutex();                                        //release the mutex

            listenerSocket.BeginAccept(AcceptConnectionCallback, listenerSocket);   //start accepting new incoming connections again.
        }


        //Converts IP address from string format to long;
        static public long IPtoLong(String addressIP)
        {
            String number = "";
            long longIpAddress = 0;
            int i = 0;
            int j = 0;

            for (int x = 0; x < addressIP.Length; x++)
            {
                if (addressIP[x] != 46) //if dot not encountered keep concatenating numbers if ip address for long parsing
                {
                    //extract ip number from the ip string
                    number = number + addressIP[x].ToString();
                }

                if (addressIP[x] == 46 || x == (addressIP.Length - 1))   //if dot encountered in string or end of ip string
                {
                    //convert the string to a number and shift to the left to create correct size and add the converted numbers together 
                    longIpAddress = longIpAddress + (long.Parse(number) << i);

                    //Increase the shift to the left after each converted number to convert nect number to correct size
                    j++;
                    i = j * 8;

                    //clear the number string for next string to be converted
                    number = "";
                }
            }

            //return converted ip address
            return longIpAddress;
        }


    }

    //Manages the connected clients
    static class clientManager
    {
        public static List<Client> Clients = new List<Client>();

        //Adds a newly connected client to the list of connected clients
        public static void AddClient(Socket socket)
        {
            Clients.Add(new Client(socket, AssignID()));                  //Add newly connected client to the list of clients
            RequestHandler.ListAddClient(Clients[Clients.Count - 1]);     //Send request to the request handler to send the newly connected client to all other connected clients  

        }

        public static void RemoveClient(int id)
        {
            int buggyIndex = -1;
            Client removedClient;
            if (Clients.Exists(ClientToRemove => ClientToRemove.clientID == id))                        //Prevent Multiple exceptions from calling for removal of the client by checking if client of such id exists
            {
                removedClient = Clients.Find(DisconnectedClient => DisconnectedClient.clientID == id);  //extract info of the client to remove in order to tell other clients to remove the client from their list.
                
                //check if the client that was disconnected was connected to a buggy. If the client was connected to a buggy, then find the buggy and mark it as free.
                if(removedClient.connectedToBuggy != -1)
                {
                    buggyIndex = clientManager.Clients.FindIndex(x => x.clientID == removedClient.connectedToBuggy);    //find where the buggy that the client was connected is in the list of clients

                    if (buggyIndex != -1)   //if the buggy is still in the list
                    {
                        clientManager.Clients[buggyIndex].inUse = false;    //make it available for other users to connect to it.
                    }
                }

                Clients.RemoveAt(Clients.FindIndex(x => x.clientID == id));        //Remove the client from the list
                RequestHandler.ListRemoveClient(removedClient);                    //send request to all connected clients to remove the disconnected client from their list
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
        public bool inUse;
        public int connectedToBuggy;

        //class constructor
        public Client(Socket socket, int id)
        {
            clientSocket = socket;              //socket is equal to the connected endpoint
            clientID = id;                      //id of the client is the id that has been uniquely generated
            callbackBuffer = new byte[1024];    //create a buffer for asynchronous receiving of data from the client
            inUse = false;                      //buggy is not used by any client at the beginning
            connectedToBuggy = -1;              //client is not connected to any buggy at the beginning 

            clientReceive();                    //Begin asynchronously receiving data
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
                    //new size of array = previous bytes in the array + bytes that arrived.
                    Array.Resize(ref unqueuedBytesBuffer, bytesLeft);

                    //copy new bytes from callback buffer into the unqued bytes are for processing.
                    Buffer.BlockCopy(callbackBuffer, 0, unqueuedBytesBuffer, copyOffset, bytesReceived); 
                    Console.WriteLine("Number of bytes available: " + bytesLeft);
                    while (bytesLeft > 0)
                    {
                        //place the request data into a temporary queue.
                        switch (unqueuedBytesBuffer[0])
                        {
                            case RequestTypes.ListAddClient: //request identified as ListAddClient
                                if (bytesLeft >= 17)
                                {
                                    //place data into temporary queue
                                    tempQueue.Enqueue(ExtractRequest(unqueuedBytesBuffer, 17));

                                    //bytes left in the unqueued byte buffer is now equal itself minus the size of the request
                                    bytesLeft -= 17;

                                    //copy the data that has not been placed into the temporary queue into the same buffer but in the first position which in effect overwrites the data that has been placed into the temporary queue.
                                    Buffer.BlockCopy(unqueuedBytesBuffer, 18, unqueuedBytesBuffer, 0, bytesLeft);

                                    //resize the buffer to the new size that is equal to the number of bytes in the buffer.
                                    Array.Resize(ref unqueuedBytesBuffer, bytesLeft);
                                }
                                else
                                {
                                    //if the command is recognised but not all required bytes arrived then jump 'breakout' to continue with the code
                                    goto breakout;
                                }
                                break;
                            case RequestTypes.ListRemoveClient:
                                if (bytesLeft >= 5)
                                {
                                    tempQueue.Enqueue(ExtractRequest(unqueuedBytesBuffer, 5));
                                    bytesLeft -= 5;
                                    Buffer.BlockCopy(unqueuedBytesBuffer, 5, unqueuedBytesBuffer, 0, bytesLeft);
                                    Array.Resize(ref unqueuedBytesBuffer, bytesLeft);
                                }
                                else
                                {
                                    //if the command is recognised but not all required bytes arrived then jump 'breakout' to continue with the code
                                    goto breakout;
                                }
                                break;
                            case RequestTypes.BuggyConnect:
                                if(bytesLeft >= 5)
                                {
                                    byte[] byteBuff = new byte[9];
                                    Buffer.BlockCopy(ExtractRequest(unqueuedBytesBuffer, 5), 0, byteBuff, 0, 5);
                                    Buffer.BlockCopy(BitConverter.GetBytes(clientID), 0, byteBuff, 5, 4);           //Also pass the client id that sent the request
                                    tempQueue.Enqueue(byteBuff);
                                    bytesLeft -= 5;
                                    Buffer.BlockCopy(unqueuedBytesBuffer, 5, unqueuedBytesBuffer, 0, bytesLeft);
                                    Array.Resize(ref unqueuedBytesBuffer, bytesLeft);
                                }
                                else
                                {
                                    goto breakout;
                                }
                                break;
                            case RequestTypes.BuggyDisconnect:
                                if (bytesLeft >= 1)
                                {
                                    //Add sender ID to the request
                                    byte[] byteBuff = new byte[5];
                                    Buffer.BlockCopy(ExtractRequest(unqueuedBytesBuffer, 1), 0, byteBuff, 0, 1);
                                    Buffer.BlockCopy(BitConverter.GetBytes(clientID), 0, byteBuff, 1, 4);           //Also pass the client id that sent the request

                                    tempQueue.Enqueue(byteBuff);
                                    bytesLeft -= 1;
                                    Buffer.BlockCopy(unqueuedBytesBuffer, 1, unqueuedBytesBuffer, 0, bytesLeft);
                                    Array.Resize(ref unqueuedBytesBuffer, bytesLeft);
                                }
                                else
                                {
                                    goto breakout;
                                }
                                break;
                            default:
                                break;
                        }

                    }

                     //if the command is recognised but not all required bytes arrived then jump to here to continue with the code
                    breakout:


                    RequestHandler.RequestQueueMutex.WaitOne();                         //Wait for signal that it's okay to enter
                    while (tempQueue.Count > 0)                                         //Keep adding to request queue until the temporary queue is empty.
                    {
                        RequestHandler.RequestQueue.Enqueue(tempQueue.Dequeue());       //Add request data to the queue
                    }
                    RequestHandler.RequestQueueMutex.ReleaseMutex();                    //Release the mutex

                    clientReceive();
                }
            }
            catch(Exception e)  //In the case of a disconnection, remove the disconnected client and update controller clients
            {
                Console.WriteLine("ReceiveCallback");
                RequestHandler.RemoveClient(clientID);  //add request to remove client to the request handler
            }
        }

        //asynchronously send data to target client
        public void clientSend(byte[] buffer)
        {
            clientSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, SendCallback, clientSocket); //send data to target client
        }

        //callback function when data was sent.
        public void SendCallback(IAsyncResult asyncResult)
        {
            //do nothing
        }

        //Used the extract the request data from client buffers that hold request data from either the server or buggy.
        private Byte[] ExtractRequest(Byte[] bytes, int size)
        {
            //create an array of size of the request
            Byte[] requestBytes = new byte[size];

            //copy the bytes from the client buffer to the new array
            Buffer.BlockCopy(bytes, 0, requestBytes, 0, size);

            //return the extracted request data
            return requestBytes;
        }

    }
}
