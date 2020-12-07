#include <BME280I2C.h>
#include "CommProtocols.h"

BME280I2C bme280;

void setupSensors(){
  setupI2C();
  
  
}


void setupBME280() {
  setupI2C();
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


//Change this to return the data
void getBME280Data() {
   float temperature, humidity, pressure;

   BME280::TempUnit tempUnit(BME280::TempUnit_Celsius);
   BME280::PresUnit presUnit(BME280::PresUnit_Pa);

   bme280.read(pressure, temperature, humidity, tempUnit, presUnit);

   Serial.print("Temp: ");
   Serial.print(temperature);
   Serial.print("°"+ String(tempUnit == BME280::TempUnit_Celsius ? 'C' :'F'));
   Serial.print("\t\tHumidity: ");
   Serial.print(humidity);
   Serial.print("% RH");
   Serial.print("\t\tPressure: ");
   Serial.print(pressure);
   Serial.println("Pa");

   delay(1000);

}
