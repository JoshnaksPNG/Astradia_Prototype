using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;
using System.Threading.Tasks;
using System.Diagnostics;

public partial class BattleScene : Node2D
{
    [Export]
    protected CombatantGroup PartyVanguard;

    [Export]
    protected CombatantGroup PartyRearguard;

    [Export]
    protected CombatantGroup AllyVanguard;

    [Export]
    protected CombatantGroup AllyRearguard;

    [Export]
    protected CombatantGroup EnemyVanguard;

    [Export]
    protected CombatantGroup EnemyRearguard;

    [Export]
    protected VBoxContainer ChoiceBox;

    [Export]
    protected VBoxContainer ActionBox;
    [Export]
    protected Control BattleActionView;

    [Export]
    protected String ActionButtonScenePath;
    protected PackedScene ActionButtonScene;


    protected int EnemySelectionIndex;
    protected int PartySelectionIndex;

    protected List<ActionContext> ActionQueue;

    protected bool ShowingBattleMotions = false;
    protected bool IsPlayerTurn = false;

    protected bool IsGeneratingMoves = false;

    protected BattleState CurrentState = BattleState.ChooseSource;
    protected TurnType CurrentTurn = TurnType.Player;

    protected Combatant SelectedSource;
    protected Array<Combatant> SelectedTargets;
    protected BattleAction SelectedAction;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ActionQueue = new();
        SelectedTargets = new();

        PartySelectionIndex = 0;
        if (PartyVanguard.Combatants.Count > 0)
        {
            PartyVanguard.Combatants[PartySelectionIndex]._Focus();
        }

        ActionButtonScene = GD.Load<PackedScene>(ActionButtonScenePath);

        _UpdateChildrenPartners();
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

            case BattleState.GenerateAllyActions:

                break;

            case BattleState.CallingQueue:
                _CallingActionQueue();
                break;

            case BattleState.BetweenTurns:
                _BetweenTurns();
                break;

            case BattleState.GenerateEnemyActions:
                _GenerateEnemyActions();
                break;

