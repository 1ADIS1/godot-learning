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

    // TODO: Export array of textures.
    [Export]
    private Texture CrossRoom;

    private Grid _map;

    //TODO: max hand size

    public override void _Ready()
    {
        _map = new Grid(GridWidth, GridHeight, CrossRoom);

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
            // TODO: create several components in one object and instantiate it.
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
    public Grid(int width, int height, Texture textures)
    {
        this.width = width;
        this.height = height;

        cells = new ArrayList(width * height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = new Cell(textures);
                Coordinate coordinate = new Coordinate(x, y);

                cells.Insert(GetCoordinateToIndex(coordinate), cell);
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