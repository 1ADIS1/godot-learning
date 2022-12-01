using Godot;
using System;
using System.Collections.Generic;
using System.Linq; // Import the C# collection query api



public class Room : Node2D
{
	[Export(PropertyHint.Range, "-1,3,1")]
	public int IntRoomType = -1;

	[Export(PropertyHint.Flags, "Top,Right,Bottom,Left")]
	public int Entrances = 0;

	public Coordinate coordinate = new Coordinate();
	public int generatedNeighbourCount = 0;

	// [Export] public PackedScene CellSprite;

	public List<Door> doors;
	public List<Walker> enemies;
	public bool isClosedOff = false;
	public Room() { 
		}
	public void PlayerWalksIn(Player player){
		isClosedOff = enemies.Count != 0;
		(GetNode("/root/Main/AudioStreamPlayer") as BackgroundPlayer).PlayMusicForType((RoomType)IntRoomType);
	}
	public void EnemyDefeated(Walker enemy){
		enemies.Remove(enemy);
		isClosedOff = enemies.Count != 0;
	}

	public void InitDoors(){
		doors = new List<Door>();
		foreach(var child in GetChildren()){
			var childDoor = child as Door;
			if (childDoor != null){
				doors.Add(childDoor);
				childDoor.room = this;
			}
		}
	}
	public void InitEnemies(){
		enemies = new List<Walker>();
		foreach(var child in GetChildren()){
			var childWalker = child as Walker;
			if (childWalker != null){
				enemies.Add(childWalker);
				childWalker.room = this;
			}
		}
	}

	public Room(Coordinate coordinate, int roomType = -1, int entrances = 0)
	{
		if (roomType < -1 || roomType > 4)
		{
			GD.PushWarning("Tried to create room with room type: " + roomType);
			roomType = -1;
		}

		this.IntRoomType = roomType;
		this.Entrances = entrances;
		this.coordinate = coordinate;

		foreach(var child in GetChildren()){
			var childDoor = child as Door;
			if (childDoor != null){
				GD.PushError(childDoor.Name);
			}
		}
	}

	public bool HasEntrances()
	{
		return Entrances > 0;
	}

	// Example: returns top entrance (1) if the given index is bottom entrance (2).
	// (0, 1, 2, 3) -> (2, 3, 0, 1)
	public static int GetOppositeEntranceIndexOf(int index)
	{
		switch (index)
		{
			case 0:
				return 2;
			case 1:
				return 3;
			case 2:
				return 0;
			case 3:
				return 1;
			default:
				return -1;
		}
	}

	public bool HasEntranceTo(int index)
	{
		int oppositeIndex = GetOppositeEntranceIndexOf(index);
		if (oppositeIndex == -1)
		{
			GD.PushError("Passed wrong entrance index" + index + " to HasEntranceTo!");
			return false;
		}
		return Utils.IsBitEnabled(Entrances, oppositeIndex);
	}

	public static List<Room> GetRoomsThatHasEntranceTo(int index, List<Room> searchSpace)
	{
		if (searchSpace == null || searchSpace.Count == 0)
		{
			GD.PushError("Passed empty rooms list in GetRoomsThatHasEntranceTo!");
			return null;
		}

		List<Room> desiredRooms = new List<Room>();
		foreach (Room room in searchSpace)
		{
			if (!room.HasEntranceTo(index))
			{
				continue;
			}
			desiredRooms.Add(room);
		}
		return desiredRooms;
	}

	// Returns list of rooms that has given entrances.
	// Example: given entrances = 1001, it will find every room with entrances == 1001. 
	public static List<Room> GetRoomsWithExactEntrances(int entrances, List<Room> searchSpace)
	{
		if (searchSpace == null || searchSpace.Count == 0)
		{
			GD.PushError("Trying to find rooms with exact entrances " + entrances + " in an empty search space!");
			return null;
		}
		List<Room> exactEntrancesRooms = new List<Room>();
		foreach (Room room in searchSpace)
		{
			if (room.Entrances == entrances)
			{
				exactEntrancesRooms.Add(room);
			}
		}
		return exactEntrancesRooms;
	}

	public bool IsGenerated()
	{
		return IntRoomType != -1;
	}

	public static String MapEntrancesToName(int entrances)
	{
		if (entrances < 0 || entrances > 15)
		{
			GD.PushError("Error mapping entrances to name!");
			return null;
		}
		String name = "";

		String[] letters = { "T", "R", "B", "L" };
		for (int index = 0; index < 4; index++)
		{
			if (Utils.IsBitEnabled(entrances, index))
			{
				name += letters[index];
			}
		}

		if (name == "")
		{
			return "NONE";
		}

		return name;
	}

	public override string ToString()
	{
		String output = "";
		output += "Room type: " + (RoomType)IntRoomType;
		output += ", Entrances: " + MapEntrancesToName(Entrances);
		output += ", Coordinate: " + coordinate.ToString();
		output += ", Neighbours: " + generatedNeighbourCount;
		return output;
	}

	internal void InitNeighbor(Room neighbor)
	{
		var closestDoorsPairs = doors
			.Select(ourDoor => 
				neighbor.doors.Select(
					theirDoor => (ourDoor,theirDoor,theirDoor.GlobalPosition.DistanceTo(ourDoor.GlobalPosition))
				).OrderBy((tuple => tuple.Item3)).First()
			).OrderBy((tuple => tuple.Item3)).First();
		closestDoorsPairs.ourDoor.nextDoor = closestDoorsPairs.theirDoor;
		closestDoorsPairs.theirDoor.nextDoor = closestDoorsPairs.ourDoor;
	}

	internal void RemoveEmptyDoors()
	{
		doors.ForEach(door =>{
			if (door.nextDoor == null){
				RemoveChild(door);
			}
		});
	}
}

public enum RoomType
{
	NULL = -1,
	DEFAULT = 0,
	SECRET = 1,
	BOSS = 2,
	SHOP = 3
}
