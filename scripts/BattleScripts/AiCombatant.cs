using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class AiCombatant : Combatant
{
    public System.Collections.Generic.Dictionary<Combatant, int> AggroTable;

    public List<Combatant> HighestAggroCombatants;
    public List<Combatant> ExposedAllies;

    public List<CombatantGroup> AlliedVanguards = new();
    protected List<Combatant> Friendlies = new();

    /** 
        <summary>
        Update the list of combatants with the highest aggro values.
        </summary>
    **/
    public void _UpdateTargetedCombatants()
    {
        int maxAggro = int.MinValue;

        foreach (var pair in AggroTable.AsEnumerable()) 
        {
            if(pair.Value > maxAggro) 
            {
                maxAggro = pair.Value;
            }
        }

        HighestAggroCombatants = (from Pair in AggroTable.AsEnumerable()
                                  where Pair.Value == maxAggro
                                  select Pair.Key).ToList();
    }

    /** 
        <summary>
        Add Aggro to a combatant
        </summary>

        <param name="source">The combatant that accrues aggro</param>
        <param name="addedAggro">Amount of aggro to add</param>
    **/
    public void _AddAggro(Combatant source, int addedAggro)
    {
        AggroTable[source] += addedAggro;
        _UpdateTargetedCombatants();
    }

    /** 
        <summary>
        Multiply a combatant's aggro
        </summary>

        <param name="source">The combatant that changes aggro</param>
        <param name="aggroAmount">Amount to multiply the aggro by</param>
    **/
    public void _MultiplyAggro(Combatant source, float aggroAmount)
    {
        AggroTable[source] = (int)(AggroTable[source] * aggroAmount);
        _UpdateTargetedCombatants();
    }

    /** 
        <summary>
        Request an Action for the AI Combatant to perform
        </summary>
    **/
    public ActionContext DecideOnAction()
    {
        //Debug.WriteLine("Deciding...");
        Random r = new Random();

        BattleAction selectedMove = Moveset[r.Next(Moveset.Count)];
        //Debug.WriteLine($"Chose {selectedMove.ID} with {selectedMove.targetNum} targets");

        Array<Combatant> targets = new();

        if (selectedMove.DoesTargetSelf)
        {
            Friendlies.Clear();
            foreach (var vanguard in AlliedVanguards)
            {
                foreach (var friend in vanguard.Combatants)
                {
                    Friendlies.Add(friend);
                }
            }
        }

        //Debug.WriteLine("Selecting Targets...");
        for (int i = 0; i < selectedMove.targetNum; ++i)
        {
            Combatant t;
            if (selectedMove.DoesTargetSelf)
            {
                t = Friendlies[r.Next(Friendlies.Count)];
            }
            else
            {
                t = HighestAggroCombatants[r.Next(HighestAggroCombatants.Count)];
            }
            
            //Combatant t = HighestAggroCombatants[0];
            targets.Add(t);
            //Debug.WriteLine($"Added target {t}");
        }
        //Debug.WriteLine($"Selected targets {targets}");

        return new(this, targets, selectedMove);
    }

    /** 
        <summary>
        Method to simply initialize an AI Combatant. 
        </summary>

        <param name="parent"> Node to add the combatant as a child to. </param>
        <param name="stats"> Stats to load to the combatant. </param>
        <param name="moveset"> Moveset to load to the combatant. </param>
        <param name="scenePath"> Resource path to the Scene resource of combatant. </param>

        <returns>Initiated AI Combatant</returns>
    **/
    new public static AiCombatant _InitCombatant(Node parent, CombatantStats stats, Json moveset, string scenePath)
    {
        AiCombatant c = GD.Load<PackedScene>(scenePath).Instantiate<AiCombatant>();

        c._SetChildrenNodes();

        c._LoadMoveset(moveset);
        c._LoadStats(stats);

        c.MagicWeaknesses = new();

        parent.AddChild(c);

        return c;
    }

    /** 
        <summary>
        Method to switch out a combatant, and have their replacement take their aggro. 
        </summary>

        <param name="original"> Original Combatant that is replaced. </param>
        <param name="replacement"> Replacement Combatant that takes original's aggro </param>
    **/
    public void _SwapCombatantAggro(Combatant original, Combatant replacement)
    {
        int swapAggro = AggroTable[original];

        AggroTable.Remove(original);

        AggroTable.Add(replacement, swapAggro);

        _UpdateTargetedCombatants();
    }

    /** 
        <summary>
        Method to initialize the aggro table for AiCombatant
        </summary>

        <param name="oppVanguard"> Opponent vanguard. </param>
        <param name="oppAllyVanguard"> Opponent ally vanguard. null by default. </param>
    **/
    public void _InitAggroTable(CombatantGroup oppVanguard, CombatantGroup? oppAllyVanguard = null)
    {
        if (AggroTable == null)
        {
            AggroTable = new();
        }

        foreach (var opp in oppVanguard.GetChildren())
        {
            AggroTable.Add((Combatant)opp, 0);
        }

        if (oppAllyVanguard != null)
        {
            foreach (var opp in oppAllyVanguard.GetChildren())
            {
                AggroTable.Add((Combatant)opp, 0);
            }
        }

        _UpdateTargetedCombatants();
    }

    /** 
        <summary>
        Method to add Vanguard to ally group
        </summary>

        <param name="vanguard"> Allied vanguard. </param>
    **/
    public void _AddAlliedVanguard(CombatantGroup vanguard)
    {
        AlliedVanguards.Add(vanguard);
    }
}
