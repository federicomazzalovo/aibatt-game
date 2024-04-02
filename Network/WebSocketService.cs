using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class WebSocketService : GodotObject
{
    private WebSocketPeer client;
    private static WebSocketService instance;

    public event EventHandler<object> ConnectionClosedEvent = delegate { };
    public event EventHandler<object> ConnectionEnstablishedEvent = delegate { };
    public event EventHandler<object> DataReceivedEvent = delegate { };

    private WebSocketService()
    {
        this.client = new WebSocketPeer();
    }

    public Error Connect(string endpoint)
    {
        this.client.Connect("connection_closed", new Callable(this, "OnConnectionClosed"));
        this.client.Connect("connection_error", new Callable(this, "OnConnectionClosed"));
        this.client.Connect("connection_established", new Callable(this, "OnConnectedEnstablished"));
        this.client.Connect("data_received", new Callable(this, "OnDataReceived"));

        return this.client.ConnectToUrl($"ws://localhost:8080/" + endpoint);
    }

    public static WebSocketService GetInstance()
    {
        if(instance == null)
            instance = new WebSocketService();

        return instance;
    }

    public void Poll()
    {
        this.client.Poll();
    }

    public void SendMessage(string message)
    {
        this.client.PutPacket(Encoding.UTF8.GetBytes(message));
    }

    private void OnConnectionClosed(bool isClose)
    {
        GD.Print($"Client closed:  {isClose}");
        ConnectionClosedEvent(this, "aaa");
    }

    private void OnConnectedEnstablished(string proto = "")
    {
        //GD.Print($"Client {id} connected with protocol: {proto}");
        this.ConnectionEnstablishedEvent(this, "aaa");
    }

    private void OnDataReceived()
    {
        byte[] bytes = this.client.GetPacket();
        string result = Encoding.UTF8.GetString(bytes, 0, bytes.Length);

        this.DataReceivedEvent(this, result);
    }
}
