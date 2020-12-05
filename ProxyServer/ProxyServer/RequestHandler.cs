using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProxyServer
{
    class RequestHandler
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
                    default:

                        break;
                }

            }
        }

    }

    //A class with a list of all possible request types.
    static class RequestTypes
    {
        public const int ListAddClient = 1;
        public const int ListRemoveClient = 2;
    }

}
