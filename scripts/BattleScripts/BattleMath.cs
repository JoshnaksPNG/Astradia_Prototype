using Godot;
using System;
using System.Collections.Generic;

public static class BattleMath
{
    /**
        <summary> Returns a double between [0, 0.9] to represent the rate of critical hits from the accuracy stat. </summary>

        <param name="accuracy"> A double in the range of [0, 1] that serves as the input of the function. </param>
    **/
    public static double crit_rate_from_accuracy(int accuracy)
    {
        double acc_percentage = ((double)(accuracy - CombatantStats.MIN_ACCURACY)) / ((double)CombatantStats.MAX_ACCURACY);

        return crit_rate_function(acc_percentage);
    }

    /**
        <summary> Returns a double between [0.0, 0.9] to represent the rate of critical hits from a double in the range of [0, 1]. </summary>

        <param name="acc_in"> A double in the range of [0.0, 1.0] that serves as the input of the function. </param>
    **/
    public static double crit_rate_function(double acc_in)
    {
        return 0.9d * acc_in;
    }


    /**
        <summary> Returns a double between [0.6, 1.0] to represent the rate of targeting a hit from the accuracy stat. </summary>

        <param name="accuracy"> A double in the range of [0.0, 1.0] that serves as the input of the function. </param>
    **/
    public static double hit_rate_from_accuracy(int accuracy)
    {
        double acc_percentage = ((double)(accuracy - CombatantStats.MIN_ACCURACY)) / ((double)CombatantStats.MAX_ACCURACY);

        return hit_rate_function(acc_percentage);
    }

    /**
        <summary> Returns a double between [0.6, 1.0] to represent the rate of targeting a hit from a double in the range of [0, 1]. </summary>

        <param name="acc_in"> A double in the range of [0, 1] that serves as the input of the function. </param>
    **/
    public static double hit_rate_function(double acc_in)
    {
        return (0.4d * acc_in) + 0.6d;
    }


    /**
        <summary> Returns a double between [0.0, 0.5] to represent the rate of landing a hit from the accuracy stat. </summary>

        <param name="evasion"> A double in the range of [0.0, 1.0] that serves as the input of the function. </param>
    **/
    public static double evade_rate_from_evasion(int evasion)
    {
        double acc_percentage = ((double)(evasion - CombatantStats.MIN_EVASION)) / ((double)CombatantStats.MAX_EVASION);

        return evade_rate_function(acc_percentage);
    }

    /**
        <summary> Returns a double between [0.0, 0.5] to represent the rate of landing a hit from a double in the range of [0, 1]. </summary>

        <param name="evasion_in"> A double in the range of [0, 1] that serves as the input of the function. </param>
    **/
    public static double evade_rate_function(double evasion_in)
    {
        return 0.5d * evasion_in;
    }


    /**
        <summary> Returns a double between [0.0, 1.0] to represent the rate of landing a hit from the source and target stats. </summary>

        <param name="source_accuracy"> The accuracy stat of the source </param>
        <param name="target_evasion"> The evasion stat of the target </param>
    **/
    public static double hit_chance(int source_accuracy, int target_evasion)
    {
        double hitrate_s = hit_rate_from_accuracy(source_accuracy);
        double inverted_evasion_t = 1d - evade_rate_from_evasion(target_evasion);

        return hitrate_s * inverted_evasion_t;
    }


    /**
        <summary> Returns a double between [0.0, 1.0] to represent the rate of landing a hit from the source and target stats. </summary>

        <param name="source_accuracy"> The accuracy stat of the source </param>
        <param name="target_evasion"> The evasion stat of the target </param>
    **/
    public static double magic_multiplier(MagicProficiency proficiency, List<Magictype> types)
    {
        int lowest_proficiency = 3;

        double magic_multiplier = 1d;

        foreach (Magictype magictype in types)
        {
            lowest_proficiency = proficiency[magictype];
        }

        switch (lowest_proficiency)
        {
            case 3:
                magic_multiplier = 2.4;
                break;

            case 2:
                magic_multiplier = 1.6;
                break;

            case 1:
                magic_multiplier = 1;
                break;

            case 0:
            default:
                magic_multiplier = 0;
                break;
        }

        return magic_multiplier;
    }
}
