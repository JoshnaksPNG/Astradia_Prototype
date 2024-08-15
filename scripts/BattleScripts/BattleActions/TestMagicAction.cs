using Godot;
using System;
using System.Diagnostics;

public partial class TestMagicAction : BattleAction
{
    public TestMagicAction() : base()
    {
        Name = "Test Magic Action";
        ID = "battle_action.test_magic";
        Description = "Test Action for Magic";
        targetNum = 1;

        Duration = 1d;

        MagicTypes.Add(Magictype.Fire);
        MagicTypes.Add(Magictype.Wind);
    }

    public override async void _ExecuteAction()
    {
        Source._PlayAnimation("attack");
        foreach (var target in Targets)
        {
            target._TakeMagicDamage(5, MagicTypes);
        }
        await ToSignal(Source.GetTree().CreateTimer(0.6d), SceneTreeTimer.SignalName.Timeout);
        Source._PlayAnimation("RESET");
    }
}
