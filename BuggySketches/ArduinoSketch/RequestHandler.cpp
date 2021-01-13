#include "RequestHandler.h"
#include "Motor.h"
#include "MySerial.h"
#include "AutonomousMode.h"

byte requestQueue[queueSize];
byte handlerRequestArray[queueSize];
int rhOldestByte = 0;
int rhNewestByte = 0;
int rhBytesInQueue = 0; //used to check whether queue is full or empty.
int pendingRequestData = 0;
byte interactionMode = 0;

void HandleRequest() {
  byte requestArray[queueSize];

  //carry out requests until there are no more bytes in the queue
  while (rhBytesInQueue > 0) {
    switch (peekQueue(&requestQueue[0], rhOldestByte)) {  //peek the first byte in the queue to see what type of request we have.
      case MOVE_BUGGY:
        RemoveQueue(&requestQueue[0], rhOldestByte, rhBytesInQueue, &requestArray[0], 2); //remove request bytes from the queue
        MoveBuggy(*(requestArray + 1)); //carry out the request
        break;
      case INTERACTION_MODE:
        RemoveQueue(&requestQueue[0], rhOldestByte, rhBytesInQueue, &requestArray[0], 2);
        InteractionMode(&requestArray[0]);
        break;
      case CURR_CONFIG_PARAM:
        RemoveQueue(&requestQueue[0], rhOldestByte, rhBytesInQueue, &requestArray[0], 2);
        CurrConfigParam(&requestArray[0]);
        break;
      case UPDATE_CONFIG_OPTION:
        RemoveQueue(&requestQueue[0], rhOldestByte, rhBytesInQueue, &requestArray[0], 6);
        UpdateConfigOption(&requestArray[0]);
        break;
      default:
        break;
    }
  }
}

//Adds n bytes to the selected queue from the source array.
void AddQueue(byte *queue, int &newestByte, int &bytesInQueue, byte *srcArray, int nBytes) {

  //iterate through n bytes in the source array
  for (int x = 0; x < nBytes; x++) {
    if (bytesInQueue < queueSize) {                 //continue if the queue is not full and is able to accept more bytes
      *(queue + newestByte) = *(srcArray + x);      //place the byte from the source array to the queue at index newestByte which points to the next free space in the queue
      bytesInQueue++;                               //byte has been added to the queue and therefore increment the bytesInQueue variable
      if (newestByte == (queueSize - 1)) {          //if data has been added to the queue at the boundary of the queue
        newestByte = 0;                             //the next free space is at position 0 of the queue
      }
      else {
        newestByte++;                               //the byte has not beed added at the boundary of the queue and therefore increment to the next free space in the queue
      }

    }
    else {
      //Array is full decide what to do.
    }
  }
}

//Removes n bytes from selected queue and returns the extracted bytes
void RemoveQueue(byte *queue, int &oldestByte, int &bytesInQueue, byte *dstArray, int nBytes) {

  //iterate through the first n bytes
  for (int i = 0; i < nBytes; i++) {
    if (bytesInQueue > 0) {
      *(dstArray + i) = *(queue + oldestByte);      //places the first byte in the queue into the destination array at index i
      bytesInQueue--;                               //number of bytes in queue decreases by 1 as one byte has been "removed"
      if (oldestByte == (queueSize - 1)) {          //if first byte in the queue has been removed from the boundary of the queue,
        oldestByte = 0;                             //first byte in the queue is now at index 0
      }
      else {
        oldestByte++;                               //the first byte in the queue was not removed at boundary therefore increment the indicator to the next byte in the queue.
      }
    }
    else {
      //Queue is empty decide what to do. Probably add some error codes.
      //-queue is empty
      //-tried to remove more bytes than there were in the queue
    }
  }
}

//Returns the first byte in the queue without actually removing the byte.
byte peekQueue(byte *queue, int oldestByte) {
  byte firstQueueValue = 0;

  firstQueueValue = *(queue + oldestByte);

  return firstQueueValue;
}

