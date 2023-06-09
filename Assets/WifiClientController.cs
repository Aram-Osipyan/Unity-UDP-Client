using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class WifiClientController : MonoBehaviour
{
    [SerializeField] private Text _serverConnectedText;
    private string _serverConnectedTextValue = "Not Connected";
    
    private IPEndPoint _serverEndPoint = null;

    [SerializeField] private Button _sendButton;

    [SerializeField] private InputField _inputField;

    [SerializeField] private Text _fromServerText;

    private string _fromServerTextValue;
    // Start is called before the first frame update
    void Start()
    {
        var t = new Thread(BroadcastLoopListener) {IsBackground = true};
        t.Start();
        _sendButton.onClick.AddListener(SendToServer);
    }

    private void SendToServer()
    {
        Debug.Log("SendToServer");
        var udpClient = new UdpClient();
        byte[] bytes = Encoding.UTF8.GetBytes($"client: {_inputField.text}");
        _serverEndPoint.Port = 2015;
        Debug.Log("End SendToServer "+udpClient.Send(bytes, bytes.Length, _serverEndPoint));
    }
    private void BroadcastLoopListener()
    {
        var udpServer = new UdpClient(new IPEndPoint(IPAddress.Any, 2016));
        while (true)
        {
            IPEndPoint remoteEndPoint = null;
            var receivedNotification = udpServer.Receive(ref remoteEndPoint);
            
            if (receivedNotification != null && receivedNotification.Length > 0)
            {
                string result = Encoding.UTF8.GetString(receivedNotification);
                _serverConnectedTextValue = $"Connected\nEcho Server: {remoteEndPoint.Address}";
                _serverEndPoint = remoteEndPoint;
                if (result.Contains("echo"))
                {
                    _fromServerTextValue = result;
                }
                Debug.Log(remoteEndPoint.Address.ToString() + " message: " + result);
            }
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        _serverConnectedText.text = _serverConnectedTextValue;
        _fromServerText.text = _fromServerTextValue;
    }
}
