#include "Arduino.h"


//Values
#define BAUD_9600       9600
#define BUAD_115200     115200

//Pins
#define ESP32_UART2_RX  16
#define ESP32_UART2_TX  17

//functions
void SetupSerial(int buadRate);
void SetupI2C();
