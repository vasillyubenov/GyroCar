using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using TechTweaking.Bluetooth;
using UnityEngine.UI;
using System.Text;
using System.Text.RegularExpressions;
using System;

public class AutoConnect : MonoBehaviour
{
    public string TargetMAC = "C8:F0:9E:52:2F:9A";
	public Text statusText;
	public GameObject player;
	[SerializeField] private float speedFactor = 20f;
	[SerializeField] private float distanceFromCamera = 500f;
    private BluetoothDevice device;

    void Awake ()
	{
		device = new BluetoothDevice();

		if (BluetoothAdapter.isBluetoothEnabled()) {
			connect ();
		} else {

			//BluetoothAdapter.enableBluetooth(); //you can by this force enabling Bluetooth without asking the user
			statusText.text = "Status : Please enable your Bluetooth";

			BluetoothAdapter.OnBluetoothStateChanged += HandleOnBluetoothStateChanged;
			BluetoothAdapter.listenToBluetoothState (); // if you want to listen to the following two events  OnBluetoothOFF or OnBluetoothON
			BluetoothAdapter.askEnableBluetooth ();//Ask user to enable Bluetooth
		}
	}

	void Start()
	{
        Camera mainCamera = Camera.main;

        // Calculate a position distanceFromCamera units straight ahead of the camera
        Vector3 positionInFrontOfCamera = mainCamera.transform.position + mainCamera.transform.forward * distanceFromCamera;

        // Set player's position to the calculated position
        player.transform.position = positionInFrontOfCamera;

        BluetoothAdapter.OnDeviceOFF += HandleOnDeviceOff;//This would mean a failure in connection! the reason might be that your remote device is OFF

		BluetoothAdapter.OnDeviceNotFound += HandleOnDeviceNotFound; //Because connecting using the 'Name' property is just searching, the Plugin might not find it!(only for 'Name').
	}

	public void connect()
	{
		statusText.text = "Status : Trying To Connect";

        /* The Property device.MacAdress doesn't require pairing. 
		 * Also Mac Adress in this library is Case sensitive,  all chars must be capital letters
		 */
        device.MacAddress = TargetMAC;
		
		/* device.Name = "My_Device";
		* 
		* Trying to identefy a device by its name using the Property device.Name require the remote device to be paired
		* but you can try to alter the parameter 'allowDiscovery' of the Connect(int attempts, int time, bool allowDiscovery) method.
		* allowDiscovery will try to locate the unpaired device, but this is a heavy and undesirable feature, and connection will take a longer time
		*/

		/*
		 * The ManageConnection Coroutine will start when the device is ready for reading.
		 */
		device.ReadingCoroutine = ManageConnection;


        statusText.text = "Status : trying to connect";

        device.connect();

	}


	//############### Handlers/Recievers #####################
	void HandleOnBluetoothStateChanged(bool isBtEnabled)
	{
		if (isBtEnabled) {
			connect ();
			//We now don't need our recievers
			BluetoothAdapter.OnBluetoothStateChanged -= HandleOnBluetoothStateChanged;
			BluetoothAdapter.stopListenToBluetoothState ();
		}
	}

	//This would mean a failure in connection! the reason might be that your remote device is OFF
	void HandleOnDeviceOff (BluetoothDevice dev)
	{
		if (!string.IsNullOrEmpty (dev.Name)) {
			statusText.text = "Status : can't connect to '" + dev.Name + "', device is OFF ";
		} else if (!string.IsNullOrEmpty (dev.MacAddress)) {
			statusText.text = "Status : can't connect to '" + dev.MacAddress + "', device is OFF ";
		}
	}

	//Because connecting using the 'Name' property is just searching, the Plugin might not find it!.
	void HandleOnDeviceNotFound (BluetoothDevice dev)
	{
		if (!string.IsNullOrEmpty (dev.Name)) {
			statusText.text = "Status : Can't find a device with the name '" + dev.Name + "', device might be OFF or not paird yet ";
		} 
	}


    public void sendHigh()
    {
        if (device != null)
        {
            //byte[] bytes = Encoding.ASCII.GetBytes("1");
            //device.send(bytes);
            device.send(System.Text.Encoding.ASCII.GetBytes("1" + (char)10));//10 is our seperator Byte (sepration between packets)
        }
    }
    
    public void sendLow()
    {
        if (device != null)
        {
            //byte[] bytes = Encoding.ASCII.GetBytes("0");
            //device.send(bytes);
            device.send(System.Text.Encoding.ASCII.GetBytes("0" + (char)10));//10 is our seperator Byte (sepration between packets)
        }
    }


    public void disconnect()
	{
		if (device != null)
			device.close ();
	}
	
	//############### Reading Data  #####################
	//Please note that you don't have to use this Couroutienes/IEnumerator, you can just put your code in the Update() method.
	IEnumerator ManageConnection(BluetoothDevice device)
	{
		statusText.text = "Status : Connected & Can read " + device.MacAddress + "/" + device.Name;

		while (device.IsReading) 
		{
			byte[] msg = device.read();

			if (msg != null)
			{
                string content = System.Text.ASCIIEncoding.ASCII.GetString(msg);
				var firstJSON = GetFirstJSON(content);
				if (firstJSON != null)
				{
					statusText.text = "MSG : " + firstJSON;
					UpdatePlayer(JsonUtility.FromJson<SensorData>(firstJSON));
				}
            }

            yield return null;
        }

        statusText.text = "Status : Done Reading ";
    }

	string GetFirstJSON(string content)
	{
        string pattern = @"\{(.*?)\}";

        Match match = Regex.Match(content, pattern);

        if (match.Success)
        {
            return "{" + match.Groups[1].Value + "}";
        }

		return null;
    }

	void UpdatePlayer(SensorData data)
	{
		if (data == null)
		{
			return;
		}

        float w = float.Parse(data.angleW);
        float x = float.Parse(data.angleX);
        float y = float.Parse(data.angleY);
        float z = float.Parse(data.angleZ);

        player.transform.localRotation = Quaternion.Lerp(player.transform.localRotation, new Quaternion(x, y, z, w), Time.deltaTime * speedFactor);
    }
	
	//############### Deregister Events  #####################
	void OnDestroy ()
	{
		BluetoothAdapter.OnDeviceOFF -= HandleOnDeviceOff;
		BluetoothAdapter.OnDeviceNotFound -= HandleOnDeviceNotFound;
		
	}
	
}
