#include "Arduino.h"

#define MOVE_BUGGY            5
#define CURR_CONFIG_PARAM     10
#define UPDATE_CONFIG_OPTION  12

const int queueSize = 1000;
extern byte requestQueue[queueSize];
extern int rhOldestByte;
extern int rhNewestByte;
extern int rhBytesInQueue; //used to check whether queue is full or empty.

void HandleRequest();
void AddQueue(byte *queue, int &newestByte, int &bytesInQueue, byte *srcArray, int nBytes);
void RemoveQueue(byte *queue, int &oldestByte, int &bytesInQueue, byte *dstArray, int nBytes);
byte peekQueue(byte *queue, int oldestByte);
void MoveBuggy(byte dir);
