#include "CommProtocols.h"
#include "Sensors.h"
#include "myWiFi.h"


void setup() {
  SetupSerial(BUAD_115200);     //initialise serial1 for printing on serial monitor and serial2 for sending to and receiving data from Arduino.
  SetupI2C();                   //initialise i2c for bme280
  SetupBME280();
  SetupWiFi();
  ConnectToServer();
  SetupListener();
}

void loop() {
  // put your main code here, to run repeatedly:

}
