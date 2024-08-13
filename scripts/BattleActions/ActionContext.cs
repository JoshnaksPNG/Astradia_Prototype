using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public class ActionContext
{
    TestCombatant Source;
    Array<TestCombatant> Targets;
    public BattleAction Action;

    public ActionContext(TestCombatant source, Array<TestCombatant> targets, BattleAction action)
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
        //Debug.WriteLine(Action.Name);

        Action._SetSource(Source);
        Action._SetTargeting(Targets);

        Action._ExecuteAction();
    }
}
