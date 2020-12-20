#include "Arduino.h"

//Defines
#define REQ_ENV_DATA  3
#define MOVE_BUGGY    5

extern SemaphoreHandle_t requestQueueMutex;

//Functions
void HandleRequest(void *parameter);
void AddQueue(byte *byteArray, int nBytes);
void RemoveQueue(byte *byteArray, int nBytes);
void SetupRequestHandler();
void SendEnvData();
