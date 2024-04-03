using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyFlyff.Session;

public partial class WebSocketService : GodotObject
{
    private WebSocketPeer client;
    private static WebSocketService instance;

    private WebSocketService()
    {
        this.client = new WebSocketPeer();
    }

    public Error Connect(string endpoint)
    {
        return this.client.ConnectToUrl($"{Configurations.WS_URL}/" + endpoint);
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

    public WebSocketPeer.State GetReadyState()
    {
        return this.client.GetReadyState();
    }

    public int GetAvailablePacketCount()
    {
        return this.client.GetAvailablePacketCount();
    }

    public string GetPacket()
    {
        var data = this.client.GetPacket();
        var value = Encoding.UTF8.GetString(data, 0, data.Length);
        GD.Print($"Received message: {value}");
        return value;
    }

    public int GetCloseCode()
    {
        return this.client.GetCloseCode();
    }

    public string GetCloseReason()
    {
        return this.client.GetCloseReason();
    }

    public void SendMessage(string message)
    {
        GD.Print($"Sending message: {message}");
        GD.Print(message.ToUtf8Buffer());
        this.client.PutPacket(message.ToUTF8());
    }
}
