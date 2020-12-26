#include "AutonomousMode.h"
#include "Motor.h"
#include "Sensors.h"

uint32_t minLightIntensityDelta = 40;
uint32_t maxObjectDistance = 10;



void SelfDrive() {
  uint32_t leftLDR, rightLDR;
  uint32_t distance;
  uint32_t lightIntensityDelta;

  ReadUSSensor(distance);
  Serial.print("Distance: ");
  Serial.println(distance);
  if (distance < maxObjectDistance) {
    MStop();
    PickDirection();
  }
  else {
    //get LDR readings and calculate the difference between the two
    readLDRValues(rightLDR, leftLDR);
    lightIntensityDelta = LightIntensityDelta(rightLDR, leftLDR);

    if ((leftLDR > rightLDR) && (lightIntensityDelta >= minLightIntensityDelta)) {
      MFordwardLeft();
    }
    else if ((leftLDR < rightLDR) && (lightIntensityDelta >= minLightIntensityDelta)) {
      MFordwardRight();
    }
    else {
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
    MRotateAnitClockwise();
    delay(500);
  }
  else if ((rightLDR >= leftLDR) && (lightIntensityDelta >= minLightIntensityDelta) && (rightDistance >= (maxObjectDistance * 2))) {    //if more light on the right (or equal to left) side and distance from object is permissible
    MRotateClockwise();
    delay(500);
  }
  else {
    MReverse();
    delay(1000);
    MRotateAnitClockwise();
    delay(500);
  }

}

uint32_t LightIntensityDelta(uint32_t rightLDR, uint32_t leftLDR) {
  int32_t delta;

  delta = leftLDR - rightLDR;
  return abs(delta);
}
