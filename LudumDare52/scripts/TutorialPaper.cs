using Godot;
using System.Collections.Generic;

public partial class TutorialPaper : StaticBody3D
{
    [Export] HarvestSystem harvestSystem;
    [Export] Furnace furnace;

    [Export] int[] furnaceMeatCapacities;
    [Export] EntranceDoor entranceDoor;
    [Export] ProgressBar MeatLoad;
    [Export] public CPUParticles3D furnaceParticles;

    List<ColorRect> papers;

    bool HadInteracted = false;
    int currentPhase;

    public override void _Ready()
    {
        var UINode = GetParent().GetNode("UI");
        papers = new List<ColorRect> { UINode.GetNode<ColorRect>("Paper1"), UINode.GetNode<ColorRect>("Paper2"), UINode.GetNode<ColorRect>("Paper3") };
        MeatLoad.Visible = false;
    }

    public void Interact()
    {
        if (HadInteracted)
        {
            GD.Print("Already interacted!");
            return;
        }

        if (currentPhase == papers.Count && !HadInteracted)
        {
            GD.Print("Cuscene!");

            HadInteracted = true;
            entranceDoor.light.LightEnergy = 0;
            MeatLoad.Visible = false;
            entranceDoor.IsEnding = true;
            furnaceParticles.QueueFree();

            return;
        }

        papers[currentPhase].Visible = true;
    }

    public override void _Input(InputEvent @event)
    {
        if (currentPhase >= papers.Count)
        {
            return;
        }
        if (Input.IsActionJustPressed("interact") && papers[currentPhase].Visible)
        {
            harvestSystem.StartGathering();
            furnace.MeatCapacity = furnaceMeatCapacities[currentPhase];

            MeatLoad.Value = 0;
            MeatLoad.MaxValue = furnace.MeatCapacity;

            papers[currentPhase].Visible = false;
            HadInteracted = true;
            MeatLoad.Visible = true;
        }
    }

    public void OnMeatProcessingEnd()
    {
        GD.Print("Go and check new note!");
        currentPhase++;
        HadInteracted = false;
    }
}
