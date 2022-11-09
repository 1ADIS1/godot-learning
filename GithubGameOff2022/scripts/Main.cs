using Godot;
using System;
using System.Collections.Generic;
using System.Collections;

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
    }

    // TODO: Listen to Space key to generate another map.
    // public override void _Input(InputEvent @event)
    // {
    //     if (@event is InputEventKey)
    //     {
    //         if (@event.IsActionPressed("ui_select"))
    //         {
    //             GD.Print("Map is being generated again!");
    //             _map = new Grid(GridSize, _roomTemplates.EmptyCell);

    //             SetStartingRoomCell(StartingCell.Instance<Cell>());

    //             // Generate neighbours of starting cell
    //             GenerateNeighbours(_map.cells[_startingCellIndex]);
    //         }
    //     }
    // }

    private void GenerateMap()
    {
        _map = new Grid(GridSize, _roomTemplates.EmptyCell);

        DrawStageMap();

        SetStartingRoomCell(StartingCell.Instance<Cell>());

        // Iterate through each cell that has neighbours and instantiate them.
        GenerateNeighbours(_map.cells[_startingCellIndex]);
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
        // oldCell.cellType = cell.cellType;
        cell.isGenerated = true;
        cell.GlobalPosition = GetCellWorldCoordinates(nodeToRemove.gridCoordinate);

        _map.cells[cellIndex] = cell;

        RemoveChild(nodeToRemove);
        nodeToRemove.QueueFree();

        AddChild(cell);
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
    Places on the map neighbours of the passed cell.
    */
    // TODO: rewrite and optimise generation of neighbours.
    // TODO: prevent generation of cells with open entrances on the edges of the map.
    // TODO: adjacent cells should have entrances to each other.
    private void GenerateNeighbours(Cell cell)
    {
        Vector2 topEntrance = new Vector2(8f, -8f);
        Vector2 rightEntrance = new Vector2(24f, 8f);
        Vector2 bottomEntrance = new Vector2(8f, 24f);
        Vector2 leftEntrance = new Vector2(-8f, 8f);

        var bottomEntrances = _roomTemplates.BottomEntranceRooms;
        var leftEntrances = _roomTemplates.LeftEntranceRooms;
        var topEntrances = _roomTemplates.TopEntranceRooms;
        var rightEntrances = _roomTemplates.RightEntranceRooms;

        Random random = new Random();

        if (_roomTemplates == null)
        {
            GD.PushError("Trying to generate cells with empty room templates class!");
            return;
        }
        else if (!cell.HasEntrances())
        {
            GD.PushWarning("Trying to generate neighbours of cell with no entrances");
            return;
        }

        foreach (Vector2 entry in cell.Entrances)
        {
            Cell cellToPlace = null;
            Coordinate coordinateToPlace = null;

            if (entry == topEntrance)
            {
                // Choose a random Bottom entrance cell.
                cellToPlace = bottomEntrances[random.Next(0, bottomEntrances.Length)].Instance<Cell>();
                coordinateToPlace = cell.gridCoordinate.Top();

                // If coordinate is not valid - do not spawn this cell.
                if (!Coordinate.IsValidCoordinate(coordinateToPlace, GridSize))
                {
                    continue;
                }
            }
            else if (entry == rightEntrance)
            {
                cellToPlace = leftEntrances[random.Next(0, leftEntrances.Length)].Instance<Cell>();
                coordinateToPlace = cell.gridCoordinate.Right();

                // If coordinate is not valid - do not spawn this cell.
                if (!Coordinate.IsValidCoordinate(coordinateToPlace, GridSize))
                {
                    continue;
                }
            }
            else if (entry == bottomEntrance)
            {
                cellToPlace = topEntrances[random.Next(0, topEntrances.Length)].Instance<Cell>();
                coordinateToPlace = cell.gridCoordinate.Bottom();

                // If coordinate is not valid - do not spawn this cell.
                if (!Coordinate.IsValidCoordinate(coordinateToPlace, GridSize))
                {
                    continue;
                }
            }
            else if (entry == leftEntrance)
            {
                cellToPlace = rightEntrances[random.Next(0, rightEntrances.Length)].Instance<Cell>();
                coordinateToPlace = cell.gridCoordinate.Left();

                // If coordinate is not valid - do not spawn this cell.
                if (!Coordinate.IsValidCoordinate(coordinateToPlace, GridSize))
                {
                    continue;
                }
            }

            // Checks if map already contains generated cell on chosen coordinates.
            if (cellToPlace == null || coordinateToPlace == null || _map.cells[_map.CoordinateToIndex(coordinateToPlace)].isGenerated)
            {
                GD.PushWarning("Failed to generate neighbour cell on a coordinate.");
                continue;
            }

            SetExistingMapCell(cellToPlace, coordinateToPlace);
            GenerateNeighbours(cellToPlace);
        }
    }
}

/**
Class for generating N x N grid.
*/
class Grid
{
    public int size;

    public List<Cell> cells;

    public Grid(int size, PackedScene emptyCell)
    {
        this.size = size;

        cells = new List<Cell>(size * size);

        for (int column = 0; column < size; column++)
        {
            for (int row = 0; row < size; row++)
            {
                Coordinate coordinate = new Coordinate(row, column);

                // TODO: optimise code for creating instance of Cell scene.
                Cell cell = emptyCell.Instance<Cell>();
                cell.gridCoordinate = coordinate;
                cell.cellType = CellType.EMPTY;
                cell.Name = coordinate.x.ToString() + coordinate.y.ToString();

                cells.Insert(CoordinateToIndex(coordinate), cell);
            }
        }
    }

    // TODO: check correctness of calculations.
    public int CoordinateToIndex(Coordinate coordinate)
    {
        return coordinate.x + coordinate.y * size;
    }

    // TODO: check correctness of calculations.
    // TODO: If grid is not squared, then the calculations might be wrong.
    public Coordinate IndexToCoordinate(int index)
    {
        return new Coordinate(index % size, index / size);
    }

    public bool IsEmpty()
    {
        return cells.Count == 0;
    }
}

// TODO: check if top, bottom and etc do exist
public class Coordinate
{
    public int x;
    public int y;

    public Coordinate(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return x.ToString() + y.ToString();
    }

    public static bool IsValidCoordinate(Coordinate coordinate, int gridSize)
    {
        if (coordinate.x < 0 || coordinate.y < 0 || coordinate.x >= gridSize || coordinate.y >= gridSize)
        {
            return false;
        }

        return true;
    }

    public Coordinate Top()
    {
        return new Coordinate(x, y - 1);
    }

    public Coordinate Right()
    {
        return new Coordinate(x + 1, y);
    }

    public Coordinate Bottom()
    {
        return new Coordinate(x, y + 1);
    }

    public Coordinate Left()
    {
        return new Coordinate(x - 1, y);
    }
}