using Godot;
using Godot.Collections;
using System;

public partial class CombatantGroup : Node2D
{
    public Array<Combatant> Combatants;

    [Export]
    int OffsetFromTop = 460;

    [Export]
    int CombatantOffset = 270;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _GetCombatantsFromChildren();

        _PositionCombatants();
    }

    public void _AddCombatant(Combatant c)
    {
        Combatants.Add(c);

        AddChild(c);
    }

    public void _PositionCombatants()
    {
        for (int i = 0; i < Combatants.Count; i++)
        {
            Combatants[i].Position = new Vector2(0, i * CombatantOffset + OffsetFromTop);
        }
    }

    public void _GetCombatantsFromChildren()
    {
        Combatants = new();

        foreach (Node2D node in GetChildren())
        {
            Combatants.Add((Combatant)node);
        }
    }

    public void _SwitchFocus(int focus_in, int focus_out)
    {
        Combatants[focus_in]._Focus();
        Combatants[focus_out]._Unfocus();
    }

    public static void _SwitchFocusByReference(Combatant focus_in, Combatant focus_out)
    {
        focus_in._Focus();
        focus_out._Unfocus();
    }

    public void _SwapChild(Combatant current, Combatant replacement)
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
