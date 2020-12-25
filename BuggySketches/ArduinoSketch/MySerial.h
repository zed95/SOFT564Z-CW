#ifdef ARDUINO
#define ARDUINO
#include "Arduino.h"
#endif

//Values
#define BAUD_9600       9600
#define BAUD_115200     115200

//Functions
void SetupSerial(int baudRate);
void ReceiveSerial();
int CheckSerial();
void SendSerial(byte *byteArray, int byteCount);
