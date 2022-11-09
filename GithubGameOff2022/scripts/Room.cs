using Godot;
using System;

public class Room : Node
{
    // [Flags]
    public RoomType RoomType;
}

public enum RoomType
{
    DEFAULT = 0,
    TREASURE = 1,
    SECRET = 2,
    SHOP = 3
}
