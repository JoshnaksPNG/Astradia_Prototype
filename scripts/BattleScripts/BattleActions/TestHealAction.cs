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
        DoesTargetSelf = true;
    }

    public override async void _ExecuteAction()
    {
        Source._PlayAnimation("attack");
        foreach (var target in Targets) 
        {
            target._Heal(10);
            target._PlayAnimation("heal");
        }
        await ToSignal(Source.GetTree().CreateTimer(0.6d), SceneTreeTimer.SignalName.Timeout);
        Source._PlayAnimation("RESET");
    }
}
