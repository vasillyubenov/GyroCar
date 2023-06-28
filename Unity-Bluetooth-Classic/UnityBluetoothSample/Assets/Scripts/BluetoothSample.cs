using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class BluetoothSample : MonoBehaviour
{
    SerialServiceManager serialReceiver;

    public string port = "COM3";
    public int baudrate = 115200;
    public GameObject player;
    [SerializeField] private float speedFactor = 15f;
    [SerializeField] private float distanceFromCamera = 500f;

    public string message { get; set; } = "0";

    void Start()
    {
        Camera mainCamera = Camera.main;

        // Calculate a position distanceFromCamera units straight ahead of the camera
        Vector3 positionInFrontOfCamera = mainCamera.transform.position + mainCamera.transform.forward * distanceFromCamera;

        // Set player's position to the calculated position
        player.transform.position = positionInFrontOfCamera;

        serialReceiver = new SerialServiceManager(port, baudrate);
        serialReceiver.callback = GetData;
    }

    [ContextMenu("Send")]
    public void Send()
    {
        serialReceiver.SendMessage(message);
    }

    private void GetData()
    {
        string data = serialReceiver.stringData;
        data = GetFirstJSON(data);
        Debug.Log("[Log]" + data);
        UpdatePlayer(JsonUtility.FromJson<SensorData>(data));
    }

    private void OnDisable()
    {
        if (serialReceiver != null)
        {
            serialReceiver.Dispose();
        }
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

        MainThreadDispatcher.RunOnMainThread(() => {
            player.transform.localRotation = Quaternion.Lerp(player.transform.localRotation, new Quaternion(x, y, z, w), Time.deltaTime * speedFactor);
        });
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
}
