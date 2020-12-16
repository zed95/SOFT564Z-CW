#include "myWiFi.h"
#include "RequestHandler.h"
#include <WiFi.h>
#include <WiFiClient.h>
#include <WiFiServer.h>


WiFiServer listener(80);            //Create a listener on port n.
WiFiClient serverClient;            //create instance of a client that will connect to the server so that the buggy can be seen by controller clients in the system.
WiFiClient controllerClient;        //instance that will connect with the controller client that has been granted permission to control the buggy.
const char* serverIP = "192.168.0.155";
const int serverPort = 11000;

void SetupWiFi() {
  //Network ssid and password

  //4 Marlborough
  //  const char* ssid = "VM210ED8";
  //  const char* password = "ysZgas5curvr";

  //46 Wilson
  const char* ssid = "VM5625627";
  const char* password = "9shxfpSCjwmb";

  //const char* ssid = "zHotspot";
  // const char* password = "Cv7k09mp!";

  //Configuration for static IP
  IPAddress myIP(192, 168, 0, 54);
  IPAddress gateway(192, 168, 0, 1);
  IPAddress subnet(255, 255, 255, 0);
  IPAddress dns1(192, 168, 0, 1);
  //IPAddress dns2(192, 168, 0, 1);
  WiFi.config(myIP, gateway, subnet, dns1);

  //Connect to WiFi
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(1000);
    Serial.println("Connecting to WiFi..");
  }
  Serial.println("Connected to the WiFi network");
  Serial.println(WiFi.localIP());   //print my ip

}

void ConnectToServer() {
  // Connect to server
  if (!serverClient.connect(serverIP, serverPort))
  {
    Serial.println("Failed to connect to server.");
  }
  Serial.println("Connected to server");
}

void SetupListener() {
  listener.begin();

  while (1) {
    controllerClient = listener.available();
    if (controllerClient) {
      break;
    }
  }
}


void SendWiFi(WiFiClient Client, byte *request, int requestSize) {
  //send a number of bytes specified by 'requestSize' starting at 'request' address
  Client.write(request, requestSize*sizeof(byte));
}

//need to de
int ReceiveWiFi(WiFiClient Client) {
  byte dataBuffer[1000];
  int byteCount = 0;

  if (Client.connected()) {
    while (Client.available() > 0) {
      //Add code to read in the data from the client
      dataBuffer[byteCount] = Client.read();
      byteCount++;
    }

    if (byteCount > 0) {
      xSemaphoreTake(requestQueueMutex, portMAX_DELAY);
      AddQueue(&dataBuffer[0], byteCount);
      xSemaphoreGive(requestQueueMutex);
    }
  }
}
