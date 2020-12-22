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
                        case RequestTypes.SendEnvData:
                            ConnectionManager.Connections[1].asyncSend(request);
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
                        case RequestTypes.MoveBuggy:
                            ConnectionManager.Connections[1].asyncSend(request);
                            break;
                        case RequestTypes.InteractionMode:
                            ConnectionManager.Connections[1].asyncSend(request);
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

        static public void SendEnvData()
        {
            List<object> request = new List<object>();
            List<int> dataType = new List<int>();
            byte[] requestByteArray = new byte[1];

            request.Add(RequestTypes.SendEnvData);
            dataType.Add(VarTypes.typeByte);

            requestByteArray = byteConverter(request, dataType, 1);

            RequestQueueMutex.WaitOne();                 //Wait for signal that it's okay to enter
            MessageHandler.RequestQueue.Enqueue(requestByteArray);
            RequestQueueMutex.ReleaseMutex();            //Release the mutex
        }


        static public void MotorControl()
        {
            //Make copies of the current states 
            byte forwardL = 0;
            byte reverseL = 0;
            byte rightL = 0;
            byte leftL = 0;
            byte direction;
            List<object> request = new List<object>();
            List<int> dataType = new List<int>();
            byte[] requestByteArray = new byte[2];

            while (true)
            {
                while (BuggyMotorControl.pauseMotorControl) { };

                if (BuggyMotorControl.forward) { forwardL = 1; } else { forwardL = 0; }
                if (BuggyMotorControl.reverse) { reverseL = 1; } else { reverseL = 0; }
                if (BuggyMotorControl.right) { rightL = 1; } else { rightL = 0; }
                if (BuggyMotorControl.left) { leftL = 1; } else { leftL = 0; }

                direction = (byte)((leftL << 3) | (rightL << 2) | (reverseL << 1) | (forwardL << 0));



                // 1111 = left, right, reverse, forward

                //0000      no motion
                //0001      forward
                //0010      reverse
                //0011      reverse & forward = no motion
                //0100      right = rotate clockwise
                //0101      forward and right = turn right
                //0110      reverse and right = reverse right
                //0111      forward, reverse and right = rotate clockwise
                //1000      left = rotate anti-clockwise
                //1001      forward and left = turn left;
                //1010      reverse and left = reverse left;
                //1011      forward, reverse and left = rotate anti-clockwise
                //1100      left and right = no motion
                //1101      left right and forward = forward
                //1110      left, right, reverse = reverse
                //1111      no motion
                if (BuggyMotorControl.previousState != direction)
                {
                    request.Add(RequestTypes.MoveBuggy);
                    dataType.Add(VarTypes.typeByte);

                    request.Add(direction);
                    dataType.Add(VarTypes.typeByte);

                    requestByteArray = byteConverter(request, dataType, 2);

                    RequestQueueMutex.WaitOne();                 //Wait for signal that it's okay to enter
                    MessageHandler.RequestQueue.Enqueue(requestByteArray);
                    RequestQueueMutex.ReleaseMutex();            //Release the mutex

                    request.Clear();
                    dataType.Clear();
                }

                BuggyMotorControl.previousState = direction;
            }
        }

        public static void InteractionMode(byte interactionMode)
        {
            List<object> request = new List<object>();
            List<int> dataType = new List<int>();
            byte[] requestByteArray = new byte[2];

            request.Add(RequestTypes.InteractionMode);
            dataType.Add(VarTypes.typeByte);

            request.Add(interactionMode);
            dataType.Add(VarTypes.typeByte);

            requestByteArray = byteConverter(request, dataType, 2);

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
        public const byte  MoveBuggy            = 5;
        public const byte  BuggyConnect         = 6;
        public const byte  BuggyConnectResponse = 7;
        public const byte  InteractionMode      = 9;    

    }

    static class VarTypes
    {
        public const int typeByte = 1;
        public const int typeInt32 = 2;
    }

    static class InteractionMode
    {
        public const byte Manual = 1;
        public const byte Configuration = 2;
        public const byte Autonomous = 3;
    }

    static class BuggyMotorControl
    {
        public static bool forward = false;
        public static bool reverse = false;
        public static bool right = false;
        public static bool left = false;
        public static byte previousState = 0;
        static Thread MotorControlInput = new Thread(MessageHandler.MotorControl);
        public volatile static bool pauseMotorControl = true;

        static public void StartMotorControl()
        {
            MotorControlInput.Start();
        }
        
        static public void PauseMotorControl()
        {
            pauseMotorControl = true;
        }

        static public void RestartMotorControl()
        {
            pauseMotorControl = false;
        }

    }
}
