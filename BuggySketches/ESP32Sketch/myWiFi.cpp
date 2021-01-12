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

byte wifiByteBuffer[1000];  //buffer that stores bytes received from the network
int wifiOldestByte = 0;     //points to the first byte in the buffer
int wifiNewestByte = 0;     //points to the next free space in the buffer
int wifiBytesInQueue = 0;   //indicate the number of bytes currently in the buffer

void SetupWiFi() {
  //Network ssid and password

  //4 Marlborough
  //  const char* ssid = "VM210ED8";
  //  const char* password = "ysZgas5curvr";

  //  //46 Wilson
  const char* ssid = "VM5625627";
  const char* password = "9shxfpSCjwmb";

  //103 Kendal
  //  const char* ssid = "OriginBroadband16477";
  //  const char* password = "Cv7k09mp!";

  //const char* ssid = "zHotspot";
  // const char* password = "Cv7k09mp!";

  //Configuration for static IP
  IPAddress myIP(192, 168, 0, 54);
  IPAddress gateway(192, 168, 0, 1);
  IPAddress subnet(255, 255, 255, 0);
  IPAddress dns1(192, 168, 0, 1);
  //IPAddress dns2(192, 168, 0, 1);
  WiFi.config(myIP, gateway, subnet, dns1);

  //Connect to WiFi network
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(1000);
    Serial.println("Connecting to WiFi..");
  }
  Serial.println("Connected to the WiFi network");
  Serial.println(WiFi.localIP());   //print my ip

}

void ConnectToServer() {
  byte indentificationRequest[2] = {16, 0};  //Generated request for the server to know what type of device the connected esp32 is.
  // Connect to server and getconnection status
  if (!serverClient.connect(serverIP, serverPort))
  {
    Serial.println("Failed to connect to server.");
  }
  else {
    Serial.println("Connected to server");
    //Send request to server to allow the server to identify the buggy as a buggy.
    SendWiFi(serverClient, &indentificationRequest[0], 2);
  }
}

void CheckConnections() {
  byte intModeRequest[2] = {9, 1};
  //clear the server client buffer so that correct representation of the connection status is given.
  serverClient.flush();
  //if connection with server is lost
  if (!serverClient.connected()) {

    //request for the interaction mode to go into manual
    if (interactionMode != INTMODE_MANUAL) {
      xSemaphoreTake(requestQueueMutex, portMAX_DELAY);
      AddQueue(&requestQueue[0], rhNewestByte, rhBytesInQueue, &intModeRequest[0], 2);
      xSemaphoreGive(requestQueueMutex);
    }

    //disconnect client controller if there is one connected to the buggy
    if (controllerClient.connected()) {
      //Disconnect controller client
      DisconnectClient();
    }
    //Try reconnecting to server
    ConnectToServer();

  }
  else if ((!controllerClient.connected()) && (serverClient.connected())) {
    //request for the interaction mode to go into manual
    if (interactionMode != INTMODE_MANUAL) {
      xSemaphoreTake(requestQueueMutex, portMAX_DELAY);
      AddQueue(&requestQueue[0], rhNewestByte, rhBytesInQueue, &intModeRequest[0], 2);
      xSemaphoreGive(requestQueueMutex);
    }
    ListenForConnections();     //listen for controller client connection attempts.
  }
}

//Disconnects the connect controller client
void DisconnectClient() {
  controllerClient.stop();
}

//Begins listening for incoming controller client connections
void SetupListener() {
  listener.begin();
}

//Listen for controller client connection attempts
void ListenForConnections() {
  controllerClient = listener.available();
  if (controllerClient) {
    Serial.println("Controller Client Connected!");
  }
}


void SendWiFi(WiFiClient Client, byte *request, int requestSize) {
  //send a number of bytes specified by 'requestSize' starting at 'request' address
  Client.write(request, requestSize * sizeof(byte));
}


int ReceiveWiFi(WiFiClient Client) {
  byte dataBuffer[1000];
  int byteCount = 0;

  if (Client.connected()) {                                       //Check whether client is connected
    while (Client.available() > 0) {                              //read the data if any is available
      dataBuffer[byteCount] = Client.read();                      //Add code to read in the data from the client
      byteCount++;                                                //record how many bytes were read
      Serial.println("Data Received");
    }

    if (byteCount > 0) {
      AddQueue(&wifiByteBuffer[0], wifiNewestByte, wifiBytesInQueue, &dataBuffer[0], byteCount);    //store the newly read bytes into a queue for processing
    }

    if (wifiBytesInQueue > 0) {                                                                     //if there are any bytes in the wifi queue waiting to be processed
      Serial.println("Adding Request to queue");
      switch (peekQueue(&wifiByteBuffer[0], wifiOldestByte)) {                                      //then peek the first byte to check what kind of request it is
        case REQ_ENV_DATA:
          if (wifiBytesInQueue >= 1) {                                                              //only place request data into request handler queue if there are at least as many bytes in the wifi queue as the specific request requires
            xSemaphoreTake(requestQueueMutex, portMAX_DELAY);                                       //take mutex before adding the to request handler queue to allow task/thread safe operations on the queue
            RemoveQueue(&wifiByteBuffer[0], wifiOldestByte, wifiBytesInQueue, &dataBuffer[0], 1);   //Remove request data from wifi queue
            AddQueue(&requestQueue[0], rhNewestByte, rhBytesInQueue, &dataBuffer[0], 1);            //and place in request handler queue
            xSemaphoreGive(requestQueueMutex);                                                      //release the mutex when operation on the request handler queue is finished
          }
          break;
        case MOVE_BUGGY:
          if (wifiBytesInQueue >= 2) {
            xSemaphoreTake(requestQueueMutex, portMAX_DELAY);
            RemoveQueue(&wifiByteBuffer[0], wifiOldestByte, wifiBytesInQueue, &dataBuffer[0], 2);
            AddQueue(&requestQueue[0], rhNewestByte, rhBytesInQueue, &dataBuffer[0], 2);
            xSemaphoreGive(requestQueueMutex);
          }
          break;
        case INTERACTION_MODE:
          if (wifiBytesInQueue >= 2) {
            xSemaphoreTake(requestQueueMutex, portMAX_DELAY);
            RemoveQueue(&wifiByteBuffer[0], wifiOldestByte, wifiBytesInQueue, &dataBuffer[0], 2);
            AddQueue(&requestQueue[0], rhNewestByte, rhBytesInQueue, &dataBuffer[0], 2);
            xSemaphoreGive(requestQueueMutex);
          }
          break;
        case CURR_CONFIG_PARAM:
          if (wifiBytesInQueue >= 2) {
            xSemaphoreTake(requestQueueMutex, portMAX_DELAY);
            RemoveQueue(&wifiByteBuffer[0], wifiOldestByte, wifiBytesInQueue, &dataBuffer[0], 2);
            AddQueue(&requestQueue[0], rhNewestByte, rhBytesInQueue, &dataBuffer[0], 2);
            xSemaphoreGive(requestQueueMutex);
          }
          break;
        case UPDATE_CONFIG_OPTION:
          if (wifiBytesInQueue >= 6) {
            xSemaphoreTake(requestQueueMutex, portMAX_DELAY);
            RemoveQueue(&wifiByteBuffer[0], wifiOldestByte, wifiBytesInQueue, &dataBuffer[0], 6);
            AddQueue(&requestQueue[0], rhNewestByte, rhBytesInQueue, &dataBuffer[0], 6);
            xSemaphoreGive(requestQueueMutex);
          }
          break;
        default:
          //Add Code to handle unrecognised request
          break;
      }
    }
  }
}
