public class Game
{
    public static string messageLog { get; set; } = "";
    private Player player1;
    private Player player2;

    public Game(Player player_1, Player player_2)
    {
        this.player1 = player_1;
        this.player2 = player_2;
    }

    public bool GameLoop()
    {
        // Hrac zadava tah

        // Check trefy
        // -> Hrac strili znova pokud trefil

        // opakujem dokud jeden hrac nema zadny lode

        (int x, int y) cPos = Console.GetCursorPosition();
        Console.BackgroundColor = ConsoleColor.Red;
        Console.Write("󰩷");
        Console.ResetColor();

        Console.SetCursorPosition(cPos.x, cPos.y);
        // Cekame
        ConsoleKeyInfo input = Console.ReadKey();
        player1.view.RenderField(player1.field);

        switch (input.Key)
        {
            case (ConsoleKey.UpArrow):
                Console.SetCursorPosition(cPos.x, cPos.y - 1);
                break;
            case (ConsoleKey.DownArrow):
                Console.SetCursorPosition(cPos.x, cPos.y + 1);
                break;
            case (ConsoleKey.LeftArrow):
                Console.SetCursorPosition(cPos.x - 1, cPos.y);
                break;
            case (ConsoleKey.RightArrow):
                Console.SetCursorPosition(cPos.x + 1, cPos.y);
                break;
            default:

                break;

        }
        if (Console.GetCursorPosition().Left == player1.field.screenPosX - 1 ||
                Console.GetCursorPosition().Top <= player1.field.screenPosY - 1)
            Console.SetCursorPosition(cPos.x, cPos.y);

        return true;
    }

    public void Render()
    {
        //PlayingField.field[0,]

    }

}

public class PlayingField
{
    // klic: 
    // w - voda

    public String[] availableTiles = { "w", "b" };

    public int sizeX { get; private set; }
    public int sizeY { get; private set; }
    public int screenPosX { get; set; }
    public int screenPosY { get; set; }

    // public String[,] _field { get; private set; }
    private Tile[,] field { get; set; }

    public PlayingField(int w, int h)
    {
        sizeX = w;
        sizeY = h;
        field = new Tile[w, h];

        // nacpat vsude vodu
        for (int _x = 0; _x < w; _x++)
            for (int _y = 0; _y < h; _y++)
                field[_x, _y] = new Tile("~", ConsoleColor.White, ConsoleColor.Blue);
        // ~ ; 󰞍 ; 󰼮
    }

    public void SetTile(String tile, int x, int y)
    {
        if (x >= sizeX || y >= sizeY)
            throw new Exception("Tile out of bounds");

        if (!availableTiles.Contains(tile))
            throw new Exception("Unknown tile");

        field[x, y].character = tile; ;
    }

    public Tile GetTile(int x, int y)
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

public struct Tile
{
    public Tile(string character, ConsoleColor fg, ConsoleColor bg)
    {
        this.character = character;
        this.fg = fg;
        this.bg = bg;
    }

    public string character = " ";
    public ConsoleColor fg = ConsoleColor.White;
    public ConsoleColor bg = ConsoleColor.Black;
}

public class Player
{
    private int pts = 0;
    public PlayingField field { get; private set; }
    public PlayerView view { get; private set; }

    public Player(PlayingField _field, PlayerView _view)
    {
        this.field = _field;
        this.view = _view;
    }
}

public class PlayerHuman : Player
{
    public PlayerHuman(PlayingField _field, PlayerView _view) : base(_field, _view) { }
}

public class PlayerComputer : Player
{
    public PlayerComputer(PlayingField _field, PlayerView _view) : base(_field, _view) { }
}

