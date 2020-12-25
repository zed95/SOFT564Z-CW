#include "Motor.h"
uint32_t buggySpeed = 255;


void SetupMotor() {
  pinMode(RIGHT_DIR, OUTPUT);
  pinMode(LEFT_DIR, OUTPUT);
  pinMode(RIGHT_BRAKE, OUTPUT);
  pinMode(LEFT_BRAKE, OUTPUT); 
}


void MStop() {
  WriteRightMotor(FORWARD, 0, BRAKE_ON);
  WriteLeftMotor(FORWARD, 0, BRAKE_ON);  
}

void MForward() {
  WriteRightMotor(REVERSE, buggySpeed, BRAKE_OFF);
  WriteLeftMotor(FORWARD, buggySpeed, BRAKE_OFF);
}

void MReverse() {
  WriteRightMotor(FORWARD, buggySpeed, BRAKE_OFF);
  WriteLeftMotor(REVERSE, buggySpeed, BRAKE_OFF);
}

void MRotateClockwise() {
  WriteRightMotor(FORWARD, buggySpeed, BRAKE_OFF);
  WriteLeftMotor(FORWARD, buggySpeed, BRAKE_OFF);
}

void MRotateAnitClockwise() {
  WriteRightMotor(REVERSE, buggySpeed, BRAKE_OFF);
  WriteLeftMotor(REVERSE, buggySpeed, BRAKE_OFF);
}

void MFordwardRight() {
  WriteRightMotor(REVERSE, buggySpeed/2, BRAKE_OFF);
  WriteLeftMotor(FORWARD, buggySpeed, BRAKE_OFF);  
}

void MFordwardLeft() {
  WriteRightMotor(REVERSE, buggySpeed, BRAKE_OFF);
  WriteLeftMotor(FORWARD, buggySpeed/2, BRAKE_OFF);   
}

void MReverseRight() {
  WriteRightMotor(FORWARD, buggySpeed/2, BRAKE_OFF);
  WriteLeftMotor(REVERSE, buggySpeed, BRAKE_OFF);   
}

void MReverseLeft() {
  WriteRightMotor(FORWARD, buggySpeed, BRAKE_OFF);
  WriteLeftMotor(REVERSE, buggySpeed/2, BRAKE_OFF); 
}


void WriteRightMotor(int dir, int spd, int brake) {
    //Set spin direction
    if(dir > 0) {
        digitalWrite(RIGHT_DIR, HIGH);  //Fordward
    }
    else {
      digitalWrite(RIGHT_DIR, LOW);  //Reverse
    }

    //Set the brake
    if(brake > 0) {
      digitalWrite(RIGHT_BRAKE, HIGH);  //Brake On
    }
    else {
      digitalWrite(RIGHT_BRAKE, LOW);  //Brake Off
    }

    //Set the speed of the motor
    if(spd < 0) {
      analogWrite(RIGHT_SPEED, 0);   //dont spin the motor
    }
    else if(spd > 255) {
      analogWrite(RIGHT_SPEED, 255);   //spin at full speed
    }
    else {
      analogWrite(RIGHT_SPEED, spd);   //spin at speed spd
    }
  
}

void WriteLeftMotor(int dir, int spd, int brake) {
    //Set spin direction
    if(dir > 0) {
        digitalWrite(LEFT_DIR, HIGH);  //Fordward
    }
    else {
      digitalWrite(LEFT_DIR, LOW);  //Reverse
    }

    //Set the brake
    if(brake > 0) {
      digitalWrite(LEFT_BRAKE, HIGH);  //Brake On
    }
    else {
      digitalWrite(LEFT_BRAKE, LOW);  //Brake Off
    }

    //Set the speed of the motor
    if(spd < 0) {
      analogWrite(LEFT_SPEED, 0);   //dont spin the motor
    }
    else if(spd > 255) {
      analogWrite(LEFT_SPEED, 255);   //spin at full speed
    }
    else {
      analogWrite(LEFT_SPEED, spd);   //spin at speed spd
    }
}
