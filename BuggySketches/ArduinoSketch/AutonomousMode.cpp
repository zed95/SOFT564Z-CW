#include "AutonomousMode.h"
#include "Motor.h"
#include "Sensors.h"

uint32_t minLightIntensityDelta = 40; 


void SelfDrive() {
  uint32_t leftLDR, rightLDR;
  uint32_t distance;
  uint32_t lightIntensityDelta;

  //This function should go in a loop while the interaction mode is set to autonomous 
  //If the interaction mode changes, the function should stop the buggy from moving before leaving the function

  //get LDR readings and calculate the difference between the two
  readLDRValues(rightLDR, leftLDR);
  lightIntensityDelta = LightIntensityDelta(rightLDR, leftLDR);

  if((leftLDR > rightLDR) && (lightIntensityDelta >= minLightIntensityDelta)) {
    //move to the left
  }
  else if((leftLDR < rightLDR) && (lightIntensityDelta >= minLightIntensityDelta)) {
    //move to the right
  }
  else {
    //move straight as the difference in the two light levels is not large enough
  }

  
  
}

uint32_t LightIntensityDelta(uint32_t rightLDR, uint32_t leftLDR) {
  int32_t delta;
    
  delta = leftLDR - rightLDR;
  return abs(delta);
}
