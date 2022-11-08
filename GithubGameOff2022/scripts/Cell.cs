using Godot;
using System;

// TODO: separate Cell and Sprite classes
public class Cell : Sprite
{
    // TODO: export cell type and set it in the editor, not in the code
    public CellType cellType;
    public Coordinate gridCoordinate;

    [Export]
    public Vector2[] Entrances;
}

public enum CellType
{
    EMPTY = 0,
}
