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
dataBME ReadBME280Data() {
   float temperature, humidity, pressure;
   dataBME bmeData;

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
   
  return CompressBmeData(pressure, temperature, humidity);
}

dataBME CompressBmeData(float presure, float temperature, float humidity) {
  dataBME bmeData;

  //Convert temperature from float to integer.
  if(temperature > 0) {
    bmeData.temperature = (uint16_t)(temperature * 100);          //multiply by 100 to turn decimal values into integers.
    bmeData.temperature = (bmeData.temperature & (0x7FFF));       //Set bit 15 to 0 to indicate positive value
  }
  else {
    bmeData.temperature = (uint16_t)((temperature * -1) * 100);   //turn negative into positive and multiply by 100 to turn decimal values into integers.
    bmeData.temperature = (bmeData.temperature | (0x01 << 15));   //Set bit 15 to 1 to indicate negative value  
  }

  //Get humidity without the decimal values as they have little relevance.
  bmeData.humidity = (uint8_t)humidity;

  //Do nothing to pressure as we don't need it.

  return bmeData;
}

uint16_t ReadLDR() {
  return analogRead(LDR_PIN);
}
