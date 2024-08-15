using Godot;
using System;
using System.Diagnostics;

public partial class Chevauchee : BattleAction
{
    public Chevauchee() : base()
    {
        Name = "Chevauchee";
        ID = "battle_action.swordsman.chevauchee";
        Description = "A Heavy Attack that Burns Target on contact";
        targetNum = 1;

        Duration = 1d;

        MagicTypes.Add(Magictype.Fire);
    }

    public override async void _ExecuteAction()
    {
        Source._PlayAnimation("attack");
        foreach (var target in Targets)
        {
            target.Effects.Add(new Burn(3, target));

            target._TakeMagicDamage(30, MagicTypes);
        }

        await ToSignal(Source.GetTree().CreateTimer(0.6d), SceneTreeTimer.SignalName.Timeout);
        Source._PlayAnimation("RESET");
    }
}
