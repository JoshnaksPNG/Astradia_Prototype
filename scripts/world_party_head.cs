using Godot;

public partial class world_party_head : CharacterBody2D
{
	[Export]
	public  float Speed = 100.0f;

	[Signal]
	public delegate void PartyMovingEventHandler();

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;


		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

		if (!direction.IsEqualApprox(Vector2.Zero))
		{
			EmitSignal(SignalName.PartyMoving);
		}

		velocity = direction.Normalized() * Speed;

		Velocity = velocity;

		MoveAndSlide();
	}

	
}
