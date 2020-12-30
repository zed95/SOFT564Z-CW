#include "CommProtocols.h"
#include "Sensors.h"
#include "myWiFi.h"
#include "RequestHandler.h"

//Sets up the ESP32
void setup() {
  SetupRequestHandler();        //sets up the task that deals with handling the requests from the buggy or controller client
  SetupAutoDataSend();          //sets up the task that automatically extracts exnvironmental data in autonomous mode
  SetupSerial(BAUD_115200);     //initialise serial1 for printing on serial monitor and serial2 for sending to and receiving data from Arduino.
  SetupI2C();                   //initialise i2c for bme280
  SetupBME280();                //initialise BME280 sensor
  SetupWiFi();                  //sets up wifi and connects to the specified network
  ConnectToServer();            //connects to the server
  SetupListener();              //listens for client controller connection requests
}


void loop() {
  // put your main code here, to run repeatedly:
  CheckConnections();             //checks the status of connections to the server and controller client
  ReceiveWiFi(controllerClient);  //Checks if any data was sent from the network and deals with the data is any is available
  ReceiveSerial();                //checks if any data was sent from the Arduino and deals with the data if any is available
  vTaskDelay(pdMS_TO_TICKS(50));  //Gives up processor time for 50ms
}
