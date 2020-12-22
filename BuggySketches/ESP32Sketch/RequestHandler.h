#include "Arduino.h"

//Defines
#define REQ_ENV_DATA          3
#define MOVE_BUGGY            5
#define INTERACTION_MODE      9
#define INTMODE_MANUAL        1
#define INTMODE_CONFIGURATION 2
#define INTMODE_AUTONOMOUS    3

extern SemaphoreHandle_t requestQueueMutex;

//Functions
void HandleRequest(void *parameter);
void AddQueue(byte *byteArray, int nBytes);
void RemoveQueue(byte *byteArray, int nBytes);
void SetupRequestHandler();
void SendEnvData();
void MoveBuggy(byte *byteArray);
void InteractionMode(byte *byteArray);
