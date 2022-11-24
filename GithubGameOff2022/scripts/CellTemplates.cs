using Godot;
using System;
using System.Collections.Generic;

public class CellTemplates : Node
{
    [Export] public PackedScene EmptyCell;

    [Export] public PackedScene[] TopEntranceCells;
    [Export] public PackedScene[] RightEntranceCells;
    [Export] public PackedScene[] BottomEntranceCells;
    [Export] public PackedScene[] LeftEntranceCells;

    [Export] public PackedScene[] OneEntranceCells;
    [Export] public PackedScene[] TwoEntranceCells;
    [Export] public PackedScene[] ThreeEntranceCells;
    [Export] public PackedScene[] FourEntranceCells;

    [Export] public PackedScene[] DeadEndCells; // Should be arranged like: bottom, left, top, right.

    [Export] public PackedScene CurrentCell;
    [Export] public PackedScene BossCell;
    [Export] public PackedScene SecretCell;

    /**
    returns the cell with the given entrances.
    */
    public Cell GetDesiredEntranceCell(int entrances)
    {
        PackedScene[][] cellsWithEntrances = {
            BottomEntranceCells,
            LeftEntranceCells,
            TopEntranceCells,
            RightEntranceCells
        };

        foreach (PackedScene[] scenes in cellsWithEntrances)
        {
            foreach (PackedScene scene in scenes)
            {
                Cell cell = scene.Instance<Cell>();
                if (cell.Entrances == entrances)
                {
                    return cell;
                }
            }
        }

        return null;
    }
}
