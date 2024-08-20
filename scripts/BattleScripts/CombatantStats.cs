using Godot;
using Godot.Collections;
using System;

public class CombatantStats
{
    private int evasion;
    private int accuracy;
    private int phys_defence;
    private int mag_defence;
    private int strength;
    private int health;

    public CombatantStats(Json stats_json, Json proficiency_json)
    {
        Godot.Collections.Dictionary<string, int> s = stats_json.Data.AsGodotDictionary<string, int>();

        Evasion = s["evasion"];
        Accuracy = s["accuracy"];
        PhysDefence = s["phys_defence"];
        MagDefence = s["mag_defence"];
        Strength = s["strength"];
        Health = s["health"];

        MagicProficiency = new(proficiency_json);
    }

    public CombatantStats(int evasion, int accuracy, int phys_defence, int mag_defence, int strength, int health)
    {
        Evasion = evasion;
        Accuracy = accuracy;
        PhysDefence = phys_defence;
        MagDefence = mag_defence;
        Strength = strength;
        Health = health;
    }


    public static int MAX_EVASION = 1000;
    public static int MIN_EVASION = 0;

    public static int MAX_ACCURACY = 1000;
    public static int MIN_ACCURACY = 0;

    public static int MAX_PHYS_DEFENCE = 1000;
    public static int MIN_PHYS_DEFENCE = 0;

    public static int MAX_MAG_DEFENCE = 1000;
    public static int MIN_MAG_DEFENCE = 0;

    public static int MAX_STRENGTH = 1000;
    public static int MIN_STRENGTH = 0;

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

    public int PhysDefence
    {
        set
        {
            if (value < MIN_PHYS_DEFENCE)
            {
                phys_defence = MIN_PHYS_DEFENCE;
            }
            else if (value > MAX_PHYS_DEFENCE)
            {
                phys_defence = MAX_PHYS_DEFENCE;
            }
            else
            {
                phys_defence = value;
            }
        }
        get
        {
            return phys_defence;
        }
    }

    public int MagDefence
    {
        set
        {
            if (value < MIN_MAG_DEFENCE)
            {
                mag_defence = MIN_MAG_DEFENCE;
            }
            else if (value > MAX_MAG_DEFENCE)
            {
                mag_defence = MAX_MAG_DEFENCE;
            }
            else
            {
                mag_defence = value;
            }
        }
        get
        {
            return mag_defence;
        }
    }

    public int Strength
    {
        set
        {
            if (value < MIN_STRENGTH)
            {
                strength = MIN_STRENGTH;
            }
            else if (value > MAX_STRENGTH)
            {
                strength = MAX_STRENGTH;
            }
            else
            {
                strength = value;
            }
        }
        get
        {
            return strength;
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

    public MagicProficiency MagicProficiency;
}
