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
        public static long clientIPAddress;
        public static int clientPort;
        public static int clientID;

        public static List<ConnectedClients> Clients = new List<ConnectedClients>();

        public static void AddClient()
        {
            Clients.Add(new ConnectedClients(clientIPAddress, clientPort, clientID));
            foreach(ConnectedClients client in Clients)
            {
                Console.WriteLine("Clients Connected" + client.clientID);
                Console.WriteLine("Clients port" + client.clientPort);
            }
        }

        public static void RemoveClient()
        {
            Clients.RemoveAt(Clients.FindIndex(x => x.clientID == clientID));
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
