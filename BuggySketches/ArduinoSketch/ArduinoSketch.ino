#include "RequestHandler.h"
#include "Motor.h"
#include "MySerial.h"
#include "Sensors.h"
#include "AutonomousMode.h"

void setup() {
  // put your setup code here, to run once:
  SetupSerial(115200);  //setup serial
  SetupMotor();     //initialise the dc motors and servo
  SetupSensors();   //initialise the ultrasonic module and LDRs
}

void loop() {
  //check if any characters are available for reading before receiving or handling the request
  if(CheckSerial()) {
    ReceiveSerial();  //receive the data
    HandleRequest();  //handle any requests
  }

  //if in autonomous mode, buggy drives itself
  if(interactionMode == INTMODE_AUTONOMOUS) {
    SelfDrive();
  }

}
