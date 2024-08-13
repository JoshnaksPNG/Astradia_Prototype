using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

public partial class TestBattleScene : Node2D
{
    [Export]
    TestEnemyGroup EnemyGroup;

    [Export]
    TestPlayerGroup PartyGroup;

    [Export]
    TestPartnerGroup PartnerGroup;

    [Export]
    VBoxContainer ChoiceBox;

    [Export]
    VBoxContainer ActionBox;
    [Export]
    Control BattleActionView;

    [Export]
    String ActionButtonScenePath;
    PackedScene ActionButtonScene;


    int EnemySelectionIndex;
    int PartySelectionIndex;

    List<ActionContext> ActionQueue;

    bool ShowingBattleMotions = false;
    bool IsPlayerTurn = false;

    BattleState CurrentState = BattleState.ChooseSource;

    TestCombatant SelectedSource;
    Array<TestCombatant> SelectedTargets;
    BattleAction SelectedAction;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ActionQueue = new List<ActionContext>();
        SelectedTargets = new Array<TestCombatant> ();

        PartySelectionIndex = 0;
        ((TestCombatant)PartyGroup.PartyMembers[PartySelectionIndex])._Focus();

        ActionButtonScene = GD.Load<PackedScene>(ActionButtonScenePath);

        Array<Node> vanguard = PartyGroup.GetChildren();
        Array<Node> rearguard = PartnerGroup.GetChildren();
        for (int i = 0; i < rearguard.Count; i++) 
        {
            //if (i < rearguard.Count)
            {
                ((TestCombatant)vanguard[i]).SwapPartner = ((TestCombatant)rearguard[i]);
                ((TestCombatant)vanguard[i]).HasPartner = true;

                ((TestCombatant)rearguard[i]).SwapPartner = ((TestCombatant)vanguard[i]);
                ((TestCombatant)rearguard[i]).HasPartner = true;

                ((TestCombatant)vanguard[i]).Moveset.Add(new SwapAction());
                ((TestCombatant)rearguard[i]).Moveset.Add(new SwapAction());
            }
            //else
            {
                //((TestCombatant)vanguard[i]).HasPartner = false;
            }
            
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

        switch (CurrentState)
        {
            case BattleState.ChooseSource:
                _ChoosingSource();
                break;

            case BattleState.ChooseAction:
                _ChoosingAction();
                break;

            case BattleState.ChooseTarget:
                _ChoosingTarget();
                break;

            case BattleState.CallingQueue:
                _CallingActionQueue();
                break;

            case BattleState.BetweenTurns:
                _BetweenTurns();
                break;
        }
    }

    public void _ChoosingSource()
    {
        if (Input.IsActionJustPressed("ui_up"))
        {
            if (PartySelectionIndex > 0)
            {
                --PartySelectionIndex;
                PartyGroup._SwitchFocus(PartySelectionIndex, PartySelectionIndex + 1);
            }
            else
            {
                PartySelectionIndex = PartyGroup.PartyMembers.Count - 1;
                PartyGroup._SwitchFocus(PartySelectionIndex, 0);
            }
        }

        if (Input.IsActionJustPressed("ui_down"))
        {
            if (PartySelectionIndex < PartyGroup.PartyMembers.Count - 1)
            {
                ++PartySelectionIndex;
                PartyGroup._SwitchFocus(PartySelectionIndex, PartySelectionIndex - 1);
            }
            else
            {
                PartySelectionIndex = 0;
                PartyGroup._SwitchFocus(PartySelectionIndex, PartyGroup.PartyMembers.Count - 1);
            }
        }

        if (Input.IsActionJustPressed("ui_interact"))
        {
            SelectedSource = (TestCombatant)PartyGroup.PartyMembers[PartySelectionIndex];
            ((TestCombatant)PartyGroup.PartyMembers[PartySelectionIndex])._Unfocus();
            PartySelectionIndex = 0;

            CurrentState = BattleState.ChooseAction;
            _SetupActions();
        }
    }

    public void _ChoosingAction()
    {
        foreach (var actionButton in ActionBox.GetChildren())
        {
            try 
            {
                ActionButton a = (ActionButton)actionButton;
                if (a.ButtonPressed)
                {
                    SelectedAction = a.AttachedAction;
                    CurrentState = BattleState.ChooseTarget;
                    _CloseActions();

                    EnemySelectionIndex = 0;
                    ((TestCombatant)EnemyGroup.enemies[EnemySelectionIndex])._Focus();
                    break;
                }
            } catch 
            {

            }
            
        }
    }

