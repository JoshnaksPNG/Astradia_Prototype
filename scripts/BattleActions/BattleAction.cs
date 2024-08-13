using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

[GlobalClass]
public abstract partial class BattleAction : Resource
{
    protected TestCombatant Source;

    protected Array<TestCombatant> Targets;
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

    public BattleAction(TestCombatant source, Array<TestCombatant> targets)
    {
        this.Source = source;

        this.Targets = new();

        foreach (var target in targets)
        {
            this.Targets.Add(target);
        }

        MagicTypes = new List<Magictype>();
    }

    public void _SetTargeting(Array<TestCombatant> targs)
    {
        Targets = new();

        foreach (var target in targs)
        {
            Targets.Add(target);
        }
    }

    public void _AddTarget(TestCombatant t)
    {
        Targets.Add(t);
    }

    public void _SetSource(TestCombatant s)
    {
        Source = s;
    }

    public abstract void _ExecuteAction();

    public void _ExecuteOnTargets(TestCombatant source, Array<TestCombatant> targs)
    {
        _SetSource(source);

        _SetTargeting(targs);

        _ExecuteAction();
    }
}
