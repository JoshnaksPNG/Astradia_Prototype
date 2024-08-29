using Godot;
using System;
using System.Collections.Generic;

public class BattleEncounterInfo
{
    public string battlefieldScenePath;
    public List<CombatantInfo> partyVanguard;
    public List<CombatantInfo> partyRearguard; 
    public List<CombatantInfo> allyVanguard;
    public List<CombatantInfo> allyRearguard;
    public List<CombatantInfo> enemyVanguard;
    public List<CombatantInfo> enemyRearguard;

    public BattleEncounterInfo(string scenePath, List<CombatantInfo> partyV, List<CombatantInfo> partyR, List<CombatantInfo> allyV, List<CombatantInfo> allyR, List<CombatantInfo> enemyV, List<CombatantInfo> enemyR) 
    {
        battlefieldScenePath = scenePath;
        partyVanguard = partyV;
        partyRearguard = partyR;
        allyVanguard = allyV;
        allyRearguard = allyR;
        enemyVanguard = enemyV;
        enemyRearguard = enemyR;
    }

    public static BattleEncounterInfo InitInfoFromJSON(Json json)
    {
        //json.

        throw new NotImplementedException();
    }

    public static BattleEncounterInfo InitInfoFromJSON(string jsonString)
    {
        throw new NotImplementedException();
        //return InitInfoFromJSON()
    }
}
