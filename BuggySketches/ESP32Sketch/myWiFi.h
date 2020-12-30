#ifndef   WIFI
#define   WIFI
#include <WiFi.h>
#include <WiFiClient.h>
#include <WiFiServer.h>
#endif

extern WiFiClient serverClient;            //create instance of a client that will connect to the server so that the buggy can be seen by controller clients in the system.
extern WiFiClient controllerClient;        //instance that will connect with the controller client that has been granted permission to control the buggy.

//Functions
void SetupWiFi();
void ConnectToServer();
void SetupListener();
void SendWiFi(WiFiClient Client, byte *request, int requestSize);
int ReceiveWiFi(WiFiClient Client);
void ListenForConnections();
void CheckConnections();
void DisconnectClient();
