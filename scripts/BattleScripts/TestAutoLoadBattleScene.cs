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

    [Export]
    Json TestStats;
    [Export]
    Json TestProficiencies;


    public override void _Ready()
    {
        base._Ready();

        Random r = new Random();

        // Party Init
        for(int i = 0; i < partyVanguardCount; ++i) 
        {
            int rand = r.Next(Movesets.Count);

            Combatant._InitCombatant(PartyVanguard, new(TestStats, TestProficiencies), Movesets[rand], "res://scenes/test_combatant.tscn");
        }

        for (int i = 0; i < partyRearguardCount && i < partyVanguardCount; ++i)
        {
            int rand = r.Next(Movesets.Count);

            Combatant._InitCombatant(PartyRearguard, new(TestStats, TestProficiencies), Movesets[rand], "res://scenes/test_combatant.tscn");
        }

        // Enemy Init
        for (int i = 0; i < enemyVanguardCount; ++i)
        {
            int rand = r.Next(Movesets.Count);

            AiCombatant._InitCombatant(EnemyVanguard, new(TestStats, TestProficiencies), Movesets[rand], "res://scenes/test_ai_combatant.tscn");
        }

        for (int i = 0; i < enemyRearguardCount && i < enemyVanguardCount; ++i)
        {
            int rand = r.Next(Movesets.Count);

            AiCombatant._InitCombatant(EnemyRearguard, new(TestStats, TestProficiencies), Movesets[rand], "res://scenes/test_ai_combatant.tscn");
        }

        // Party Opps
        foreach (var comb in PartyVanguard.GetChildren())
        {
            ((Combatant)comb)._InitOpponents(EnemyVanguard, EnemyRearguard);
        }

        foreach (var comb in PartyRearguard.GetChildren())
        {
            ((Combatant)comb)._InitOpponents(EnemyVanguard, EnemyRearguard);
        }

        // Enemy Opps
        foreach (var comb in EnemyVanguard.GetChildren())
        {
            ((Combatant)comb)._InitOpponents(PartyVanguard, PartyRearguard);
            ((AiCombatant)comb)._InitAggroTable(PartyVanguard, AllyVanguard);
        }

        foreach (var comb in EnemyRearguard.GetChildren())
        {
            ((Combatant)comb)._InitOpponents(PartyVanguard, PartyRearguard);
            ((AiCombatant)comb)._InitAggroTable(PartyVanguard, AllyVanguard);
        }

        _UpdateChildrenPartners();

        PartyVanguard._GetCombatantsFromChildren();
        PartyVanguard._PositionCombatants();

        PartyRearguard._GetCombatantsFromChildren();
        PartyRearguard._PositionCombatants();

        EnemyVanguard._GetCombatantsFromChildren();
        EnemyVanguard._PositionCombatants();

        EnemyRearguard._GetCombatantsFromChildren();
        EnemyRearguard._PositionCombatants();


        PartySelectionIndex = 0;
        PartyVanguard.Combatants[PartySelectionIndex]._Focus();
    }
}
