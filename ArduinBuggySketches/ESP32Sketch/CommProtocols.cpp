#include "CommProtocols.h"
#include <Wire.h>


void SetupSerial(int baudRate) {
    Serial.begin(baudRate);
    Serial2.begin(baudRate, SERIAL_8N1, ESP32_UART2_RX, ESP32_UART2_TX);      //ESP32 UART connected to the Arduino Mega to exchange data
}


//I need to decide which format the message is going to be sent in.
void SendSerial(int message, int serial) {

    if(serial == 0) {
      //send via serial 0
    }
    else {
      //send via serial 1
    }
  
}

void SetupI2C() {
  Wire.begin(); //setup ESP32 as i2c master.
}

void serialEvent() {
  
}
