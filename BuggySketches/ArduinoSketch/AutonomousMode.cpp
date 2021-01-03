#include "AutonomousMode.h"
#include "Motor.h"
#include "Sensors.h"

//default minimum difference between light sensors and maximum permissiable distance from object.
uint32_t minLightIntensityDelta = 40;
uint32_t maxObjectDistance = 10;



void SelfDrive() {
  uint32_t leftLDR, rightLDR;
  uint32_t distance;
  uint32_t lightIntensityDelta;

  //Read proximity from any object using the ultrsonic module
  ReadUSSensor(distance);

  //If distance to object is smaller than the maximum permissiable distance in cm 
  if (distance < maxObjectDistance) {
    MStop();                            //stop the buggy
    PickDirection();                    //pick a direction to move to avoid the object and move
  }
  else {                                //otherwise
    //get LDR readings and calculate the difference between the two
    readLDRValues(rightLDR, leftLDR);
    lightIntensityDelta = LightIntensityDelta(rightLDR, leftLDR);

    //if the intensity difference is large enough and there is more light to the left of the buggy then move left
    if ((leftLDR > rightLDR) && (lightIntensityDelta >= minLightIntensityDelta)) {
      MFordwardLeft();
    }
    //if difference is big enough and more light on the right side then move right
    else if ((leftLDR < rightLDR) && (lightIntensityDelta >= minLightIntensityDelta)) {
      MFordwardRight();
    }
    else {  //otherwise move forward
      MForward();
    }
  }

}

void PickDirection() {
  uint32_t leftDistance, rightDistance;
  uint32_t leftLDR, rightLDR;
  uint32_t lightIntensityDelta;
  byte dir;

  //Read the distance to any object on the left side
  MoveServo(LEFT);
  delay(500);
  ReadUSSensor(leftDistance);

  //Read the distance to any object on the right side
  MoveServo(RIGHT);
  delay(500);
  ReadUSSensor(rightDistance);

  //Position Ultrasonic sensor ahead of the buggy
  MoveServo(STRAIGHT);

  //read LDR values and get delta
  readLDRValues(rightLDR, leftLDR);
  lightIntensityDelta = LightIntensityDelta(rightLDR, leftLDR);


  if ((leftLDR >= rightLDR) && (lightIntensityDelta >= minLightIntensityDelta) && (leftDistance >= (maxObjectDistance * 2))) {          //if more light on the left (or equal to right) side and distance from object is permissible
    MRotateAnitClockwise(); //Rotate left to move away from the obstacle
    delay(500); //let it rotate enough
  }
  else if ((rightLDR >= leftLDR) && (lightIntensityDelta >= minLightIntensityDelta) && (rightDistance >= (maxObjectDistance * 2))) {    //if more light on the right (or equal to left) side and distance from object is permissible
    MRotateClockwise();   //Rotate right to move away from the obstacle
    delay(500); //let it rotate enough
  }
  else {  //reverse and turn left to go somewhere else
    MReverse();
    delay(1000);  //let the buggy reverse far enough
    MRotateAnitClockwise();
    delay(500); //let the buggy rotate left for long enough
  }

}

//calculates the difference in light intensity between the right and left buggy ldr
uint32_t LightIntensityDelta(uint32_t rightLDR, uint32_t leftLDR) {
  int32_t delta;

  delta = leftLDR - rightLDR; //subtract one from another
  return abs(delta);          //make it popsitive.
}
