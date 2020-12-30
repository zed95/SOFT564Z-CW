#include "CommProtocols.h"
#include "RequestHandler.h"
#include <Wire.h>

byte serialByteBuffer[1000];  //buffer that stores bytes received from the Arduino
int serialOldestByte = 0;     //points to the first byte in the buffer
int serialNewestByte = 0;     //points to the next free space in the buffer
int serialBytesInQueue = 0;   //indicate the number of bytes currently in the buffer


void SetupSerial(int baudRate) {
  Serial.begin(baudRate);   //serial used for debugging purposes
  Serial2.begin(baudRate, SERIAL_8N1, ESP32_UART2_RX, ESP32_UART2_TX);      //ESP32 UART connected to the Arduino Mega to exchange data
}


//Sends data to the Arduino
void SendSerial(byte *byteArray, int byteCount, int serial) {
  //Send data either to Arduino or debug
  if (serial == SERIAL1) {
    for (int x = 0; x < byteCount; x++) {
      Serial.write(*(byteArray + x));
    }
  }
  else {
    //write the bytes to serial for transmission
    for (int x = 0; x < byteCount; x++) {
      Serial2.write(*(byteArray + x));
    }
  }
}

//Receives data from the Arduino and places them into the request buffer if all request data has been sent otherwise stores in the serialByteBuffer until enough data is available.
void ReceiveSerial() {
  byte dataBuffer[1000];
  int byteCount = 0;

  //while there are any bytes available for reading
  while (Serial2.available() > 0) {
    //read the data from arduino
    dataBuffer[byteCount] = Serial2.read();
    //count the number of bytes read
    byteCount++;
  }

  //if any data has been read place it into the serialByteBuffer
  if (byteCount > 0) {
    AddQueue(&serialByteBuffer[0], serialNewestByte, serialBytesInQueue, &dataBuffer[0], byteCount);
  }

  //if there are any bytes in the serial buffer queue then try to place the requests into the request handler queue if there are enough data in the buffer for the request.
  if (serialBytesInQueue > 0) {
    switch (peekQueue(&serialByteBuffer[0], serialOldestByte)) {  //look at the first byte in the queue to determine the type of request we are dealing with
      case SEND_CURR_CONFIG:
        if (serialBytesInQueue >= 5) {                                                                  //if there are enough bytes in the serial buffer queue for the request then continue
          xSemaphoreTake(requestQueueMutex, portMAX_DELAY);                                             //take the mutex to safely add the request data to the request handler queue
          RemoveQueue(&serialByteBuffer[0], serialOldestByte, serialBytesInQueue, &dataBuffer[0], 5);   //Remove request from serial buffer queue
          AddQueue(&requestQueue[0], rhNewestByte, rhBytesInQueue, &dataBuffer[0], 5);                  //and place request in request queue
          xSemaphoreGive(requestQueueMutex);                                                            //release the mutex 
        }
        break;
      case CONFIG_UPDATE_STATUS:
        if (serialBytesInQueue >= 2) {
          xSemaphoreTake(requestQueueMutex, portMAX_DELAY);
          RemoveQueue(&serialByteBuffer[0], serialOldestByte, serialBytesInQueue, &dataBuffer[0], 2);   //Remove from wifi buffer
          AddQueue(&requestQueue[0], rhNewestByte, rhBytesInQueue, &dataBuffer[0], 2);          //and place in request buffer
          xSemaphoreGive(requestQueueMutex);
        }
        break;
      default:
        //unrecognised request: error in sending the bytes
        break;
    }
  }

}

//sets up the i2c as master
void SetupI2C() {
  Wire.begin(); //setup ESP32 as i2c master.
}
