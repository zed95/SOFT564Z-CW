#include "Arduino.h"

//Defines
#define REQ_ENV_DATA  3

//Functions
void HandleRequest(void *parameter);
void AddQueue(byte *byteArray, int nBytes);
void RemoveQueue(byte *byteArray, int nBytes);
