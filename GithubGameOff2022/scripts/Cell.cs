using Godot;
using System;

// TODO: separate Cell and Sprite classes
public class Cell : Sprite
{
    public Coordinate gridCoordinate;

    public int generatedNeighbourCount = 0;

    [Export(PropertyHint.Flags, "Top,Right,Bottom,Left")]
    public int Entrances;

    public bool isGenerated = false;

    public override void _Ready()
    {
        GD.Print(Entrances);
    }

    public bool HasEntrances()
    {
        return Entrances > 0x0;
    }
}
