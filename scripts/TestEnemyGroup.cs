using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class TestEnemyGroup : Node2D
{
    public Array<Node> enemies;

    const int EnemyOffset = 160;
    const int OffsetFromTop = 60;

    [Export]
    VBoxContainer ChoiceBox;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        enemies = GetChildren();

        for (int i = 0; i < enemies.Count; i++) 
        {
            ((Node2D)enemies[i]).Position = new Vector2(0, i * EnemyOffset + OffsetFromTop);
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        
    }

    public void _SwitchFocus(int a, int b)
    {
        ((TestCombatant)enemies[a])._Focus();
        ((TestCombatant)enemies[b])._Unfocus();

    }
}
