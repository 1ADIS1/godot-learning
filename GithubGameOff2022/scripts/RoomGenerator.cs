using Godot;
using System.Collections.Generic;

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

    private void InstantiateRoom(Room room)
    {
        if (room == null)
        {
            GD.PushError("Trying to instantiate room == null!");
            return;
        }

        switch ((RoomType)room.IntRoomType)
        {
            case RoomType.NULL:
                break;
            case RoomType.DEFAULT:
                Node2D instantiatedRoom = _roomTemplates.FourEntranceRooms[0].Instance<Node2D>();
                instantiatedRoom.GlobalPosition = GetRoomWorldCoordinates(room.GridCell.gridCoordinate);
                AddChild(instantiatedRoom);
                break;
        }
    }

    public Vector2 GetRoomWorldCoordinates(Coordinate coordinate)
    {
        return new Vector2(coordinate.x * StepSizeX, coordinate.y * StepSizeY);
    }
}
