using Godot;
using System;

public partial class TestHealAction : BattleAction
{
    public TestHealAction() : base()
    {
        Name = "Test Heal Action";
        ID = "battle_action.test_heal";
        Description = "Test Action for Heal";
        targetNum = 1;

        Duration = 0.5d;
    }

    public override void _ExecuteAction()
    {
        foreach(var target in Targets) 
        {
            target._Heal(10);
        }
    }
}
