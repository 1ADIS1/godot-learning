using Godot;
using System;

// TODO: separate Cell and Sprite classes
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
        GD.Print("Constructor without parameters was called!");
    }

    public Cell(CellType cellType, Coordinate gridCoordinate)
    {
        GD.Print("Constructor with parameters was called!");

        this.cellType = cellType;
        this.gridCoordinate = gridCoordinate;

        // TODO: get rid of this
        switch (cellType)
        {
            case CellType.EMPTY:
                Texture = EmptyCellTexture;
                // Set transparency
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
