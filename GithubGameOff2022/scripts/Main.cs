using Godot;
using System;
using System.Collections.Generic;
using System.Collections;

public class Main : Node
{
    [Export]
    public int GridWidth = 9;
    [Export]
    public int GridHeight = 9;

    //TODO: max hand size

    public override void _Ready()
    {
        Grid grid = new Grid(GridWidth, GridHeight);
    }
}

class Grid
{
    int width;
    int height;

    ArrayList cells;

    public Grid(int width, int height)
    {
        this.width = width;
        this.height = height;

        cells = new ArrayList(width * height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = new Cell();
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