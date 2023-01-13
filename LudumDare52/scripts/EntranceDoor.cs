using Godot;
using System;

public partial class EntranceDoor : StaticBody3D
{
    [Export] public AnimationPlayer animationPlayer;
    [Export] private PlayerController player;
    [Export] private HarvestSystem harvestSystem;
    [Export] private Furnace furnace;
    [Export] private Timer meatProcessingTimer;
    [Export] public Light3D light;
    [Export] public ColorRect EndingScreen;
    public bool IsMeatProcessing = false;

    public bool IsEnding = false;

    [Signal]
    public delegate void MeatProcessingEndedEventHandler();

    public void Interact()
    {
        if (IsEnding == true)
        {
            animationPlayer.Play("Ending");
            player.SetPhysicsProcess(false);
            player.SetProcess(false);
            return;
        }

        if (IsMeatProcessing)
        {
            GD.Print("Meat is being processed...");
            return;
        }

        if (furnace.meatLoaded < furnace.MeatCapacity)
        {
            GD.Print("Load meat first!");
            return;
        }

        IsMeatProcessing = true;
        animationPlayer.Play("ProcessMeat");
        meatProcessingTimer.Start();
        harvestSystem.StopGathering();
        if (player.meat.Visible)
        {
            player.meat.Visible = false;
        }

        GD.Print("Meat Processing has started!");
    }

    public void MeatProcessingTimerTimeout()
    {
        GD.Print("All meat has burnt!");
        animationPlayer.Stop();
        furnace.meatLoaded = 0;
        IsMeatProcessing = false;
        EmitSignal("MeatProcessingEnded");
    }
}
