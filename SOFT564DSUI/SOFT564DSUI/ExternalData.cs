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

            Clients.Exists(x => x.clientID == 1000);
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


    static class EnvData
    {
        public static int temperature = 0;
        public static float ftemperature = 0;
        public static int humidity = 0;
        public static int lIntensity = 0;

        //Checks whether the temperature is negative or positive and converts to the correct magnitude.
        public static void TempToFloat()
        {
            if((temperature & 0x8000) == 0x8000)
            {
                temperature = (temperature & 0x3fff) * (-1);
            }

            ftemperature = temperature * 0.01f;

        }
    }

    static class BuggyConnectResponse
    {
        public static byte response = 0;
        public const byte ConnectPermitted = 1;
        public const byte BuggyInUse = 2;
    }
}
