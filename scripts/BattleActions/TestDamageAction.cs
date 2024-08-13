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

    private TestDamageAction(TestCombatant source, Array<TestCombatant> targets) : base(source, targets)
    {
        Name = "Test Damage Action";
        ID = "battle_action.test_damage";
        Description = "Test Action for Damage";
        targetNum = 1;
    }

    public override void _ExecuteAction()
    {
        foreach(var target in this.Targets) 
        {
            target._TakeDamage(10);
        }
    }

}
