using Godot;
using System;

public partial class world_party_follower : CharacterBody2D
{
	public const float Speed = 100.0f;

	[Export]
	NavigationAgent2D navAgent;

	[Export]
	CharacterBody2D partyHead;

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
			return;
		}

        navAgent.TargetPosition = partyHead.Position;

		if (navAgent.IsTargetReached())
		{
			Position = partyHead.Position;
		}
		else 
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
