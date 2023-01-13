using Godot;
using System;
using System.Collections.Generic;

public partial class HarvestSystem : Node3D
{
    public int meatCollected;
    public int explodedMeat;
    private List<Node> gatheringSpots = new List<Node>(4);
    [Export] private PlayerController player;
    [Export] private ColorRect screenColor;
    [Export] private float ScreenColorFadeDuration;
    [Export] int lives;
    AudioStreamPlayer3D audioPlayer;

    public override void _Ready()
    {
        screenColor.Color = new Color(1, 1, 1, 0);

        audioPlayer = GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D");

        foreach (Node gatheringSpot in GetChildren())
        {
            // I know this is unsafe, but I have no time.
            gatheringSpots.Add(gatheringSpot);
        }
    }

    public void StartGathering()
    {
        foreach (Node gatheringSpot in gatheringSpots)
        {
            gatheringSpot.Call("ProcessGrowth");
        }
    }

    public void StopGathering()
    {
        foreach (Node gatheringSpot in gatheringSpots)
        {
            gatheringSpot.Call("StopGrowth");
        }
    }

    public void GatheringSpotInteracted(MeatController meatController)
    {
        if (meatController.CanHarvest && !player.meat.Visible)
        {
            GD.Print("Meat " + Name + " was gathered!");
            meatController.Harvest();
            meatCollected++;
            player.meat.Visible = true;
        }
        else
        {
            GD.Print("Plant is still growing or the player is holding the meat!");
        }
    }

    public void MeatGotDestoroyed(MeatController meatController)
    {
        GD.Print("Meat " + meatController.Name + " got destroyed");
        GD.Print(audioPlayer);
        audioPlayer.Stream = GD.Load<AudioStream>("res://sound/Damage.wav");
        audioPlayer.Play();
        explodedMeat++;
        TweenScreenColor();
        player.ShakeCamera();

        if (explodedMeat >= lives)
        {
            GetTree().ReloadCurrentScene();
        }
    }

    public void TweenScreenColor()
    {
        var screenTween = screenColor.CreateTween();
        screenTween.TweenProperty(screenColor, "color", new Color("ff2d3b70"), ScreenColorFadeDuration);
        screenTween.TweenProperty(screenColor, "color", new Color(1, 1, 1, 0), ScreenColorFadeDuration);
        screenTween.Play();
    }
}