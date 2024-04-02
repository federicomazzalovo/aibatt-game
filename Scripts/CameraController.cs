using Godot;
using System;

public partial class CameraController : Node3D
{
	private Node3D player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetTree().GetNodesInGroup("Player")[0] as Node3D;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		this.GlobalPosition = this.player.GlobalPosition;
	}
}
