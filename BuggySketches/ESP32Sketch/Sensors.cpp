#include <BME280I2C.h>
#include "CommProtocols.h"
#include "Sensors.h"
#include "RequestHandler.h"

BME280I2C bme280;
TaskHandle_t  AutoEnvDataSend_;
int dataExtractionPeriod = 1000;

//Setup the BME280 sensor using BME280I2C library code.
void SetupBME280() {
  while(!bme280.begin()) {
    Serial.println("Could not find BME280 sensor!");
    delay(1000);    
  }

  switch(bme280.chipModel())
  {
     case BME280::ChipModel_BME280:
       Serial.println("Found BME280 sensor! Success.");
       break;
     case BME280::ChipModel_BMP280:
       Serial.println("Found BMP280 sensor! No Humidity available.");
       break;
     default:
       Serial.println("Found UNKNOWN sensor! Error!");
  }
  
}

//Gets temperature, humidity and pressure from bme280
void ReadBME280Data(byte *byteArray) {
   float temperature, humidity, pressure;
   uint16_t temperature_16b, pressure_16b;
   uint8_t humidity_8b;
   byte *dataBytes;

   //Set what units the data is going to be extracted in and read the data from the sensor
   BME280::TempUnit tempUnit(BME280::TempUnit_Celsius);
   BME280::PresUnit presUnit(BME280::PresUnit_hPa);
   bme280.read(pressure, temperature, humidity, tempUnit, presUnit);

  //Convert temperature from float to integer to reduce the sizes.
  if(temperature > 0) {
    temperature_16b = (uint16_t)(temperature * 100);          //multiply by 100 to turn decimal values into integers.
    temperature_16b = (temperature_16b & (0x7FFF));       //Set bit 15 to 0 to indicate positive value
  }
  else {
    temperature_16b = (uint16_t)((temperature * -1) * 100);   //turn negative into positive and multiply by 100 to turn decimal values into integers.
    temperature_16b = (temperature_16b | (0x01 << 15));   //Set bit 15 to 1 to indicate negative value  
  }
   
  //Get humidity without the decimal values as they have little relevance.
  humidity_8b = (uint8_t)humidity;

  //Store the compressed BME280 data in the passed array
  dataBytes = (uint8_t*)&temperature_16b;   //Point dataBytes to first byte of temperature variable
  *byteArray =  dataBytes[0];     //Store first byte of temperature
  *(byteArray + 1) =  dataBytes[1];     //Store second byte of temperature
  dataBytes = (uint8_t*)&humidity_8b;      //Point dataBytes to humidity variable
  *(byteArray + 2) =  dataBytes[0];     //Store the humidity byte
  
  //Do nothing to pressure as we don't need it.
}



void ReadLDR(byte *byteArray) {
  uint16_t ldrVal;
  byte *dataBytes;

  //read the environmental light intensity values
  ldrVal = analogRead(LDR_PIN);

  //dataBytes now points to the first byte of the 16bit ldrVal
  dataBytes = (uint8_t*)&ldrVal;

  //save the ldr bytes into the byte array whose starting address has been pointed to.
  *byteArray = dataBytes[0];
  *(byteArray + 1) = dataBytes[1];
}


//Sets up a task for when autonomous mode is selected so that data can be sent by the buggy autonomously
void SetupAutoDataSend() {
    xTaskCreatePinnedToCore(
    AutoEnvDataSend,                  // Function that should be called
    "Autonomous Data Transmission",   // Name of the task (for debugging)
    10000,                            // Stack size (bytes)
    NULL,                             // Parameter to pass
    1,                                // Task priority
    &AutoEnvDataSend_,                // Task handle
    1
  );

  //susepend the task until autonomous mode is selected.
  vTaskSuspend(AutoEnvDataSend_);
}

//Function that runs when the task is active
void AutoEnvDataSend(void *parameter) {
  while(1) {
    //run the function that extracts the environmental data and sends it to the controller client
    SendEnvData();
    //after data has been sent pause the task for n ms dictated by dataExtractionPeriod
    vTaskDelay(pdMS_TO_TICKS(dataExtractionPeriod));
  }
}

//suspends the task that extracts environmental data automatically
void SuspendAutoDataSend() {
   vTaskSuspend(AutoEnvDataSend_);
}

//resumes the task that extracts environmental data automatically
void ResumeAutoDataSend() {
  vTaskResume(AutoEnvDataSend_);
}
