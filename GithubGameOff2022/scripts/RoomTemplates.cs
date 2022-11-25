using Godot;
using System.Collections.Generic;

public class RoomTemplates : Node
{
    [Export] public PackedScene[] DefaultRooms;
    [Export] public PackedScene[] SecretRooms;
    [Export] public PackedScene[] BossRooms;

    public List<Room> GetDesiredRooms(int entrances, int roomType)
    {
        List<Room> rooms = new List<Room>();

        PackedScene[][] roomTypes = {
            DefaultRooms,
            SecretRooms,
            BossRooms
        };

        if (roomType > roomTypes.Length)
        {
            GD.PushError("Incorrect room type: " + (RoomType)roomType);
            return null;
        }

        foreach (PackedScene packedScene in roomTypes[roomType])
        {
            Room room = packedScene.Instance<Room>();
            if (room.Entrances == entrances)
            {
                rooms.Add(room);
            }
        }

        return rooms;
    }
}
