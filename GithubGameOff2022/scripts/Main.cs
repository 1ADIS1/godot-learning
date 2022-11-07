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

    // TODO: Export array of cells.
    [Export]
    public PackedScene Cell;

    private Grid _map;

    //TODO: max hand size

    public override void _Ready()
    {
        _map = new Grid(GridWidth, GridHeight, Cell);

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
}

class Grid
{
    public int width;
    public int height;

    public List<Cell> cells;

    public Grid(int width, int height, PackedScene packedCell)
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
                Cell cell = packedCell.Instance<Cell>();
                cell.Texture = cell.EmptyCellTexture;
                cell.gridCoordinate = coordinate;
                cell.cellType = CellType.EMPTY;
                cell.Modulate = new Color(1f, 1f, 1f, 0.5f);

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