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

    // Sufficient number of rooms to generate.
    [Export] public int EnoughRoomsNumber = 16;

    // Queue of generated rooms.
    public Queue<Cell> rooms = new Queue<Cell>();

    // Cell to instantiate
    [Export]
    public PackedScene CellTemplate;

    [Export]
    public PackedScene StartingCell;

    private Grid _map;

    private RoomTemplates _roomTemplates;

    private int _startingCellIndex;

    //TODO: max hand size

    // TODO: current room is filled with blue animated polygon.
    public override void _Ready()
    {
        // TODO: refactor searching for room templates
        _roomTemplates = GetChild<RoomTemplates>(0);

        GenerateMap();

        PrintRooms();

        // Generated room with one neighbour with chance 50/50 becomes dead-end

        // All dead-ends will be turned into the special rooms

        // If the cell with one neighbour is not dead end, then spawn secret room adjacent to it. 
    }

    public void PrintRooms()
    {
        GD.Print("\n-------------------------------------------------------------------\n");
        foreach (Cell cell in rooms)
        {
            GD.Print("Room: ", cell.Name, " with ", cell.generatedNeighbourCount, " neighbours was added to the queue.");
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
        _map = new Grid(GridSize, _roomTemplates.EmptyCell);

        DrawStageMap();

        SetStartingRoomCell(StartingCell.Instance<Cell>());

        // Iterate through each cell that has neighbours and instantiate them.
        GenerateNeighbours(_map.cells[_startingCellIndex]);

        GD.Print("Neighbour count of starting room is: ", _map.cells[_startingCellIndex].generatedNeighbourCount);
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

        RemoveChild(nodeToRemove);
        nodeToRemove.QueueFree();

        AddChild(cell);
        rooms.Enqueue(cell);
        GD.Print("Added child with name ", cell.Name, " and neighbour count", cell.generatedNeighbourCount);
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
        if (_roomTemplates == null)
        {
            GD.PushError("Trying to generate cells with empty room templates class!");
            return;
        }
        else if (!cell.HasEntrances())
        {
            return;
        }

        PackedScene[][] cellsWithEntrances = {
            _roomTemplates.BottomEntranceRooms,
            _roomTemplates.LeftEntranceRooms,
            _roomTemplates.TopEntranceRooms,
            _roomTemplates.RightEntranceRooms
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

            // If cellToPlace has more than one generated neighbour, give up.
            if (!MoreThanOneNeighbourGenerationCheck(cellToPlace))
            {
                continue;
            }

            // If we already have enough rooms, give up.
            // if ()

            // Random 50% chance, give up.
            // if ()

            // Otherwise, mark the neighbour cell as having a room in it, and add it to the queue.
            SetExistingMapCell(cellToPlace, coordinateToPlace);
            GenerateNeighbours(cellToPlace);
        }
    }

    private bool MoreThanOneNeighbourGenerationCheck(Cell cellToPlace)
    {
        var cellToPlaceNeigbours = _map.GetNeighbours(cellToPlace);
        int generatedNeighboursCount = 0;
        if (cellToPlaceNeigbours == null)
        {
            return false;
        }

        foreach (Cell neighbour in cellToPlaceNeigbours)
        {
            if (!neighbour.isGenerated)
            {
                continue;
            }
            generatedNeighboursCount++;
        }
        if (cellToPlaceNeigbours != null && generatedNeighboursCount > 1)
        {
            return false;
        }

        return true;
    }
}
