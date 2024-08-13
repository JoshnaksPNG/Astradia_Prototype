using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class TestPlayerGroup : Node2D
{
    public Array<Node> PartyMembers;

    const int MemberOffset = 160;
    const int OffsetFromTop = 60;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        PartyMembers = GetChildren();

        for (int i = 0; i < PartyMembers.Count; i++)
        {
            ((Node2D)PartyMembers[i]).Position = new Vector2(0, i * MemberOffset + OffsetFromTop);
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void _on_test_enemy_group_next_player()
    {
    }

    public void _SwitchFocus(int a, int b)
    {
        ((TestCombatant)PartyMembers[a])._Focus();
        ((TestCombatant)PartyMembers[b])._Unfocus();
    }

    public void _UpdateChildren()
    {
        PartyMembers = GetChildren();
    }

    public void _SwapChild(TestCombatant current, TestCombatant replacement)
    {
        int IDX = PartyMembers.IndexOf(current);
        
        if (IDX == -1)
        {
            throw new Exception("\"current\" not found in Partymembers");
        }
        else 
        {
            PartyMembers[IDX] = replacement;
        }
    }
}
