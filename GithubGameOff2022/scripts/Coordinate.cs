// TODO: check if top, bottom and etc do exist
public class Coordinate
{
    public int x;
    public int y;

    public Coordinate(int x, int y)
    {
        this.x = x;
        this.y = y;
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
}
