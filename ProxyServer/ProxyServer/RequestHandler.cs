using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace ProxyServer
{
    static class RequestHandler
    {
        static public bool newClient = false;
        static public bool removeClient = false;
        static public Queue<Byte[]> RequestQueue = new Queue<byte[]>();
        public static Mutex RequestQueueMutex = new Mutex();

        static public void HandleRequest()
        {
            Byte[] request;
            int count = 0;

            while (true)
            {
                while (true)
                {
                    //Console.WriteLine("Request handler is requesting queue access");
                    RequestQueueMutex.WaitOne();                 //Wait for signal that it's okay to enter
                    if (RequestQueue.Count > 0)
                    {
                        Console.WriteLine("Request handler has queue access");
                        request = new byte[RequestQueue.Peek().Length];
                        request = RequestQueue.Dequeue(); //Remove request data to the queue
                        count = request.Length;         //Count the number of bytes in the messages buffer.
                        Console.WriteLine("Request handler is releasing mutex");
                        RequestQueueMutex.ReleaseMutex();            //Release the mutex
                        break;
                    }
                    else
                    {
                        RequestQueueMutex.ReleaseMutex();            //Release the mutex
                    }
                }


                for (int x = 0; x < request.Length; x++)
                {
                    Console.WriteLine(request[x]);
                }
                Console.WriteLine("Number of bytes in buffer: " + count);


                //Turn the data back into appropriate format based on type of request.
                switch (request[0])
                {
                    case RequestTypes.ListAddClient:
                        sendAllClients(request);
                        sendClientAll();
                        break;
                    case RequestTypes.ListRemoveClient:
                        sendAllClients(request);
                        break;
                    case RequestTypes.BuggyConnect:
                        BuggyConnect(BitConverter.ToInt32(request, 1), BitConverter.ToInt32(request, 5));
                        break;
                    case RequestTypes.BuggyConnectResponse:
                        SendResponse(request);
                        break;
                    case RequestTypes.BuggyDisconnect:
                        BuggyDisconnect(BitConverter.ToInt32(request, 1));
                        break;
                    case RequestTypes.AddNewClient:
                        AddNewClient();
                        break;
                    case RequestTypes.RemoveClient:
                        clientManager.RemoveClient(BitConverter.ToInt32(request, 1));
                        break;
                    default:

                        break;
                }

            }
        }


        static public void AddNewClient()
        {
            Server.SocketQueueMutex.WaitOne();
            clientManager.AddClient(Server.SocketQueue.Dequeue());
            Server.SocketQueueMutex.ReleaseMutex();
        }

        static public void RemoveClient(int clientID)
        {
            List<object> request = new List<object>();
            List<int> dataType = new List<int>();
            byte[] requestByteArray = new byte[5];

            request.Add(RequestTypes.RemoveClient);
            dataType.Add(VarTypes.typeByte);

            request.Add(clientID);
            dataType.Add(VarTypes.typeInt32);

            requestByteArray = byteConverter(request, dataType, 5);

            RequestQueueMutex.WaitOne();
            RequestQueue.Enqueue(requestByteArray);
            RequestQueueMutex.ReleaseMutex();
        } 

        static public void BuggyConnect(int buggyID, int senderID)
        {
            int index;
            int controllerClientIndex;
            List<object> request = new List<object>();
            List<int> dataType = new List<int>();
            byte[] requestByteArray = new byte[9];

            request.Add(RequestTypes.BuggyConnectResponse);
            dataType.Add(VarTypes.typeByte);


            index = clientManager.Clients.FindIndex(x => x.clientID == buggyID);
            controllerClientIndex = clientManager.Clients.FindIndex(x => x.clientID == senderID);

            if (!clientManager.Clients[index].inUse)
            {
                clientManager.Clients[index].inUse = true;
                clientManager.Clients[controllerClientIndex].connectedToBuggy = buggyID;
                request.Add(BuggyConnectResponse.ConnectPermitted);
                dataType.Add(VarTypes.typeByte);
            }
            else
            {
                request.Add(BuggyConnectResponse.BuggyInUse);
                dataType.Add(VarTypes.typeByte);
            }

            request.Add(senderID);
            dataType.Add(VarTypes.typeInt32);

            requestByteArray = byteConverter(request, dataType, 6);

            RequestQueueMutex.WaitOne();
            RequestQueue.Enqueue(requestByteArray);
            RequestQueueMutex.ReleaseMutex();

        }

        static public void BuggyDisconnect(int senderID)
        {
            int controllerClientIndex;
            int buggyID, buggyIndex;

            //Find the buggy id controller client is connected to and the buggy's index in the client list
            controllerClientIndex = clientManager.Clients.FindIndex(x => x.clientID == senderID);
            buggyID = clientManager.Clients[controllerClientIndex].connectedToBuggy;
            buggyIndex = clientManager.Clients.FindIndex(x => x.clientID == buggyID);

            //clear the buggy id that the client was connected to and set the buggy use status to false
            clientManager.Clients[controllerClientIndex].connectedToBuggy = -1;
            clientManager.Clients[buggyIndex].inUse = false;
        }

        static public void SendResponse(byte[] request)
        {
            int targetClient = BitConverter.ToInt32(request, 2);
            int index = clientManager.Clients.FindIndex(x => x.clientID == targetClient);

            Array.Resize(ref request, 2);   //resize the array to the actual amount of bytes to be sent
            clientManager.Clients[index].clientSend(request);
        }

        static public byte[] byteConverter(List<object> request, List<int> dataTypes, int byteCount)
        {
            byte[] byteArray = new byte[byteCount];
            int offset = 0;

            //Convert all datatypes to bytes and put into a byte array in preparation for transmission
            for (int x = 0; x < request.Count; x++)
            {
                switch (dataTypes[x])
                {
                    case VarTypes.typeByte:
                        Buffer.BlockCopy(BitConverter.GetBytes((byte)request[x]), 0, byteArray, offset, 1);
                        offset += 1;
                        break;
                    case VarTypes.typeInt32:
                        Buffer.BlockCopy(BitConverter.GetBytes((int)request[x]), 0, byteArray, offset, 4);
                        offset += 4;
                        break;
                    case VarTypes.typeLong:
                        Buffer.BlockCopy(BitConverter.GetBytes((long)request[x]), 0, byteArray, offset, 8);
                        offset += 8;
                        break;
                    default:
                        break;
                }
            }

            return byteArray;
        }

        static public void ListAddClient(Client client)
        {
            List<object> request = new List<object>();
            List<int> dataType = new List<int>();
            byte[] requestByteArray = new byte[17];
            String StrIPAddr;
            IPEndPoint endPoint;

            request.Add(RequestTypes.ListAddClient);
            dataType.Add(VarTypes.typeByte);

            endPoint = (IPEndPoint)client.clientSocket.RemoteEndPoint;
            IPAddress addr = endPoint.Address;
            StrIPAddr = addr.ToString();
            request.Add(Server.IPtoLong(StrIPAddr));
            dataType.Add(VarTypes.typeLong);

            request.Add(endPoint.Port);
            dataType.Add(VarTypes.typeInt32);

            request.Add(client.clientID);
            dataType.Add(VarTypes.typeInt32);

            requestByteArray = RequestHandler.byteConverter(request, dataType, 17);

            RequestQueueMutex.WaitOne();
            RequestQueue.Enqueue(requestByteArray);
            RequestQueueMutex.ReleaseMutex();

        }

        static public void ListRemoveClient(Client client)
        {
            List<object> request = new List<object>();
            List<int> dataType = new List<int>();
            byte[] requestByteArray = new byte[5];

            request.Add(RequestTypes.ListRemoveClient);
            dataType.Add(VarTypes.typeByte);

            request.Add(client.clientID);
            dataType.Add(VarTypes.typeInt32);

            requestByteArray = RequestHandler.byteConverter(request, dataType, 5);

            RequestQueueMutex.WaitOne();
            RequestQueue.Enqueue(requestByteArray);
            RequestQueueMutex.ReleaseMutex();
        }



        public static void sendAllClients(byte[] request)
        {

            foreach (Client clients in clientManager.Clients)
            {
                
                try
                {
                    if (request[0] == RequestTypes.ListAddClient)
                    {
                        //prevents the data about the newest client being sent to itself
                        if (clients.clientID != clientManager.Clients[clientManager.Clients.Count - 1].clientID)
                        {
                            clients.clientSocket.BeginSend(request, 0, request.Length, SocketFlags.None, clients.SendCallback, clients.clientSocket); //send data to the other clients
                        }
                    }
                    else if(request[0] == RequestTypes.ListRemoveClient)
                    {
                        clients.clientSocket.BeginSend(request, 0, request.Length, SocketFlags.None, clients.SendCallback, clients.clientSocket); //send data to the other clients
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception in sendAllClients");
                }
            }
        }


        public static void sendClientAll()
        {
            List<object> request = new List<object>();
            List<int> dataType = new List<int>();
            byte[] requestByteArray = new byte[17];
            String StrIPAddr;
            IPEndPoint endPoint;

            foreach (Client clients in clientManager.Clients)
            {
                try
                {
                    request.Add(RequestTypes.ListAddClient);
                    dataType.Add(VarTypes.typeByte);

                    endPoint = (IPEndPoint)clients.clientSocket.RemoteEndPoint;
                    IPAddress addr = endPoint.Address;
                    StrIPAddr = addr.ToString();
                    request.Add(Server.IPtoLong(StrIPAddr));
                    dataType.Add(VarTypes.typeLong);

                    request.Add(endPoint.Port);
                    dataType.Add(VarTypes.typeInt32);

                    request.Add(clients.clientID);
                    dataType.Add(VarTypes.typeInt32);

                    requestByteArray = RequestHandler.byteConverter(request, dataType, 17);

                    if (clients.clientID != clientManager.Clients[clientManager.Clients.Count - 1].clientID)        //Do not send information about the newest client to itself
                    {
                        clientManager.Clients[clientManager.Clients.Count - 1].clientSocket.BeginSend(requestByteArray, 0, requestByteArray.Length, SocketFlags.None, clients.SendCallback, clients.clientSocket); //send data to the other clients
                    }

                    request.Clear();
                    dataType.Clear();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception in sendAllClients");
                }
            }
        }

    }


    //A class with a list of all possible request types.
    static class RequestTypes
    {
        public const byte ListAddClient        = 1;
        public const byte ListRemoveClient     = 2;
        public const byte BuggyConnect         = 6;
        public const byte BuggyConnectResponse = 7;
        public const byte BuggyDisconnect      = 8;
        public const byte AddNewClient         = 14;
        public const byte RemoveClient         = 15;
    }


    static class VarTypes
    {
        public const int typeByte = 1;
        public const int typeInt32 = 2;
        public const int typeLong = 3;
    }

    static class BuggyConnectResponse
    {
        public const byte ConnectPermitted = 1;
        public const byte BuggyInUse = 2;
    }

}
