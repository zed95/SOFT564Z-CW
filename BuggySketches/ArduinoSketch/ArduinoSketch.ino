#include "RequestHandler.h"
#include "Motor.h"
#include "MySerial.h"
#include "Sensors.h"
#include "AutonomousMode.h"

void setup() {
  // put your setup code here, to run once:
  SetupSerial(115200);
  SetupMotor();
  SetupSensors();
}

void loop() {
  // put your main code here, to run repeatedly:
  if(CheckSerial()) {
    ReceiveSerial();
    HandleRequest();
  }

  if(interactionMode == INTMODE_AUTONOMOUS) {
    SelfDrive();
  }

}
