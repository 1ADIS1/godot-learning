using Godot;
using System;
using System.Collections.Generic;

public class Door : Sprite
{
    public Door nextDoor;

    public readonly Vector2 TOP = new Vector2(144, 16);
    public readonly Vector2 RIGHT = new Vector2(272, 96);
    public readonly Vector2 BOTTOM = new Vector2(144, 176);
    public readonly Vector2 LEFT = new Vector2(16, 96);

    public void _on_EnterZone_area_entered()
    {
        if (nextDoor == null)
        {
            GD.PushError("Trying to enter non-linked door!");
            return;
        }
        Player player = GetNode<Player>("Player");
        player.GlobalPosition = nextDoor.GlobalPosition;
    }

    public override void _Ready()
    {

    }

    // private void InitializeDoor()
    // {
    //     switch (Position)
    //     {
    //         case TOP:
    //             break;
    //     }
    // }
}

public enum DoorType
{
    TOP,
    RIGHT,
    BOTTOM,
    LEFT
}