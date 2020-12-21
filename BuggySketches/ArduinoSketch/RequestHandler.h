#include "Arduino.h"

#define MOVE_BUGGY      5


void HandleRequest();
void AddQueue(byte *byteArray, int nBytes);
void RemoveQueue(byte * byteArray, int nBytes);
void MoveBuggy(byte dir);
