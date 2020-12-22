
#define LDR_PIN   36


//Functions
void SetupBME280();
void ReadBME280Data(byte *byteArray);
void ReadLDR(byte *byteArray);
void SetupAutoDataSend();
void AutoEnvDataSend(void *parameter);
void SuspendAutoDataSend();
void ResumeAutoDataSend();
