using Godot;
using System;
using System.Collections.Generic;

// TODO: move all room generation process to RoomGenerator class
public class Main : Node
{
    [Signal]
    delegate void RoomsReady(List<Room> rooms);

    // Defines GridSize x GridSize rooms
    [Export]
    private int GridSize = 9;

    // Offset for spawning rooms in the world coordinates.
    [Export] public int RoomOffsetX = 300;
    [Export] public int RoomOffsetY = 200;
    [Export] public Vector2 roomCenterOffset = new Vector2(144f, 96f);

    // TODO: Sufficient number of rooms to generate.
    [Export] public int ExpectedRoomsNumber = 16;

    [Export] public int SecretRoomsNumber = 2;

    // TODO: move to "Level.cs".
    public Room startingRoom;

    // TODO: make array of various starting rooms :D and instantiate one of them.
    [Export] public PackedScene StartingRoomScene;

    private Grid _map;
    private int _mapRadius = 0;

    private RoomTemplates _roomTemplates;

    private int _startingRoomIndex;
    private int _bossRoomIndex;

    private List<Coordinate> PossibleCoordinatesForSecretRooms = new List<Coordinate>();
    private List<Coordinate> _secretRooms = new List<Coordinate>();

    private int _currentRoomsNumber;

    public override void _Ready()
    {
        // TODO: refactor searching for room templates
        _roomTemplates = GetNode<RoomTemplates>("RoomTemplates");
        _roomTemplates.InitializeRoomTemplates();
        _map = new Grid(GridSize);

        SetStartingRoom(StartingRoomScene.Instance<Room>());

        // Iterate through each room that has neighbours and instantiate them.
        GenerateNeighbours(_map.rooms[_startingRoomIndex]);

        // If generation did not produce expected number of rooms - try again.
        if (_currentRoomsNumber < ExpectedRoomsNumber)
        {
            GetTree().ReloadCurrentScene();
        }

        // TODO: refactor.
        // Room farthestRoom = startingRoom;
        // CalculateMapRadius(farthestRoom, _mapRadius);
        // GD.Print("The farthest room is: ", farthestRoom);
        // GD.Print("Map radius is: ", _mapRadius);

        // TODO: refactor.
        // Find boss room.
        // _map.rooms[_map.CoordinateToIndex(farthestRoom.GridCell.gridCoordinate)].AddChild(_roomTemplates.BossCell.Instance<Sprite>());
        // farthestRoom.IntRoomType = (int)RoomType.BOSS;

        // Generate secret rooms with neigbour more than two.
        // TODO: fix secret room generating adjacent to another secret room.
        // TODO: fix secret room generating adjacent to boss room.
        // TODO: add entrances from adjacent neighbours to secret room.
        if (PossibleCoordinatesForSecretRooms == null || PossibleCoordinatesForSecretRooms.Count == 0)
        {
            GD.PushError("Possible coordinates for secret rooms are empty!");
            return;
        }
        foreach (Coordinate possibleSecretRoomCoordinate in PossibleCoordinatesForSecretRooms)
        {
            if (SecretRoomsNumber == _secretRooms.Count)
            {
                GD.Print("Secret rooms generation finished");
                break;
            }
            GenerateSecretRoom(possibleSecretRoomCoordinate);
        }
        if (_secretRooms.Count < SecretRoomsNumber)
        {
            GD.PushError("Failed to generate " + SecretRoomsNumber + " secret rooms");
        }

        PrintRooms();

        EmitSignal(nameof(RoomsReady), _map.rooms);

        PositionPlayerAndCamera();
    }

    // TODO: refactor.
    /**
    returns the distance between the given room and the furthest room.
    */
    // public void CalculateMapRadius(Room start, int radius)
    // {
    //     if (start == null || start.generatedNeighbourCount == 0)
    //     {
    //         GD.PushError("Trying to find radius of room being null or lacking neighbours!");
    //         return;
    //     }

    //     for (int i = 0; i < start.generatedNeighbourCount; i++)
    //     {
    //         if (start.neighbours[i] == null)
    //         {
    //             return;
    //         }

    //         CalculateMapRadius(start.neighbours[i], radius++);
    //     }
    // }

    public void PrintRooms()
    {
        GD.Print("\n-------------------------------------------------------------------\n");
        foreach (Room room in _map.rooms)
        {
            GD.Print(room.ToString());
        }
        GD.Print("Number of rooms in grid is ", _map.rooms.Count);
        GD.Print("\n-------------------------------------------------------------------\n");
    }

