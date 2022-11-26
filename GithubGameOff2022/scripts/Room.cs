using Godot;
using System;
using System.Collections.Generic;

public class Room : Node2D
{
    [Export(PropertyHint.Range, "-1,4,1")]
    public int IntRoomType;

    [Export(PropertyHint.Flags, "Top,Right,Bottom,Left")]
    public int Entrances;

    public Cell GridCell;

    public List<Room> neighbours { get; } = new List<Room>() {
        null,
        null,
        null,
        null
    };

    public Room() { }

    public Room(int roomType, Cell gridCell, List<Room> neighbours)
    {
        if (roomType < -1 || roomType > 4)
        {
            roomType = -1;
        }

        this.IntRoomType = roomType;
        this.Entrances = gridCell.Entrances;
        this.GridCell = gridCell;
        this.neighbours = neighbours;
    }

    public bool HasEntrances()
    {
        return Entrances > 0;
    }

    public bool HasNeighbours()
    {
        return neighbours != null && neighbours.Count != 0;
    }

    public bool AddNeighbour()
    {
        return true;
    }
}

public enum RoomType
{
    NULL = -1,
    DEFAULT = 0,
    SECRET = 1,
    BOSS = 2
}
