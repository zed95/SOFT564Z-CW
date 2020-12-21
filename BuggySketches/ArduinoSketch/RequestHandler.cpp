#include "RequestHandler.h"
#include "Motor.h"

const int queueSize = 1000;
byte requestQueue[queueSize];
byte handlerRequestArray[queueSize];
int oldestByte = 0;
int newestByte = 0;
int bytesInQueue = 0; //used to check whether queue is full or empty.
int pendingRequestData = 0;

void HandleRequest() {

  while (bytesInQueue > 0) {
    //only extract new request if previous request has been handled
    if (!pendingRequestData) {
      RemoveQueue(&handlerRequestArray[0], 1);
    }
    
    switch (handlerRequestArray[0]) {
      case 1:
        break;
      case 2:
        //----Prototype for the above requirement---//
        if (bytesInQueue < 2) {       //if the request that requires two more bytes and the buffer has not got them then
          pendingRequestData = 1;     //set the flag that a request has not been handleed yet
          return;                     //and return to the loop to extract more data.
        }
        else {
          RemoveQueue(&handlerRequestArray[1], 2);
          pendingRequestData = 0;
          //Call the function to carry out the request
        }
        break;
      case MOVE_BUGGY:
        if (bytesInQueue < 1) {
          pendingRequestData = 1;     //set the flag that a request has not been handleed yet
          return;                     //and return to the loop to extract more data.
        }
          else {
            RemoveQueue(&handlerRequestArray[1], 1);
            MoveBuggy(handlerRequestArray[1]);
            pendingRequestData = 0;
          }
          break;
        default:
          break;
        }
    }
  }

  void AddQueue(byte *byteArray, int nBytes) {

    for (int x = 0; x < nBytes; x++) {
      if (bytesInQueue < queueSize) {
        requestQueue[newestByte] = *(byteArray + x);
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

  void RemoveQueue(byte * byteArray, int nBytes) {

    for (int i = 0; i < nBytes; i++) {
      if (bytesInQueue > 0) {
        *(byteArray + i) = requestQueue[oldestByte];
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
