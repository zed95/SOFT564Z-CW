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
        public byte[] buffer = new byte[1460];

        static Thread RequestHandlerThread = new Thread(MessageHandler.HandleRequest);

        static public void initClient(String serverIP, Int32 port)
        {
            try
            {
                if (!RequestHandlerThread.IsAlive)
                {
                    RequestHandlerThread.Start();
                }
                ConnectionManager.AddClient(IPtoLong(serverIP), port);

                ConnectionEstablished = true;
            }   
              catch(Exception e)
             {
                ConnectionEstablished = false;
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

    static class ConnectionManager
    {
        public static List<ConnectionInstance> Connections = new List<ConnectionInstance>();

        public static void AddClient(long ip, int port)
        {
            Connections.Add(new ConnectionInstance(ip, port, AssignID()));                            //Add new connection to the list
        }

        public static void RemoveClient(int id)
        {
            if (Connections.Exists(ClientToRemove => ClientToRemove.connectionID == id))              //Prevent Multiple exceptions from calling for removal of the client by checking if client of such id exists
            {
                Connections.RemoveAt(Connections.FindIndex(x => x.connectionID == id));               //Remove the client from the list
                //change the request type numbers into meaningful constants.
            }
        }

        //Generates a unique client id for newly connected clients.
        private static int AssignID()
        {
            int newID = 0;

            foreach (ConnectionInstance client in Connections)
            {
                newID += client.connectionID;
            }

            newID += Connections.Count;

            return newID;
        }

    }

    class ConnectionInstance
    {
        public int connectionID;
        private Socket socket;
        private IPEndPoint endPoint;
        public Byte[] callbackBuffer = new byte[1460];
        private Byte[] unqueuedBytesBuffer = new byte[0];


        public ConnectionInstance(long ip, int port, int id)
        {
            connectionID = id;
            ConnectClientTo(ip, port);
        }

        private void ConnectClientTo(long ip, int port)
        {
            try
            {
                //The massive number represents the ip address
                endPoint = new IPEndPoint(ip, port);
                socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp); //setup a new socket to connect to the server.
                socket.BeginConnect(endPoint, ConnectionCallback, null);    //Start attempting to connect to the remote host.

                //ConnectionEstablished = true;
            }
            catch (Exception e)
            {
                //ConnectionEstablished = false;
            }
        }

        private void ConnectionCallback(IAsyncResult asyncResult)
        {
            socket.EndConnect(asyncResult);     //stop requesting connection when connected;
            //Start listening for incoming bytes.
            socket.BeginReceive(callbackBuffer, 0, callbackBuffer.Length, SocketFlags.None, ReceiveCallback, null); //start listening for incoming data from the server.
        }


        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            //Get the number of received bytes
            int bytesReceived = socket.EndReceive(asyncResult);
            int bytesLeft = bytesReceived + unqueuedBytesBuffer.Length; //the number of bytes is equal to the number of bytes that have not yet been processed + the number of new bytes that arrived.
            int copyOffset = unqueuedBytesBuffer.Length;        //new bytes to be copy at an offset of the of the old array before its size increases.
            Queue<Byte[]> tempQueue = new Queue<byte[]>();

            //if bytes available then do something
            if (bytesReceived > 0)
            {
                Array.Resize(ref unqueuedBytesBuffer, bytesLeft);   //new size of array = previous bytes in the array + bytes that arrived.
                Buffer.BlockCopy(callbackBuffer, 0, unqueuedBytesBuffer, copyOffset, bytesReceived); //copy new bytes from callback buffer into the unqued bytes are for processing.
                Console.WriteLine("Number of bytes available: " + bytesLeft);
                while (bytesLeft > 0)
                {
                    for(int x = 0; x < bytesLeft; x++)
                    {
                        Console.WriteLine("unqueued buffer bytes: " + unqueuedBytesBuffer[x]);
                    }
                    switch (unqueuedBytesBuffer[0])     //if the data sent does not match any request types the while loop freezes as nothing is currently done with that byte. Need to fix that.----------mkhn
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
                        case RequestTypes.RecEnvData:
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

                    /*
                     * The weakpoint of this approach is that if there is more than 0 bytes in the buffer, the code will be stuck in the loop and won't let the program fetch more bytes.
                     * The break statements in the switch break out of the switch statement and not the loop.
                     * if I have a break outside of the switch it will breakout of the loop after one command in queued even though there may be more commands in the queue.
                     */
                     
                }

                //if the command is recognised but not all required bytes arrived then jump to here to continue with the code
                breakout:

                Console.WriteLine("Connection " + connectionID + " is requesting queue access");
                MessageHandler.RequestQueueMutex.WaitOne();                 //Wait for signal that it's okay to enter
                Console.WriteLine("Connection " + connectionID + " has queue access");
                while (tempQueue.Count > 0) //Keep adding to request queue until the temporary queue is empty.
                {
                    MessageHandler.RequestQueue.Enqueue(tempQueue.Dequeue());               //Add request data to the queue
                }
                Console.WriteLine("Connection " + connectionID + " is releasing mutex");
                MessageHandler.RequestQueueMutex.ReleaseMutex();            //Release the mutex
            }

            //start waiting for bytes again. 
            socket.BeginReceive(callbackBuffer, 0, callbackBuffer.Length, SocketFlags.None, ReceiveCallback, null);
        }


        public void asyncSend(List<object> request, List<int> dataType, int byteCount)
        {
            byte[] buffer = new byte[byteCount];
            int offset = 0;
            

            //Convert all datatypes to bytes and put into a byte array in preparation for transmission
            for(int x = 0; x < request.Count; x++)
            {
                switch(dataType[x])
                {
                    case VarTypes.typeByte:
                        Buffer.BlockCopy(BitConverter.GetBytes((byte)request[x]), 0, buffer, offset, 1);
                        offset += 1;
                        break;
                    case VarTypes.typeInt32:
                        Buffer.BlockCopy(BitConverter.GetBytes((byte)request[x]), 0, buffer, offset, 4);
                        offset += 4;
                        break;
                    default:
                        break;
                }
            }
            
            Console.WriteLine("Sending");
            socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, SendCallback, socket);     //start the transmission process
        }

        //asynchronous sending callback witht the result of the transmission attempt.
        private void SendCallback(IAsyncResult asyncResult)
        {

        }

        private Byte[] ExtractRequest(Byte[] bytes, int size)
        {
            Byte[] requestBytes = new byte[size];

            Buffer.BlockCopy(bytes, 0, requestBytes, 0, size);
            return requestBytes;
        }

    }

}
