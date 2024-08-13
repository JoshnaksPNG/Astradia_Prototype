using Godot;
using Godot.Collections;
using System;

public partial class CombatantGroup : Node2D
{
    public Array<TestCombatant> Combatants;

    [Export]
    int OffsetFromTop = 60;

    [Export]
    int CombatantOffset = 160;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _GetCombatantsFromChildren();

        _PositionCombatants();
    }

    public void _AddCombatant(TestCombatant c)
    {
        Combatants.Add(c);

        AddChild(c);
    }

    public void _PositionCombatants()
    {
        for (int i = 0; i < Combatants.Count; i++)
        {
            ((Node2D)Combatants[i]).Position = new Vector2(0, i * CombatantOffset + OffsetFromTop);
        }
    }

    public void _GetCombatantsFromChildren()
    {
        Combatants = new();

        foreach (Node2D node in GetChildren())
        {
            Combatants.Add((TestCombatant)node);
        }
    }

    public void _SwitchFocus(int a, int b)
    {
        ((TestCombatant)Combatants[a])._Focus();
        ((TestCombatant)Combatants[b])._Unfocus();
    }

    public void _SwapChild(TestCombatant current, TestCombatant replacement)
    {
        int IDX = Combatants.IndexOf(current);

        if (IDX == -1)
        {
            throw new Exception("\"current\" not found in Partymembers");
        }
        else
        {
            Combatants[IDX] = replacement;
        }
    }
}
