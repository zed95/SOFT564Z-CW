#include "RequestHandler.h"
#include "Sensors.h"
#include "myWiFi.h"

TaskHandle_t  Task1;
SemaphoreHandle_t requestQueueMutex;
const int queueSize = 1000;
byte requestQueue[queueSize];
int oldestByte = 0;
int newestByte = 0;
int bytesInQueue = 0; //used to check whether queue is full or empty.

void SetupRequestHandler() {
  requestQueueMutex = xSemaphoreCreateMutex();
  
  xTaskCreatePinnedToCore(
    HandleRequest,    // Function that should be called
    "Request Handler",  // Name of the task (for debugging)
    10000,            // Stack size (bytes)
    NULL,            // Parameter to pass
    1,               // Task priority
    &Task1,          // Task handle
    1
  );
}


void HandleRequest(void *parameter) {
  byte requestArray[queueSize];

  while (1) {
    while (bytesInQueue > 0) {
      Serial.println("Im here");
      xSemaphoreTake(requestQueueMutex, portMAX_DELAY);
      RemoveQueue(&requestArray[0], 1);
      Serial.println(requestArray[0]);
      switch (requestArray[0]) {
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

          //----Prototype for the above requirement---//
          if (bytesInQueue < 2) {
            xSemaphoreGive(requestQueueMutex);                      //Give up mutex to allow more data to be added to queue
            while (bytesInQueue < 2) {}
            xSemaphoreTake(requestQueueMutex, portMAX_DELAY);
            RemoveQueue(&requestArray[1], 2);
            xSemaphoreGive(requestQueueMutex);
            //Call the function to carry out the request
          }
          else {
            RemoveQueue(&requestArray[1], 2);
            xSemaphoreGive(requestQueueMutex);
            //Call the function to carry out the request
          }
          break;
        case REQ_ENV_DATA:
           //Prototype is not needed in this case as this request only takes the request ID
           //Just call the function to extract the data and send it over to the client.
           //Create a handler function that carries out the request.
           Serial.println("Here Now");
           SendEnvData();
           xSemaphoreGive(requestQueueMutex);
          break;
        default:
          xSemaphoreGive(requestQueueMutex);
          break;
      }
    }
    vTaskDelay(pdMS_TO_TICKS(50));
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

void SendEnvData() {
  byte envData[6];

  envData[0] = 4;               //Indicate request type
  ReadBME280Data(&envData[1]);  //Get BME280 data
  ReadLDR(&envData[4]);
  Serial.println(envData[0]);
  Serial.println(envData[1]);
  Serial.println(envData[2]);
  Serial.println(envData[3]);
  Serial.println(envData[4]);
  Serial.println(envData[5]);
  SendWiFi(controllerClient, &envData[0], sizeof(envData));
  
}
