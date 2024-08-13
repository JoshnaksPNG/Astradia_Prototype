using Godot;
using Godot.Collections;
using System;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Threading.Tasks;

public partial class SwapAction : BattleAction
{
    public static double TransitionTime = 0.6d;
    public static double VanguardWait = 0.4d;
    public static double TransitionSpeed = 0.3d;

    public static float FrameDelay = 0.01f;

    public static double SuccessMargin = 2;

    double VanguardX;
    double RearguardX;

    double PriorT;
    double AfterT;
    double DeltaT;

    public double TotalDistance;
    public double Speed;

    public SwapAction() : base()
    {
        Name = "Swap Partners";
        ID = "battle_action.swap.swap_partners";
        Description = "Change the combatant in the battle, keeps agro";
        targetNum = 0;

        Duration = 2d;

        TotalDistance = Math.Abs(VanguardX - RearguardX);
        Speed = TotalDistance / TransitionTime;
    }

    public override void _ExecuteAction()
    {
        if (!Source.HasPartner)
        {
            throw new Exception("Source Combatant has no Partner!");
        }

        VanguardX = Source.GlobalPosition.X;
        RearguardX = Source.SwapPartner.GlobalPosition.X;

        _MovePositions();
    }

    private async void _MovePositions()
    {
        TestCombatant vanguard = Source;
        TestCombatant rearguard = Source.SwapPartner;

        vanguard._PlayAnimation("swap_out");
        await ToSignal(vanguard.GetTree().CreateTimer(0.6d), SceneTreeTimer.SignalName.Timeout);
        vanguard._PlayAnimation("RESET");

        Node vParent = vanguard.GetParent();
        Node rParent = rearguard.GetParent();

        vParent.RemoveChild(vanguard);
        rParent.RemoveChild(rearguard);

        vParent.AddChild(rearguard);
        rParent.AddChild(vanguard);

        ((TestPlayerGroup)vParent)._SwapChild(vanguard, rearguard);
        ((TestPartnerGroup)rParent)._SwapChild(rearguard, vanguard);

        rearguard._PlayAnimation("swap_in");
        await ToSignal(vanguard.GetTree().CreateTimer(0.6d), SceneTreeTimer.SignalName.Timeout);
        rearguard._PlayAnimation("RESET");
    }

    public TestCombatant _GetSource()
    {
        return Source;
    }
}
