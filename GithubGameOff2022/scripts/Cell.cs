using Godot;
using System;

public class Cell : Sprite
{
    // TODO: export enum
    public CellType cellType;
    public Coordinate gridCoordinate;

    [Export]
    public Texture EmptyCellTexture;

    // Parameterless constructor for bloody C#
    public Cell()
    {

    }

    public Cell(CellType cellType, Coordinate gridCoordinate)
    {
        this.cellType = cellType;
        this.gridCoordinate = gridCoordinate;

        // TODO: get rid of this
        switch (cellType)
        {
            case CellType.EMPTY:
                Texture = EmptyCellTexture;
                // Set transparency
                // GD.Print("Instantiated empty cell!");
                break;
            default:
                Texture = EmptyCellTexture;
                break;

        }
    }
}

public enum CellType
{
    EMPTY = 0,
}
