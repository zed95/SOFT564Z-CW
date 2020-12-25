#include "CommProtocols.h"
#include "RequestHandler.h"
#include <Wire.h>

byte serialByteBuffer[1000];
int serialOldestByte = 0;
int serialNewestByte = 0;
int serialBytesInQueue = 0;


void SetupSerial(int baudRate) {
  Serial.begin(baudRate);
  Serial2.begin(baudRate, SERIAL_8N1, ESP32_UART2_RX, ESP32_UART2_TX);      //ESP32 UART connected to the Arduino Mega to exchange data
}


//I need to decide which format the message is going to be sent in.
void SendSerial(byte *byteArray, int byteCount, int serial) {
  Serial.println("Sending Via Serial");
  if (serial == SERIAL1) {
    for (int x = 0; x < byteCount; x++) {
      Serial.write(*(byteArray + x));
    }
  }
  else {
    for (int x = 0; x < byteCount; x++) {
      Serial2.write(*(byteArray + x));
    }
  }
}

void ReceiveSerial() {
  byte dataBuffer[1000];
  int byteCount = 0;

  while (Serial2.available() > 0) {
    //Add code to read in the data from the client
    dataBuffer[byteCount] = Serial2.read();
    Serial.println(dataBuffer[byteCount]);
    byteCount++;
  }

  if (byteCount > 0) {
    Serial.println("Send Curr Config 0.000001");
    AddQueue(&serialByteBuffer[0], serialNewestByte, serialBytesInQueue, &dataBuffer[0], byteCount);
  }
  Serial.println("Send Curr Config 0.000002");
  if (serialBytesInQueue > 0) {
    Serial.println("Send Curr Config 0.000003");
    Serial.print("Data thats in the first queue : ");
    Serial.println(peekQueue(&serialByteBuffer[0], serialOldestByte));
    switch (peekQueue(&serialByteBuffer[0], serialOldestByte)) {
      case SEND_CURR_CONFIG:
        Serial.println("Send Curr Config 0.01");
        if (serialBytesInQueue >= 5) {
          Serial.println("Send Curr Config 0.02");
          xSemaphoreTake(requestQueueMutex, portMAX_DELAY);
          Serial.println("Send Curr Config 0.03");
          RemoveQueue(&serialByteBuffer[0], serialOldestByte, serialBytesInQueue, &dataBuffer[0], 5);   //Remove from wifi buffer
          AddQueue(&requestQueue[0], rhNewestByte, rhBytesInQueue, &dataBuffer[0], 5);          //and place in request buffer
          xSemaphoreGive(requestQueueMutex);
          Serial.println("Send Curr Config 0");
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

        break;
    }
  }

}


void SetupI2C() {
  Wire.begin(); //setup ESP32 as i2c master.
}
