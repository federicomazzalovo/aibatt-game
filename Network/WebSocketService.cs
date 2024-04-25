using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyFlyff.Session;
using Newtonsoft.Json;

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
		var result = this.client.ConnectToUrl($"{Configurations.WS_URL}/" + endpoint);

		return result;
	}

	public static WebSocketService GetInstance()
	{
		if (instance == null)
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
		return Encoding.UTF8.GetString(data, 0, data.Length);
	}

	public int GetCloseCode()
	{
		return this.client.GetCloseCode();
	}

	public string GetCloseReason()
	{
		return this.client.GetCloseReason();
	}

	public void SendSignalRConnection()
	{
		string msg = JsonConvert.SerializeObject(new SignalRHandshake());
		this.SendMessage(msg);
	}

	public void SendHandShake(WebSocketHandshake handshake)
	{
		string msgToSend = JsonConvert.SerializeObject(handshake);

		SignalRMessage msg = new SignalRMessage();
		msg.target = "HandshakeConnect";
		msg.arguments = new List<string> { msgToSend };
		msg.type = 1;
		this.SendMessage(JsonConvert.SerializeObject(msg));
	}

	public void SendMovement(WebSocketParams webSocketParams)
	{
		string msgToSend = JsonConvert.SerializeObject(webSocketParams);
		SignalRMessage msg = new SignalRMessage();
		msg.target = "ReceiveMessage";
		msg.arguments = new List<string> { msgToSend };
		msg.type = 1;
		this.SendMessage(JsonConvert.SerializeObject(msg));
	}

	public void SendMessage(string message)
	{
		message += "";
		//GD.Print(message);
		this.client.Send(Encoding.UTF8.GetBytes(message), WebSocketPeer.WriteMode.Text);
	}
}