    // Listen to Space key to restart scene.
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey)
        {
            if (@event.IsActionPressed("ui_select"))
            {
                GetTree().ReloadCurrentScene();
            }
        }
    }

    private void PositionPlayerAndCamera()
    {
        GD.Print("Centering the player and camera...");

        Camera2D camera = GetNodeOrNull<Camera2D>("Camera2D");
        if (camera == null)
        {
            GD.PushError("Camera cannot be found!");
            return;
        }

        Player player = GetNode<Player>("Player");
        player.GlobalPosition = _map.rooms[_startingRoomIndex].GlobalPosition + roomCenterOffset;
        camera.GlobalPosition = player.GlobalPosition;
    }

    private void SetRoom(Room room, Coordinate coordinate)
    {
        if (_map.IsEmpty())
        {
            GD.PushError("Tried to set room in the empty grid!");
            return;
        }

        room.coordinate = coordinate;
        int roomIndex = _map.CoordinateToIndex(room.coordinate);

        // TODO: should I delete roomToReplace from scene?
        var roomToReplace = _map.rooms[roomIndex];
        GD.Print("#####");
        GD.Print("Replacing room: " + roomToReplace.ToString());
        GD.Print("With room: ", room.ToString());
        GD.Print("#####");

        room.Name = roomToReplace.Name;
        room.GlobalPosition = GetRoomWorldCoordinates(roomToReplace.coordinate);

        // Increment generated neighbour count of adjacent rooms.
        // For every generated neighbour, increment this room's neighbour counter.
        var neighbours = _map.GetNeighbours(room, true);
        room.generatedNeighbourCount = 0;
        if (neighbours != null)
        {
            foreach (Room neighbour in neighbours)
            {
                _map.rooms[_map.CoordinateToIndex(neighbour.coordinate)].generatedNeighbourCount++;
                room.generatedNeighbourCount++;
            }
        }
        _currentRoomsNumber++;

        _map.rooms[roomIndex] = room;

        AddChild(room.Duplicate());
    }

    // Replaces middle room in the grid with starting room and fills in starting room index;
    private void SetStartingRoom(Room startingRoom)
    {
        if (_map.IsEmpty())
        {
            GD.PushError("Tried to set starting room in the empty grid!");
            return;
        }

        startingRoom.coordinate = new Coordinate(GridSize / 2, GridSize % 2);
        _startingRoomIndex = _map.CoordinateToIndex(startingRoom.coordinate);

        SetRoom(startingRoom, startingRoom.coordinate);
    }

    // Boss room is always the furthest one from the start (estimated by radius from starting room)
    // TODO: Boss room must be always a dead-end. 
    private void SetBossRoom(Room bossRoom)
    {

    }

    private Vector2 GetRoomWorldCoordinates(Coordinate coordinate)
    {
        return new Vector2(coordinate.x * RoomOffsetX, coordinate.y * RoomOffsetY);
    }

    /**
    Generates given rooms's neighbours on the map.
    */
    // TODO: take random direction.
    private void GenerateNeighbours(Room room)
    {
        if (_roomTemplates == null)
        {
            GD.PushError("Trying to generate rooms with empty room templates class!");
            return;
        }
        else if (!room.HasEntrances())
        {
            GD.PushError(GetClass() + ": room does not have entrances!");
            return;
        }

        if (room.IntRoomType == (int)RoomType.NULL)
        {
            GD.PushError(GetClass() + ": Trying to generate room with type null!");
            return;
        }
        // Gets data of the rooms of the given type.
        RoomsData possibleRooms = _roomTemplates.roomsDatas[room.IntRoomType];

        Random random = new Random();

        for (int index = 0; index < 4; index++)
        {
            Room roomToPlace = null;
            Coordinate coordinateToPlace = null;
            Coordinate[] adjacentCoordinates = room.coordinate.GetAdjacentCoordinates();

            // Check if entrance exists.
            if (Utils.IsBitEnabled(room.Entrances, index))
            {
                // Choose a random room with entrance opposite of the current one.
                List<Room> roomsWithEntranceToIndex = Room.GetRoomsThatHasEntranceTo(index,
                    possibleRooms.roomsByEntrances[Room.GetOppositeEntranceIndexOf(index)]);

                if (roomsWithEntranceToIndex == null || roomsWithEntranceToIndex.Count == 0)
                {
                    GD.PushError(GetClass() + ": Failed to find rooms with entrances to entrance " + Room.MapEntrancesToName(index));
                    continue;
                }
                roomToPlace = roomsWithEntranceToIndex[random.Next(roomsWithEntranceToIndex.Count)];
                coordinateToPlace = adjacentCoordinates[index];
            }

            // If nothing was chosen.
            if (roomToPlace == null || coordinateToPlace == null)
            {
                continue;
            }

            roomToPlace.coordinate = coordinateToPlace;

            // If coordinate is not valid or room is already occupied, give up.
            if (!Coordinate.IsValidCoordinate(coordinateToPlace, GridSize) ||
                _map.rooms[_map.CoordinateToIndex(coordinateToPlace)].IsGenerated())
            {
                continue;
            }

            // If chosen coordinate is on the edge - replace chosen room with special one.
            if (Coordinate.CheckIfEdge(coordinateToPlace, GridSize))
            {
                GD.Print("Coordinate ", coordinateToPlace.ToString(), " is edge!");
                // Get special rooms with entrance to the passed entrance.
                if (_roomTemplates.specialRooms == null || _roomTemplates.specialRooms.Count == 0)
                {
                    GD.PushError(GetClass() + ": special rooms list is empty or null!");
                    continue;
                }
                List<Room> viableEntranceSpecialRooms = Room.GetRoomsThatHasEntranceTo(index, _roomTemplates.specialRooms);
                if (viableEntranceSpecialRooms == null || viableEntranceSpecialRooms.Count == 0)
                {
                    GD.PushError(GetClass() + ": couldn't find special rooms with entrance to " + Room.MapEntrancesToName(index));
                    continue;
                }
                roomToPlace = viableEntranceSpecialRooms[random.Next(viableEntranceSpecialRooms.Count)];
                roomToPlace.coordinate = coordinateToPlace;
            }

            // If cellToPlace has more than one generated neighbour, give up.
            // TODO: with 50% chance spawn instead secret room.
            if (!MoreThanOneNeighbourGenerationCheck(roomToPlace))
            {
                PossibleCoordinatesForSecretRooms.Add(roomToPlace.coordinate);
                continue;
            }

            SetRoom(roomToPlace, coordinateToPlace);
            GenerateNeighbours(roomToPlace);
        }
    }

    // TODO: refactor.
    // TODO: fix bug that from some rooms there could be no entrance to secret room.
    // TODO: Secret room might be generated with 50% chance.

    // Secret room will make entrances to itself through the neighbours.
    // Secret room cannot be generated with less than two neighbours.
    private bool GenerateSecretRoom(Coordinate coordinate)
    {
        Room room = _map.rooms[_map.CoordinateToIndex(coordinate)];
        List<Room> secretRoomNeighbours = _map.GetNeighbours(room, true);
        int secretRoomEntrances = 0;
        int entrancesNumber = 0;

        if (secretRoomNeighbours == null || secretRoomNeighbours.Count < 2)
        {
            return false;
        }

        // Create entrances for secret room.
        foreach (Room neighbour in secretRoomNeighbours)
        {
            // If neighbour is on the top of the current cell and has entrance to bottom, 
            // then add entrance to the top.

            if (neighbour.coordinate.Equals(room.coordinate.Top()) && neighbour.HasEntranceTo(2))
            {
                secretRoomEntrances = Utils.EnableBit(secretRoomEntrances, 0);
                entrancesNumber++;
            }
            else if (neighbour.coordinate.Equals(room.coordinate.Bottom()) && neighbour.HasEntranceTo(0))
            {
                secretRoomEntrances = Utils.EnableBit(secretRoomEntrances, 2);
                entrancesNumber++;
            }
            else if (neighbour.coordinate.Equals(room.coordinate.Right()) && neighbour.HasEntranceTo(3))
            {
                secretRoomEntrances = Utils.EnableBit(secretRoomEntrances, 1);
                entrancesNumber++;
            }
            else if (neighbour.coordinate.Equals(room.coordinate.Left()) && neighbour.HasEntranceTo(1))
            {
                secretRoomEntrances = Utils.EnableBit(secretRoomEntrances, 3);
                entrancesNumber++;
            }
        }

        if (entrancesNumber < 2)
        {
            return false;
        }

        // Choose a random secret room with exact entrances as we defined.
        var secretRoomsByEntrances = _roomTemplates.roomsDatas[(int)RoomType.SECRET].roomsByEntrances;
        List<Room> suitableRooms = new List<Room>();
        foreach (List<Room> secretRoomsByEntrance in secretRoomsByEntrances)
        {
            List<Room> foundEntrances = Room.GetRoomsWithExactEntrances(secretRoomEntrances, secretRoomsByEntrance);
            if (foundEntrances != null || foundEntrances.Count != 0)
            {
                suitableRooms.AddRange(foundEntrances);
            }
        }

        if (suitableRooms == null || suitableRooms.Count == 0)
        {
            GD.PushError("Failed to find suitable secret rooms with entrances " + Room.MapEntrancesToName(secretRoomEntrances));
            return false;
        }
        Random random = new Random();
        Room chosenSuitableSecretRoom = suitableRooms[random.Next(suitableRooms.Count)];

        if (chosenSuitableSecretRoom == null)
        {
            GD.PushError("Failed to find a secret room with entrances: " + Room.MapEntrancesToName(secretRoomEntrances));
            return false;
        }

        chosenSuitableSecretRoom.coordinate = room.coordinate;
        room = chosenSuitableSecretRoom;

        SetRoom(room, room.coordinate);
        _secretRooms.Add(room.coordinate);

        return true;
    }

    private bool MoreThanOneNeighbourGenerationCheck(Room roomToPlace)
    {
        List<Room> generatedNeighbours = _map.GetNeighbours(roomToPlace, true);
        if (generatedNeighbours == null || generatedNeighbours.Count == 0)
        {
            return false;
        }

        if (generatedNeighbours.Count > 1)
        {
            return false;
        }

        return true;
    }
}
