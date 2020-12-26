#include "Sensors.h"

void SetupSensors() {
  SetupLDRSensors();
  SetupUltrasonicSensor();
}

void SetupLDRSensors() {
  pinMode(RIGHT_LDR_PIN, INPUT);
  pinMode(LEFT_LDR_PIN, INPUT);
}

void SetupUltrasonicSensor() {
  pinMode(US_ECHO_PIN, INPUT);
  pinMode(US_TRIG_PIN, OUTPUT);
}

//reads buggy ldr values
void readLDRValues(uint32_t &rightLDR, uint32_t &leftLDR) {
  rightLDR = analogRead(RIGHT_LDR_PIN);
  leftLDR = analogRead(LEFT_LDR_PIN);
}

void ReadUSSensor(uint32_t &distance) {
  long duration = 0;
  
  digitalWrite(US_TRIG_PIN, LOW);
  delayMicroseconds(2);
  digitalWrite(US_TRIG_PIN, HIGH);
  delayMicroseconds(10);
  digitalWrite(US_TRIG_PIN, LOW);

  duration = pulseIn(US_ECHO_PIN, HIGH);
  distance = duration * 0.034f/2;
}
