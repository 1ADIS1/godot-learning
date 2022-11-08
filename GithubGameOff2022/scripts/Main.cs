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

    private int startingCellIndex;

    //TODO: max hand size

    public override void _Ready()
    {
        // TODO: refactor searching for room templates
        _roomTemplates = GetChild<RoomTemplates>(0);

        _map = new Grid(GridWidth, GridHeight, _roomTemplates.EmptyCell);

        DrawStageMap();

        startingCellIndex = SetStartingRoomCell(StartingCell.Instance<Cell>());
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
    private void SetExistingMapCell(Cell cell)
    {
        if (_map.IsEmpty())
        {
            GD.PushError("Tried to set existing map cell in the empty grid!");
            return;
        }

        int cellIndex = _map.CoordinateToIndex(cell.gridCoordinate);
        var oldCell = GetNode<Cell>(_map.cells[cellIndex].gridCoordinate.ToString());

        oldCell.Name = cell.Name;
        oldCell.Texture = cell.Texture;
        oldCell.cellType = cell.cellType;
        oldCell.gridCoordinate = cell.gridCoordinate;
        oldCell.Modulate = new Color(1f, 1f, 1f, 1f);
        oldCell.GlobalPosition = GetCellWorldCoordinates(cell.gridCoordinate);
        _map.cells[cellIndex] = oldCell;
    }

    // TODO: ?
    private void RemoveMapCell(Cell cell)
    {
    }

    // Replaces middle cell in the grid with starting cell and returns its index.
    private int SetStartingRoomCell(Cell startingCell)
    {
        if (_map.IsEmpty())
        {
            GD.PushError("Tried to set starting map cell in the empty grid!");
            return 0;
        }

        Coordinate startingCellCoordinate = new Coordinate(GridWidth / 2, GridHeight / 2);
        int startingCellIndex = _map.CoordinateToIndex(startingCellCoordinate);

        GD.Print("Index of starting cell: ", startingCellIndex);
        startingCell.gridCoordinate = startingCellCoordinate;

        SetExistingMapCell(_map.cells[startingCellIndex]);

        return startingCellIndex;
    }

    private Vector2 GetCellWorldCoordinates(Coordinate coordinate)
    {
        return new Vector2(coordinate.row * rowDrawStep, coordinate.column * columnDrawStep);
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
}