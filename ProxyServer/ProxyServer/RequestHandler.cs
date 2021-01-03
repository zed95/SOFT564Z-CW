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
        static Thread RequestHandlerThread = new Thread(RequestHandler.HandleRequest);

        static public void StartRequestHandlerThread()
        {
            if (!RequestHandlerThread.IsAlive)
            {
                RequestHandlerThread.Start();
            }
        }

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
                        request = new byte[RequestQueue.Peek().Length];
                        request = RequestQueue.Dequeue(); //Remove request data to the queue
                        count = request.Length;         //Count the number of bytes in the messages buffer.
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
                    case RequestTypes.IsAlive:
                        CheckAllClients(request);
                        break;
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


        static public void IsAlive()
        {
            List<object> request = new List<object>();
            List<int> dataType = new List<int>();
            byte[] requestByteArray = new byte[1];

            while (true)
            {
                request.Add(RequestTypes.IsAlive);
                dataType.Add(VarTypes.typeByte);

                requestByteArray = byteConverter(request, dataType, 1);

                RequestQueueMutex.WaitOne();
                RequestQueue.Enqueue(requestByteArray);
                RequestQueueMutex.ReleaseMutex();

                request.Clear();
                dataType.Clear();
                Thread.Sleep(500);
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
            int buggyIndex;
            int controllerClientIndex;
            List<object> request = new List<object>();
            List<int> dataType = new List<int>();
            byte[] requestByteArray = new byte[9];

            request.Add(RequestTypes.BuggyConnectResponse);
            dataType.Add(VarTypes.typeByte);

            //find where the clients reside in the connected client list. if -1 returned, client no longer exists in the list andf therefore has been disconnected.
            buggyIndex = clientManager.Clients.FindIndex(x => x.clientID == buggyID);
            controllerClientIndex = clientManager.Clients.FindIndex(x => x.clientID == senderID);

            if (buggyIndex != -1)   //Proceed if the buggy is still connected
            {
                if (!clientManager.Clients[buggyIndex].inUse)
                {
                    clientManager.Clients[buggyIndex].inUse = true;
                    clientManager.Clients[controllerClientIndex].connectedToBuggy = buggyID;
                    request.Add(BuggyConnectResponse.ConnectPermitted);
                    dataType.Add(VarTypes.typeByte);
                }
                else
                {
                    request.Add(BuggyConnectResponse.BuggyInUse);
                    dataType.Add(VarTypes.typeByte);
                }
            }
            else //otherwise send response back to controller client that buggy is no longer connected to the server
            {
                request.Add(BuggyConnectResponse.BuggyConnectionLost);
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
            //Check if the buggy is still connected by checking the result of buggyIndex. If -1, the buggy has been disconnected
            if (buggyIndex != (-1))
            {
                clientManager.Clients[buggyIndex].inUse = false;
            }
        }

        //fulfil the request to send response to target controller client
        static public void SendResponse(byte[] request)
        {
            //find the list index of the target client to send the response to
            int targetClient = BitConverter.ToInt32(request, 2);
            int index = clientManager.Clients.FindIndex(x => x.clientID == targetClient);

            Array.Resize(ref request, 2);   //resize the array to the actual amount of bytes to be sent
            clientManager.Clients[index].clientSend(request);
        }

        //converts requests and its data to an array of bytes in preparation for transmission over the network
        static public byte[] byteConverter(List<object> request, List<int> dataTypes, int byteCount)
        {
            byte[] byteArray = new byte[byteCount];
            int offset = 0;

            //Convert all data in the request array into bytes. before the data can be converted from the object list, it needs to be cast to the appopriate datatype.
            //This is the reason the dataTypes list exists. It holds the datatypes of the data that has been placed into the request list.
            //Convert all datatypes to bytes and put into a byte array in preparation for transmission
            for (int x = 0; x < request.Count; x++)
            {
                switch (dataTypes[x])
                {
                    //copy the data from the request array into to byteArray as bytes then increase the offset by the number of bytes that the data consisted of to allow the
                    //next piece of data from the request array to be copied into the byteArray without overwriting data that's already in there.
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

        //sends a request to all connected clients to see if they are still connected.
        public static void CheckAllClients(byte[] request)
        {

            foreach (Client clients in clientManager.Clients)
            {
                try
                {
                    clients.clientSocket.BeginSend(request, 0, request.Length, SocketFlags.None, clients.SendCallback, clients.clientSocket); //send data to the other clients
                }
                catch
                {
                    //error in sending isAlive request to the client. Remove it as it disconnected.
                    Console.WriteLine("Begind Send Exception");
                    Console.WriteLine("Removing Client: " + clients.clientID);
                    RemoveClient(clients.clientID);
                }
            }
        }

        //send data to all clients
        public static void sendAllClients(byte[] request)
        {

            foreach (Client clients in clientManager.Clients)
            {
                
                try
                {
                    if (request[0] == RequestTypes.ListAddClient)   //if the request is for other clients to add a client to the list:
                    {
                        //prevents the data about the newest client being sent to itself
                        if (clients.clientID != clientManager.Clients[clientManager.Clients.Count - 1].clientID)
                        {
                            clients.clientSocket.BeginSend(request, 0, request.Length, SocketFlags.None, clients.SendCallback, clients.clientSocket); //send data to the other clients
                        }
                    }
                    else if(request[0] == RequestTypes.ListRemoveClient) //if the request is for other clients to delete a disconnected client from their list
                    {
                        clients.clientSocket.BeginSend(request, 0, request.Length, SocketFlags.None, clients.SendCallback, clients.clientSocket); //send data to the other clients
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception in sendAllClients");
                    //remove the client who disconnected when attempt was made to send data to it
                    RemoveClient(clients.clientID);
                }
            }
        }


        //sned newly connected client information about all other connected clients.
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
                    //Add request type to the request object list and the request's type to the dataType list
                    request.Add(RequestTypes.ListAddClient);
                    dataType.Add(VarTypes.typeByte);

                    //extract ipaddress of the client and convert it to a long and add it to the request list. The add the its datatype to the dataType list
                    endPoint = (IPEndPoint)clients.clientSocket.RemoteEndPoint;
                    IPAddress addr = endPoint.Address;
                    StrIPAddr = addr.ToString();
                    request.Add(Server.IPtoLong(StrIPAddr));
                    dataType.Add(VarTypes.typeLong);

                    //add the port
                    request.Add(endPoint.Port);
                    dataType.Add(VarTypes.typeInt32);

                    //add client id
                    request.Add(clients.clientID);
                    dataType.Add(VarTypes.typeInt32);

                    //convert the data from the request list to a byte array for sending over the network
                    requestByteArray = RequestHandler.byteConverter(request, dataType, 17);

                    if (clients.clientID != clientManager.Clients[clientManager.Clients.Count - 1].clientID)        //Do not send information about the newest client to itself
                    {
                        clientManager.Clients[clientManager.Clients.Count - 1].clientSocket.BeginSend(requestByteArray, 0, requestByteArray.Length, SocketFlags.None, clients.SendCallback, clients.clientSocket); //send data to the other clients
                    }

                    //clear the lists for the next iteration
                    request.Clear();
                    dataType.Clear();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception in sendAllClients");
                    //remove client that was disconnected
                    RemoveClient(clients.clientID);
                }
            }
        }

    }


    //A class with a list of all possible request types.
    static class RequestTypes
    {
        public const byte IsAlive              = 0;
        public const byte ListAddClient        = 1;
        public const byte ListRemoveClient     = 2;
        public const byte BuggyConnect         = 6;
        public const byte BuggyConnectResponse = 7;
        public const byte BuggyDisconnect      = 8;
        public const byte AddNewClient         = 14;
        public const byte RemoveClient         = 15;
    }


    //a class of variable types used in the byte converter to convert request data into bytes
    static class VarTypes
    {
        public const int typeByte = 1;
        public const int typeInt32 = 2;
        public const int typeLong = 3;
    }

    //class containing all the responses to the controller client asking for permission to connect to a buggy
    static class BuggyConnectResponse
    {
        public const byte ConnectPermitted = 1;
        public const byte BuggyInUse = 2;
        public const byte BuggyConnectionLost = 3;
    }

}
