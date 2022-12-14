// TODO: check if top, bottom and etc do exist
public class Coordinate
{
    public int x;
    public int y;

    public Coordinate()
    {
        this.x = 0;
        this.y = 0;
    }

    public Coordinate(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Coordinate(Coordinate other)
    {
        this.x = other.x;
        this.y = other.y;
    }

    public override string ToString()
    {
        return x.ToString() + y.ToString();
    }

    public static bool IsValidCoordinate(Coordinate coordinate, int gridSize)
    {
        if (coordinate == null)
        {
            return false;
        }

        if (coordinate.x < 0 || coordinate.y < 0 || coordinate.x >= gridSize || coordinate.y >= gridSize)
        {
            return false;
        }

        return true;
    }

    public Coordinate[] GetAdjacentCoordinates()
    {
        Coordinate[] arr = { Top(), Right(), Bottom(), Left() };
        return arr;
    }

    // Checks if the given coordinate is located on the edge of the map, either on x or y.
    public static bool CheckIfEdge(Coordinate coordinate, int gridSize)
    {
        if (coordinate.x == 0 || coordinate.x == gridSize - 1 ||
            coordinate.y == 0 || coordinate.y == gridSize - 1)
        {
            return true;
        }

        return false;
    }

    public Coordinate Top()
    {
        return new Coordinate(x, y - 1);
    }

    public Coordinate Right()
    {
        return new Coordinate(x + 1, y);
    }

    public Coordinate Bottom()
    {
        return new Coordinate(x, y + 1);
    }

    public Coordinate Left()
    {
        return new Coordinate(x - 1, y);
    }

    public override bool Equals(object obj)
    {
        Coordinate coordinate = (Coordinate)obj;
        if (coordinate == null)
        {
            throw new System.Exception("Error casting object to class Coordinate");
        }
        return x == coordinate.x && y == coordinate.y;
    }

    public override int GetHashCode()
    {
        return (x, y).GetHashCode();
    }

    /**
    Defines operation: Coordinate * multiplier, where multiplier is integer value.
    */
    public static Coordinate operator *(Coordinate coordinate, int multiplier)
        => new Coordinate(coordinate.x * multiplier, coordinate.y * multiplier);
}
