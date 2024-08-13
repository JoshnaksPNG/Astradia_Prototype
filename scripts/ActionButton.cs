using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class ActionButton : Button
{
	public BattleAction AttachedAction;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Text = AttachedAction.Name;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
