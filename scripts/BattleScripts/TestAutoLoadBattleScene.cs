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
    int allyVanguardCount;

    [Export]
    int allyRearguardCount;

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

        // Ally Init
        for (int i = 0; i < allyVanguardCount; ++i)
        {
            int rand = r.Next(Movesets.Count);

            AiCombatant._InitCombatant(AllyVanguard, new(TestStats, TestProficiencies), Movesets[rand], "res://scenes/test_ai_combatant.tscn");
        }

        for (int i = 0; i < allyRearguardCount && i < allyVanguardCount; ++i)
        {
            int rand = r.Next(Movesets.Count);

            AiCombatant._InitCombatant(AllyRearguard, new(TestStats, TestProficiencies), Movesets[rand], "res://scenes/test_ai_combatant.tscn");
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

        // Enemy Opps and Friendlies
        foreach (var comb in EnemyVanguard.GetChildren())
        {
            ((Combatant)comb)._InitOpponents(PartyVanguard, PartyRearguard);
            ((AiCombatant)comb)._InitAggroTable(PartyVanguard, AllyVanguard);

            ((AiCombatant)comb)._AddAlliedVanguard(EnemyVanguard);
        }

        foreach (var comb in EnemyRearguard.GetChildren())
        {
            ((Combatant)comb)._InitOpponents(PartyVanguard, PartyRearguard);
            ((AiCombatant)comb)._InitAggroTable(PartyVanguard, AllyVanguard);

            ((AiCombatant)comb)._AddAlliedVanguard(EnemyVanguard);
        }

        // Ally Opps and Friendlies
        foreach (var comb in AllyVanguard.GetChildren())
        {
            ((Combatant)comb)._InitOpponents(EnemyVanguard, EnemyRearguard);
            ((AiCombatant)comb)._InitAggroTable(EnemyVanguard);

            ((AiCombatant)comb)._AddAlliedVanguard(AllyVanguard);
            ((AiCombatant)comb)._AddAlliedVanguard(PartyVanguard);
        }

        foreach (var comb in AllyRearguard.GetChildren())
        {
            ((Combatant)comb)._InitOpponents(EnemyVanguard, EnemyRearguard);
            ((AiCombatant)comb)._InitAggroTable(EnemyVanguard);

            ((AiCombatant)comb)._AddAlliedVanguard(AllyVanguard);
            ((AiCombatant)comb)._AddAlliedVanguard(PartyVanguard);
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

        AllyVanguard._GetCombatantsFromChildren();
        AllyVanguard._PositionCombatants();

        AllyRearguard._GetCombatantsFromChildren();
        AllyRearguard._PositionCombatants();

        PartySelectionIndex = 0;
        PartyVanguard.Combatants[PartySelectionIndex]._Focus();
    }
}
