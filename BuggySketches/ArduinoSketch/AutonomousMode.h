#include "Arduino.h"

extern uint32_t minLightIntensityDelta;
extern uint32_t maxObjectDistance;

//Functions
void SelfDrive();
uint32_t LightIntensityDelta(uint32_t rightLDR, uint32_t leftLDR);
void PickDirection();
