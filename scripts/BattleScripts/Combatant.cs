using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class Combatant : CharacterBody2D
{
    Sprite2D FocusSprite;
    ProgressBar HealthBar;
    AnimationPlayer Animator;

    public List<BattleAction> Moveset;
    public CombatantStats Stats;

    public List<BattleEffect> Effects;

    public List<MagicWeakness> MagicWeaknesses;
    
    public Combatant SwapPartner;
    public bool HasPartner = false;

    public BattleAction SwitchAction;

    public List<Combatant> AllOpponents; 

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
        _SetChildrenNodes();

        if (Moveset == null)
        {
            Moveset = new();
        }

        if (Effects == null) 
        {
            Effects = new();
        }
        
        if (MagicWeaknesses == null) 
        {
            MagicWeaknesses = new();
        }

        if (AllOpponents == null) 
        {
            AllOpponents = new();
        }
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
            if (Moveset == null)
            {
                Moveset = new();
            }

            //Moveset = MovesetParser.ParseAndFetchActions(movesetJSON);
            List<BattleAction> fetched_actions = MovesetParser.ParseAndFetchActions(movesetJSON);
            foreach (var move in fetched_actions) 
            {
                Moveset.Add(move);
            }
        }
        catch (Exception e)
        {
            throw new Exception("Error Loading Moveset from JSON: \n" + e.Message);
        }
    }

    /** 
        <summary>
        Method to simply initialize a Combatant. 
        </summary>

        <param name="parent"> Node to add the combatant as a child to. </param>
        <param name="stats"> Stats to load to the combatant. </param>
        <param name="moveset"> Moveset to load to the combatant. </param>
        <param name="scenePath"> Resource path to the Scene resource of combatant. </param>

        <returns>Initiated Combatant</returns>
    **/
    public static Combatant _InitCombatant(Node parent, CombatantStats stats, Json moveset, string scenePath)
    {
        Combatant c = GD.Load<PackedScene>(scenePath).Instantiate<Combatant>();

        c._SetChildrenNodes();

        c._LoadMoveset(moveset);
        c._LoadStats(stats);

        c.MagicWeaknesses = new();

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
        Public method to damage Combatant.
        </summary>

        <param name="dmg"> Amount of health to damage the combatant by. </param>
    **/
    public void _TakeMagicDamage(int dmg, List<Magictype> magictypes)
    {
        float appliedDamage = dmg;

        foreach(var weakness in MagicWeaknesses) 
        {
            if (magictypes.Contains(weakness.Type))
            {
                appliedDamage *= weakness.DamageMultiplier;

                weakness._OnExploited(this);
            }
        }

        CurrentHealth -= (int)appliedDamage;
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

    /** 
        <summary>
        Initiate the Opponent List for Combatant.
        </summary>

        <param name="oppVanguard"> Node containing the Opposing Vanguard. </param>
        <param name="oppRearguard"> Node containing the Opposing Rearguard. </param>
    **/
    public void _InitOpponents(Node2D oppVanguard, Node2D oppRearguard)
    {
        if (AllOpponents == null)
        {
            AllOpponents = new();
        }

        foreach (var child in oppVanguard.GetChildren())
        {
            AllOpponents.Add((Combatant)child);
        }

        foreach (var child in oppRearguard.GetChildren())
        {
            AllOpponents.Add((Combatant)child);
        }
    }

    /** 
        <summary>
        Add Opponent to Combatant Opponent List
        </summary>

        <param name="opp"> Combatant to add as opponent </param>
    **/
    public void _AddOpponent(Combatant opp) 
    {
        AllOpponents.Add(opp);
    }

    // Protected Methods
    protected void _UpdateHeathBar()
    {
        HealthBar.Value = CurrentHealth;
    }

    protected void _PlayHurtAnimation()
    {
        Animator.Play("hurt");
    }

    protected void _SetChildrenNodes()
    {
        FocusSprite = GetNode<Sprite2D>("Focus");
        HealthBar = GetNode<ProgressBar>("HealthBar");
        Animator = GetNode<AnimationPlayer>("AnimationPlayer");
    }
}
