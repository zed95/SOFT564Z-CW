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
        static public bool configStatusUpdate = false;
        static public bool removeClient = false;
        static public Queue<Byte[]> RequestQueue = new Queue<byte[]>();
        public static Mutex RequestQueueMutex = new Mutex();
        static Thread RequestHandlerThread = new Thread(MessageHandler.HandleRequest);

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
                    RequestQueueMutex.WaitOne();                 //Wait for signal that it's okay to enter. Only check if there is anything in the queue if handler has the mutex.
                    if (RequestQueue.Count > 0)                  //if there are any requests in the queue
                    {   
                        //Create request data buffer with the same size as the next element to come out of the queue.
                        request = new byte[RequestQueue.Peek().Length];

                        //Remove request data to the queue
                        request = RequestQueue.Dequeue();

                        //Count the number of bytes in the request buffer.
                        count = request.Length;
                        
                        //Release the mutex
                        RequestQueueMutex.ReleaseMutex();            
                        break;
                    }
                    else
                    {
                        //Release the mutex
                        RequestQueueMutex.ReleaseMutex();            
                    }
                }

                //*--* to delete after testing
                for (int x = 0; x < request.Length; x++)
                {
                    Console.WriteLine(request[x]);
                }
                Console.WriteLine("Number of bytes in buffer: " + count);

                //Carry out the request
                switch (request[0])
                {
                    case RequestTypes.ListAddClient:
                        ListAddClient(request);
                        break;
                    case RequestTypes.ListRemoveClient:
                        clientManager.clientID = BitConverter.ToInt32(request, 1);
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
                    case RequestTypes.BuggyDisconnect:
                        if(ConnectionManager.ConnectionLost == false)  //if connection with server is not lost 
                        {
                            try
                            {
                                ConnectionManager.Connections[0].asyncSend(request);    //Notify the server that the controller client is giving up control of the buggy.
                            }
                            catch
                            {
                                //index of buggy is now 0 (by default server index is 0 but on disconnection it is removed and the next object in the list becomes index 0)
                                ConnectionManager.Connections[0].DisconnectClient();    //Begin the process of disconnecting from the buggy.
                            }

                            try
                            {
                                ConnectionManager.Connections[1].DisconnectClient();    //Begin the process of disconnecting from the buggy.
                            }
                            catch
                            {

                            }
                        }
                        else
                        {
                            //index of buggy is now 0 (by default server index is 0 but on disconnection it is removed and the next object in the list becomes index 0)
                            if (ConnectionManager.Connections.Count > 0)
                            {
                                ConnectionManager.Connections[0].DisconnectClient();    //Begin the process of disconnecting from the buggy.
                            }
                        }
                        break;
                    case RequestTypes.MoveBuggy:
                        SendBuggy(request);
                        break;
                    case RequestTypes.InteractionMode:
                        SendBuggy(request);
                        break;
                    case RequestTypes.CurrConfigParam:
                        SendBuggy(request);
                        break;
                    case RequestTypes.SendCurrConfig:
                        BuggyConfigurationData.currConfigParam = BitConverter.ToInt32(request, 1);
                        break;
                    case RequestTypes.UpdateConfigOption:
                        SendBuggy(request);
                        break;
                    case RequestTypes.ConfigUpdateStatus:
                        BuggyConfigurationData.configUpdateStatus = request[1];
                        configStatusUpdate = true;
                        break;
                    case RequestTypes.BuggyOrClient:
                        ConnectionManager.Connections[0].asyncSend(request);
                        break;
                    default:

                        break;
                }

            }
        }

        //Sends request to the buggy
        static public void SendBuggy(byte[] request)
        {
            try     //Attempt to send request to the buggy
            {
                ConnectionManager.Connections[1].asyncSend(request);
            }
            catch   //if sending did not succeed then remove the buggy from connected clients list
            {
                ConnectionManager.connectionStatus(ConnectionManager.Connections[1].connectionID, 2);
            }
        }

        static public void ListAddClient(byte[] request)
        {
            //Convert the data in the request to actual ip address, port and client id.
            clientManager.clientIPAddress = BitConverter.ToInt64(request, 1);
            clientManager.clientPort = BitConverter.ToInt32(request, 9);
            clientManager.clientID = BitConverter.ToInt32(request, 13);

            //*--* to delete after testing
            Console.WriteLine("Here");
            Console.WriteLine(clientManager.clientIPAddress);
            Console.WriteLine(clientManager.clientPort);
            Console.WriteLine(clientManager.clientID);

            //call the function to add the client to the list witht he converted data.
            clientManager.AddClient();
            newClient = true;
        }

        static public void BuggyConnect(int buggyID)
        {
            List<object> request = new List<object>();                      //create a list that holds the reqquest data
            List<int> dataType = new List<int>();                           //Create a list that holds data types
            byte[] requestByteArray = new byte[5];                          //Create an array of required size for the request

            //Add data to the list and and the type of the data to another
            request.Add(RequestTypes.BuggyConnect);
            dataType.Add(VarTypes.typeByte);

            request.Add(buggyID);
            dataType.Add(VarTypes.typeInt32);

            //conver the data in the request list to an array of bytes for transmission
            requestByteArray = byteConverter(request, dataType, 5);

            RequestQueueMutex.WaitOne();                                        //Wait for mutes
            MessageHandler.RequestQueue.Enqueue(requestByteArray);              //Add the data to request arrary
            RequestQueueMutex.ReleaseMutex();                                   //Release the mutex
        }

        static public void BuggyDisconnect()
        {
            List<object> request = new List<object>();
            List<int> dataType = new List<int>();
            byte[] requestByteArray = new byte[1];

            request.Add(RequestTypes.BuggyDisconnect);
            dataType.Add(VarTypes.typeByte);

            requestByteArray = byteConverter(request, dataType, 1);

            RequestQueueMutex.WaitOne();                 
            MessageHandler.RequestQueue.Enqueue(requestByteArray);
            RequestQueueMutex.ReleaseMutex();            
        }

        static public void SendEnvData()
        {
            List<object> request = new List<object>();
            List<int> dataType = new List<int>();
            byte[] requestByteArray = new byte[1];

            request.Add(RequestTypes.SendEnvData);
            dataType.Add(VarTypes.typeByte);

            requestByteArray = byteConverter(request, dataType, 1);

            RequestQueueMutex.WaitOne();                 
            MessageHandler.RequestQueue.Enqueue(requestByteArray);
            RequestQueueMutex.ReleaseMutex();            
        }


        static public void CurrConfigParam(byte configurationOption)
        {
            List<object> request = new List<object>();
            List<int> dataType = new List<int>();
            byte[] requestByteArray = new byte[2];

            request.Add(RequestTypes.CurrConfigParam);
            dataType.Add(VarTypes.typeByte);

            request.Add(configurationOption);
            dataType.Add(VarTypes.typeByte);

            requestByteArray = byteConverter(request, dataType, 2);

            RequestQueueMutex.WaitOne();                 
            MessageHandler.RequestQueue.Enqueue(requestByteArray);
            RequestQueueMutex.ReleaseMutex();            
        }

        static public void UpdateConfigOption(byte configurationOption, Int32 configurationParameter)
        {
            List<object> request = new List<object>();
            List<int> dataType = new List<int>();
            byte[] requestByteArray = new byte[2];

            request.Add(RequestTypes.UpdateConfigOption);
            dataType.Add(VarTypes.typeByte);

            request.Add(configurationOption);
            dataType.Add(VarTypes.typeByte);

            request.Add(configurationParameter);
            dataType.Add(VarTypes.typeInt32);

            requestByteArray = byteConverter(request, dataType, 6);

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
                //wait until interaction mode is switched to manual.
                while (BuggyMotorControl.pauseMotorControl) { };

                //record buggy control key presses and releases
                if (BuggyMotorControl.forward) { forwardL = 1; } else { forwardL = 0; }
                if (BuggyMotorControl.reverse) { reverseL = 1; } else { reverseL = 0; }
                if (BuggyMotorControl.right) { rightL = 1; } else { rightL = 0; }
                if (BuggyMotorControl.left) { leftL = 1; } else { leftL = 0; }

                //put the direction data together into one
                direction = (byte)((leftL << 3) | (rightL << 2) | (reverseL << 1) | (forwardL << 0));


                //Combinations of the keys meaning
                //1111 = left, right, reverse, forward
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

                //dont send another request to the buggy keys haven't changed so that queue is not filled with identical requests
                if (BuggyMotorControl.previousState != direction)
                {
                    request.Add(RequestTypes.MoveBuggy);
                    dataType.Add(VarTypes.typeByte);

                    request.Add(direction);
                    dataType.Add(VarTypes.typeByte);

                    requestByteArray = byteConverter(request, dataType, 2);

                    RequestQueueMutex.WaitOne();                 
                    MessageHandler.RequestQueue.Enqueue(requestByteArray);
                    RequestQueueMutex.ReleaseMutex();            

                    //clear the lists
                    request.Clear();
                    dataType.Clear();
                }

                //save the current state as previous state to compare against the key presses next time
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

            RequestQueueMutex.WaitOne();                
            MessageHandler.RequestQueue.Enqueue(requestByteArray);
            RequestQueueMutex.ReleaseMutex();      
        }

        public static void BuggyOrClient()
        {
            List<object> request = new List<object>();
            List<int> dataType = new List<int>();
            byte[] requestByteArray = new byte[2];

            request.Add(RequestTypes.BuggyOrClient);
            dataType.Add(VarTypes.typeByte);

            request.Add(BuggyOrControllerClient.ControllerClient);
            dataType.Add(VarTypes.typeByte);

            requestByteArray = byteConverter(request, dataType, 2);

            RequestQueueMutex.WaitOne();
            MessageHandler.RequestQueue.Enqueue(requestByteArray);
            RequestQueueMutex.ReleaseMutex();
        }

        static public byte[] byteConverter(List<object> request, List<int> dataTypes, int byteCount)
        {
            byte[] byteArray = new byte[byteCount];
            int offset = 0;

            //Convert all data in the request array into bytes. before the data can be converted from the object list, it needs to be cast to the appopriate datatype.
            //This is the reason the dataTypes list exists. It holds the datatypes of the data that has been placed into the request list.
            for (int x = 0; x < request.Count; x++)
            {
                //copy the data from the request array into to byteArray as bytes then increase the offset by the number of bytes that the data consisted of to allow the
                //next piece of data from the request array to be copied into the byteArray without overwriting data that's already in there.
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

    //A class with a list of all possible request types that the controller client can take.
    static class RequestTypes
    {
        public const byte IsAlive              = 0;
        public const byte ListAddClient        = 1;
        public const byte ListRemoveClient     = 2;
        public const byte SendEnvData          = 3;
        public const byte RecEnvData           = 4;
        public const byte MoveBuggy            = 5;
        public const byte BuggyConnect         = 6;
        public const byte BuggyConnectResponse = 7;
        public const byte BuggyDisconnect      = 8;
        public const byte InteractionMode      = 9;
        public const byte CurrConfigParam      = 10;
        public const byte SendCurrConfig       = 11;
        public const byte UpdateConfigOption   = 12;
        public const byte ConfigUpdateStatus   = 13;
        public const byte BuggyOrClient        = 16;
    }

    //a class of variable types used in the byte converter to convert request data into bytes
    static class VarTypes
    {
        public const int typeByte = 1;
        public const int typeInt32 = 2;
    }

    //a class containing definitions of the available interaction modes 
    static class InteractionMode
    {
        public const byte Manual = 1;
        public const byte Configuration = 2;
        public const byte Autonomous = 3;
    }

    //A class containing buggy control related data and functions. 
    static class BuggyMotorControl
    {
        //variables containing data about whether a buggy control key has been pressed or not
        public static bool forward = false;
        public static bool reverse = false;
        public static bool right = false;
        public static bool left = false;

        //holds the previous combination of key presses usued to prevent identical control requests being sent to the buggy
        public static byte previousState = 0;

        //thread used to periodically check and record the buggy control key presses and releases
        static Thread MotorControlInput = new Thread(MessageHandler.MotorControl);

        //variable used to stop control requests being sent to the buggy when not in manual control mode
        public volatile static bool pauseMotorControl = true;

        //Starts the thread that records the buggy control presses and releases
        static public void StartMotorControl()
        {
            if (!MotorControlInput.IsAlive)
            {
                MotorControlInput.Start();
            }
        }
        
        //stops the buggy control key presses/releases from being recorded
        static public void PauseMotorControl()
        {
            pauseMotorControl = true;
        }

        //restarts the recording of buggy control key presses/releases
        static public void RestartMotorControl()
        {
            pauseMotorControl = false;
        }

    }

    //a class containing definitions of the available configuration options
    static class ConfigurationOptions
    {
        public const byte AutonomousDataT     = 1;
        public const byte MaxObjectDistance   = 2;
        public const byte BuggySpeed          = 3;
        public const byte LightIntensityDelta = 4;
    }

    static class BuggyOrControllerClient
    {
        public const byte Buggy = 0;
        public const byte ControllerClient = 1;
    }
}
