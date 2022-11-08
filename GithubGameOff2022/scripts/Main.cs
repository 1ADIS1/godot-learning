using Godot;
using System;
using System.Collections.Generic;
using System.Collections;

public class Main : Node
{
    [Export]
    private int GridWidth = 9;
    [Export]
    private int GridHeight = 9;

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

    public override void _Ready()
    {
        // TODO: refactor searching for room templates
        _roomTemplates = GetChild<RoomTemplates>(0);

        _map = new Grid(GridWidth, GridHeight, _roomTemplates.EmptyCell);

        DrawStageMap();

        SetStartingRoomCell(StartingCell.Instance<Cell>());

        GD.Print("Starting cell index: ", _startingCellIndex);
        GD.Print("Name of starting cell: ", _map.cells[_startingCellIndex].Name);
        if (_map.cells[_startingCellIndex].Entrances == null)
        {
            GD.PushError("Entrances of cell are null");
            return;
        }

        // Generate neighbours of starting cell
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
        Coordinate startingCellCoordinate = new Coordinate(GridWidth / 2, GridHeight / 2);
        _startingCellIndex = _map.CoordinateToIndex(startingCellCoordinate);
        startingCell.gridCoordinate = startingCellCoordinate;

        SetExistingMapCell(startingCell, startingCellCoordinate);
    }

    private Vector2 GetCellWorldCoordinates(Coordinate coordinate)
    {
        return new Vector2(coordinate.row * rowDrawStep, coordinate.column * columnDrawStep);
    }

    /**
    Places on the map neighbours of the passed cell.
    */
    private void GenerateNeighbours(Cell cell)
    {
        Vector2 topEntrance = new Vector2(8f, -8f);
        Vector2 rightEntrance = new Vector2(24f, 8f);
        Vector2 bottomEntrance = new Vector2(8f, 24f);
        Vector2 leftEntrance = new Vector2(-8f, 8f);

        Random random = new Random();

        if (_roomTemplates == null)
        {
            GD.PushError("Trying to generate cells with empty room templates class!");
        }

        foreach (Vector2 entry in cell.Entrances)
        {
            if (entry == topEntrance)
            {
                // Choose random top entrance cell.
                var bottomEntrances = _roomTemplates.BottomEntranceRooms;
                Cell cellToPlace = bottomEntrances[random.Next(0, bottomEntrances.Length)].Instance<Cell>();

                // TODO: check if top, bottom and etc do exist
                // TODO: fix bug with incrorrect global coordinates
                Coordinate coordinateToPlace = cell.gridCoordinate.Left();

                // TODO: Place selected cell in the grid.
                GD.Print("Cell ", cellToPlace.Name, " will be set on the coords: ", coordinateToPlace);
                cellToPlace.gridCoordinate = coordinateToPlace;
                SetExistingMapCell(cellToPlace, coordinateToPlace);
            }
        }
    }
}

class Grid
{
    public int width;
    public int height;

    public List<Cell> cells;

    public Grid(int width, int height, PackedScene emptyCell)
    {
        this.width = width;
        this.height = height;

        cells = new List<Cell>(width * height);

        for (int row = 0; row < width; row++)
        {
            for (int column = 0; column < height; column++)
            {
                Coordinate coordinate = new Coordinate(row, column);

                // TODO: optimise code for creating instance of Cell scene.
                Cell cell = emptyCell.Instance<Cell>();
                cell.gridCoordinate = coordinate;
                cell.cellType = CellType.EMPTY;
                cell.Name = coordinate.row.ToString() + coordinate.column.ToString();

                cells.Insert(CoordinateToIndex(coordinate), cell);
            }
        }
    }

    // TODO: check correctness of calculations.
    public int CoordinateToIndex(Coordinate coordinate)
    {
        return coordinate.column + coordinate.row * width;
    }

    // TODO: check correctness of calculations.
    // TODO: If grid is not squared, then the calculations might be wrong.
    public Coordinate IndexToCoordinate(int index)
    {
        return new Coordinate(index / width, index % height);
    }

    public bool IsEmpty()
    {
        return cells.Count == 0;
    }
}

public class Coordinate
{
    public int row;
    public int column;

    public Coordinate(int row, int column)
    {
        this.row = row;
        this.column = column;
    }

    public override string ToString()
    {
        return row.ToString() + column.ToString();
    }

    public Coordinate Top()
    {
        return new Coordinate(row - 1, column);
    }

    public Coordinate Right()
    {
        return new Coordinate(row, column + 1);
    }

    public Coordinate Bottom()
    {
        return new Coordinate(row + 1, column);
    }

    public Coordinate Left()
    {
        return new Coordinate(row, column - 1);
    }
}