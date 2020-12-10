#include "RequestHandler.h"

TaskHandle_t  Task1;
SemaphoreHandle_t requestQueueMutex;
const int queueSize = 1000;
byte requestQueue[queueSize];
int oldestByte = 0;
int newestByte = 0;
int bytesInQueue = 0; //used to check whether queue is full or empty.

void SetupRequestHandler() {
  xTaskCreatePinnedToCore(
    HandleRequest,    // Function that should be called
    "Request Handler",  // Name of the task (for debugging)
    1000,            // Stack size (bytes)
    NULL,            // Parameter to pass
    1,               // Task priority
    &Task1,          // Task handle
    1
  );
}


void HandleRequest(void *parameter) {
byte requestArray[queueSize];

  while (1) {
    while(bytesInQueue > 0) {
      xSemaphoreTake(requestQueueMutex, portMAX_DELAY);
       RemoveQueue(&requestArray[0], 1);
       switch(requestArray[0]) {
        case 1:
          //Now remove n bytes based on the request type.
          //when calling the RemoveQueue function I will have to pass the requestArray[1] as the [0] position has the request type in it.
          //Check whether there are enough bytes in the array for the request type.
          //If there is not enough bytes don't carry out the operation and just save the request type until there are more bytes in the queue.
        break;
        case 2:
          //Now remove n bytes based on the request type.
          //when calling the RemoveQueue function I will have to pass the requestArray[1] as the [0] position has the request type in it.
          //Check whether there are enough bytes in the array for the request type.
          //If there is not enough bytes don't carry out the operation and just save the request type until there are more bytes in the queue.
        break;
        default:
        break;
       } 
      xSemaphoreGive(requestQueueMutex);

      //Dont carry out the below operations if there aren't enough bytes in the buffer for the next operation and wait until more arrive.
      switch(requestArray[0]) {
        case 1:
          //Call the function/s to carry out the requests
        break;
        case 2:
          //Call the function/s to carry out the requests
        break;
        default:
        break;
       } 
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

void RemoveQueue(byte *byteArray, int nBytes) {

  for(int i = 0; i < nBytes; i++) {
    if(bytesInQueue > 0) { 
      *(byteArray + i) = requestQueue[oldestByte];
      bytesInQueue--;
      if(oldestByte == (queueSize -1)) {
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
