using Godot;
using System;
using Newtonsoft.Json;

public partial class Player : CharacterNode
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;
	private bool stopMessageSent = true;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	private Node3D camera;

	public override void Initialize(Character character)
	{
		base.Initialize(character);
		this.SyncCamera();
	}

	public override void _Ready()
	{
		camera = GetParent().GetNode<Node3D>("CameraController");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = this.Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;

		this.CalculateMovingDirection();

		// Get the input direction and handle the movement/deceleration.
		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_backwards");
		if (inputDir != Vector2.Zero)
		{
			// Rotates left or right
			var rotAmount = inputDir.X * ((float)delta * 3);
			this.Rotation = new Vector3(0, this.Rotation.Y - rotAmount, 0);
			this.SyncCamera();

			Vector3 direction = (this.Transform.Basis * new Vector3(0, 0, inputDir.Y)).Normalized();

			// Move forward or backward
			if (direction != Vector3.Zero)
			{
				GD.Print($"Direction: {direction}");
				velocity.X = direction.X * Speed;
				velocity.Z = direction.Z * Speed;
			}
			else
			{
				velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
				velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
			}

			this.Velocity = velocity;
			this.MoveAndSlide();

			this.stopMessageSent = false;
			this.UpdatePosition(rotAmount);
		}
		else
		{
			if (!this.stopMessageSent)
			{
				this.stopMessageSent = true;
				this.UpdatePosition(0);
			}
		}
	}

	private void SyncCamera()
	{
		this.camera.Rotation = new Vector3(this.camera.Rotation.X, Rotation.Y, camera.Rotation.Z);
	}

	private void CalculateMovingDirection()
	{
		if (Input.IsActionPressed("move_forward"))
			this.MoveDirection = MoveDirection.Up;
		else if (Input.IsActionPressed("move_backwards"))
			this.MoveDirection = MoveDirection.Down;
		else if (Input.IsActionPressed("move_left"))
			this.MoveDirection = MoveDirection.Left;
		else if (Input.IsActionPressed("move_right"))
			this.MoveDirection = MoveDirection.Right;
		else
			this.MoveDirection = MoveDirection.None;
	}

	private void UpdatePosition(float rotationAmount)
	{
		var webSocketParams = new WebSocketParams()
		{
			CharacterId = this.Character.Id,
			PositionX = this.Position.X,
			PositionY = this.Position.Y,
			PositionZ = this.Position.Z,
			RotationX = this.Rotation.X,
			RotationY = this.Rotation.Y,
			RotationZ = this.Rotation.Z,
			//		rotationAmount = rotationAmount,
			ActionType = (int)ActionType.Movement,
			MoveDirection = (int)this.MoveDirection,
			IsConnected = true
		};
		WebSocketService.GetInstance().SendMovement(webSocketParams);
	}

	public override void UpdateCharacter(WebSocketParams param)
	{
		if (this.Character.Hp > 0 && param.Hp == 0)
			this.Kill();

		if (param.Hp > 0 && !this.IsPhysicsProcessing())
			this.SetPhysicsProcess(true);

		this.MoveDirection = (MoveDirection)param.MoveDirection;
		this.Character.Position = new CharacterPosition(param.PositionX, param.PositionY, param.PositionZ);
		this.Character.Rotation = new CharacterRotation(param.RotationX, param.RotationY, param.RotationZ);
		this.Character.Hp = param.Hp;

		this.RenderLifeStatus();

		// update only if not moving to avoid posilag
		if (this.MoveDirection == MoveDirection.None)
		{
			this.Position = new Vector3(param.PositionX, param.PositionY, param.PositionZ);
			this.Rotation = new Vector3(param.RotationX, param.RotationY, param.RotationZ);
		}
	}
}
