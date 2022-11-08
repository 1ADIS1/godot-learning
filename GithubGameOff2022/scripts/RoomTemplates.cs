using Godot;
using System;

public class RoomTemplates : Node
{
    [Export]
    public PackedScene EmptyCell;

    [Export]
    public PackedScene[] TopEntranceRooms;
    [Export]
    public PackedScene[] RightEntranceRooms;
    [Export]
    public PackedScene[] BottomEntranceRooms;
    [Export]
    public PackedScene[] LeftEntranceRooms;
}
