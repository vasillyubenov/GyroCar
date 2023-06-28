using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class BluetoothSample : MonoBehaviour
{
    SerialServiceManager serialReceiver;

    public string port = "COM3";
    public int baudrate = 115200;
    public PlayerMovement player;
    [SerializeField] private float speedFactor = 15f;

    public string message { get; set; } = "0";

    void Start()
    {
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
            player.rotationAngle = y;
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
