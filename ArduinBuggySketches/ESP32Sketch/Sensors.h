
#define LDR_PIN   15


struct dataBME {
  uint16_t temperature;
  uint8_t humidity;
  int pressure;
};

//Functions
void SetupBME280();
dataBME ReadBME280Data();
dataBME CompressBmeData(float presure, float temperature, float humidity);
uint16_t ReadLDR();
