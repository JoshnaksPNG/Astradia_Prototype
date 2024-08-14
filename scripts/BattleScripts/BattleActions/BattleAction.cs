using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

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

    public void _SetTargeting(Array<Combatant> targs)
    {
        Targets = new();

        foreach (var target in targs)
        {
            Targets.Add(target);
        }
    }

    public void _AddTarget(Combatant t)
    {
        Targets.Add(t);
    }

    public void _SetSource(Combatant s)
    {
        Source = s;
    }

    public abstract void _ExecuteAction();

    public void _ExecuteOnTargets(Combatant source, Array<Combatant> targs)
    {
        _SetSource(source);

        _SetTargeting(targs);

        _ExecuteAction();
    }
}