            case BattleState.CallEnemyActions:
                _CallEnemyActions();
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
                PartyVanguard._SwitchFocus(PartySelectionIndex, PartySelectionIndex + 1);
            }
            else
            {
                PartySelectionIndex = PartyVanguard.Combatants.Count - 1;
                PartyVanguard._SwitchFocus(PartySelectionIndex, 0);
            }
        }

        if (Input.IsActionJustPressed("ui_down"))
        {
            if (PartySelectionIndex < PartyVanguard.Combatants.Count - 1)
            {
                ++PartySelectionIndex;
                PartyVanguard._SwitchFocus(PartySelectionIndex, PartySelectionIndex - 1);
            }
            else
            {
                PartySelectionIndex = 0;
                PartyVanguard._SwitchFocus(PartySelectionIndex, PartyVanguard.Combatants.Count - 1);
            }
        }

        if (Input.IsActionJustPressed("ui_interact"))
        {
            SelectedSource = PartyVanguard.Combatants[PartySelectionIndex];
            PartyVanguard.Combatants[PartySelectionIndex]._Unfocus();
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

                    _CloseActions();
                    EnemySelectionIndex = 0;

                    if (SelectedAction.targetNum != 0)
                    {
                        EnemyVanguard.Combatants[EnemySelectionIndex]._Focus();
                    }

                    CurrentState = BattleState.ChooseTarget;

                    break;
                }
            }
            catch
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
                EnemyVanguard._SwitchFocus(EnemySelectionIndex, EnemySelectionIndex + 1);
            }
            else
            {
                EnemySelectionIndex = EnemyVanguard.Combatants.Count - 1;
                EnemyVanguard._SwitchFocus(EnemySelectionIndex, 0);
            }
        }

        if (Input.IsActionJustPressed("ui_down"))
        {
            if (EnemySelectionIndex < EnemyVanguard.Combatants.Count - 1)
            {
                ++EnemySelectionIndex;
                EnemyVanguard._SwitchFocus(EnemySelectionIndex, EnemySelectionIndex - 1);
            }
            else
            {
                EnemySelectionIndex = 0;
                EnemyVanguard._SwitchFocus(EnemySelectionIndex, EnemyVanguard.Combatants.Count - 1);
            }
        }

        if (Input.IsActionJustPressed("ui_interact"))
        {
            SelectedTargets.Add(EnemyVanguard.Combatants[EnemySelectionIndex]);

            if (SelectedAction.targetNum == SelectedTargets.Count)
            {
                ActionContext currentAction = new(SelectedSource, SelectedTargets, SelectedAction);

                ActionQueue.Add(currentAction);
                SelectedTargets.Clear();

                EnemyVanguard.Combatants[EnemySelectionIndex]._Unfocus();

                if (ActionQueue.Count < PartyVanguard.Combatants.Count)
                {
                    PartySelectionIndex = 0;
                    PartyVanguard.Combatants[PartySelectionIndex]._Focus();
                    CurrentState = BattleState.ChooseSource;
                }
                else
                {
                    if (AllyVanguard != null && AllyVanguard.Combatants.Count > 0)
                    {
                        CurrentState = BattleState.GenerateAllyActions;
                    }
                    else
                    {
                        CurrentState = BattleState.CallingQueue;
                    }
                }
            }
        }

        if (SelectedAction.targetNum == 0)
        {
            ActionContext currentAction = new(SelectedSource, SelectedTargets, SelectedAction);

            ActionQueue.Add(currentAction);
            SelectedTargets.Clear();

            CurrentState = ActionQueue.Count >= PartyVanguard.Combatants.Count ? BattleState.CallingQueue : BattleState.ChooseSource;
            EnemyVanguard.Combatants[EnemySelectionIndex]._Unfocus();

            if (ActionQueue.Count < PartyVanguard.Combatants.Count)
            {
                PartySelectionIndex = 0;
                PartyVanguard.Combatants[PartySelectionIndex]._Focus();
                CurrentState = BattleState.ChooseSource;
            }
            else
            {
                if (AllyVanguard != null && AllyVanguard.Combatants.Count > 0)
                {
                    CurrentState = BattleState.GenerateAllyActions;
                }
                else
                {
                    CurrentState = BattleState.CallingQueue;
                }
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
        foreach (var combatant in EnemyVanguard.Combatants)
        {
            List<BattleEffect> removedEffects = new();

            foreach (var effect in ((Combatant)combatant).Effects)
            {
                effect._ExecuteEffect();

                if (effect.Timer <= 0)
                {
                    removedEffects.Add(effect);
                }
            }

            foreach (var effect in removedEffects)
            {
                ((Combatant)combatant).Effects.Remove(effect);
            }
        }

        // Update Party Stuffs
        foreach (var combatant in PartyVanguard.Combatants)
        {
            List<BattleEffect> removedEffects = new();

            foreach (var effect in ((Combatant)combatant).Effects)
            {
                effect._ExecuteEffect();

                if (effect.Timer <= 0)
                {
                    removedEffects.Add(effect);
                }
            }

            foreach (var effect in removedEffects)
            {
                ((Combatant)combatant).Effects.Remove(effect);
            }
        }

        if (CurrentTurn == TurnType.Player)
        {
            CurrentState = BattleState.GenerateEnemyActions;
            CurrentTurn = TurnType.Enemy;
        }
        else
        {
            CurrentState = BattleState.ChooseSource;
            CurrentTurn = TurnType.Player;

            PartySelectionIndex = 0;
            PartyVanguard.Combatants[PartySelectionIndex]._Focus();
        }
    }

    public async Task _GenerateEnemyActions()
    {
        if(!IsGeneratingMoves) 
        {
            IsGeneratingMoves = true;

            foreach (var enemy in EnemyVanguard.Combatants)
            {
                AiCombatant e = (AiCombatant)enemy;

                ActionContext ctx = e.DecideOnAction();

                ActionQueue.Add(ctx);
            }

            CurrentState = BattleState.CallEnemyActions;

            IsGeneratingMoves = false;
        }
    }

    public async Task _CallEnemyActions()
    {
        if (!ShowingBattleMotions)
        {
            await _CallActions();

            CurrentState = BattleState.BetweenTurns;
        }
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

        foreach (var enemy in EnemyVanguard.Combatants)
        {
            enemy._Unfocus();
        }

        foreach (var member in PartyVanguard.Combatants)
        {
            member._Unfocus();
        }

        EnemyVanguard.Combatants[EnemySelectionIndex]._Focus();
        PartyVanguard.Combatants[PartySelectionIndex]._Focus();
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

    public void _UpdateChildrenPartners()
    {
        Array<Node> vanguard = PartyVanguard.GetChildren();
        Array<Node> rearguard = PartyRearguard.GetChildren();

        for (int i = 0; i < rearguard.Count; i++)
        {
            //if (i < rearguard.Count)
            {
                ((Combatant)vanguard[i]).SwapPartner = ((Combatant)rearguard[i]);
                ((Combatant)vanguard[i]).HasPartner = true;

                ((Combatant)rearguard[i]).SwapPartner = ((Combatant)vanguard[i]);
                ((Combatant)rearguard[i]).HasPartner = true;

                ((Combatant)vanguard[i]).Moveset.Add(new SwapAction());
                ((Combatant)rearguard[i]).Moveset.Add(new SwapAction());
            }
            //else
            {
                //((Combatant)vanguard[i]).HasPartner = false;
            }

        }

        Array<Node> enemy_vanguard = EnemyVanguard.GetChildren();
        Array<Node> enemy_rearguard = EnemyRearguard.GetChildren();

        for (int i = 0; i < enemy_rearguard.Count; i++)
        {
            //if (i < rearguard.Count)
            {
                ((Combatant)enemy_vanguard[i]).SwapPartner = ((Combatant)enemy_rearguard[i]);
                ((Combatant)enemy_vanguard[i]).HasPartner = true;

                ((Combatant)enemy_rearguard[i]).SwapPartner = ((Combatant)enemy_vanguard[i]);
                ((Combatant)enemy_rearguard[i]).HasPartner = true;

                ((Combatant)enemy_vanguard[i]).Moveset.Add(new SwapAction());
                ((Combatant)enemy_rearguard[i]).Moveset.Add(new SwapAction());
            }
            //else
            {
                //((Combatant)vanguard[i]).HasPartner = false;
            }

        }
    }

    public enum BattleState
    {
        // Player Turn
        ChooseTarget,
        ChooseAction,
        ChooseSource,

        GenerateAllyActions,

        CallingQueue,


        // Inter-Turn
        BetweenTurns,


        // Enemy Turn
        GenerateEnemyActions,
        CallEnemyActions,
    }

    public enum TurnType 
    {
        Player,
        Enemy,
    }
}
