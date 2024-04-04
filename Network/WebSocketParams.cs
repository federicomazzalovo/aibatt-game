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

public class WebSocketParams
{
    public string sessionId { get; set; }
    public int characterId { get; set; }
    public float positionX { get; set; } 
    public float positionY { get; set; }
    public float positionZ { get; set; }
    public float rotationX { get; set; } 
    public float rotationY { get; set; }
    public float rotationZ { get; set; }
    public int moveDirection { get; set; }
    public double hp { get; set; }
    public double initHp { get; set; }
    public int level { get; set; }
    public int classId { get; set; }
    public bool isConnected { get; set; }
    public int targetId { get; set; }
    public int actionType { get; set; }
}
