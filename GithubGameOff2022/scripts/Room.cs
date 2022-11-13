using Godot;
using System;

public class Room : Node
{
    [Export(PropertyHint.Range, "-1,4,1")]
    public int IntRoomType;

    [Export(PropertyHint.Flags, "Top,Right,Bottom,Left")]
    public int Entrances;

    public Cell GridCell;

    public Room() { }

    public Room(int roomType, Cell gridCell)
    {
        if (roomType < -1 || roomType > 4)
        {
            roomType = -1;
        }

        this.IntRoomType = roomType;
        this.GridCell = gridCell;
    }

    public bool HasEntrances()
    {
        return Entrances > 0x0;
    }
}

public enum RoomType
{
    NULL = -1,
    DEFAULT = 0,
    TREASURE = 1,
    SECRET = 2,
    SHOP = 3,
    BOSS = 4
}
