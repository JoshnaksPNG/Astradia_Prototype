using Godot;
using System;
using System.Diagnostics;

public class TestBurn : BattleEffect
{
    TestCombatant Target;

    public TestBurn(int timer, TestCombatant target) : base(timer)
    {
        Target = target;
    }

    public override void _AddEffect()
    {

    }

    public override void _ExecuteEffect()
    {
        Target._TakeDamage(5);

        Debug.WriteLine("Test Burn");

        Timer -= 1;
    }

    public override void _RemoveEffect()
    {

    }
}
