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
			this.Rotation = new Vector3(0, this.Rotation.Y - (inputDir.X * ((float)delta * 3)), 0);
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
			this.UpdatePosition();
		}
		else
		{
			if (!this.stopMessageSent)
			{
				this.stopMessageSent = true;
				this.UpdatePosition();
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

	private void UpdatePosition()
	{
		string message = JsonConvert.SerializeObject(new WebSocketParams()
		{
			characterId = this.Character.Id,
			positionX = this.Position.X,
			positionY = this.Position.Y,
			positionZ = this.Position.Z,
			rotationX = this.Rotation.X,
			rotationY = this.Rotation.Y,
			rotationZ = this.Rotation.Z,
			actionType = (int)ActionType.Movement,
			moveDirection = (int)this.MoveDirection,
			isConnected = true
		});
		WebSocketService.GetInstance().SendMessage(message);
	}

	public override void UpdateCharacter(WebSocketParams param)
	{
		if (this.Character.Hp > 0 && param.hp == 0)
			this.Kill();

		if (param.hp > 0 && !this.IsPhysicsProcessing())
			this.SetPhysicsProcess(true);

		this.MoveDirection = (MoveDirection)param.moveDirection;
		this.Character.Position = new CharacterPosition(param.positionX, param.positionY, param.positionZ);
		this.Character.Rotation = new CharacterRotation(param.rotationX, param.rotationY, param.rotationZ);
		this.Character.Hp = param.hp;

		this.RenderLifeStatus();

		// update only if not moving to avoid posilag
		if (this.MoveDirection == MoveDirection.None)
		{
			this.Position = new Vector3(param.positionX, param.positionY, param.positionZ);
			this.Rotation = new Vector3(param.rotationX, param.rotationY, param.rotationZ);
		}
	}
}
