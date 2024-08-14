using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;
using System.Threading.Tasks;

public partial class BattleScene : Node2D
{
    [Export]
    protected CombatantGroup PartyVanguard;

    [Export]
    protected CombatantGroup PartyRearGuard;

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

    protected BattleState CurrentState = BattleState.ChooseSource;

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
                    CurrentState = BattleState.ChooseTarget;
                    _CloseActions();

                    EnemySelectionIndex = 0;
                    EnemyVanguard.Combatants[EnemySelectionIndex]._Focus();
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

                CurrentState = ActionQueue.Count >= PartyVanguard.GetChildren().Count ? BattleState.CallingQueue : BattleState.ChooseSource;
                EnemyVanguard.Combatants[EnemySelectionIndex]._Unfocus();

                if (ActionQueue.Count < PartyVanguard.GetChildren().Count)
                {
                    PartySelectionIndex = 0;
                    PartyVanguard.Combatants[PartySelectionIndex]._Focus();
                }
            }
        }

        if (SelectedAction.targetNum == 0)
        {
            ActionContext currentAction = new(SelectedSource, SelectedTargets, SelectedAction);

            ActionQueue.Add(currentAction);
            SelectedTargets.Clear();

            CurrentState = ActionQueue.Count >= PartyVanguard.GetChildren().Count ? BattleState.CallingQueue : BattleState.ChooseSource;
            EnemyVanguard.Combatants[EnemySelectionIndex]._Unfocus();

            if (ActionQueue.Count < PartyVanguard.GetChildren().Count)
            {
                PartySelectionIndex = 0;
                PartyVanguard.Combatants[PartySelectionIndex]._Focus();
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
        foreach (var combatant in EnemyVanguard.GetChildren())
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
        foreach (var combatant in PartyVanguard.GetChildren())
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
        PartyVanguard.Combatants[PartySelectionIndex]._Focus();

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
        Array<Node> rearguard = PartyRearGuard.GetChildren();
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
