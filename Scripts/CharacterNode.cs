using Godot;
using System;

public abstract partial class CharacterNode : CharacterBody3D
{
	public Character Character { get; private set; }
	protected MoveDirection MoveDirection = MoveDirection.None;

	public void Initialize(Character character)
	{
		this.Character = character;
		this.Position = new Vector3(character.Position.x, character.Position.y, character.Position.z);
		this.Rotation = new Vector3(character.Rotation.x, character.Rotation.y, character.Rotation.z);
	}

	public abstract void UpdateCharacter(WebSocketParams param);

	protected void Kill()
	{
		// this.animatedSprite.Play("death");
		this.SetPhysicsProcess(false);
		this.Killed();
	}

	public void RenderLifeStatus()
	{
		// this.HpLabel.Text = Character.Hp.ToString();
	}

	protected virtual void Killed() { }
}
