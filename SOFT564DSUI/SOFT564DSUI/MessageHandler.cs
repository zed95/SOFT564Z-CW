using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;



namespace SOFT564DSUI
{

    //Used to decode the message/command/request and decide what to do with it.
    //At the moment it is experimental. When I come up with a proper message format I will update this class.
    static class MessageHandler
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
                            clientManager.clientIPAddress = BitConverter.ToInt64(request, 1);
                            clientManager.clientPort = BitConverter.ToInt32(request, 9);
                            clientManager.clientID = BitConverter.ToInt32(request, 13);
                            Console.WriteLine("Here");
                            Console.WriteLine(clientManager.clientIPAddress);
                            Console.WriteLine(clientManager.clientPort);
                            Console.WriteLine(clientManager.clientID);

                            clientManager.AddClient();
                            newClient = true;
                            break;
                        case RequestTypes.ListRemoveClient:
                            clientManager.clientID = BitConverter.ToInt32(request, 2);

                            clientManager.RemoveClient();
                            removeClient = true;

                            break;
                        case RequestTypes.RecEnvData:
                        Console.WriteLine("Received env data");
                            EnvData.temperature = BitConverter.ToInt16(request, 1);
                            EnvData.TempToFloat();
                            EnvData.humidity = request[3];
                            EnvData.lIntensity = BitConverter.ToInt16(request, 4);
                            break;
                        case RequestTypes.BuggyConnect:
                            ConnectionManager.Connections[0].asyncSend(request);
                            break;
                        case RequestTypes.BuggyConnectResponse:
                            BuggyConnectResponse.response = request[1];
                            break;
                        default:

                            break;
                    }

            }
        }


        static public void BuggyConnect(int buggyID)
        {
            List<object> request = new List<object>();
            List<int> dataType = new List<int>();
            byte[] requestByteArray = new byte[5];

            request.Add(RequestTypes.BuggyConnect);
            dataType.Add(VarTypes.typeByte);

            request.Add(buggyID);
            dataType.Add(VarTypes.typeInt32);

            requestByteArray = byteConverter(request, dataType, 5);

            RequestQueueMutex.WaitOne();                 //Wait for signal that it's okay to enter
            MessageHandler.RequestQueue.Enqueue(requestByteArray);
            RequestQueueMutex.ReleaseMutex();            //Release the mutex
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
        public const byte  ListAddClient        = 1;
        public const byte  ListRemoveClient     = 2;
        public const byte  SendEnvData          = 3;
        public const byte  RecEnvData           = 4;
        public const byte  BuggyConnect         = 6;
        public const byte  BuggyConnectResponse = 7;

    }

    static class VarTypes
    {
        public const int typeByte = 1;
        public const int typeInt32 = 2;
    }
}
