using Godot;
using System;
using Newtonsoft.Json;

public partial class Enemy : CharacterNode
{
	public const float Speed = 5.0f;

	private Vector3 newPosition;
	private Vector3 newRotation;
	private float rotationAmount;
	private Vector3 oldRotation;
	private MoveDirection latestRotationDirection = MoveDirection.None;

	public override void Initialize(Character character)
	{
		base.Initialize(character);
		GD.Print("Enemy initialized");
		GD.Print("Rotation: " + this.Rotation);
	}

	public override void _Ready()
	{
		this.newPosition = this.Position;

		this.newRotation = this.Rotation;
	}

	public override void _PhysicsProcess(double delta)
	{
		this.Rotate(delta);

		this.CalculateVelocity();

		this.MoveAndSlide();
	}

	private void Rotate(double delta)
	{
		if (this.rotationAmount != 0)
		{
			this.Rotation = new Vector3(0, this.Rotation.Y - this.rotationAmount, 0);
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
			this.Position = this.newPosition;
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		this.Velocity = velocity;
	}

	public override void UpdateCharacter(WebSocketParams param)
	{
		if (param.rotationAmount != 0)
			GD.Print(JsonConvert.SerializeObject(param));
		this.MoveDirection = (MoveDirection)param.moveDirection;
		if (this.MoveDirection != MoveDirection.None && this.MoveDirection != MoveDirection.Up && this.MoveDirection != MoveDirection.Down)
			this.latestRotationDirection = this.MoveDirection;

		this.newPosition = new Vector3(param.positionX, param.positionY, param.positionZ);
		this.oldRotation = this.newRotation;
		this.newRotation = new Vector3(param.rotationX, param.rotationY, param.rotationZ);
		this.rotationAmount = param.rotationAmount;
	}
}
