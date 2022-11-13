using Godot;
using System;
using System.Collections.Generic;

public class Main : Node
{
    // Defines GridSize x GridSize cells
    [Export]
    private int GridSize = 9;

    // Offset of cell in the wolrd coordinates.
    [Export]
    private int rowDrawStep = 20;
    [Export]
    private int columnDrawStep = 20;

    // TODO: Sufficient number of rooms to generate.
    [Export] public int ExpectedNumberOfRooms = 16;

    [Export] public int SecretRoomChance = 50;

    // Queue of generated rooms.
    // TODO: make class "Level", which will contain rooms and other stuff.
    public List<Room> rooms = new List<Room>();

    // Cell to instantiate
    [Export] public PackedScene CellTemplate;

    [Export] public PackedScene StartingCell;

    private Grid _map;

    private CellTemplates _cellTemplates;

    private int _startingCellIndex;

    private List<Coordinate> _secretCells = new List<Coordinate>();

    private int _currentRoomsNumber;

    //TODO: max hand size

    // TODO: current room is filled with blue animated polygon.
    public override void _Ready()
    {
        // TODO: refactor searching for room templates
        _cellTemplates = GetChild<CellTemplates>(0);

        GenerateMap();

        // If generation did not produce expected number of rooms - try again.
        if (_currentRoomsNumber < ExpectedNumberOfRooms)
        {
            GetTree().ReloadCurrentScene();
        }

        // Mark first room as current, last room as boss room
        // TODO: prevent boss room spawning adjacent to the starting cell.
        // TODO: prevent boss room spawning near secret room.
        // TODO: rework this.
        _map.cells[_startingCellIndex].AddChild(_cellTemplates.CurrentCell.Instance<Sprite>());
        _map.cells[_map.CoordinateToIndex(rooms[rooms.Count - 1].GridCell.gridCoordinate)].AddChild(_cellTemplates.BossCell.Instance<Sprite>());
        rooms[rooms.Count - 1].IntRoomType = 4;

        // Generated rooms with one neighbour with chance 50/50 becomes *special room*.
        GenerateSecretRooms();

        PrintRooms();

        // If the cell with one neighbour is not dead end, then spawn secret room adjacent to it. 
    }

