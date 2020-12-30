#include "Arduino.h"

//Defines
//Types of requests
#define REQ_ENV_DATA          3
#define REC_ENV_DATA          4
#define MOVE_BUGGY            5
#define INTERACTION_MODE      9
#define CURR_CONFIG_PARAM     10
#define SEND_CURR_CONFIG      11
#define UPDATE_CONFIG_OPTION  12
#define CONFIG_UPDATE_STATUS  13


//Types of interaction modes
#define INTMODE_MANUAL        1
#define INTMODE_CONFIGURATION 2
#define INTMODE_AUTONOMOUS    3

//Types of configuration options
#define AUTONOMOUS_DATA_T     1
#define MAX_OBJECT_DISTANCE   2
#define BUGGY_SPEED           3
#define LIGHT_INTENSITY_DELTA 4

//Configuration result responses 
#define CONFIG_UPDATE_STATUS_OK   1


const int queueSize = 1000;                   //defines the size of the queues
extern SemaphoreHandle_t requestQueueMutex;   //request handler mutex to allow safe addition/removal of data to/from the queue 
extern byte requestQueue[queueSize];          //request handler queue of size queue size.
extern int rhOldestByte;                      //points to the first byte in the queue
extern int rhNewestByte;                      //points to the next free space in the queue
extern int rhBytesInQueue;                    //used to check whether queue is full or empty or how many bytes are in the queue.
extern int interactionMode;

//Functions
void HandleRequest(void *parameter);
void AddQueue(byte *queue, int &newestByte, int &bytesInQueue, byte *srcArray, int nBytes);
void RemoveQueue(byte *queue, int &oldestByte, int &bytesInQueue, byte *dstArray, int nBytes);
void SetupRequestHandler();
void SendEnvData();
void MoveBuggy(byte *byteArray);
void InteractionMode(byte *byteArray);
void SendCurrConfig(byte *byteArray);
void CurrConfigParam(byte *byteArray);
byte peekQueue(byte *queue, int oldestByte);
void UpdateConfigOption(byte *byteArray);
int ToInt(byte *byteArray);
void ConfigUpdateStatus(byte updateStatus);
