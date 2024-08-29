using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class LoadedBattleScene : BattleScene
{
    public static LoadedBattleScene InstantiateBattleScene(string battleScenePath, IList<CombatantInfo> partyVanguard, IList<CombatantInfo> partyRearguard, IList<CombatantInfo> allyVanguard, IList<CombatantInfo> allyRearguard, IList<CombatantInfo> enemyVanguard, IList<CombatantInfo> enemyRearguard)
    {
        LoadedBattleScene scene = GD.Load<PackedScene>(battleScenePath).Instantiate<LoadedBattleScene>();

        scene.InitiateAllCombatants(partyVanguard, partyRearguard, allyVanguard, allyRearguard, enemyVanguard, enemyRearguard);

        return scene;
    }

    public void InitiateAllCombatants(IList<CombatantInfo> partyVanguard, IList<CombatantInfo> partyRearguard, IList<CombatantInfo> allyVanguard, IList<CombatantInfo> allyRearguard, IList<CombatantInfo> enemyVanguard, IList<CombatantInfo> enemyRearguard)
    {
        // Party Init
        for (int i = 0; i < partyVanguard.Count; ++i)
        {
            CombatantInfo info = partyVanguard[i];
            Combatant._InitCombatant(PartyVanguard, new CombatantStats(info.StatsData, info.ProficiencyData), info.MovesetData, info.Path);
        }

        for (int i = 0; i < partyRearguard.Count && i < partyVanguard.Count; ++i)
        {
            CombatantInfo info = partyRearguard[i];
            Combatant._InitCombatant(PartyRearguard, new CombatantStats(info.StatsData, info.ProficiencyData), info.MovesetData, info.Path);
        }

        // Enemy Init
        for (int i = 0; i < enemyVanguard.Count; ++i)
        {
            CombatantInfo info = enemyVanguard[i];
            AiCombatant._InitCombatant(EnemyVanguard, new CombatantStats(info.StatsData, info.ProficiencyData), info.MovesetData, info.Path);
        }

        for (int i = 0; i < enemyRearguard.Count && i < enemyVanguard.Count; ++i)
        {
            CombatantInfo info = enemyRearguard[i];
            AiCombatant._InitCombatant(EnemyRearguard, new CombatantStats(info.StatsData, info.ProficiencyData), info.MovesetData, info.Path);
        }

        // Ally Init
        for (int i = 0; i < allyVanguard.Count; ++i)
        {
            CombatantInfo info = allyVanguard[i];
            AiCombatant._InitCombatant(AllyVanguard, new CombatantStats(info.StatsData, info.ProficiencyData), info.MovesetData, info.Path);
        }

        for (int i = 0; i < allyRearguard.Count && i < allyVanguard.Count; ++i)
        {
            CombatantInfo info = allyRearguard[i];
            AiCombatant._InitCombatant(AllyRearguard, new CombatantStats(info.StatsData, info.ProficiencyData), info.MovesetData, info.Path);
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
    }
}
