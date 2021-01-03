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

//reads the ultrasonic senosr and converts it into distance in centimeters
void ReadUSSensor(uint32_t &distance) {
  long duration = 0;
  long val = 0;

  //send out 10 pulses and get their average. Done in order to get a more stable reading of distance
  for (int x = 0; x < 10; x++) {
    digitalWrite(US_TRIG_PIN, LOW);
    delayMicroseconds(2);
    digitalWrite(US_TRIG_PIN, HIGH);
    delayMicroseconds(10);
    digitalWrite(US_TRIG_PIN, LOW);

    val = pulseIn(US_ECHO_PIN, HIGH);
    duration += (val * 0.1);
  }

  /*
   * 0.034 = speed of sound in cm per microsecond
   * duration is the average duration of 10 readings
   * divided by 2 because the distance that the saves travel is from the transmitter to the object and then from the object to the receiver.
   */
  distance = (duration * 0.034f) / 2;
}
