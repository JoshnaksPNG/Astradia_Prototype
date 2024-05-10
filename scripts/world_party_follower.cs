using Godot;
using System;

public partial class world_party_follower : CharacterBody2D
{
	public const float Speed = 100.0f;

	[Export]
	NavigationAgent2D navAgent;

	[Export]
	CharacterBody2D partyHead;

	[Export]
	Node2D partyNode;

	[Export]
	int partyIndex;

	private enum charFacing 
	{
		Down,
		Up,
		Left,
		Right,
	}

	private charFacing facing;

	private bool shouldMove = false;

	public override void _Ready()
	{
		navAgent = GetNode<NavigationAgent2D>("./NavAgent");
		partyHead = GetNode<CharacterBody2D>("../../PartyHead");
		facing = charFacing.Down;
	}

	public override void _PhysicsProcess(double delta)
	{
		if(!shouldMove) 
		{
			//return;
		}

		if (partyIndex == 0)
		{
			navAgent.TargetPosition = partyHead.Position;
		}
		else
		{
			navAgent.TargetPosition = partyNode.GetChild<Node2D>(partyIndex - 1).Position;
		}

		

		if (navAgent.IsTargetReached() && partyIndex == 0)
		{
			GD.Print("test");
			Position = partyHead.Position;
		}
		else if(partyIndex == 0 || Position.DistanceTo(navAgent.TargetPosition) > 20)
		{
			Vector2 direction = navAgent.GetNextPathPosition() - GlobalPosition;

			//if(direction.)

			Velocity = direction.Normalized() * Speed;

			MoveAndSlide();
		}

		shouldMove = false;
	}

	public override void _Process(double delta)
	{

		base._Process(delta);
	}

	public void _on_party_head_party_moving()
	{
		shouldMove = true;
	}

}
