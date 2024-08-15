using Godot;
using System;
using System.Diagnostics;

public partial class TestMultiTargetAction : BattleAction
{
    public TestMultiTargetAction() : base()
    {
        Name = "Test Multiple Target Action";
        ID = "battle_action.test_multi_targ";
        Description = "Test Action for Multiple Targets";
        targetNum = 3;

        Duration = 1d;
    }

    public override async void _ExecuteAction()
    {
        Source._PlayAnimation("attack");
        foreach (var target in Targets)
        {
            target._TakeDamage(10);
        }
        await ToSignal(Source.GetTree().CreateTimer(0.6d), SceneTreeTimer.SignalName.Timeout);
        Source._PlayAnimation("RESET");
    }
}
