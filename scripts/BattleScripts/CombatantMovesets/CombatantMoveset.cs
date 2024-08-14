using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class CombatantMoveset : Resource
{


    public List<Type> _CombatantMoves = new List<Type>() 
    {
        typeof (TestDamageAction),
    };

    CombatantMoveset()
    {
        
    }
}
