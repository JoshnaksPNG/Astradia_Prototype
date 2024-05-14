using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class NPCScript : Node2D
{
	[Export]
	Area2D InteractionArea;
	
	[Export]
	Node2D PartyHead;
	
	[Export]
	Json Behavior;
	Dictionary<string, string> BehaviorDictionary;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BehaviorDictionary = Behavior.Data.AsGodotDictionary<string, string>();

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(InteractionArea != null)
		{
			if(InteractionArea.OverlapsBody(PartyHead))
			{
				if (Input.IsActionJustPressed("ui_interact"))
				{
					npc_interact();
				}
			}
		}
	}

	public void npc_interact()
	{
        Debug.WriteLine(BehaviorDictionary["on_interact"]);
    }
}
