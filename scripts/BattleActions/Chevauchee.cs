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

    public override void _ExecuteAction()
    {
        foreach (var target in Targets)
        {
            target.Effects.Add(new TestBurn(3, target));

            target._TakeDamage(30);
        }
    }
}
