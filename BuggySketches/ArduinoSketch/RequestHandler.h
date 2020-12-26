#include "Arduino.h"

#define REQ_ENV_DATA          3
#define MOVE_BUGGY            5
#define INTERACTION_MODE      9
#define CURR_CONFIG_PARAM     10
#define SEND_CURR_CONFIG      11
#define UPDATE_CONFIG_OPTION  12
#define CONFIG_UPDATE_STATUS  13

#define INTMODE_MANUAL        1
#define INTMODE_CONFIGURATION 2
#define INTMODE_AUTONOMOUS    3

#define AUTONOMOUS_DATA_T     1
#define MAX_OBJECT_DISTANCE   2
#define BUGGY_SPEED           3
#define LIGHT_INTENSITY_DELTA 4

#define CONFIG_UPDATE_STATUS_OK   1

const int queueSize = 1000;
extern byte requestQueue[queueSize];
extern int rhOldestByte;
extern int rhNewestByte;
extern int rhBytesInQueue; //used to check whether queue is full or empty.
extern byte interactionMode;

void HandleRequest();
void AddQueue(byte *queue, int &newestByte, int &bytesInQueue, byte *srcArray, int nBytes);
void RemoveQueue(byte *queue, int &oldestByte, int &bytesInQueue, byte *dstArray, int nBytes);
byte peekQueue(byte *queue, int oldestByte);
void MoveBuggy(byte dir);
int ToInt(byte *byteArray);
void UpdateConfigOption(byte *byteArray);
void CurrConfigParam(byte *byteArray);
void SendCurrConfig(uint32_t parameter);
void ConfigUpdateStatus(byte updateStatus);
void InteractionMode(byte *byteArray);
