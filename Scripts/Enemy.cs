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
		this.Rotate(delta);

		this.CalculateVelocity();

		this.MoveAndSlide();
	}

	private void Rotate(double delta)
	{
		if (this.MoveDirection == MoveDirection.Right || this.MoveDirection == MoveDirection.Left)
		{
			var rotDirection = (this.MoveDirection == MoveDirection.Left) ? -1 : 1;
			var newRotation = new Vector3(0, this.Rotation.Y - (rotDirection * ((float)delta * 3)), 0);
			this.Rotation = newRotation;
		}
		else
		{
			this.Rotation = this.currentRotation;
		}
	}


	private void CalculateVelocity()
	{
		Vector3 velocity = this.Velocity;

		if (this.MoveDirection == MoveDirection.Up || this.MoveDirection == MoveDirection.Down)
		{
			var movingForward = (this.MoveDirection == MoveDirection.Up) ? -1 : 1;
			var direction = (this.Transform.Basis * new Vector3(0, 0, movingForward)).Normalized();
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			this.Position = this.currentPosition;
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		this.Velocity = velocity;
	}

	public override void UpdateCharacter(WebSocketParams param)
	{
		this.MoveDirection = (MoveDirection)param.moveDirection;
		this.currentPosition = new Vector3(param.positionX, param.positionY, param.positionZ);
		this.currentRotation = new Vector3(param.rotationX, param.rotationY, param.rotationZ);
	}
}
