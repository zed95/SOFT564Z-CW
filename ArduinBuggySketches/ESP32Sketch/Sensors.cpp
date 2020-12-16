#include <BME280I2C.h>
#include "CommProtocols.h"
#include "Sensors.h"

BME280I2C bme280;

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

   BME280::TempUnit tempUnit(BME280::TempUnit_Celsius);
   BME280::PresUnit presUnit(BME280::PresUnit_hPa);

   bme280.read(pressure, temperature, humidity, tempUnit, presUnit);

//   Serial.print("Temp: ");
//   Serial.print(temperature);
//   Serial.print("°"+ String(tempUnit == BME280::TempUnit_Celsius ? 'C' :'F'));
//   Serial.print("\t\tHumidity: ");
//   Serial.print(humidity);
//   Serial.print("% RH");
//   Serial.print("\t\tPressure: ");
//   Serial.print(pressure);
//   Serial.println("Pa");
//   delay(1000);


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
  
  ldrVal = analogRead(LDR_PIN);
  Serial.println(ldrVal);
  
  dataBytes = (uint8_t*)&ldrVal;
  *byteArray = dataBytes[0];
  *(byteArray + 1) = dataBytes[1];
  
  
}
