using Godot;
using System;

public class RoomTemplates : Node
{
    [Export]
    public PackedScene EmptyCell;

    [Export]
    public Cell[] TopEntranceRooms;
}
