using Godot;
using System.Collections.Generic;

/**
Class for generating N x N grid of rooms.
*/
class Grid
{
    public int n;

    public List<Room> rooms;

    public Grid(int n)
    {
        this.n = n;

        rooms = new List<Room>(n * n);

        for (int column = 0; column < n; column++)
        {
            for (int row = 0; row < n; row++)
            {
                Coordinate coordinate = new Coordinate(row, column);

                Room room = new Room(coordinate);
                room.Name = coordinate.x.ToString() + coordinate.y.ToString();

                rooms.Insert(CoordinateToIndex(coordinate), room);
            }
        }
    }

    // TODO: check correctness of calculations.
    public int CoordinateToIndex(Coordinate coordinate)
    {
        return coordinate.x + coordinate.y * n;
    }

    // TODO: check correctness of calculations.
    // TODO: If grid is not squared, then the calculations might be wrong.
    public Coordinate IndexToCoordinate(int index)
    {
        return new Coordinate(index % n, index / n);
    }

    /**
    Returns adjacent rooms (4 max).

    If onlyGeneratedNeighbours is true - returns rooms, 
    which were generated in the grid and are adjacent to the current one.
    */
    public List<Room> GetNeighbours(Room room, bool onlyGeneratedNeighbours = false)
    {
        List<Room> neighbours = new List<Room>(room.generatedNeighbourCount);

        if (!Coordinate.IsValidCoordinate(room.coordinate, n))
        {
            GD.PushError("Trying to get neighbours of cell {" + room.Name + "} with invalid coordinate!");
            return neighbours;
        }

        var adjacentCoordinates = room.coordinate.GetAdjacentCoordinates();
        foreach (Coordinate adjacentCoordinate in adjacentCoordinates)
        {
            if (!Coordinate.IsValidCoordinate(adjacentCoordinate, n))
            {
                continue;
            }

            Room neighbour = rooms[CoordinateToIndex(adjacentCoordinate)];

            if (!onlyGeneratedNeighbours)
            {
                neighbours.Add(neighbour);
            }

            if (neighbour.IsGenerated())
            {
                neighbours.Add(neighbour);
            }
        }

        return neighbours;
    }

    public bool IsEmpty()
    {
        return rooms.Count == 0;
    }
}
