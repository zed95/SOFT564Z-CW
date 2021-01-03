#include "Motor.h"
#include <Servo.h> 

Servo ultrasonicServo;
uint32_t buggySpeed = 255;

//initialise motor and servo pins
void SetupMotor() {
  pinMode(RIGHT_DIR, OUTPUT);
  pinMode(LEFT_DIR, OUTPUT);
  pinMode(RIGHT_BRAKE, OUTPUT);
  pinMode(LEFT_BRAKE, OUTPUT); 

  ultrasonicServo.attach(SERVO_PIN);
}

//move servo to a specified position
void MoveServo(int pos) {
  ultrasonicServo.write(pos);  // set servo to mid-point. 90 - face forward, left - 180, right - 0
}

//Stop the buggy
void MStop() {
  WriteRightMotor(FORWARD, 0, BRAKE_ON);
  WriteLeftMotor(FORWARD, 0, BRAKE_ON);  
}

//Move buggy forward
void MForward() {
  WriteRightMotor(REVERSE, buggySpeed, BRAKE_OFF);
  WriteLeftMotor(FORWARD, buggySpeed, BRAKE_OFF);
}

//Make the buggy move backward
void MReverse() {
  WriteRightMotor(FORWARD, buggySpeed, BRAKE_OFF);
  WriteLeftMotor(REVERSE, buggySpeed, BRAKE_OFF);
}

//rotate the buggy right
void MRotateClockwise() {
  WriteRightMotor(FORWARD, buggySpeed, BRAKE_OFF);
  WriteLeftMotor(FORWARD, buggySpeed, BRAKE_OFF);
}

//rotate the buggy left
void MRotateAnitClockwise() {
  WriteRightMotor(REVERSE, buggySpeed, BRAKE_OFF);
  WriteLeftMotor(REVERSE, buggySpeed, BRAKE_OFF);
}

//make the buggy turn right while also moving forward
void MFordwardRight() {
  WriteRightMotor(REVERSE, buggySpeed/2, BRAKE_OFF);
  WriteLeftMotor(FORWARD, buggySpeed, BRAKE_OFF);  
}

//make the buggy turn left while also moving forward
void MFordwardLeft() {
  WriteRightMotor(REVERSE, buggySpeed, BRAKE_OFF);
  WriteLeftMotor(FORWARD, buggySpeed/2, BRAKE_OFF);   
}

//make the buggy reverse while also turning right
void MReverseRight() {
  WriteRightMotor(FORWARD, buggySpeed/2, BRAKE_OFF);
  WriteLeftMotor(REVERSE, buggySpeed, BRAKE_OFF);   
}

//make the buggy reverse while also turning left
void MReverseLeft() {
  WriteRightMotor(FORWARD, buggySpeed, BRAKE_OFF);
  WriteLeftMotor(REVERSE, buggySpeed/2, BRAKE_OFF); 
}

//set the right motor pins to move the motor or stop it
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

//set the left motor pins to move the motor or stop it
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
