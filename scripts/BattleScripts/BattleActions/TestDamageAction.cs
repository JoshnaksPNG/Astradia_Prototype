using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class TestDamageAction : BattleAction
{
    public TestDamageAction() : base() 
    {
        Name = "Test Damage Action";
        ID = "battle_action.test_damage";
        Description = "Test Action for Damage";
        targetNum = 1;

        Duration = 1d;
    }

    private TestDamageAction(Combatant source, Array<Combatant> targets) : base(source, targets)
    {
        Name = "Test Damage Action";
        ID = "battle_action.test_damage";
        Description = "Test Action for Damage";
        targetNum = 1;
    }

    public override async void _ExecuteAction()
    {
        Source._PlayAnimation("attack");
        foreach (var target in this.Targets) 
        {
            double hitChance = BattleMath.hit_chance(Source.Stats.Accuracy, target.Stats.Evasion);

            if (_ActionHits(hitChance))
            {
                target._TakeDamage(10);
            }
        }
        await ToSignal(Source.GetTree().CreateTimer(0.6d), SceneTreeTimer.SignalName.Timeout);
        Source._PlayAnimation("RESET");
    }

}
