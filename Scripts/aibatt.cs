using Godot;
using System;

public partial class aibatt : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	private Node3D camera;

	public override void _Ready()
	{
		camera = GetParent().GetNode<Node3D>("CameraController");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_backwards");
		Vector3 direction = (Transform.Basis * new Vector3(0, 0, inputDir.Y)).Normalized();

		// Rotates left or right
		Rotation = new Vector3(0, Rotation.Y - (inputDir.X * ((float)delta * 3)), 0);
		camera.Rotation = new Vector3(camera.Rotation.X, Rotation.Y, camera.Rotation.Z);

		// Move forward or backward
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
