using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum MoveDirection
{
	None = 0,
	Up = 1,
	Right = 2,
	Down = 3,
	Left = 4
}

public enum ActionType
{
	None = 0,
	Movement = 1,
	Attack = 2
}

public class SignalRHandshake
{
	public string protocol { get; set; } = "json";
	public int version { get; set; } = 1;
}

public class SignalRMessage
{
	public int type { get; set; }
	public string target { get; set; }
	public List<string> arguments { get; set; }
}

public class WebSocketParams
{
	public string SessionId { get; set; }
	public int CharacterId { get; set; }
	public float PositionX { get; set; }
	public float PositionY { get; set; }
	public float PositionZ { get; set; }
	public float RotationX { get; set; }
	public float RotationY { get; set; }
	public float RotationZ { get; set; }
	//	public float rotationAmount { get; set; }
	public int MoveDirection { get; set; }
	public double Hp { get; set; }
	public double InitHp { get; set; }
	public int Level { get; set; }
	public int ClassId { get; set; }
	public bool IsConnected { get; set; }
	public int TargetId { get; set; }
	public int ActionType { get; set; }
}