    public void PrintRooms()
    {
        GD.Print("\n-------------------------------------------------------------------\n");
        foreach (Room room in rooms)
        {
            GD.Print("Room: ", room.GridCell.Name, " with type ", (RoomType)room.IntRoomType, " and ", room.GridCell.generatedNeighbourCount, " neighbours was added to the queue.");
        }
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

    private void GenerateMap()
    {
        _map = new Grid(GridSize, _cellTemplates.EmptyCell);

        DrawStageMap();

        SetStartingRoomCell(StartingCell.Instance<Cell>());

        // Iterate through each cell that has neighbours and instantiate them.
        GenerateNeighbours(_map.cells[_startingCellIndex]);
    }

    private void GenerateSecretRooms()
    {
        if (_secretCells.Count == 0)
        {
            GD.PushWarning("No secret rooms was generated!");
            return;
        }

        foreach (Coordinate secretCoordinate in _secretCells)
        {
            _map.cells[_map.CoordinateToIndex(secretCoordinate)].AddChild(_cellTemplates.SecretCell.Instance<Sprite>());
            // TODO: set room type in the rooms list
        }
    }

    private void DrawStageMap()
    {
        if (_map.IsEmpty())
        {
            GD.PushError("Tried to draw empty grid!");
            return;
        }

        for (int i = 0; i < _map.cells.Count; i++)
        {
            CreateMapCell(_map.cells[i]);
        }
    }

    // TODO: center the camera.
    private void PositionTheCamera()
    {

    }

    private void CreateMapCell(Cell cell)
    {
        if (_map.IsEmpty())
        {
            GD.PushError("Tried to create map cell in the empty grid!");
            return;
        }

        // Position cell in the world.
        cell.Translate(GetCellWorldCoordinates(cell.gridCoordinate));
        AddChild(cell);
    }

    //TODO: refactor this madness.
    private void SetExistingMapCell(Cell cell, Coordinate coordinate)
    {
        if (_map.IsEmpty())
        {
            GD.PushError("Tried to set existing map cell in the empty grid!");
            return;
        }

        cell.gridCoordinate = coordinate;
        int cellIndex = _map.CoordinateToIndex(cell.gridCoordinate);

        // Remove previous node
        var nodeToRemove = GetNode<Cell>(_map.cells[cellIndex].gridCoordinate.ToString());

        cell.Name = nodeToRemove.Name;
        cell.isGenerated = true;
        cell.GlobalPosition = GetCellWorldCoordinates(nodeToRemove.gridCoordinate);

        _map.cells[cellIndex] = cell;

        // Increment generated neighbour count of adjacent cells.
        // For every generated neighbour, increment this cell's neighbour counter.
        var neighbours = _map.GetNeighbours(cell);
        if (neighbours != null)
        {
            foreach (Cell neighbour in neighbours)
            {
                if (!neighbour.isGenerated)
                {
                    continue;
                }
                _map.cells[_map.CoordinateToIndex(neighbour.gridCoordinate)].generatedNeighbourCount++;
                cell.generatedNeighbourCount++;
            }
        }

        // Increment current number of rooms.
        _currentRoomsNumber++;

        RemoveChild(nodeToRemove);
        nodeToRemove.QueueFree();

        AddChild(cell);
        AddRoom(cell);
        // GD.Print("Added child with name ", cell.Name, " and neighbour count", cell.generatedNeighbourCount);
    }

    private void AddRoom(Cell cell)
    {
        if (!cell.isGenerated)
        {
            GD.PushError("Trying to add non-generated cell to rooms list!");
            return;
        }

        Room room = new Room((int)RoomType.DEFAULT, cell);

        rooms.Add(room);
    }

    // Replaces middle cell in the grid with starting cell and fills in starting cell index;
    private void SetStartingRoomCell(Cell startingCell)
    {
        if (_map.IsEmpty())
        {
            GD.PushError("Tried to set starting map cell in the empty grid!");
            return;
        }

        // TODO: aaaaaand refactor another madness.
        Coordinate startingCellCoordinate = new Coordinate(GridSize / 2, GridSize / 2);
        _startingCellIndex = _map.CoordinateToIndex(startingCellCoordinate);
        startingCell.gridCoordinate = startingCellCoordinate;

        SetExistingMapCell(startingCell, startingCellCoordinate);
    }

    private Vector2 GetCellWorldCoordinates(Coordinate coordinate)
    {
        return new Vector2(coordinate.x * rowDrawStep, coordinate.y * columnDrawStep);
    }

    /**
    Generates given cell's neighbours on the map.
    */
    // TODO: prevent generation of cells with open entrances on the edges of the map.
    // TODO: adjacent cells should have entrances to each other.
    private void GenerateNeighbours(Cell cell)
    {
        if (_cellTemplates == null)
        {
            GD.PushError("Trying to generate cells with empty room templates class!");
            return;
        }
        else if (!cell.HasEntrances())
        {
            return;
        }

        PackedScene[][] cellsWithEntrances = {
            _cellTemplates.BottomEntranceCells,
            _cellTemplates.LeftEntranceCells,
            _cellTemplates.TopEntranceCells,
            _cellTemplates.RightEntranceCells
        };

        Random random = new Random();

        for (int index = 0; index < 4; index++)
        {
            Cell cellToPlace = null;
            Coordinate coordinateToPlace = null;
            Coordinate[] adjacentCoordinates = cell.gridCoordinate.GetAdjacentCoordinates();

            // Check if entrance exists
            if (Utils.IsBitEnabled(cell.Entrances, index))
            {
                // Choose a random entrance cell.
                cellToPlace = cellsWithEntrances[index][random.Next(0, cellsWithEntrances[index].Length)].Instance<Cell>();
                coordinateToPlace = adjacentCoordinates[index];
            }

            // If nothing was chosen.
            if (cellToPlace == null || coordinateToPlace == null)
            {
                continue;
            }

            cellToPlace.gridCoordinate = coordinateToPlace;

            // If coordinate is not valid or cell is already occupied, give up.
            if (!Coordinate.IsValidCoordinate(coordinateToPlace, GridSize) ||
                _map.cells[_map.CoordinateToIndex(coordinateToPlace)].isGenerated)
            {
                continue;
            }

            // If we already have enough rooms, give up.
            // if (_currentRoomsNumber >= ExpectedNumberOfRooms)
            // {
            //     continue;
            // }

            // If coordinateToPlace is on the edge - replace chosen cell with the dead end.
            if (Coordinate.CheckIfEdge(coordinateToPlace, GridSize))
            {
                cellToPlace = _cellTemplates.DeadEndCells[index].Instance<Cell>();
                cellToPlace.gridCoordinate = coordinateToPlace;
            }

            // If cellToPlace has more than one generated neighbour, give up.
            // TODO: with 50% chance spawn instead secret room.
            if (!MoreThanOneNeighbourGenerationCheck(cellToPlace))
            {
                //TODO: spawn secret room with only entrances to the adjacent cells
                if (!TryGenerateSecretCell(cellToPlace))
                {
                    continue;
                }
            }

            // Otherwise, mark the neighbour cell as having a room in it, and add it to the queue.
            SetExistingMapCell(cellToPlace, coordinateToPlace);
            GenerateNeighbours(cellToPlace);
        }
    }

    // TODO: add exceptions.
    // TODO: refactor this bloody chaos.
    // TODO: fix bug when secrets are not generating.
    // Secret room cannot be generate with only one neighbour.
    // Secret room might be generated with 50% chance
    private bool TryGenerateSecretCell(Cell cell)
    {
        // if (!Utils.Chance(SecretRoomChance))
        // {
        //     return false;
        // }

        GD.Print("Cell, ", cell.gridCoordinate, " trying to become a secret room.");

        Coordinate secretRoomCoordinate = cell.gridCoordinate;
        List<Cell> secretCellNeighbours = _map.GetGeneratedNeighbours(cell);
        int secretCellEntrances = 0;
        int entrancesNumber = 0;

        if (secretCellNeighbours == null || secretCellNeighbours.Count < 2)
        {
            GD.Print("Not enough neighbours for secret room!");
            return false;
        }

        // Create entrances for secret room.
        foreach (Cell neighbour in secretCellNeighbours)
        {
            // If neighbour is on the top of the current cell and has entrance to bottom, 
            // then add entrance to the top.
            if (neighbour.gridCoordinate.Equals(cell.gridCoordinate.Top()) && Utils.IsBitEnabled(neighbour.Entrances, 2))
            {
                secretCellEntrances = Utils.EnableBit(secretCellEntrances, 0);
                entrancesNumber++;
            }
            else if (neighbour.gridCoordinate.Equals(cell.gridCoordinate.Bottom()) && Utils.IsBitEnabled(neighbour.Entrances, 0))
            {
                secretCellEntrances = Utils.EnableBit(secretCellEntrances, 2);
                entrancesNumber++;
            }
            else if (neighbour.gridCoordinate.Equals(cell.gridCoordinate.Right()) && Utils.IsBitEnabled(neighbour.Entrances, 3))
            {
                secretCellEntrances = Utils.EnableBit(secretCellEntrances, 1);
                entrancesNumber++;
            }
            else if (neighbour.gridCoordinate.Equals(cell.gridCoordinate.Left()) && Utils.IsBitEnabled(neighbour.Entrances, 1))
            {
                secretCellEntrances = Utils.EnableBit(secretCellEntrances, 3);
                entrancesNumber++;
            }
            else
            { // TODO: get rid? Prevents spawning secrets with no entrance to some cells.
                return false;
            }
        }

        if (entrancesNumber < 2)
        {
            return false;
        }

        // Choose a cell with exact entrances as we defined.
        PackedScene[][] entrances = {
            _cellTemplates.OneEntranceCells,
            _cellTemplates.TwoEntranceCells,
            _cellTemplates.ThreeEntranceCells,
            _cellTemplates.FourEntranceCells
        };

        // Take random corresponding cell in range [0, cells.Length - 1).
        Random random = new Random();
        cell = entrances[entrancesNumber - 1][random.Next(0, entrances[entrancesNumber - 1].Length)].Instance<Cell>();
        cell.gridCoordinate = secretRoomCoordinate;

        _secretCells.Add(cell.gridCoordinate);

        return true;
    }

    private bool MoreThanOneNeighbourGenerationCheck(Cell cellToPlace)
    {
        List<Cell> generatedNeighbours = _map.GetGeneratedNeighbours(cellToPlace);
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
