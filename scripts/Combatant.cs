using Godot;
using System;
using System.Collections.Generic;

public partial class Combatant : CharacterBody2D
{
    Sprite2D FocusSprite;
    ProgressBar HealthBar;
    AnimationPlayer Animator;

    public List<BattleAction> Moveset;
    public CombatantStats Stats;

    public List<BattleEffect> Effects;
    
    public Combatant SwapPartner;
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

    public bool CanAct = true;

    public override void _Ready()
    {
        Agro = 0;

        HasPartner = false;

        Effects = new();
    }

    /** 
        <summary>
        Load a stats object to the combatant.
        </summary>

        <param name="stats"> Stats to load to the combatant. </param>
    **/
    public void _LoadStats(CombatantStats stats)
    {
        Stats = stats;

        CurrentHealth = Stats.Health;
        HealthBar.MaxValue = Stats.Health;
    }

    /** 
        <summary>
        Load a moveset to a combatant.
     
        Takes a JSON object, formatted for a moveset.
        </summary>
    
        <param name="movesetJSON"> Moveset to load to the combatant. </param>
    **/
    public void _LoadMoveset(Json movesetJSON)
    {
        try
        {
            Moveset = MovesetParser.ParseAndFetchActions(movesetJSON);
        }
        catch (Exception e)
        {
            throw new Exception("Error Loading Moveset from JSON: \n" + e.Message);
        }
    }

    /** 
        <summary>
        Method to simply initialize a Combatant. 
        Returns Combatant in case caller wants to perform more functions on it.
        </summary>

        <param name="parent"> Node to add the combatant as a child to. </param>
        <param name="stats"> Stats to load to the combatant. </param>
        <param name="moveset"> Moveset to load to the combatant. </param>
        <param name="scenePath"> Resource path to the Scene resource of combatant. </param>
    **/
    public static Combatant _InitCombatant(Node parent, CombatantStats stats, Json moveset, string scenePath)
    {
        Combatant c = GD.Load<PackedScene>(scenePath).Instantiate<Combatant>();

        c.FocusSprite = c.GetNode<Sprite2D>("Focus");
        c.HealthBar = c.GetNode<ProgressBar>("HealthBar");
        c.Animator = c.GetNode<AnimationPlayer>("AnimationPlayer");

        c._LoadMoveset(moveset);
        c._LoadStats(stats);

        parent.AddChild(c);

        return c;
    }

    /** 
        <summary>
        Show the Focus Sprite on the combatant.
        </summary>
    **/
    public void _Focus()
    {
        FocusSprite.Visible = true;
    }

    /** 
        <summary>
        Hide the Focus Sprite on the combatant.
        </summary>
    **/
    public void _Unfocus()
    {
        FocusSprite.Visible = false;
    }

    /** 
        <summary>
        Public method to damage Combatant.
        </summary>

        <param name="dmg"> Amount of health to damage the combatant by. </param>
    **/
    public void _TakeDamage(int dmg)
    {
        CurrentHealth -= dmg;
    }

    /** 
        <summary>
        Public method to heal Combatant.
        </summary>

        <param name="dmg"> Amount of health to heal the combatant by. </param>
    **/
    public void _Heal(int dmg)
    {
        CurrentHealth += dmg;

        if (CurrentHealth > Stats.Health)
        {
            CurrentHealth = Stats.Health;
        }
    }

    /** 
        <summary>
        Play an animation from the combatant's animator.
        </summary>

        <param name="animationName"> Name of the animation to play. </param>
    **/
    public void _PlayAnimation(string animationName)
    {
        Animator.Play(animationName);
    }

    // Private Methods

    private void _UpdateHeathBar()
    {
        HealthBar.Value = CurrentHealth;
    }

    private void _PlayHurtAnimation()
    {
        Animator.Play("hurt");
    }
}
