using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Threading.Tasks;

public partial class SwapAction : BattleAction
{
    public static double TransitionTime = 0.6d;

    public SwapAction() : base()
    {
        Name = "Swap Partners";
        ID = "battle_action.swap.swap_partners";
        Description = "Change the combatant in the battle, keeps agro";
        targetNum = 0;
    }

    public override void _ExecuteAction()
    {
        if (!Source.HasPartner)
        {
            throw new Exception("Source Combatant has no Partner!");
        }

        foreach(var opp in Source.AllOpponents) 
        {
            if(opp is AiCombatant) 
            {
                ((AiCombatant)opp)._SwapCombatantAggro(Source, Source.SwapPartner);
            }
        }

        _MovePositions();
    }

    private async void _MovePositions()
    {
        Combatant vanguard = Source;
        Combatant rearguard = Source.SwapPartner;

        vanguard._PlayAnimation("swap_out");
        await ToSignal(vanguard.GetTree().CreateTimer(TransitionTime), SceneTreeTimer.SignalName.Timeout);
        vanguard._PlayAnimation("RESET");

        Node vParent = vanguard.GetParent();
        Node rParent = rearguard.GetParent();

        vParent.RemoveChild(vanguard);
        rParent.RemoveChild(rearguard);

        vParent.AddChild(rearguard);
        rParent.AddChild(vanguard);

        ((CombatantGroup)vParent)._SwapChild(vanguard, rearguard);
        ((CombatantGroup)rParent)._SwapChild(rearguard, vanguard);

        rearguard._PlayAnimation("swap_in");
        await ToSignal(vanguard.GetTree().CreateTimer(TransitionTime), SceneTreeTimer.SignalName.Timeout);
        rearguard._PlayAnimation("RESET");
    }

    public Combatant _GetSource()
    {
        return Source;
    }

}
