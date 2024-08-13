using Godot;
using System;
using System.Diagnostics;

public partial class TestPartnerGroup : TestPlayerGroup
{
    public new void _SwapChild(TestCombatant current, TestCombatant replacement)
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
