using Godot;
using System;

public enum CellType
{
    EMPTY = 0,
}

public class Cell : Sprite
{
    [Export(PropertyHint.Flags, "Empty")] int CellType;

    public Cell()
    {
        GD.Print("Instantiated cell with cell type: ", CellType);
    }
}
