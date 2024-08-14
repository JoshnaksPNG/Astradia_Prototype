using Godot;
using Godot.Collections;
using System;

public class CombatantStats
{
    private int evasion;
    private int accuracy;
    private int defence;
    private int health;

    public CombatantStats(Json json)
    {
        /// Dictionary<string, int> 
    }

    public CombatantStats(int evasion, int accuracy, int defence, int health)
    {
        Evasion = evasion;
        Accuracy = accuracy;
        Defence = defence;
        Health = health;
    }


    public static int MAX_EVASION = 1000;
    public static int MIN_EVASION = 0;

    public static int MAX_ACCURACY = 1000;
    public static int MIN_ACCURACY = 0;

    public static int MAX_DEFENCE = 1000;
    public static int MIN_DEFENCE = 0;

    public static int MAX_HEALTH = 9999;
    public static int MIN_HEALTH = 0;

    public int Evasion
    {
        set
        {
            if (value < MIN_EVASION)
            {
                evasion = MIN_EVASION;

            }
            else if (value > MAX_EVASION)
            {
                evasion = MAX_EVASION;
            }
            else
            {
                evasion = value;
            }
        }
        get
        {
            return evasion;
        }
    }

    public int Accuracy
    {
        set
        {
            if (value < MIN_ACCURACY)
            {
                accuracy = MIN_ACCURACY;
            }
            else if (value > MAX_ACCURACY)
            {
                accuracy = MAX_ACCURACY;
            }
            else
            {
                accuracy = value;
            }
        }
        get
        {
            return accuracy;
        }
    }

    public int Defence
    {
        set
        {
            if (value < MIN_DEFENCE)
            {
                defence = MIN_DEFENCE;
            }
            else if (value > MAX_DEFENCE)
            {
                defence = MAX_DEFENCE;
            }
            else
            {
                defence = value;
            }
        }
        get
        {
            return defence;
        }
    }

    public int Health
    {
        set
        {
            if (value < MIN_HEALTH)
            {
                health = MIN_HEALTH;
            }
            else if (value > MAX_HEALTH)
            {
                health = MAX_HEALTH;
            }
            else
            {
                health = value;
            }
        }
        get
        {
            return health;
        }
    }
}
