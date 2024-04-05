using Godot;
using System;

public partial class Enemy : CharacterNode
{
	public const float Speed = 5.0f;

	private Vector3 currentPosition;
	private Vector3 currentRotation;
	public override void _Ready()
	{
		this.currentPosition = this.Position;

		this.currentRotation = this.Rotation;
	}

	public override void _PhysicsProcess(double delta)
	{
		GD.Print($"Enemy direction: {this.MoveDirection}");
		if (this.MoveDirection == MoveDirection.Right || this.MoveDirection == MoveDirection.Left)
		{
			var rotDirection = (this.MoveDirection == MoveDirection.Left) ? -1 : 1;
			var newRotation = new Vector3(0, this.Rotation.Y - (rotDirection * ((float)delta * 3)), 0);
			if (newRotation.Y > this.currentRotation.Y)
				this.Rotation = this.currentRotation;
			else
				this.Rotation = newRotation;
		}
		else
		{
			this.Rotation = this.currentRotation;
		}

		Vector3 velocity = this.Velocity;

		if (this.MoveDirection == MoveDirection.Up || this.MoveDirection == MoveDirection.Down)
		{
			var directionX = (this.MoveDirection == MoveDirection.Up) ? 1 : -1;
			velocity.X = directionX * Speed;

			var directionZ = (this.MoveDirection == MoveDirection.Down) ? 1 : -1;
			velocity.Z = directionZ * Speed;
		}
		else
		{
			this.Position = this.currentPosition;
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		this.Velocity = velocity;
		this.MoveAndSlide();
	}

	public override void UpdateCharacter(WebSocketParams param)
	{
		this.MoveDirection = (MoveDirection)param.moveDirection;
		this.currentPosition = new Vector3(param.positionX, param.positionY, param.positionZ);
		this.currentRotation = new Vector3(param.rotationX, param.rotationY, param.rotationZ);
	}
}
