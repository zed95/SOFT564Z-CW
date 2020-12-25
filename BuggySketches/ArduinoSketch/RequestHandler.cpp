#include "RequestHandler.h"
#include "Motor.h"

byte requestQueue[queueSize];
byte handlerRequestArray[queueSize];
int rhOldestByte = 0;
int rhNewestByte = 0;
int rhBytesInQueue = 0; //used to check whether queue is full or empty.
int pendingRequestData = 0;

void HandleRequest() {
  byte requestArray[queueSize];
  
  while (rhBytesInQueue > 0) {
    switch (peekQueue(&requestQueue[0], rhOldestByte)) {
      case MOVE_BUGGY:
        RemoveQueue(&requestQueue[0], rhOldestByte, rhBytesInQueue, &requestArray[0], 2);
        MoveBuggy(*(requestArray + 1));
        break;
      case CURR_CONFIG_PARAM:
        RemoveQueue(&requestQueue[0], rhOldestByte, rhBytesInQueue, &requestArray[0], 2);
        //Add Function
        break;
      case UPDATE_CONFIG_OPTION:
        RemoveQueue(&requestQueue[0], rhOldestByte, rhBytesInQueue, &requestArray[0], 6);
        //Add Function
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
