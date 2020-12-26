#include "Arduino.h"

#define RIGHT_DIR     12
#define LEFT_DIR      13
#define RIGHT_SPEED   3
#define LEFT_SPEED    11
#define RIGHT_BRAKE   9
#define LEFT_BRAKE    8

#define FORWARD       1
#define REVERSE       0
#define BRAKE_ON      1
#define BRAKE_OFF     0

#define SERVO_PIN     9

extern uint32_t buggySpeed;


void SetupMotor();
void MStop();
void MForward();
void MReverse();
void MRotateClockwise();
void MRotateAnitClockwise();
void MFordwardRight();
void MFordwardLeft();
void MReverseRight();
void MReverseLeft();
void WriteRightMotor(int dir, int spd, int brake);
void WriteLeftMotor(int dir, int spd, int brake);
