#include "RequestHandler.h"
#include "Motor.h"
#include "MySerial.h"

void setup() {
  // put your setup code here, to run once:
  SetupSerial(115200);
  SetupMotor();
}

void loop() {
  // put your main code here, to run repeatedly:
  if(CheckSerial()) {
    ReceiveSerial();
    HandleRequest();
  }

}
