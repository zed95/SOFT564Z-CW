#include "Arduino.h"

#define LEFT_LDR_PIN    A7
#define RIGHT_LDR_PIN   A6
#define US_ECHO_PIN     52
#define US_TRIG_PIN     53



//Functions
void SetupSensors();
void SetupLDRSensors();
void readLDRValues(uint32_t &rightLDR, uint32_t &leftLDR);
void SetupUltrasonicSensor();
void ReadUSSensor(uint32_t &distance);
