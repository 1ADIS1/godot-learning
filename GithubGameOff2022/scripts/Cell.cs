using Godot;
using System;

// TODO: separate Cell and Sprite classes
public class Cell : Sprite
{
    // TODO: export cell type and set it in the editor, not in the code
    public CellType cellType;
    public Coordinate gridCoordinate;

    [Export]
    // Get rid of vectors and just place ints from 1 to 4;
    public Vector2[] Entrances;

    public bool isGenerated = false;

    public bool HasEntrances()
    {
        return Entrances != null;
    }
}

public enum CellType
{
    EMPTY = 0,
}
