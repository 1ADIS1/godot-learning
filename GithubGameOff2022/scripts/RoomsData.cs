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
}
