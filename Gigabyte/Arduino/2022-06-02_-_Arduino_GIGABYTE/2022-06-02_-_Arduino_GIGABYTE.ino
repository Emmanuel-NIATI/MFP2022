// Le programme :

// 1. Les bibliothèques :
#include <ArduinoBLE.h>


// 2. Les variables globales :

BLEService ledService("19B10000-E8F2-537E-4F6C-D104768A1214");

BLEByteCharacteristic ledCharacteristic("19B10011-E8F2-537E-4F6C-D104768A1214", BLERead | BLEWrite);

const int led = 12;
const int fan1 = 11;
const int fan2 = 10;
const int fan3 = 9;


// 3. Les fonctions :

void printData(const unsigned char data[], int length)
{
  
  for (int i = 0; i < length; i++)
  {
    unsigned char b = data[i];

    if (b < 16)
    {

      Serial.print("0");

    }

    Serial.print(b, HEX);

  }

}

void blePeripheralConnectedHandler(BLEDevice central)
{
  // central connected event handler
  Serial.print("Connected event, central: ");
  Serial.println(central.address());
}

void blePeripheralDisconnectedHandler(BLEDevice central)
{
  // central disconnected event handler
  Serial.print("Disconnected event, central: ");
  Serial.println(central.address());
}

void ledCharacteristicWrittenHandler(BLEDevice central, BLECharacteristic characteristic)
{
  // central wrote new value to characteristic, update LED
  Serial.print("Characteristic event, written: ");

  if (characteristic.value())
  {
    
    if (characteristic.canRead())
    {

      // read the characteristic value
      characteristic.read();

      if (characteristic.valueLength() > 0)
      {
        
        // print out the value of the characteristic
        printData(characteristic.value(), characteristic.valueLength());
      }

    }

  }
  else
  {
    
  }

}


// 4. Le programme d'initialisation, il ne s'exécute qu'une fois :

void setup()
{

  Serial.begin(9600);

  while (!Serial);
  
  pinMode(led, OUTPUT);
  digitalWrite(led, LOW);

  // begin initialization
  if (!BLE.begin())
  {
    Serial.println("starting Bluetooth® Low Energy failed!");

    while (1);
  }

  BLE.setLocalName("AdvNano33IOT");
  BLE.setConnectable(true);
  BLE.setAdvertisedService(ledService);
  
  ledService.addCharacteristic(ledCharacteristic);
 
  BLE.addService(ledService);

  BLE.setAppearance(0x8000);

  // assign event handlers for connected, disconnected to peripheral
  BLE.setEventHandler(BLEConnected, blePeripheralConnectedHandler);
  BLE.setEventHandler(BLEDisconnected, blePeripheralDisconnectedHandler);

  // assign event handlers for characteristic
  ledCharacteristic.setEventHandler(BLEWritten, ledCharacteristicWrittenHandler);

  BLE.advertise();

  Serial.println("Bluetooth® device active, waiting for connections...");
  
}


// 5. Le programme principal, il s'exécute en boucle

void loop()
{

  // Arduino NANO 33 IoT is a peripheral scanned by a central
  BLEDevice central = BLE.central();

  if( central )
  {
      

  }

}
