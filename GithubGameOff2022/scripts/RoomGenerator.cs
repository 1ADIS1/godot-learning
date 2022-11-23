using Godot;
using System.Collections.Generic;

public class RoomGenerator : Node
{
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
            // Spawn corresponding room from room templates.
            switch ((RoomType)room.IntRoomType)
            {
                case RoomType.NULL:
                    break;
                case RoomType.DEFAULT:

                    break;
            }
        }

        AddChild(_roomTemplates.FourEntranceRooms[0].Instance<Room>());
    }
}
