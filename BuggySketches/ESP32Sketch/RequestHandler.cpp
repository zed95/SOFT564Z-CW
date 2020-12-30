#include "RequestHandler.h"
#include "Sensors.h"
#include "myWiFi.h"
#include "CommProtocols.h"

TaskHandle_t  Task1;
SemaphoreHandle_t requestQueueMutex;  //request handler mutex to allow safe addition/removal of data to/from the queue
byte requestQueue[queueSize]; //request handler queue of size queue size.
int rhOldestByte = 0;         //points to the first byte in the queue
int rhNewestByte = 0;         //points to the next free space in the queue
int rhBytesInQueue = 0;       //used to check whether queue is full or empty.
int interactionMode = 0;

//Create task and mutex to allow requests to be serviced in real-time safely
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
    while (rhBytesInQueue > 0) {                                   //continue until there are no more requests in the queue
      xSemaphoreTake(requestQueueMutex, portMAX_DELAY);            //wait to take the mutex to allow safe removal of bytes from the queue
      switch (peekQueue(&requestQueue[0], rhOldestByte)) {         //look at the first first element in the request handler queue to see what type of request has been sent. The service the request based on the type of request it is.
        case REQ_ENV_DATA:
          RemoveQueue(&requestQueue[0], rhOldestByte, rhBytesInQueue, &requestArray[0], 1);   //remove the request data from the queue
          SendEnvData();                                                                      //call function that fulfils the request
          xSemaphoreGive(requestQueueMutex);                                                  //release the mutex
          break;
        case MOVE_BUGGY:
          RemoveQueue(&requestQueue[0], rhOldestByte, rhBytesInQueue, &requestArray[0], 2);
          MoveBuggy(&requestArray[0]);
          xSemaphoreGive(requestQueueMutex);
          break;
        case INTERACTION_MODE:
          RemoveQueue(&requestQueue[0], rhOldestByte, rhBytesInQueue, &requestArray[0], 2);
          InteractionMode(&requestArray[0]);
          xSemaphoreGive(requestQueueMutex);
          break;
        case CURR_CONFIG_PARAM:
          RemoveQueue(&requestQueue[0], rhOldestByte, rhBytesInQueue, &requestArray[0], 2);
          CurrConfigParam(&requestArray[0]);
          xSemaphoreGive(requestQueueMutex);
          break;
        case SEND_CURR_CONFIG:
          RemoveQueue(&requestQueue[0], rhOldestByte, rhBytesInQueue, &requestArray[0], 5);
          SendCurrConfig(&requestArray[0]);
          xSemaphoreGive(requestQueueMutex);
          break;
        case UPDATE_CONFIG_OPTION:
          RemoveQueue(&requestQueue[0], rhOldestByte, rhBytesInQueue, &requestArray[0], 6);
          UpdateConfigOption(&requestArray[0]);
          xSemaphoreGive(requestQueueMutex);
          break;
        case CONFIG_UPDATE_STATUS:
          RemoveQueue(&requestQueue[0], rhOldestByte, rhBytesInQueue, &requestArray[0], 2);
          ConfigUpdateStatus(requestArray[1]);
          xSemaphoreGive(requestQueueMutex);
          break;
        default:
          xSemaphoreGive(requestQueueMutex);
          //request unrecognised: error in sending the request.
          break;
      }
    }
    vTaskDelay(pdMS_TO_TICKS(50));
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
      *(dstArray + i) = *(queue + oldestByte);  //places the first byte in the queue into the destination array at index i
      bytesInQueue--;                           //number of bytes in queue decreases by 1 as one byte has been "removed"
      if (oldestByte == (queueSize - 1)) {      //if first byte in the queue has been removed from the boundary of the queue,
        oldestByte = 0;                         //first byte in the queue is now at index 0
      }
      else {
        oldestByte++;                           //the first byte in the queue was not removed at boundary therefore increment the indicator to the next byte in the queue.
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

//Extartcs and send environmental data back to the controller client
void SendEnvData() {
  byte envData[6];

  envData[0] = REC_ENV_DATA;    //Indicate request type
  ReadBME280Data(&envData[1]);  //Get BME280 data
  ReadLDR(&envData[4]);         //get LDR data
  SendWiFi(controllerClient, &envData[0], sizeof(envData));   //Send over network

}

//sneds the request to the arduino which controls the motrs of the buggy.
void MoveBuggy(byte *byteArray) {
  //Rekay request to the arduino.
  SendSerial(byteArray, 2,  SERIAL2);
}

//Enable/disable appropriate functionality based on chosen interaction mode
void InteractionMode(byte *byteArray) {
  //Relay the request to the Arduino
  SendSerial(byteArray, 2,  SERIAL2);

  //based on the interaction mode chosen enable/disable functionality. Index 1 of the array contains information about the chosen interaction mode
  switch (*(byteArray + 1)) {
    case INTMODE_MANUAL:
      interactionMode = INTMODE_MANUAL;
      SuspendAutoDataSend();
      break;
    case INTMODE_CONFIGURATION:
      interactionMode = INTMODE_CONFIGURATION;
      SuspendAutoDataSend();    //Suspepnd the task that extracts the environmental data automatically and sends it to the controller client.
      break;
    case INTMODE_AUTONOMOUS:
      interactionMode = INTMODE_AUTONOMOUS;
      ResumeAutoDataSend();     //Restart the task that extracts the environmental data automatically and sends it to the controller client.
      break;
    default:
      interactionMode = INTMODE_MANUAL;
      SuspendAutoDataSend();
      break;
  }
}

//extratcs and sends the current configuration parameter for the selected option by the controller client
void CurrConfigParam(byte *byteArray) {
  byte byteBuffer[5];
  byte *p;

  switch (*(byteArray + 1)) {
    case AUTONOMOUS_DATA_T:
      //Construct the request by packaging all the needed bytes into the array for transmission.
      p = (byte*)&dataExtractionPeriod;               //point to the first byte of dataExtractionPeriod
      byteBuffer[0] = (byte)SEND_CURR_CONFIG;         //request type goes to position 0
      for (int x = 0; x < 4; x++) {                   //place the bytes of dataExtractionPeriod into the array after request type.
        byteBuffer[x + 1] = *(p + x);
      }

      //send the request
      SendCurrConfig(&byteBuffer[0]);
      break;
    case MAX_OBJECT_DISTANCE:
      //send the request to buggy as this configuration does not reside on ESP32
      SendSerial(byteArray, 2, SERIAL2);
      break;
    case BUGGY_SPEED:
      //send the request to buggy as this configuration does not reside on ESP32
      SendSerial(byteArray, 2, SERIAL2);
      break;
    case LIGHT_INTENSITY_DELTA:
      //send the request to buggy as this configuration does not reside on ESP32
      SendSerial(byteArray, 2, SERIAL2);
      break;
    default:
      //unrecognised configuration option: do nothing
      break;
  }
}

//Updates the selected configuration option with new parameters
void UpdateConfigOption(byte *byteArray) {

  //Find and update the selected option by the controller client
  switch (*(byteArray + 1)) {
    case AUTONOMOUS_DATA_T:                             //Update the new task delay for the automatic extraction of data in autonomous mode
      dataExtractionPeriod = ToInt(&byteArray[2]);      //save the new delay into dataExtractionPeriod
      ConfigUpdateStatus(CONFIG_UPDATE_STATUS_OK);      //sned status back to controller client: update was successful
      break;
    case MAX_OBJECT_DISTANCE:
      //send the request to buggy as this configuration does not reside on ESP32
      SendSerial(byteArray, 6, SERIAL2);
      break;
    case BUGGY_SPEED:
      //send the request to buggy as this configuration does not reside on ESP32
      SendSerial(byteArray, 6, SERIAL2);
      break;
    case LIGHT_INTENSITY_DELTA:
      //send the request to buggy as this configuration does not reside on ESP32
      SendSerial(byteArray, 6, SERIAL2);
      break;
    default:
      //unrecognised configuration option: do nothing
      break;
  }
}

//convert array of bytes into an integer
int ToInt(byte *byteArray) {
  int intResult = 0;

  //convert bytes to integer
  for (int x = 0; x < 4; x++) {
    intResult = (intResult | (*(byteArray + x) << (8 * x)));
  }

  return intResult;
}

//Sends the current configuration for the selected configuration option
void SendCurrConfig(byte *byteArray) {
  byte byteBuffer[5];

  //place configuration parameter bytes into the array for transmission over the network
  for (int x = 0; x < 5; x++) {
    byteBuffer[x] = *(byteArray + x);
  }

  SendWiFi(controllerClient, &byteBuffer[0], sizeof(byteBuffer));
}

//Sends the result of configuration option update
void ConfigUpdateStatus(byte updateStatus) {
  byte byteBuffer[2];

  //create the request
  byteBuffer[0] = CONFIG_UPDATE_STATUS;
  byteBuffer[1] = updateStatus;

  SendWiFi(controllerClient, &byteBuffer[0], sizeof(byteBuffer));
}
