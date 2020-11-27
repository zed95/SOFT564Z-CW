using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace SOFT564DSUI
{
    //This class handles the management of the external data arrving to the client.
    //accepts commands from the server instructing the update of connected client list.
    static class ExternalData   //Static class cannot have objects created. Only methods of that class can be used.
    {
        
    }


    static class clientManager
    {
        private static long clientIPAddress;
        private static int clientPort;
        public static int clientID;

        public static List<ConnectedClients> Clients = new List<ConnectedClients>();

        public static void AddClient(Byte[] data)
        {
            typeConverter(data);
            Clients.Add(new ConnectedClients(clientIPAddress, clientPort, clientID));

        }

        public static void RemoveClient(Byte[] data)
        {
            typeConverter(data);
            Clients.RemoveAt(Clients.FindIndex(x => x.clientID == clientID));

        }

        static private void typeConverter(Byte[] data)
        {
            Console.WriteLine(data[0]);
            switch(data[0])
            {
                case 0:
                    break;
                case 1:
                    clientIPAddress = BitConverter.ToInt64(data, 2);
                    clientPort = BitConverter.ToInt32(data, 10);
                    clientID = BitConverter.ToInt32(data, 14);
                    break;
                case 2:
                    clientID = BitConverter.ToInt32(data, 2);
                    Console.WriteLine(clientID);
                    break;
            }

        }
    }

    class ConnectedClients
    {
        public long ipAddress;
        public int clientPort;
        public int clientID;
        public byte[] buffer;

        public ConnectedClients(long IPAddress, int port, int id)
        {
            ipAddress = IPAddress;
            clientPort = port;
            clientID = id;
        }
    }
}
