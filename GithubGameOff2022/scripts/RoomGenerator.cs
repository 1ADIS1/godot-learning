using Godot;
using System;
using System.Collections.Generic;

//TODO: generate corresponding room with entrances;
// TODO: link doors
// TODO: player teleportation.
// Linker of doors and other stuff.
public class RoomGenerator : Node
{
    public Vector2 roomCenterOffset = new Vector2(144f, 96f);

    private RoomTemplates _roomTemplates;

    public void _on_Main_RoomsReady(List<Room> rooms)
    {
        GD.Print("Rooms generation finished signal was received!");
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
}
