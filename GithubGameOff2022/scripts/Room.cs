using Godot;
using System;
using System.Collections.Generic;

public class Room : Node2D
{
    [Export(PropertyHint.Range, "-1,3,1")]
    public int IntRoomType = -1;

    [Export(PropertyHint.Flags, "Top,Right,Bottom,Left")]
    public int Entrances = 0;

    public Coordinate coordinate = new Coordinate();
    public int generatedNeighbourCount = 0;

    // [Export] public PackedScene CellSprite;

    public Room() { }

    public Room(Coordinate coordinate, int roomType = -1, int entrances = 0)
    {
        if (roomType < -1 || roomType > 4)
        {
            GD.PushWarning("Tried to create room with room type: " + roomType);
            roomType = -1;
        }

        this.IntRoomType = roomType;
        this.Entrances = entrances;
        this.coordinate = coordinate;
    }

    public bool HasEntrances()
    {
        return Entrances > 0;
    }

    // Returns true if this room has entrance to the given direction.
    // Where 0 is top and 3 is left.
    public bool HasEntranceTo(int index)
    {
        if (!Utils.BitMaskIndexFormatCheck(Entrances, index))
        {
            GD.PushError(GetClass() + ": passed wrong index format | " + index);
            return false;
        }
        return Utils.IsBitEnabled(Entrances, index);
    }

    public static List<Room> GetRoomsThatHasEntranceTo(int index, List<Room> searchSpace)
    {
        if (searchSpace == null || searchSpace.Count == 0)
        {
            GD.PushError("Passed empty rooms list in GetRoomsThatHasEntranceTo!");
            return null;
        }

        List<Room> desiredRooms = new List<Room>();
        foreach (Room room in searchSpace)
        {
            if (!Utils.BitMaskIndexFormatCheck(room.Entrances, index))
            {
                GD.PushError("Room has wrong entrance format in GetRoomsThatHasEntranceTo | "
                    + index + " | " + room.Entrances);
                return null;
            }

            if (!room.HasEntranceTo(index))
            {
                continue;
            }
            desiredRooms.Add(room);
        }
        return desiredRooms;
    }

    // Returns list of rooms that has given entrances.
    // Example: given entrances = 1001, it will find every room with entrances == 1001. 
    public static List<Room> GetRoomsWithExactEntrances(int entrances, List<Room> searchSpace)
    {
        if (searchSpace == null || searchSpace.Count == 0)
        {
            GD.PushError("Trying to find rooms with exact entrances " + entrances + " in an empty search space!");
            return null;
        }
        List<Room> exactEntrancesRooms = new List<Room>();
        foreach (Room room in searchSpace)
        {
            if (room.Entrances == entrances)
            {
                exactEntrancesRooms.Add(room);
            }
        }
        return exactEntrancesRooms;
    }

    public bool IsGenerated()
    {
        return IntRoomType != -1;
    }

    public static String MapEntrancesToName(int entrances)
    {
        if (entrances < 0 || entrances > 15)
        {
            GD.PushError("Error mapping entrances to name!");
            return null;
        }
        String name = "";

        String[] letters = { "T", "R", "B", "L" };
        for (int index = 0; index < 4; index++)
        {
            if (Utils.IsBitEnabled(entrances, index))
            {
                name += letters[index];
            }
        }

        if (name == "")
        {
            return "NONE";
        }

        return name;
    }

    public override string ToString()
    {
        String output = "";
        output += "Room type: " + (RoomType)IntRoomType;
        output += ", Entrances: " + Entrances;
        output += ", Coordinate: " + coordinate.ToString();
        output += ", Neighbours: " + generatedNeighbourCount;
        return output;
    }
}

public enum RoomType
{
    NULL = -1,
    DEFAULT = 0,
    SECRET = 1,
    BOSS = 2,
    SHOP = 3
}
