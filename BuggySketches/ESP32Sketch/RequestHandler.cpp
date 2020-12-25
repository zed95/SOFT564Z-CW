#include "RequestHandler.h"
#include "Sensors.h"
#include "myWiFi.h"
#include "CommProtocols.h"

TaskHandle_t  Task1;
SemaphoreHandle_t requestQueueMutex;
byte requestQueue[queueSize];
int rhOldestByte = 0;
int rhNewestByte = 0;
int rhBytesInQueue = 0; //used to check whether queue is full or empty.

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
    while (rhBytesInQueue > 0) {
      Serial.println("Im here");
      xSemaphoreTake(requestQueueMutex, portMAX_DELAY);
      switch (peekQueue(&requestQueue[0], rhOldestByte)) {
        case REQ_ENV_DATA:
          //Prototype is not needed in this case as this request only takes the request ID
          RemoveQueue(&requestQueue[0], rhOldestByte, rhBytesInQueue, &requestArray[0], 1);
          Serial.println("Here Now");
          SendEnvData();
          xSemaphoreGive(requestQueueMutex);
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
          Serial.println("Send Curr Config 1");
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
          break;
      }
    }
    vTaskDelay(pdMS_TO_TICKS(50));
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

void SendEnvData() {
  byte envData[6];

  envData[0] = 4;               //Indicate request type
  ReadBME280Data(&envData[1]);  //Get BME280 data
  ReadLDR(&envData[4]);
  SendWiFi(controllerClient, &envData[0], sizeof(envData));

}

void MoveBuggy(byte *byteArray) {
  SendSerial(byteArray, 2,  SERIAL2);
}

void InteractionMode(byte *byteArray) {
  SendSerial(byteArray, 2,  SERIAL2);

  switch (*(byteArray + 1)) {
    case INTMODE_MANUAL:
      SuspendAutoDataSend();
      break;
    case INTMODE_CONFIGURATION:
      SuspendAutoDataSend();
      break;
    case INTMODE_AUTONOMOUS:
      ResumeAutoDataSend();
      break;
    default:
      SuspendAutoDataSend();
      break;
  }
}

void CurrConfigParam(byte *byteArray) {
  byte byteBuffer[5];
  byte *p;

  switch (*(byteArray + 1)) {
    case AUTONOMOUS_DATA_T:
      p = (byte*)&dataExtractionPeriod;               //point to the first byte of dataExtractionPeriod
      byteBuffer[0] = (byte)SEND_CURR_CONFIG;         //request type goes to position 0
      for (int x = 0; x < 4; x++) {                   //place the bytes of dataExtractionPeriod into the array after request type.
        byteBuffer[x + 1] = *(p + x);
      }

      SendCurrConfig(&byteBuffer[0]);
      break;
    case MAX_OBJECT_DISTANCE:
      //send the request to buggy
      SendSerial(byteArray, 2, SERIAL2);
      break;
    case BUGGY_SPEED:
      SendSerial(byteArray, 2, SERIAL2);
      break;
    case LIGHT_INTENSITY_DELTA:
      SendSerial(byteArray, 2, SERIAL2);
      break;
    default:
      break;
  }
}

void UpdateConfigOption(byte *byteArray) {
  

  switch (*(byteArray + 1)) {
    case AUTONOMOUS_DATA_T:
      dataExtractionPeriod = ToInt(&byteArray[2]);
      ConfigUpdateStatus(CONFIG_UPDATE_STATUS_OK);
      break;
    case MAX_OBJECT_DISTANCE:
      //send the request to buggy
      SendSerial(byteArray, 6, SERIAL2);
      break;
    case BUGGY_SPEED:
      SendSerial(byteArray, 6, SERIAL2);
      break;
    case LIGHT_INTENSITY_DELTA:
      SendSerial(byteArray, 6, SERIAL2);
      break;
    default:
      break;
  }
}

int ToInt(byte *byteArray) {
  int intResult = 0;

  for(int x = 0; x < 4; x++) {
    intResult = (intResult | (*(byteArray + x) << (8 * x)));
  }

  return intResult;
}

void SendCurrConfig(byte *byteArray) {
  byte byteBuffer[5];

  //place configuration parameter into the array
  for (int x = 0; x < 5; x++) {
    byteBuffer[x] = *(byteArray + x);
  }

  Serial.println("Send Curr Config 2");
  SendWiFi(controllerClient, &byteBuffer[0], sizeof(byteBuffer));
}

void ConfigUpdateStatus(byte updateStatus) {
  byte byteBuffer[2];

  byteBuffer[0] = CONFIG_UPDATE_STATUS;
  byteBuffer[1] = updateStatus;

  SendWiFi(controllerClient, &byteBuffer[0], sizeof(byteBuffer));
}
