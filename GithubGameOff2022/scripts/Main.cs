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
    public PackedScene Cell;

    private Grid _map;

    private RoomTemplates _roomTemplates;

    //TODO: max hand size

    public override void _Ready()
    {
        // TODO: refactor searching for room templates
        _roomTemplates = GetChild<RoomTemplates>(0);

        _map = new Grid(GridWidth, GridHeight, _roomTemplates.EmptyCell);

        DrawStageMap();
    }

    private void DrawStageMap()
    {
        if (_map == null)
        {
            GD.PushError("Tried to draw empty grid!");
            return;
        }

        for (int i = 0; i < _map.cells.Count; i++)
        {
            var cell = _map.cells[i];

            // Position cell in the world.
            cell.Translate(new Vector2(cell.gridCoordinate.row * rowDrawStep,
                cell.gridCoordinate.column * columnDrawStep));
            AddChild(cell);
        }
    }

    // TODO: center the camera.
    private void PositionTheCamera()
    {

    }

    // TODO: Spawn rooms cells
    private void PlaceMapCell()
    {
        if (_map == null)
        {
            GD.PushError("Tried to place map cell in the empty grid!");
            return;
        }
    }

    // TODO: set starting room cell and return its index.
    // Assumes starting cell is cross entrance room in the middle of the grid.
    // Places starting room in the grid and returns its index.
    private int SetStartingRoomCell()
    {
        // _map.cells[]

        return 0;
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
}