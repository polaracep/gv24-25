public class Game
{
    public string messageLog { get; set; } = "";


    static bool StartGame(PlayingField field, Player player_1, Player player_2)
    {

        return true;
    }

    public static bool GameLoop()
    {
        // Hrac zadava tah

        // Check trefy
        // -> Hrac strili znova pokud trefil

        // opakujem dokud jeden hrac nema zadny lode

        return true;
    }


    public static void Render()
    {
        //PlayingField.field[0,]

    }

}

public class PlayingField
{
    // klic: 
    // w - voda
    //

    public char[] availableTiles = { 'w', 'b' };

    public int sizeX { get; private set; }
    public int sizeY { get; private set; }

    public char[,] field { get; private set; }

    public PlayingField(int x, int y)
    {
        sizeX = x;
        sizeY = y;
        field = new char[sizeX, sizeY];

        // nacpat vsude vodu
        for (int _x = 0; _x < x; _x++)
            for (int _y = 0; _y < y; _y++)
                field[_x, _y] = 'w';
    }

    public void SetTile(char tile, int x, int y)
    {
        if (x >= sizeX || y >= sizeY)
            throw new Exception("Tile out of bounds");

        if (!availableTiles.Contains(tile))
            throw new Exception("Unknown tile");

        field[x, y] = tile;
    }

    public char GetTile(int x, int y)
    {
        if (x >= sizeX || y >= sizeY)
            throw new Exception("Tile out of bounds");

        return field[x, y];
    }

    // for debug
    public void PrintField()
    {
        for (int i = 0; i < sizeX; i++) // Iterate over rows
        {
            for (int j = 0; j < sizeY; j++) // Iterate over columns
            {
                Console.Write(this.field[i, j] + " "); // Print each element
            }
            Console.WriteLine(); // Move to the next line after each row
        }

    }

    public int Size()
    {
        return field.Length;
    }
}

public class Player
{
    private int pts = 0;
    public PlayingField field { get; private set; }

    public Player(PlayingField myField)
    {
        this.field = myField;
    }
}

public class PlayerHuman : Player
{
    public PlayerHuman(PlayingField myField) : base(myField) { }
}

public class PlayerComputer : Player
{
    public PlayerComputer(PlayingField myField) : base(myField) { }
}

