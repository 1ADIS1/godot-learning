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
    private int rowDrawStep = 16;
    [Export]
    private int columnDrawStep = 16;

    // TODO: Export array of cells.
    // [Export]
    // public PackedScene EmptyCell;

    private Grid _map;

    //TODO: max hand size

    public override void _Ready()
    {
        // EmptyCell = GD.Load<PackedScene>("res://scenes/cells/EmptyCell.tscn");
        _map = new Grid(GridWidth, GridHeight);

        DrawStageMap();
    }

    private void DrawStageMap()
    {
        if (_map == null)
        {
            return;
        }

        for (int i = 0; i < _map.cells.Count; i++)
        {
            // TODO: Instantiate

            // Position cell in the world coordinates.
            var cell = _map.cells[i];
            cell.Translate(new Vector2(cell.gridCoordinate.row * rowDrawStep,
                cell.gridCoordinate.column * columnDrawStep));
            AddChild(cell);
        }
    }
}

class Grid
{
    public int width;
    public int height;

    public List<Cell> cells;

    public Grid(int width, int height)
    {
        this.width = width;
        this.height = height;

        cells = new List<Cell>(width * height);

        for (int row = 0; row < width; row++)
        {
            for (int column = 0; column < height; column++)
            {
                Coordinate coordinate = new Coordinate(row, column);
                Cell cell = new Cell(CellType.EMPTY, coordinate);

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