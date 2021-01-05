Running The Server
==================
1. Open the server in Visual Studio.
2. Click Start to run the code and then click "Start Server" button in the application to run the server. To stop the server from running press Shift+F5.

Running The Controller Client GUI
=================================
1. Open the Controller Client in Visual Studio.
2. Click Start to run the code.
	a. To connect to the server, input the server's ip address and port into the 'IP Address' and 'Port' fields respectively and then click the 'Connect' button.
	b. To stop the application press Shift+F5.

Running the Buggy
=================
1. Upload the ESP32 sketch and the Arduino sketch to the ESP32 and Arduino Mega respectively.
2. When uploading the sketch to ESP32, the button to the right of the usb port needs to be pressed to allow new sketch to be uploaded. ESP32 may need resetting after the sketch is uploaded.
3. The ESP32 will automatically attempt to connect to the network and then to the server. If the ESP32 is unable to make a connection with the network, it may need to be reset.

Connecting Controller Client to a buggy
======================================= 
1.The buggy and the controller client need to be connected to the server first.
2.Once both are connected, select a buggy from the available clients list and click the 'Connect' button under 'Buggy Controls'


Setting Up the System On Another Network
========================================
1. Run the server.
2. in command prompt type: "netstat -ano". Find the local address that has been configured to listen on port 11000 as this is the port that the server is configured to work on.
3. in the ESP32 Arduino sketch, myWifi.c file contains the connection information. serverIP and serverPort are the variables ESP32 will use to connect to the server. the local address from the previous step is to be input into the serverIP variable and the port into serverPort.
4. The ESP32 also needs network ssid and password to connect to the network. the ssid of the network goes into the "ssid" variablees as a string and the password goes into the "password" field as a string.
5. The modified code will have to be re-uploaded onto the buggy in order for the changes to be applied.
6. The controller client connects to the local IP of the server and port obtained from step 2.
7. Once all the above steps have been taken, the system can be used on the network.