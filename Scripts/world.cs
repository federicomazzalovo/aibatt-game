using Godot;
using System;

public partial class world : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.CreateTrees();
	}

	private void CreateTrees()
	{
		GD.Print(DateTime.Now + " - Creating trees");
		var treeNode = this.GetNode<Node3D>("Tree");
		
		var random = new Random();
		for (int i = 0; i < 100; i++)
		{
			var newTree = treeNode.Duplicate() as Node3D;
			newTree.Name = "Tree" + i;		
			newTree.Position = new Vector3(random.Next(-100, 100), treeNode.Position.Y, random.Next(-100, 100));
			newTree.Visible = true;
			this.AddChild(newTree);
		}

		GD.Print(DateTime.Now + " - Done creating trees");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
