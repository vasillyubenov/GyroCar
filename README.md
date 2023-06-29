Car Driving Simulator With Gyroscope

This project uses an ESP32 WROOM NodeMCU DevKit V1 along with an MPU6050 accelerometer to create a simulation of a driving a car with a steerinf wheel as your controller game. The data from the MPU6050 is used to track the movements of the steering wheel and send it to a Unity application on your smartphone via Bluetooth. The Unity application simulates the game of based on the movements received.
Prerequisites

    ESP32 WROOM NodeMCU DevKit V1
    MPU6050 Accelerometer
    Unity
    Arduino IDE

Setup
Hardware Connection

    Connect the MPU6050 to your ESP32.
        Connect VCC to 3.3V on the ESP32.
        Connect GND to GND.
        Connect SDA to GPIO21 (SDA).
        Connect SCL to GPIO22 (SCL).
        Connect INT to GPIO23 (INT).

    Ensure that your ESP32 board is connected to your computer via a USB cable.

Software Configuration

    Download and install the Arduino IDE from the official Arduino website.
    Follow the Arduino IDE setup instructions mentioned in the previous response to install the ESP32 add-on.
    Select the ESP32 DevKit V1 from the Board Manager.

MPU6050 Library

    Open the Arduino IDE and navigate to "Sketch" -> "Include Library" -> "Manage Libraries".
    In the library manager, search for "MPU6050" and install the library by Jeff Rowberg.

Unity Setup (Currently using Unity 2019.4.18f1 LTS or higher)

    Download and install Unity from the official Unity website.
    Open the Unity project that's included in this repository.

Code Upload

    Download the source code from this repository.
    Open the .ino file in the Arduino IDE.
    Press the upload button to compile and upload the code to your ESP32.

Bluetooth Pairing

    The Unity AutoConnect script has MAC Address that for now you have to manully add (TODO: Add NFC reading to open an with current MAC)
    Once the ESP32 is running, open your smartphone's Bluetooth settings.
    Look for the ESP32 and pair with it.
    Once paired, open the Unity application on your smartphone.

Running the Simulator

    Hold the MPU6050 in your hand (this will act as your table tennis racket).
    Open the Unity application on your smartphone (ensure Bluetooth is connected).
    Your movements with the MPU6050 will simulate the table steering wheel movements in the Unity application.
