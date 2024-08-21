using Godot;
using Godot.Collections;
using System;

public class MagicProficiency
{
    private Dictionary<string, int> proficiencies;

    public MagicProficiency(Json json)
    {
        proficiencies = json.Data.AsGodotDictionary<string, int>();
    }

    /**
        <summary>
        Indexer for MagicProficiency class
        </summary>

        <param name="magictype"> The type of magic to get the proficiency for </param>
    **/
    public int this[Magictype magictype]
    {
        get { return proficiencies[magictype.ToString().ToLowerInvariant()]; }
        set { proficiencies[magictype.ToString().ToLowerInvariant()] = value; }
    }
}
