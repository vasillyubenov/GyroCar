#include "I2Cdev.h"
#include "MPU6050_6Axis_MotionApps20.h"
#include "Wire.h"
#include <Arduino_JSON.h>
#include "BluetoothSerial.h"
#include <Arduino.h>

#if !defined(CONFIG_BT_ENABLED) || !defined(CONFIG_BLUEDROID_ENABLED)
#error Bluetooth is not enabled! Please run `make menuconfig` to and enable it
#endif

BluetoothSerial SerialBT;

// Json Variable to Hold Sensor Readings
JSONVar readings;

MPU6050 mpu;

// MPU control/status vars
bool dmpReady = false;  // set true if DMP init was successful
uint8_t devStatus;      // return status after each device operation (0 = success, !0 = error)
uint16_t packetSize;    // expected DMP packet size (default is 42 bytes)
uint16_t fifoCount;     // count of all bytes currently in FIFO
uint8_t fifoBuffer[64]; // FIFO storage buffer

// Previous acceleration
VectorInt16 prevAccel;
// Filter parameter
float alpha = 0.5;

// orientation/motion vars
Quaternion q;           // [w, x, y, z]         quaternion container
VectorInt16 aa;         // [x, y, z]            accel sensor measurements
VectorInt16 aaReal;     // [x, y, z]            gravity-free accel sensor measurements
VectorInt16 aaWorld;    // [x, y, z]            world-frame accel sensor measurements
VectorFloat gravity;    // [x, y, z]            gravity vector
VectorInt16 velocity; // [x, y, z] velocity vector
VectorInt16 position; // [x, y, z] position vector
float euler[3];         // [psi, theta, phi]    Euler angle container
float ypr[3];           // [yaw, pitch, roll]   yaw/pitch/roll container and gravity vector

void setup() {
  Wire.begin();
  Wire.setClock(400000); // 400kHz I2C clock. Comment this line if having compilation difficulties

  Serial.begin(115200);
  SerialBT.begin("ESP32_ForTest"); //Bluetooth device name
  Serial.println("The device started, now you can pair it with bluetooth!");
  while (!Serial); // wait for Leonardo enumeration, others continue immediately

  mpu.initialize();
  devStatus = mpu.dmpInitialize();
  mpu.setXGyroOffset(54); //++
  mpu.setYGyroOffset(-21); //--
  mpu.setZGyroOffset(5);

  if (devStatus == 0) {
    mpu.setDMPEnabled(true);
    // set our DMP Ready flag so the main loop() function knows it's okay to use it
    dmpReady = true;
    // get expected DMP packet size for later comparison
    packetSize = mpu.dmpGetFIFOPacketSize();
  } else {
    Serial.println("Error!");
  }
}

void loop() {
  if (!dmpReady) {
    delay(10);
    return;
  }

  int  mpuIntStatus = mpu.getIntStatus();
  fifoCount = mpu.getFIFOCount();

  if ((mpuIntStatus & 0x10) || fifoCount == 1024) { // check if overflow
    mpu.resetFIFO();
  } else if (mpuIntStatus & 0x02) {
    while (fifoCount < packetSize) fifoCount = mpu.getFIFOCount();

    mpu.getFIFOBytes(fifoBuffer, packetSize);
    fifoCount -= packetSize;

    String data = GetData().c_str();
    SerialBT.print(data);
    Serial.println(data);
  }
}

String GetData() {
  mpu.dmpGetQuaternion(&q, fifoBuffer);
  readings["angleX"] = String(q.x);
  readings["angleY"] = String(q.y);
  readings["angleZ"] = String(q.z);
  readings["angleW"] = String(q.w);

  return JSON.stringify(readings);
}