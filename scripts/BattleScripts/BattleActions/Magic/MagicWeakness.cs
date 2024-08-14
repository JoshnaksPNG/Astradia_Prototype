using Godot;
using System;

public abstract class MagicWeakness
{
    public readonly Magictype Type;

    public readonly float DamageMultiplier;

    public abstract void _OnExploited(Combatant combatant);
}
