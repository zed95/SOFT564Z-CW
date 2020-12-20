#include "CommProtocols.h"
#include <Wire.h>


void SetupSerial(int baudRate) {
    Serial.begin(baudRate);
    Serial2.begin(baudRate, SERIAL_8N1, ESP32_UART2_RX, ESP32_UART2_TX);      //ESP32 UART connected to the Arduino Mega to exchange data
}


//I need to decide which format the message is going to be sent in.
void SendSerial(byte *byteArray, int byteCount, int serial) {

    if(serial == SERIAL1) {
      for(int x = 0; x < byteCount; x++) {
        Serial.write(*(byteArray + x));
      }
    }
    else {
      for(int x = 0; x < byteCount; x++) {
        Serial2.write(*(byteArray + x));
      }
    }
}

void SetupI2C() {
  Wire.begin(); //setup ESP32 as i2c master.
}

void serialEvent() {
  
}
