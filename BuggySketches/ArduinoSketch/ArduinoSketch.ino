#include "RequestHandler.h"
#include "Motor.h"

void setup() {
  // put your setup code here, to run once:
  Serial1.begin(115200);
  Serial.begin(115200);
  setupMotor();

}

void loop() {
  // put your main code here, to run repeatedly:
  if(Serial1.available()) {
    getRequest();
    HandleRequest();
  }

}

void getRequest() {
  int byteCount = 0;
  byte requestArray[1000];

  while(Serial1.available()) {
    requestArray[byteCount] = Serial1.read();
    Serial.println(requestArray[byteCount]);
    byteCount++;
  }

  AddQueue(&requestArray[0], byteCount);
}
