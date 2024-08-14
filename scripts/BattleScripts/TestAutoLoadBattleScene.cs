using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;

public partial class TestAutoLoadBattleScene : BattleScene
{
    [Export]
    int partyVanguardCount;

    [Export]
    int partyRearguardCount;

    [Export]
    int enemyVanguardCount;

    [Export]
    int enemyRearguardCount;

    [Export]
    Array<Json> Movesets;

    public override void _Ready()
    {
        base._Ready();

        Random r = new Random();

        // Party Stuff
        for(int i = 0; i < partyVanguardCount; ++i) 
        {
            int rand = r.Next(Movesets.Count);

            Combatant._InitCombatant(PartyVanguard, new CombatantStats(500, 500, 500, 100), Movesets[rand], "res://scenes/test_combatant.tscn");
        }

        for (int i = 0; i < partyRearguardCount && i < partyVanguardCount; ++i)
        {
            int rand = r.Next(Movesets.Count);

            Combatant._InitCombatant(PartyRearGuard, new CombatantStats(500, 500, 500, 100), Movesets[rand], "res://scenes/test_combatant.tscn");
        }

        // Enemy Stuff
        for (int i = 0; i < enemyVanguardCount; ++i)
        {
            int rand = r.Next(Movesets.Count);

            Combatant._InitCombatant(EnemyVanguard, new CombatantStats(500, 500, 500, 100), Movesets[rand], "res://scenes/test_combatant.tscn");
        }

        for (int i = 0; i < enemyRearguardCount && i < enemyVanguardCount; ++i)
        {
            int rand = r.Next(Movesets.Count);

            Combatant._InitCombatant(EnemyRearguard, new CombatantStats(500, 500, 500, 100), Movesets[rand], "res://scenes/test_combatant.tscn");
        }

        _UpdateChildrenPartners();

        PartyVanguard._GetCombatantsFromChildren();
        PartyVanguard._PositionCombatants();

        PartyRearGuard._GetCombatantsFromChildren();
        PartyRearGuard._PositionCombatants();

        EnemyVanguard._GetCombatantsFromChildren();
        EnemyVanguard._PositionCombatants();

        EnemyRearguard._GetCombatantsFromChildren();
        EnemyRearguard._PositionCombatants();


        PartySelectionIndex = 0;
        PartyVanguard.Combatants[PartySelectionIndex]._Focus();
    }
}
