using Godot;
using System;
using System.Collections.Generic;

public class Door : Sprite
{
	public Door nextDoor;
	public Room room;
	public readonly Vector2 TOP = new Vector2(144, 16);
	public readonly Vector2 RIGHT = new Vector2(272, 96);
	public readonly Vector2 BOTTOM = new Vector2(144, 176);
	public readonly Vector2 LEFT = new Vector2(16, 96);



public void _on_EnterZone_body_entered(object area)
{
		if (room.isClosedOff)
			return;
		if (nextDoor == null)
		{
			GD.PushError("Trying to enter non-linked door!");
			return;
		}
		Player player = area as Player;
		if (player == null)
			return;
		var direction = GlobalPosition.DirectionTo(nextDoor.GlobalPosition);
		player.SetCentreOfGlobalMassPosition( nextDoor.GlobalPosition+ direction*20);
		(GetNode("/root/Main") as Main).CenterCamera(nextDoor.room);
		nextDoor.room.PlayerWalksIn(player);
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
