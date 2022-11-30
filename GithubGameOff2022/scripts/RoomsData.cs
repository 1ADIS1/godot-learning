using Godot;
using System.Collections.Generic;

// Class for managing room scenes.
public class RoomsData
{
    PackedScene[] roomScenes;
    RoomType roomsType;

    public List<List<Room>> roomsByEntrances = new List<List<Room>>{
        new List<Room>(), // entrances to the top
        new List<Room>(), // entrances to the right
        new List<Room>(), // entrances to the bottom
        new List<Room>() // entrances to the left
    };

    public RoomsData(PackedScene[] roomScenes, RoomType roomsType = RoomType.DEFAULT)
    {
        this.roomScenes = roomScenes;
        this.roomsType = roomsType;

        SplitRoomsByEntrances();
    }

    private void SplitRoomsByEntrances()
    {
        if (roomScenes == null || roomScenes.Length == 0)
        {
            GD.PushError("Rooms data of type " + roomsType + " was instantiated without room scenes!");
            return;
        }
        foreach (PackedScene scene in roomScenes)
        {
            Room room = scene.Instance<Room>();
            for (int index = 0; index < 4; index++)
            {
                if (Utils.IsBitEnabled(room.Entrances, index))
                {
                    roomsByEntrances[index].Add(room);
                }
            }
        }
    }

    // Gets index of room entrance, i.e. 0 - top, 1 - right, 2 - bottom, 3 - left
    // And returns corresponding rooms with entrance to the opposite side of this entrance. 
    // TODO: replace switch with formula for mapping (0, 1, 2, 3) -> (2, 3, 0, 1)
    public List<Room> GetRoomsWithEntranceTo(int entranceIndex)
    {
        List<Room> roomsWithEntranceTo = new List<Room>();
        switch (entranceIndex)
        {
            case 0:
                roomsWithEntranceTo = roomsByEntrances[2];
                break;
            case 1:
                roomsWithEntranceTo = roomsByEntrances[3];
                break;
            case 2:
                roomsWithEntranceTo = roomsByEntrances[0];
                break;
            case 3:
                roomsWithEntranceTo = roomsByEntrances[1];
                break;
        }
        return roomsWithEntranceTo;
    }
}
