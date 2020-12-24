#include "myWiFi.h"
#include "RequestHandler.h"
#include <WiFi.h>
#include <WiFiClient.h>
#include <WiFiServer.h>


WiFiServer listener(80);            //Create a listener on port n.
WiFiClient serverClient;            //create instance of a client that will connect to the server so that the buggy can be seen by controller clients in the system.
WiFiClient controllerClient;        //instance that will connect with the controller client that has been granted permission to control the buggy.
const char* serverIP = "192.168.0.95";
const int serverPort = 11000;

byte wifiByteBuffer[1000];
int wifiOldestByte = 0;
int wifiNewestByte = 0;
int wifiBytesInQueue = 0;

void SetupWiFi() {
  //Network ssid and password

  //4 Marlborough
  //  const char* ssid = "VM210ED8";
  //  const char* password = "ysZgas5curvr";

  //  //46 Wilson
  //  const char* ssid = "VM5625627";
  //  const char* password = "9shxfpSCjwmb";

  //103 Kendal
  const char* ssid = "OriginBroadband16477";
  const char* password = "Cv7k09mp!";

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
      Serial.println("Controller Client Connected!");
      break;
    }
  }
}


void SendWiFi(WiFiClient Client, byte *request, int requestSize) {
  //send a number of bytes specified by 'requestSize' starting at 'request' address
  Client.write(request, requestSize * sizeof(byte));
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
      Serial.println("Data Received");
    }

    if (byteCount > 0) {
      AddQueue(wifiByteBuffer, wifiNewestByte, wifiBytesInQueue, &dataBuffer[0], byteCount);
    }

    if (wifiBytesInQueue > 0) {
      Serial.println("Adding Request to queue");
      switch (peekQueue(&wifiByteBuffer[0], wifiOldestByte)) {
        case REQ_ENV_DATA:
          xSemaphoreTake(requestQueueMutex, portMAX_DELAY);
          RemoveQueue(&wifiByteBuffer[0], wifiOldestByte, wifiBytesInQueue, &dataBuffer[0], 1);   //Remove from wifi buffer
          AddQueue(&requestQueue[0], rhNewestByte, rhBytesInQueue, &dataBuffer[0], 1);          //and place in request buffer
          xSemaphoreGive(requestQueueMutex);
          break;
        case MOVE_BUGGY:
          if (wifiBytesInQueue >= 2) {
            xSemaphoreTake(requestQueueMutex, portMAX_DELAY);
            RemoveQueue(&wifiByteBuffer[0], wifiOldestByte, wifiBytesInQueue, &dataBuffer[0], 2);   //Remove from wifi buffer
            AddQueue(&requestQueue[0], rhNewestByte, rhBytesInQueue, &dataBuffer[0], 2);          //and place in request buffer
            xSemaphoreGive(requestQueueMutex);
          }
          break;
        case INTERACTION_MODE:
          if (wifiBytesInQueue >= 2) {
            xSemaphoreTake(requestQueueMutex, portMAX_DELAY);
            RemoveQueue(&wifiByteBuffer[0], wifiOldestByte, wifiBytesInQueue, &dataBuffer[0], 2);   //Remove from wifi buffer
            AddQueue(&requestQueue[0], rhNewestByte, rhBytesInQueue, &dataBuffer[0], 2);          //and place in request buffer
            xSemaphoreGive(requestQueueMutex);
          }
          break;
        case CURR_CONFIG_PARAM:
          if (wifiBytesInQueue >= 2) {
            xSemaphoreTake(requestQueueMutex, portMAX_DELAY);
            RemoveQueue(&wifiByteBuffer[0], wifiOldestByte, wifiBytesInQueue, &dataBuffer[0], 2);   //Remove from wifi buffer
            AddQueue(&requestQueue[0], rhNewestByte, rhBytesInQueue, &dataBuffer[0], 2);          //and place in request buffer
            xSemaphoreGive(requestQueueMutex);
          }
          break;
        case UPDATE_CONFIG_OPTION:
          if (wifiBytesInQueue >= 6) {
            xSemaphoreTake(requestQueueMutex, portMAX_DELAY);
            RemoveQueue(&wifiByteBuffer[0], wifiOldestByte, wifiBytesInQueue, &dataBuffer[0], 6);   //Remove from wifi buffer
            AddQueue(&requestQueue[0], rhNewestByte, rhBytesInQueue, &dataBuffer[0], 6);          //and place in request buffer
            xSemaphoreGive(requestQueueMutex);
          }
          break;
        default:

          break;
      }
    }
  }
}
