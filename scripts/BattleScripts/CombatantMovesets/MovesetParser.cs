using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public static class MovesetParser
{
    public static List<BattleAction> allActions = new()
    {
        new TestDamageAction(),
        new TestHealAction(),
        new TestMagicAction(),
        new TestMultiTargetAction(),
        new Chevauchee(),
        new TestStatsAction(),
    };

    public static List<BattleAction> FetchActions(List<string> actionNames)
    {
        List<BattleAction> actionQuery =
            (from action in allActions
            where actionNames.Contains(action.ID)
            select action).ToList();

        return actionQuery;
    }

    public static List<string> ParseActionsJSON(Json json) 
    {
        Godot.Collections.Dictionary<string, string[]> a = json.Data.AsGodotDictionary<string, string[]>();

        return a["moves"].ToList();
    }

    public static List<BattleAction> ParseAndFetchActions(Json json)
    {
        return FetchActions(ParseActionsJSON(json));
    }
}
