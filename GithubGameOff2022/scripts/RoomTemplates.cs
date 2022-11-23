using Godot;
using System;

public class RoomTemplates : Node
{
    [Export] public PackedScene[] FourEntranceRooms;
    [Export] public PackedScene[] ThreeEntranceRooms;
    [Export] public PackedScene[] TwoEntranceRooms;
    [Export] public PackedScene[] OneEntranceRooms;
}
