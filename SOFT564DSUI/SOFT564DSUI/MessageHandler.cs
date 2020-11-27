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
            switch(Message[0])
            {
                case RequestTypes.ListAddClient:
                    clientManager.AddClient(Message);
                    newClient = true;
                    break;
                case RequestTypes.ListRemoveClient:
                    clientManager.RemoveClient(Message);
                    removeClient = true;

                    break;
                default:

                    break;
            }
        }

    }

    static class RequestTypes
    {
        public const int ListAddClient = 1;
        public const int ListRemoveClient = 2;
    }
}
