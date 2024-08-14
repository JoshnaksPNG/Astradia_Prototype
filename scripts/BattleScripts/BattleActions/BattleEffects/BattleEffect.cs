using Godot;
using System;

public abstract class BattleEffect
{
    public int Timer;


    public BattleEffect(int timer) 
    {
        Timer = timer;
    }

    public abstract void _AddEffect();

    public abstract void _ExecuteEffect();

    public abstract void _RemoveEffect();
}
