using Godot;
using System;

public abstract partial class CharacterNode : CharacterBody3D
{
	public Character Character { get; private set; }

	public void Initialize(Character character)
	{
		this.Position = new Vector3(character.Position.x, character.Position.y, character.Position.z);
	}

	public virtual void UpdateCharacter(WebSocketParams param)
	{
		if (this.Character.Hp > 0 && param.hp == 0)
			this.Kill();

		if(param.hp > 0 && !this.IsPhysicsProcessing())
			this.SetPhysicsProcess(true);

		this.Character.Position = new CharacterPosition(param.positionX, param.positionY, param.positionZ);
		this.Character.Hp = param.hp;

		this.RenderLifeStatus();

		MoveDirection moveDirection = (MoveDirection)param.moveDirection;
		if (moveDirection == MoveDirection.None)
			this.Position = new Vector3(param.positionX, param.positionY, param.positionZ);
	}

	private void Kill()
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
