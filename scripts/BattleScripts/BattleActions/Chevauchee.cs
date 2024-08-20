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
            double hitChance = BattleMath.hit_chance(Source.Stats.Accuracy, target.Stats.Evasion);

            if (_ActionHits(hitChance))
            {
                double magic_multiplier = BattleMath.magic_multiplier(Source.Stats.MagicProficiency, MagicTypes);

                Debug.WriteLine($"Magic Multiplier: {magic_multiplier}");

                target.Effects.Add(new Burn(3, target));

                target._TakeMagicDamage((int)(30 * magic_multiplier), MagicTypes);
            }
        }

        await ToSignal(Source.GetTree().CreateTimer(0.6d), SceneTreeTimer.SignalName.Timeout);
        Source._PlayAnimation("RESET");
    }
}
