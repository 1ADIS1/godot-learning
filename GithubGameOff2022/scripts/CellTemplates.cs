using Godot;
using System;

[Obsolete("This class is obsolete, use RoomTemplates instead.")]
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
}
