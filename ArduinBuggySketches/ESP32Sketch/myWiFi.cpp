#include "myWiFi.h"
#include <WiFi.h>
#include <WiFiClient.h>
#include <WiFiServer.h>


WiFiServer listener(80);            //Create a listener on port n.
WiFiClient serverClient;            //create instance of a client that will connect to the server so that the buggy can be seen by controller clients in the system.
WiFiClient controllerClient;        //instance that will connect with the controller client that has been granted permission to control the buggy.
const char* serverIP = "192.168.0.24";
const int serverPort = 11000;

void SetupWiFi() {
  //Network ssid and password
  const char* ssid = "VM210ED8";
  const char* password = "ysZgas5curvr";

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
 
}

void ConnectToServer() {
    // Connect to server
  Serial.println(WiFi.localIP());   //print my ip
  while(controllerClient.connect(serverIP, serverPort)) 
  {
      Serial.println("Connecting to server..");
  }
  Serial.println("Connected to server");
}

void SetupListener() {
    listener.begin();
}


void SendWiFi(WiFiClient Client, byte *request, int requestSize) {
  
  for(int x = 0; x <= requestSize; x++) {
    Client.write(*(request + x));
  }
}

//need to de
int ReceiveWiFi(WiFiClient Client) {

  while (Client.available() > 0) {
      //Add code to read in the data from the client
  }

}
