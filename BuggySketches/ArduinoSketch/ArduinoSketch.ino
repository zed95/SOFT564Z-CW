#include "RequestHandler.h"
#include "Motor.h"

byte serialByteBuffer[1000];
int serialOldestByte = 0;
int serialNewestByte = 0;
int serialBytesInQueue = 0;

void setup() {
  // put your setup code here, to run once:
  Serial1.begin(115200);
  Serial.begin(115200);
  setupMotor();

}

void loop() {
  // put your main code here, to run repeatedly:
  if(Serial1.available()) {
    getRequest();
    HandleRequest();
  }

}

void getRequest() {
  int byteCount = 0;
  byte dataBuffer[1000];

  while (Serial1.available() > 0) {
    dataBuffer[byteCount] = Serial1.read();
    byteCount++;
  }

  if (byteCount > 0) {
    AddQueue(&serialByteBuffer[0], serialNewestByte, serialBytesInQueue, &dataBuffer[0], byteCount);
  }

  if (serialBytesInQueue > 0) {
    switch (peekQueue(&serialByteBuffer[0], serialOldestByte)) {
      case MOVE_BUGGY:
        if (serialBytesInQueue >= 2) {
          RemoveQueue(&serialByteBuffer[0], serialOldestByte, serialBytesInQueue, &dataBuffer[0], 2);   //Remove from wifi buffer
          AddQueue(&requestQueue[0], rhNewestByte, rhBytesInQueue, &dataBuffer[0], 2);          //and place in request buffer
        }
        break;
      case CURR_CONFIG_PARAM:
        if (serialBytesInQueue >= 2) {
          RemoveQueue(&serialByteBuffer[0], serialOldestByte, serialBytesInQueue, &dataBuffer[0], 2);   //Remove from wifi buffer
          AddQueue(&requestQueue[0], rhNewestByte, rhBytesInQueue, &dataBuffer[0], 2);          //and place in request buffer
        }
        break;
      case UPDATE_CONFIG_OPTION:
        if (serialBytesInQueue >= 6) {
          RemoveQueue(&serialByteBuffer[0], serialOldestByte, serialBytesInQueue, &dataBuffer[0], 6);   //Remove from wifi buffer
          AddQueue(&requestQueue[0], rhNewestByte, rhBytesInQueue, &dataBuffer[0], 6);          //and place in request buffer
        }
      default:

        break;
    }
  }






  
}
