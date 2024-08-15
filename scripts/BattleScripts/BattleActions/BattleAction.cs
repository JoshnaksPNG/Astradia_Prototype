using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[GlobalClass]
public abstract partial class BattleAction : Resource
{
    protected Combatant Source;

    protected Array<Combatant> Targets;
    public int targetNum;

    public string Name;
    public string Description;

    public string ID;

    public List<Magictype> MagicTypes;

    public List<BattleEffect> Effects;

    public double Duration;

    public BattleAction()
    {
        this.Targets = new();
        MagicTypes = new List<Magictype>();
    }

    public BattleAction(Combatant source, Array<Combatant> targets)
    {
        this.Source = source;

        this.Targets = new();

        foreach (var target in targets)
        {
            this.Targets.Add(target);
        }

        MagicTypes = new List<Magictype>();
    }

    /** 
        <summary>
        Set BattleAction targets. 
        </summary>

        <param name="targs"> Godot Array of targeted Combatants. </param>
    **/
    public void _SetTargeting(Array<Combatant> targs)
    {
        Targets = new();

        foreach (var target in targs)
        {
            Targets.Add(target);
        }
    }

    /** 
        <summary>
        Add Combatant to BattleAction targets. 
        </summary>

        <param name="target"> Combatant to be added to target array. </param>
    **/
    public void _AddTarget(Combatant target)
    {
        Targets.Add(target);
    }

    /** 
        <summary>
        Set Combatant as BattleAction source. 
        </summary>

        <param name="source"> Combatant to be set as action source. </param>
    **/
    public void _SetSource(Combatant source)
    {
        Source = source;
    }

    /** 
        <summary>
        Execute the given action on all targets. 
        </summary>
    **/
    public abstract void _ExecuteAction();

    /** 
        <summary>
        Set Action source and targets, followed by executing the action. 
        </summary>

        <param name="source"> Combatant to be set as action source. </param>
        <param name="targs"> Godot Array of targeted Combatants. </param>
    **/
    public void _ExecuteOnTargets(Combatant source, Array<Combatant> targs)
    {
        _SetSource(source);

        _SetTargeting(targs);

        _ExecuteAction();
    }
}
