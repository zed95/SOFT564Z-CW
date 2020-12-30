#include "Arduino.h"

//Defines
//Values
#define BAUD_9600       9600
#define BAUD_115200     115200
#define SERIAL1         1
#define SERIAL2         2

//Pins
#define ESP32_UART2_RX  16
#define ESP32_UART2_TX  17

//functions
void SetupSerial(int buadRate);
void SetupI2C();
void SendSerial(byte *byteArray, int byteCount, int serial);
void ReceiveSerial();
