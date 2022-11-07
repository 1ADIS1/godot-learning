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

    // TODO: Export array of cells.
    [Export]
    public PackedScene EmptyCell;

    private Grid _map;

    //TODO: max hand size

    public override void _Ready()
    {
        EmptyCell = GD.Load<PackedScene>("res://scenes/cells/EmptyCell.tscn");
        _map = new Grid(GridWidth, GridHeight, EmptyCell.Instance());

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
            // AddChild(_map.cells[i]);
        }
    }
}

class Grid
{
    public int width;
    public int height;

    public ArrayList cells;

    // TODO: pass array of textures.
    public Grid(int width, int height, Node emptyCell)
    {
        this.width = width;
        this.height = height;

        cells = new ArrayList(width * height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // TODO: make correct offset of empty cell in the world coordinates
                Coordinate coordinate = new Coordinate(x, y);

                cells.Insert(GetCoordinateToIndex(coordinate), emptyCell);
            }
        }
    }

    public int GetCoordinateToIndex(Coordinate coordinate)
    {
        return 0;
    }
}

class Coordinate
{
    int x;
    int y;

    public Coordinate(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}