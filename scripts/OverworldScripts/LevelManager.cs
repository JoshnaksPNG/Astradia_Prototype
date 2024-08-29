using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public partial class LevelManager : Node2D
{
	[Export]
	Area2D battleTriggerArea;

    [Export]
    Area2D splitTriggerArea;

    [Export]
	Json Moveset;

    [Export]
    Json Stats;

    [Export]
    Json Proficiency;

    [Export]
    Camera2D PartyCamera;

    PartyManager partyManager;
    bool splitting = false;
    bool enteringBattle = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (battleTriggerArea.HasOverlappingBodies())
		{
			enterBattle();
        }

        if (splitTriggerArea.HasOverlappingBodies() && !splitting)
        {
            splitLeScreen();
        }
    }

    public async Task<LoadedBattleScene> enterBattle()
    {
        //SceneTree tree = GetTree();

        List<CombatantInfo> pV = new();
        List<CombatantInfo> pR = new();

        pV.Add(new("res://scenes/test_combatant.tscn", Moveset, Stats, Proficiency));
        pV.Add(new("res://scenes/test_combatant.tscn", Moveset, Stats, Proficiency));
        pV.Add(new("res://scenes/test_combatant.tscn", Moveset, Stats, Proficiency));

        pR.Add(new("res://scenes/test_combatant.tscn", Moveset, Stats, Proficiency));
        pR.Add(new("res://scenes/test_combatant.tscn", Moveset, Stats, Proficiency));

        List<CombatantInfo> aV = new();
        List<CombatantInfo> aR = new();

        aV.Add(new("res://scenes/test_ai_combatant.tscn", Moveset, Stats, Proficiency));
        aR.Add(new("res://scenes/test_ai_combatant.tscn", Moveset, Stats, Proficiency));

        List<CombatantInfo> eV = new();
        List<CombatantInfo> eR = new();

        eV.Add(new("res://scenes/test_ai_combatant.tscn", Moveset, Stats, Proficiency));
        eV.Add(new("res://scenes/test_ai_combatant.tscn", Moveset, Stats, Proficiency));

        eR.Add(new("res://scenes/test_ai_combatant.tscn", Moveset, Stats, Proficiency));

        LoadedBattleScene battleScene = LoadedBattleScene.InstantiateBattleScene("res://scenes/basic_loaded_battle_scene.tscn", pV, pR, aV, aR, eV, eR);

        //Debug.WriteLine("shitter");
        //Node current = tree.Root.GetChildren()[0];
        //tree.Root.AddChild(battleScene);
        //tree.Root.RemoveChild(current);

        return battleScene;
    }

    public async void InitiateBattle(BattleEncounterInfo battleInfo)
    {
        enteringBattle = true;

        SceneTree tree = GetTree();
        ScreenSplitter splitter = GD.Load<PackedScene>("res://scenes/screen_splitter.tscn").Instantiate<ScreenSplitter>();

        splitter.SetCamera(PartyCamera.GetViewport());

        Node current = tree.Root.GetChildren()[0];
        tree.Root.AddChild(splitter);
        tree.Root.RemoveChild(current);

        Task<LoadedBattleScene> battleEntered = LoadBattleSceneAsync(battlefieldScenePath, partyVanguard, partyRearguard, allyVanguard, allyRearguard, enemyVanguard, enemyRearguard);

        splitter.Animator.Play("flash");

        await ToSignal(splitter.GetTree().CreateTimer(0.75d), SceneTreeTimer.SignalName.Timeout);
        await battleEntered;

        tree.Root.AddChild(battleEntered.Result);
        tree.Root.MoveChild(battleEntered.Result, 0);
        splitter.moving = true;

        await ToSignal(splitter, "ScreenSplitterPanelsClear");
        tree.Root.RemoveChild(splitter);
    }

    public async Task<LoadedBattleScene> LoadBattleSceneAsync(string battlefieldScenePath, IList<CombatantInfo> partyVanguard, IList<CombatantInfo> partyRearguard, IList<CombatantInfo> allyVanguard, IList<CombatantInfo> allyRearguard, IList<CombatantInfo> enemyVanguard, IList<CombatantInfo> enemyRearguard)
    {
        return LoadedBattleScene.InstantiateBattleScene(battlefieldScenePath, partyVanguard, partyRearguard, allyVanguard, allyRearguard, enemyVanguard, enemyRearguard);
    }

    public async void splitLeScreen()
    {
        splitting = true;
        SceneTree tree = GetTree();
        ScreenSplitter splitter = GD.Load<PackedScene>("res://scenes/screen_splitter.tscn").Instantiate<ScreenSplitter>();

        splitter.SetCamera(PartyCamera.GetViewport());

        Node current = tree.Root.GetChildren()[0];
        tree.Root.AddChild(splitter);
        tree.Root.RemoveChild(current);

        Task<LoadedBattleScene> battleEntered = enterBattle();

        splitter.Animator.Play("flash");

        await ToSignal(splitter.GetTree().CreateTimer(0.75d), SceneTreeTimer.SignalName.Timeout);
        await battleEntered;

        tree.Root.AddChild(battleEntered.Result);
        tree.Root.MoveChild(battleEntered.Result, 0);
        splitter.moving = true;

        await ToSignal(splitter, "ScreenSplitterPanelsClear");
        tree.Root.RemoveChild(splitter);
    }
}