//calls appropriate move buggy function based on the received command
void MoveBuggy(byte dir) {

  //0000      no motion
  //0001      forward
  //0010      reverse
  //0011      reverse & forward = no motion
  //0100      right = rotate clockwise
  //0101      forward and right = turn right
  //0110      reverse and right = reverse right
  //0111      forward, reverse and right = rotate clockwise
  //1000      left = rotate anti-clockwise
  //1001      forward and left = turn left;
  //1010      reverse and left = reverse left;
  //1011      forward, reverse and left = rotate anti-clockwise
  //1100      left and right = no motion
  //1101      left right and forward = forward
  //1110      left, right, reverse = reverse
  //1111      no motion
  
  switch (dir) {
    case 0:
      MStop();
      break;
    case 1:
      MForward();
      break;
    case 2:
      MReverse();
      break;
    case 3:
      MStop();
      break;
    case 4:
      MRotateClockwise();
      break;
    case 5:
      MFordwardRight();
      break;
    case 6:
      MReverseRight();
      break;
    case 7:
      MRotateClockwise();
      break;
    case 8:
      MRotateAnitClockwise();
      break;
    case 9:
      MFordwardLeft();
      break;
    case 10:
      MReverseLeft();
      break;
    case 11:
      MRotateAnitClockwise();
      break;
    case 12:
      MStop();
      break;
    case 13:
      MForward();
      break;
    case 14:
      MReverse();
      break;
    case 15:
      MStop();
      break;
    default:
      MStop();
      break;
  }
}

//sneds the current configuration parameter based on the chosen option by the controller client
void CurrConfigParam(byte *byteArray) {

  switch (*(byteArray + 1)) {
    case MAX_OBJECT_DISTANCE:
      SendCurrConfig(maxObjectDistance);
      break;
    case BUGGY_SPEED:
      SendCurrConfig(buggySpeed);
      break;
    case LIGHT_INTENSITY_DELTA:
      SendCurrConfig(minLightIntensityDelta);
      break;
    default:
      break;
  }
}

//Sends the current configuration parameter back to ESP32 to transmit to the controller client
void SendCurrConfig(uint32_t parameter) {
  byte byteBuffer[5];
  byte *p;

  byteBuffer[0] = (byte)SEND_CURR_CONFIG;         //request type goes to position 0
  p = (byte*)&parameter;                          //point to the first byte of parameter
  for (int x = 0; x < 4; x++) {                   //place the bytes of parameter into the array after request type.
    byteBuffer[x + 1] = *(p + x);
  }

  Serial.println("Sending Current Configuration");
  SendSerial(&byteBuffer[0], 5);                  //send via uart
}

//updates the configuration option parameter with a new value chosen by the controller client
void UpdateConfigOption(byte *byteArray) {
  switch (*(byteArray + 1)) {
    case MAX_OBJECT_DISTANCE:
      maxObjectDistance = ToInt(&byteArray[2]);       //convert value bytes to integer
      ConfigUpdateStatus(CONFIG_UPDATE_STATUS_OK);    //sends the result back to the controller client
      break;
    case BUGGY_SPEED:
      buggySpeed = ToInt(&byteArray[2]);
      ConfigUpdateStatus(CONFIG_UPDATE_STATUS_OK);
      break;
    case LIGHT_INTENSITY_DELTA:
      minLightIntensityDelta = ToInt(&byteArray[2]);
      ConfigUpdateStatus(CONFIG_UPDATE_STATUS_OK);
      break;
    default:
      break;
  }
}

//converts integer bytes back to an integer
int ToInt(byte *byteArray) {
  int intResult = 0;

  for (int x = 0; x < 4; x++) {
    intResult = (intResult | (*(byteArray + x) << (8 * x)));
  }

  return intResult;
}

//constructs the request and sends it via uart back to esp32
void ConfigUpdateStatus(byte updateStatus) {
  byte byteBuffer[2];

  byteBuffer[0] = CONFIG_UPDATE_STATUS;   //request type
  byteBuffer[1] = updateStatus;           //request data

  SendSerial(&byteBuffer[0], 2);          //send via serial
}

//modifies the interaction mode
void InteractionMode(byte *byteArray) {
  switch (*(byteArray + 1)) {
    case INTMODE_MANUAL:
      interactionMode = INTMODE_MANUAL;               //change interaction mode to manual
      MStop();                                        //if the mode is not autonomous stop the buggy from moving
      break;
    case INTMODE_CONFIGURATION:
      interactionMode = INTMODE_CONFIGURATION;
      MStop();
      break;
    case INTMODE_AUTONOMOUS:
      interactionMode = INTMODE_AUTONOMOUS;
      break;
    default:
      interactionMode = INTMODE_MANUAL;
      MStop();
      break;
  }
}
