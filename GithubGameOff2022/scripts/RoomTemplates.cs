using Godot;
using System;
using System.Collections.Generic;

public class RoomTemplates : Node
{
    [Export] public PackedScene[] DefaultRooms;
    [Export] public PackedScene[] SecretRooms;
    [Export] public PackedScene[] BossRooms;
    [Export] public PackedScene[] ShopRooms;

    // Contains lists of rooms by types in order: default, secret, boss, and etc.
    public List<RoomsData> roomsDatas = new List<RoomsData>();

    // Special room is always a dead-end (room with only one entrance) and of any room type, except:
    // Null, Secret, Boss.
    public List<Room> specialRooms = new List<Room>();

    public void InitializeRoomTemplates()
    {
        Array roomTypes = Enum.GetValues(typeof(RoomType));
        List<PackedScene[]> scenesByTypes = new List<PackedScene[]>{
            DefaultRooms,
            SecretRooms,
            BossRooms,
            ShopRooms
        };
        if (scenesByTypes.Count != roomTypes.Length - 1)
        {
            GD.PushError(GetClass() + ": Number of room types " + roomTypes.Length
                + " and room scenes " + scenesByTypes.Count + " do not match!");
            return;
        }
        foreach (RoomType roomType in roomTypes)
        {
            // TODO: don't skip null type, but rather try to make some I'M ERROR type of rooms.
            if (roomType == RoomType.NULL)
            {
                continue;
            }

            // For every type of rooms, instantiates its scenes and splits them by entrances. 
            roomsDatas.Add(new RoomsData(scenesByTypes[(int)roomType], roomType));
        }

        specialRooms = GetSpecialRooms();
    }

    private List<Room> GetSpecialRooms()
    {
        if (roomsDatas == null || roomsDatas.Count == 0)
        {
            GD.PushError(GetClass() + " : Tried to get special room with empty rooms datas!");
            return null;
        }
        List<Room> specialRooms = new List<Room>();
        List<RoomsData> searchSpace = new List<RoomsData>();
        searchSpace.Add(roomsDatas[(int)RoomType.DEFAULT]);
        searchSpace.Add(roomsDatas[(int)RoomType.SHOP]);

        foreach (Room room in specialRooms)
        {
            // If this is a dead-end room.
            if (Utils.CountEnabledBits(room.Entrances) != 1)
            {
                continue;
            }
            specialRooms.Add(room);
        }
        return specialRooms;
    }
}
