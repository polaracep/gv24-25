public enum Rotation
{
    DOWN,
    LEFT,
}

public abstract class Boat
{
    public (int X, int Y) startPos { get; set; }
    public int size { get; protected set; }
    public Rotation rotation { get; set; }
    public Tile appearance { get; protected set; }

    public Boat((int x, int y) _startPos, Rotation rot, Tile app)
    {
        startPos = _startPos;
        appearance = app;
        rotation = rot;
    }
    public Boat() : this((0, 0), Rotation.LEFT, new Tile("B", ConsoleColor.White, ConsoleColor.Blue))
    {
    }
}

class SimpleBoat : Boat
{
    public SimpleBoat((int x, int y) _startPos, Rotation rot) :
        base(_startPos, rot, new Tile("S", ConsoleColor.Green, ConsoleColor.DarkGreen))
    {
        this.size = 2;
    }
    public SimpleBoat() : this((0, 0), Rotation.LEFT)
    {
    }

}

class LongerBoat : Boat
{
    public LongerBoat((int x, int y) _startPos, Rotation rot) :
        base(_startPos, rot, new Tile("T", ConsoleColor.Cyan, ConsoleColor.DarkGreen))
    {
        this.size = 3;
    }
    public LongerBoat() : this((0, 0), Rotation.LEFT)
    {
    }
}

class LongestBoat : Boat
{
    public LongestBoat((int x, int y) _startPos, Rotation rot) :
        base(_startPos, rot, new Tile("T", ConsoleColor.Magenta, ConsoleColor.DarkGreen))
    {
        this.size = 5;
    }
    public LongestBoat() : this((0, 0), Rotation.LEFT)
    {
    }
}
