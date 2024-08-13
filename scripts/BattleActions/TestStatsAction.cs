using Godot;
using System;
using System.Diagnostics;

public partial class TestStatsAction : BattleAction
{
    public TestStatsAction() : base()
    {
        Name = "Test Stats Action";
        ID = "battle_action.test.stats_action";
        Description = "Test Action for Combatant Stats";
        targetNum = 1;

        Duration = 1d;
    }

    public override void _ExecuteAction()
    {
        foreach (var target in Targets)
        {
            float hitChance = ((float)Source.Stats.Accuracy / 1000);
            Debug.WriteLine("hitchance:" + hitChance);

            if (_ActionHits(hitChance))
            {
                target._TakeDamage(3 * (target.Stats.Accuracy / 100));
            }
        }
    }

    public bool _ActionHits(float hitChance)
    {
        Random r = new();

        return r.NextDouble() < (double) hitChance;
    }
}
