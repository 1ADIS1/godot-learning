using Godot;
using System;
using System.Collections.Generic;

//TODO: generate corresponding room with entrances;
// TODO: link doors
// TODO: teleportation
// Room types generation.
public class RoomGenerator : Node
{
    [Export] public int StepSizeX = 300;
    [Export] public int StepSizeY = 200;

    public Vector2 roomCenterOffset = new Vector2(144f, 96f);

    private RoomTemplates _roomTemplates;

    public void _on_Main_CellsReady(List<Room> rooms)
    {
        _roomTemplates = GetNodeOrNull<RoomTemplates>("RoomTemplates");
        if (_roomTemplates == null)
        {
            GD.PushError("Could not find room templates node!");
            return;
        }

        foreach (Room room in rooms)
        {
            InstantiateRoom(room);
        }
    }

    /*
     Instantiates random room with the same type and entrances in world coordinates.
     */
    private void InstantiateRoom(Room room)
    {
        if (room == null)
        {
            GD.PushError("Trying to instantiate room == null!");
            return;
        }

        List<Room> correspondingTypeAndEntrancesRoom = _roomTemplates.GetDesiredRooms(room.Entrances, room.IntRoomType);
        if (correspondingTypeAndEntrancesRoom == null)
        {
            GD.PushError("Failed to find room with type " + (RoomType)room.IntRoomType + " and entrances " + Cell.MapEntrancesToName(room.Entrances));
        }

        // Pick random room.
        Random random = new Random();
        Room roomToInstantiate = correspondingTypeAndEntrancesRoom[random.Next(0, correspondingTypeAndEntrancesRoom.Count)];

        roomToInstantiate.GlobalPosition = GetRoomWorldCoordinates(room.GridCell.gridCoordinate);
        AddChild(roomToInstantiate);
    }

    public Vector2 GetRoomWorldCoordinates(Coordinate coordinate)
    {
        return new Vector2(coordinate.x * StepSizeX, coordinate.y * StepSizeY);
    }
}
