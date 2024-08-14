using Godot;
using System;
using System.Diagnostics;

public class Burn : BattleEffect
{
    Combatant Target;

    public Burn(int timer, Combatant target) : base(timer)
    {
        Target = target;
    }

    public override void _AddEffect()
    {

    }

    public override void _ExecuteEffect()
    {
        Target._TakeMagicDamage(5, new() { Magictype.Fire });

        Timer -= 1;
    }

    public override void _RemoveEffect()
    {

    }
}
