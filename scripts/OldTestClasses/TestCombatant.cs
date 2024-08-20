using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata;

public partial class TestCombatant : CharacterBody2D
{
	[Export]
	Sprite2D FocusSprite;

	[Export]
	ProgressBar HealthBar;

	[Export]
	AnimationPlayer Animator;

	[Export]
	Json MovesetJSON;
    public List<BattleAction> Moveset;

	public CombatantStats Stats;

	public List<BattleEffect> Effects;

	[Export]
	public TestCombatant SwapPartner;
	public bool HasPartner;

    public int Agro;

    public int current_health;
	public int CurrentHealth 
	{
		set 
		{ 
			if (value < current_health && value < Stats.Health)
			{
                _PlayHurtAnimation();
            }

            current_health = value;

            _UpdateHeathBar();
        }
		get
		{
			return current_health;
		}
	}

	private bool CanAct = true;

    public override void _Ready()
    {
        //Stats = new(500, 500, 500, 100);

        //CurrentHealth = Stats.Health;
		//HealthBar.MaxValue = Stats.Health;

		Agro = 0;

		try 
		{
            Moveset = MovesetParser.ParseAndFetchActions(MovesetJSON);
        } catch
		{
			throw new Exception("Combatant Missing Moveset JSON");
		}

		HasPartner = SwapPartner != null;

		Effects = new();
    }

    public override void _PhysicsProcess(double delta)
	{
		
	}

	private void _UpdateHeathBar()
	{
		HealthBar.Value = CurrentHealth;
	}

	private void _PlayHurtAnimation()
	{
		Animator.Play("hurt");
	}

	public void _Focus()
	{
		FocusSprite.Visible = true;
	}

    public void _Unfocus()
    {
        FocusSprite.Visible = false;
    }

	public void _TakeDamage(int dmg)
	{
		CurrentHealth -= dmg;
	}

	public void _Heal(int dmg)
	{
		CurrentHealth += dmg;

		if(CurrentHealth > Stats.Health) 
		{
			CurrentHealth = Stats.Health;
		}
	}

	public void _PlayAnimation(string s)
	{
		Animator.Play(s);
	}
}
