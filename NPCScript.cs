using Godot;
using System;

public partial class NPCScript : Node2D
{
	[Export]
	Area2D InteractionArea;
	
	[Export]
	Node2D PartyHead;
	
	[Export]
	Godot.File Behavior;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(InteractionArea != null)
		{
			if(InteractionArea.OverlapsBody(PartyHead))
			{
				GD.Print("Interaction Area Reached");
			}
		}
	}
}
