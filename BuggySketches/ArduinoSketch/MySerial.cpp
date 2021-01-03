#include "MySerial.h"
#include "RequestHandler.h"

byte serialByteBuffer[1000];
int serialOldestByte = 0;
int serialNewestByte = 0;
int serialBytesInQueue = 0;

void SetupSerial(int baudRate) {
  Serial.begin(115200);
  Serial1.begin(115200, SERIAL_8N1);
}

void ReceiveSerial() {
  int byteCount = 0;
  byte dataBuffer[1000];

  //read bytes while they are available
  while (Serial1.available() > 0) {
    dataBuffer[byteCount] = Serial1.read();   //read the bytes into the data buffer
    byteCount++;                              //increment byte count to keep track of the number of bytes read
  }

  //add data to the serial queue if there is anything to add
  if (byteCount > 0) {
    AddQueue(&serialByteBuffer[0], serialNewestByte, serialBytesInQueue, &dataBuffer[0], byteCount);
  }

  //continue processing the bytes until there are none left to process
  while (serialBytesInQueue > 0) {
    switch (peekQueue(&serialByteBuffer[0], serialOldestByte)) {    //peek to see what kind of request we are dealing with
      case MOVE_BUGGY:
        if (serialBytesInQueue >= 2) {  //check if there are enough bytes in the serial buffer to add this request to the request handler queue for processing. If there are enough
          RemoveQueue(&serialByteBuffer[0], serialOldestByte, serialBytesInQueue, &dataBuffer[0], 2);   //Remove from the data from serial buffer
          AddQueue(&requestQueue[0], rhNewestByte, rhBytesInQueue, &dataBuffer[0], 2);          //and place in request buffer
        }
        else {  //otherwise
          goto breakout;  //jump to breakout in order to wait for more data to be read.
        }
        break;
      case INTERACTION_MODE:
        if (serialBytesInQueue >= 2) {
          RemoveQueue(&serialByteBuffer[0], serialOldestByte, serialBytesInQueue, &dataBuffer[0], 2);  
          AddQueue(&requestQueue[0], rhNewestByte, rhBytesInQueue, &dataBuffer[0], 2);          
        }
        else {
          goto breakout;
        }
        break;
      case CURR_CONFIG_PARAM:
        if (serialBytesInQueue >= 2) {
          RemoveQueue(&serialByteBuffer[0], serialOldestByte, serialBytesInQueue, &dataBuffer[0], 2);  
          AddQueue(&requestQueue[0], rhNewestByte, rhBytesInQueue, &dataBuffer[0], 2);          
        }
        else {
          goto breakout;
        }
        break;
      case UPDATE_CONFIG_OPTION:
        if (serialBytesInQueue >= 6) {
          RemoveQueue(&serialByteBuffer[0], serialOldestByte, serialBytesInQueue, &dataBuffer[0], 6);   
          AddQueue(&requestQueue[0], rhNewestByte, rhBytesInQueue, &dataBuffer[0], 6);          
        }
        else {
          goto breakout;
        }
      default:
          goto breakout;
        break;
    }
  }

  //code jumps to this label when there are bytes in buffer but there aren't enough of them for the request type. This prevents the program being stuck in the loop and allows it to wait and read more data.
  breakout:
  //leave function and wait for more data to arrive
  return;
}

//sends the data back to esp32 via uart
void SendSerial(byte *byteArray, int byteCount) {
  for (int x = 0; x < byteCount; x++) {
    Serial1.write(*(byteArray + x));
  }
}

//check if there are any bytes available to be read
int CheckSerial() {
  return Serial1.available();
}
