using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace SOFT564DSUI
{

    //Used to decode the message/command/request and decide what to do with it.
    //At the moment it is experimental. When I come up with a proper message format I will update this class.
    static class MessageHandler
    {
        static public bool newClient = false;
        static public bool removeClient = false;
        static public void HandleRequest(Byte[] Message)
        {
            byte[] Request;
            int bytesProcessed = 0;

            int count = Buffer.ByteLength(Message);         //Count the number of bytes in the messages buffer.
            for(int x = 0; x < Message.Length; x++)         
            {
                Console.WriteLine(Message[x]);
            }
            Console.WriteLine("Number of bytes in buffer: " + count);

            while (bytesProcessed != count)     //Loop until all bytes passed to the handler have been processed.
            {

                Request = getRequest(Message);
                switch (Request[0])     //After extracting the request from the messages buffer, shift the contents of the buffer that many bytes so that next request can be extracted. 
                {
                    case RequestTypes.ListAddClient:
                        Buffer.BlockCopy(Message, 18, Message, 0, Message.Length - 18);
                        bytesProcessed += 18;
                        break;
                    case RequestTypes.ListRemoveClient:
                        Buffer.BlockCopy(Message, 6, Message, 0, Message.Length - 6);
                        bytesProcessed += 6;
                        break;
                    default:
                        Console.WriteLine("Unknown Request.");
                        break;
                }
                //Add check whether the request has all the required data.

                //Turn the data back into appropriate format based on type of request.
                switch (Request[0])
                {
                    case RequestTypes.ListAddClient:
                        clientManager.clientIPAddress = BitConverter.ToInt64(Request, 2);
                        clientManager.clientPort = BitConverter.ToInt32(Request, 10);
                        clientManager.clientID = BitConverter.ToInt32(Request, 14);
                        Console.WriteLine("Here");
                        Console.WriteLine(clientManager.clientIPAddress);
                        Console.WriteLine(clientManager.clientPort);
                        Console.WriteLine(clientManager.clientID);

                        clientManager.AddClient();
                        newClient = true;
                        break;
                    case RequestTypes.ListRemoveClient:
                        clientManager.clientID = BitConverter.ToInt32(Request, 2);

                        clientManager.RemoveClient();
                        removeClient = true;

                        break;
                    default:

                        break;
                }

                //clear the request holder after request has been serviced so its ready for the next request to be placed.
                Array.Clear(Request, 0, Request.Length);
            }
        }

        //extracts a single request from the Messages buffer based on the type of request it is.
        static private Byte[] getRequest(Byte[] Message)
        {
            Byte[] RequestData = null;

            //The type of request is always stored in the first byte.
            switch (Message[0])
            {
                case RequestTypes.ListAddClient:
                    RequestData = ArrCopy(Message, 18);
                    break;
                case RequestTypes.ListRemoveClient:
                    RequestData = ArrCopy(Message, 6);
                    break;
                default:
                    Console.WriteLine("Unknown Request.");
                    break;
            }


            return RequestData;
        }

        //Copies a specified number of bytes from one byte array to another.
        static private Byte[] ArrCopy(Byte[] Arr, int BytesToCopy)
        {
            Byte[] data = new byte[BytesToCopy];

            Buffer.BlockCopy(Arr, 0, data, 0, BytesToCopy);

            return data;
        }

    }

    //A class with a list of all possible request types.
    static class RequestTypes
    {
        public const int ListAddClient = 1;
        public const int ListRemoveClient = 2;
    }
}
