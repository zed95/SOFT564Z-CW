using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
                        break;
                    case RequestTypes.ListRemoveClient:
                        break;
                    case RequestTypes.BuggyConnect:
                        BuggyConnect(BitConverter.ToInt32(request, 1), BitConverter.ToInt32(request, 5));
                        break;
                    case RequestTypes.BuggyConnectResponse:
                        SendResponse(request);
                        break;
                    default:

                        break;
                }

            }
        }


        static public void BuggyConnect(int buggyID, int senderID)
        {
            int index;
            List<object> request = new List<object>();
            List<int> dataType = new List<int>();
            byte[] requestByteArray = new byte[9];

            request.Add(RequestTypes.BuggyConnectResponse);
            dataType.Add(VarTypes.typeByte);


            index = clientManager.Clients.FindIndex(x => x.clientID == buggyID);
            if(!clientManager.Clients[index].inUse)
            {
                clientManager.Clients[index].inUse = true;
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

    }

    //A class with a list of all possible request types.
    static class RequestTypes
    {
        public const int ListAddClient = 1;
        public const int ListRemoveClient = 2;
        public const byte BuggyConnect = 6;
        public const byte BuggyConnectResponse = 7;
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
