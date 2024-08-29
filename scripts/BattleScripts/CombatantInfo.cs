using Godot;
using System;

public partial class CombatantInfo
{
    public string Path;
    public Json MovesetData;
    public Json StatsData;
    public Json ProficiencyData;

    public CombatantInfo(string path, Json movesetData, Json statsData, Json proficiencyData)
    {
        Path = path;
        MovesetData = movesetData;
        StatsData = statsData;
        ProficiencyData = proficiencyData;
    }
}
