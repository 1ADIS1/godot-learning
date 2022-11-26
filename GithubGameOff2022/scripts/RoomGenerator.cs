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
            GD.Print("Trying to instantiate room with entrances: ", Cell.MapEntrancesToName(room.Entrances));
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
        if (correspondingTypeAndEntrancesRoom == null || correspondingTypeAndEntrancesRoom.Count == 0)
        {
            GD.PushError("Failed to find room with type " + (RoomType)room.IntRoomType + " and entrances " + Cell.MapEntrancesToName(room.Entrances));
            return;
        }

        // Pick random room.
        Random random = new Random();
        GD.Print("Index of chosen room: " + random.Next(0, correspondingTypeAndEntrancesRoom.Count));
        GD.Print("Count of corresponding rooms with type " + (RoomType)room.IntRoomType + " is " + correspondingTypeAndEntrancesRoom.Count);
        Room roomToInstantiate = correspondingTypeAndEntrancesRoom[random.Next(0, correspondingTypeAndEntrancesRoom.Count)];

        roomToInstantiate.GlobalPosition = GetRoomWorldCoordinates(room.GridCell.gridCoordinate);
        AddChild(roomToInstantiate);
    }

    // TODO: implement 1 to 4 room tree. Each room will have up to 4 children.
    /**
    1) Gets adjacent rooms.
    2) If the current room has door to the adjacent one, which has door to current:
        link door from the current one to adjacent and do the same for the adjacent door.
    */
    private void LinkRoomTransitions()
    {

    }

    private void RemoveDisconnectedDoors()
    {

    }

    public Vector2 GetRoomWorldCoordinates(Coordinate coordinate)
    {
        return new Vector2(coordinate.x * StepSizeX, coordinate.y * StepSizeY);
    }
}
