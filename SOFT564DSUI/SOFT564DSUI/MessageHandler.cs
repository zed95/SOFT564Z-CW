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
        static public void HandleRequest(Byte[] Message)
        {
            switch(Message[0])
            {
                case RequestTypes.UpdateClientList:
                    clientManager.AddClient(Message);
                    newClient = true;
                    break;
                default:

                    break;
            }
        }

    }

    static class RequestTypes
    {
        public const int UpdateClientList = 1;
    }
}