    public void _ChoosingTarget()
    {
        if (Input.IsActionJustPressed("ui_up"))
        {
            if (EnemySelectionIndex > 0)
            {
                --EnemySelectionIndex;
                EnemyGroup._SwitchFocus(EnemySelectionIndex, EnemySelectionIndex + 1);
            }
            else
            {
                EnemySelectionIndex = EnemyGroup.enemies.Count - 1;
                EnemyGroup._SwitchFocus(EnemySelectionIndex, 0);
            }
        }

        if (Input.IsActionJustPressed("ui_down"))
        {
            if (EnemySelectionIndex < EnemyGroup.enemies.Count - 1)
            {
                ++EnemySelectionIndex;
                EnemyGroup._SwitchFocus(EnemySelectionIndex, EnemySelectionIndex - 1);
            }
            else
            {
                EnemySelectionIndex = 0;
                EnemyGroup._SwitchFocus(EnemySelectionIndex, EnemyGroup.enemies.Count - 1);
            }
        }

        if (Input.IsActionJustPressed("ui_interact"))
        {
            SelectedTargets.Add((TestCombatant)EnemyGroup.enemies[EnemySelectionIndex]);

            if (SelectedAction.targetNum == SelectedTargets.Count)
            {
                ActionContext currentAction = new(SelectedSource, SelectedTargets, SelectedAction);

                ActionQueue.Add(currentAction);
                SelectedTargets.Clear();

                CurrentState = ActionQueue.Count >= PartyGroup.GetChildren().Count ? BattleState.CallingQueue : BattleState.ChooseSource;
                ((TestCombatant)EnemyGroup.enemies[EnemySelectionIndex])._Unfocus();

                if (ActionQueue.Count < PartyGroup.GetChildren().Count)
                {
                    PartySelectionIndex = 0;
                    ((TestCombatant)PartyGroup.PartyMembers[PartySelectionIndex])._Focus();
                }
            }
        }

        if (SelectedAction.targetNum == 0)
        {
            ActionContext currentAction = new(SelectedSource, SelectedTargets, SelectedAction);

            ActionQueue.Add(currentAction);
            SelectedTargets.Clear();

            CurrentState = ActionQueue.Count >= PartyGroup.GetChildren().Count ? BattleState.CallingQueue : BattleState.ChooseSource;
            ((TestCombatant)EnemyGroup.enemies[EnemySelectionIndex])._Unfocus();

            if (ActionQueue.Count < PartyGroup.GetChildren().Count)
            {
                PartySelectionIndex = 0;
                ((TestCombatant)PartyGroup.PartyMembers[PartySelectionIndex])._Focus();
            }
        }


    }

    public async void _CallingActionQueue()
    {
        if (!ShowingBattleMotions)
        {
            await _CallActions();

            CurrentState = BattleState.BetweenTurns;
        }
    }

    public void _BetweenTurns()
    {
        // Update Enemy Stuffs
        foreach (var combatant in EnemyGroup.GetChildren())
        {
            List<BattleEffect> removedEffects = new();

            foreach (var effect in ((TestCombatant)combatant).Effects)
            {
                effect._ExecuteEffect();

                if (effect.Timer <= 0)
                {
                    removedEffects.Add(effect);
                }
            }

            foreach (var effect in removedEffects)
            {
                ((TestCombatant)combatant).Effects.Remove(effect);
            }
        }

        // Update Party Stuffs
        foreach (var combatant in PartyGroup.GetChildren())
        {
            List<BattleEffect> removedEffects = new();

            foreach (var effect in ((TestCombatant)combatant).Effects)
            {
                effect._ExecuteEffect();

                if (effect.Timer <= 0)
                {
                    removedEffects.Add(effect);
                }
            }

            foreach (var effect in removedEffects)
            {
                ((TestCombatant)combatant).Effects.Remove(effect);
            }
        }

        CurrentState = BattleState.ChooseSource;
    }

    public async Task _CallActions()
    {
        _HideChoiceBox();
        ShowingBattleMotions = true;

        foreach (ActionContext action in ActionQueue)
        {
            action._ExecuteAction();

            await ToSignal(GetTree().CreateTimer(1d), SceneTreeTimer.SignalName.Timeout);
        }

        ActionQueue.Clear();

        PartySelectionIndex = 0;
        ((TestCombatant)PartyGroup.PartyMembers[PartySelectionIndex])._Focus();

        ShowingBattleMotions = false;
        _ShowChoiceBox();
        

        return;
    }

    public void _ShowChoiceBox()
    {
        ChoiceBox.Show();
    }

    public void _HideChoiceBox()
    {
        ChoiceBox.Hide();
    }

    public void _ResetFocus()
    {
        EnemySelectionIndex = 0;
        PartySelectionIndex = 0;

        foreach (var enemy in EnemyGroup.enemies)
        {
            ((TestCombatant)enemy)._Unfocus();
        }

        foreach (var member in PartyGroup.PartyMembers)
        {
            ((TestCombatant)member)._Unfocus();
        }

        ((TestCombatant)EnemyGroup.enemies[EnemySelectionIndex])._Focus();
        ((TestCombatant)PartyGroup.PartyMembers[PartySelectionIndex])._Focus();
    }

    public void _on_attack_pressed()
    {
        _HideChoiceBox();
    }

    public void _SetupActions()
    {
        foreach (var action in SelectedSource.Moveset)
        {
            var actionbutton = (ActionButton)ActionButtonScene.Instantiate();

            actionbutton.AttachedAction = action;

            ActionBox.AddChild(actionbutton);
        }

        BattleActionView.Show();
    }

    public void _CloseActions()
    {
        BattleActionView.Hide();

        foreach (var child in ActionBox.GetChildren())
        {
            child.QueueFree();
        }
    }

    public enum BattleState
    {
        // Player Turn
        ChooseTarget,
        ChooseAction,
        ChooseSource,
        CallingQueue,

        // Inter-Turn
        BetweenTurns,


        // Enemy Turn

    }
}
