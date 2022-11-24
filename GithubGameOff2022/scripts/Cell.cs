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

    public static String MapEntrancesToName(int entrances)
    {
        if (entrances < 0 || entrances > 15)
        {
            GD.PushError("Error mapping entrances to name!");
            return null;
        }
        String name = "";

        String[] letters = { "L", "B", "R", "T" };
        for (int index = 0; index < 4; index++)
        {
            if (Utils.IsBitEnabled(entrances, index))
            {
                name += letters[index];
            }
        }

        return name;
    }
}
