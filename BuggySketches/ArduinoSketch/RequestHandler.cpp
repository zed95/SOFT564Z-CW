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

  while (rhBytesInQueue > 0) {
    switch (peekQueue(&requestQueue[0], rhOldestByte)) {
      case MOVE_BUGGY:
        RemoveQueue(&requestQueue[0], rhOldestByte, rhBytesInQueue, &requestArray[0], 2);
        MoveBuggy(*(requestArray + 1));
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

void AddQueue(byte *queue, int &newestByte, int &bytesInQueue, byte *srcArray, int nBytes) {

  for (int x = 0; x < nBytes; x++) {
    if (bytesInQueue < queueSize) {
      *(queue + newestByte) = *(srcArray + x);
      bytesInQueue++;
      if (newestByte == (queueSize - 1)) {
        newestByte = 0;
      }
      else {
        newestByte++;
      }

    }
    else {
      //Array is full decide what to do.
    }
  }
}

void RemoveQueue(byte *queue, int &oldestByte, int &bytesInQueue, byte *dstArray, int nBytes) {

  for (int i = 0; i < nBytes; i++) {
    if (bytesInQueue > 0) {
      *(dstArray + i) = *(queue + oldestByte);
      bytesInQueue--;
      if (oldestByte == (queueSize - 1)) {
        oldestByte = 0;
      }
      else {
        oldestByte++;
      }
    }
    else {
      //Queue is empty decide what to do. Probably add some error codes.
      //-queue is empty
      //-tried to remove more bytes than there were in the queue
    }
  }
}

byte peekQueue(byte *queue, int oldestByte) {
  byte firstQueueValue = 0;

  firstQueueValue = *(queue + oldestByte);

  return firstQueueValue;
}


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


void SendCurrConfig(uint32_t parameter) {
  byte byteBuffer[5];
  byte *p;

  byteBuffer[0] = (byte)SEND_CURR_CONFIG;         //request type goes to position 0
  p = (byte*)&parameter;                          //point to the first byte of parameter
  for (int x = 0; x < 4; x++) {                   //place the bytes of parameter into the array after request type.
    byteBuffer[x + 1] = *(p + x);
  }

  Serial.println(byteBuffer[0]);
  Serial.println(byteBuffer[1]);
  Serial.println(byteBuffer[2]);
  Serial.println(byteBuffer[3]);
  Serial.println(byteBuffer[4]);
  SendSerial(&byteBuffer[0], 5);
}

void UpdateConfigOption(byte *byteArray) {
  switch (*(byteArray + 1)) {
    case MAX_OBJECT_DISTANCE:
      maxObjectDistance = ToInt(&byteArray[2]);
      ConfigUpdateStatus(CONFIG_UPDATE_STATUS_OK);
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

int ToInt(byte *byteArray) {
  int intResult = 0;

  for (int x = 0; x < 4; x++) {
    intResult = (intResult | (*(byteArray + x) << (8 * x)));
  }

  return intResult;
}

void ConfigUpdateStatus(byte updateStatus) {
  byte byteBuffer[2];

  byteBuffer[0] = CONFIG_UPDATE_STATUS;
  byteBuffer[1] = updateStatus;

  SendSerial(&byteBuffer[0], 2);
}

void InteractionMode(byte *byteArray) {
  switch (*(byteArray + 1)) {
    case INTMODE_MANUAL:
      interactionMode = INTMODE_MANUAL;
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
