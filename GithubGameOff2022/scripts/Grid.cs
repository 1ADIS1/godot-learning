using Godot;
using System.Collections.Generic;
using System.Collections;
using System;

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

    /**
    Returns adjacent cells (4 max).

    If onlyGeneratedNeighbours is true - returns cells, 
    which were generated in the grid and are adjacent to the current one.
    */
    public List<Cell> GetNeighbours(Cell cell, bool onlyGeneratedNeighbours = false)
    {
        List<Cell> neighbours = new List<Cell>(cell.generatedNeighbourCount);

        // TODO: fix bug, when trying getting neighbours of dead end cell on the edge, function gives null.
        if (!Coordinate.IsValidCoordinate(cell.gridCoordinate, size))
        {
            GD.PushError("Trying to get neighbours of cell {" + cell.Name + "} with invalid coordinate!");
            return neighbours;
        }

        var adjacentCoordinates = cell.gridCoordinate.GetAdjacentCoordinates();
        foreach (Coordinate adjacentCoordinate in adjacentCoordinates)
        {
            if (!Coordinate.IsValidCoordinate(adjacentCoordinate, size))
            {
                continue;
            }

            Cell neighbour = cells[CoordinateToIndex(adjacentCoordinate)];

            if (!onlyGeneratedNeighbours)
            {
                neighbours.Add(neighbour);
            }

            if (neighbour.isGenerated)
            {
                neighbours.Add(neighbour);
            }
        }

        return neighbours;
    }

    public bool IsEmpty()
    {
        return cells.Count == 0;
    }
}
