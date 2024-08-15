using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public class ActionContext
{
    Combatant Source;
    Array<Combatant> Targets;
    public BattleAction Action;

    public ActionContext(Combatant source, Array<Combatant> targets, BattleAction action)
    {
        Source = source;
        Targets = new();

        foreach (var target in targets)
        {
            Targets.Add(target);
        }

        Action = action;
    }

    public void _ExecuteAction()
    {
        Action._SetSource(Source);
        Action._SetTargeting(Targets);

        Action._ExecuteAction();
    }
}
