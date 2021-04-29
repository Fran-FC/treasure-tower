
public enum Direction
{
    N, E, S, O
}

public static class DirectionExtensions
{
    public static Direction Opposite(this Direction direction)
    {
        return (int)direction < 2 ? (direction + 2) : (direction - 2);
    }

}