public class Game
{
    public static string messageLog { get; set; } = "";
    private Player player1;
    private Player player2;

    private Player[] players;

    public Game(Player player_1, Player player_2)
    {
        this.player1 = player_1;
        this.player2 = player_2;
        players = [player_1, player_2];
    }

    private int playerIndex;

    public bool GameLoop()
    {
        // zmena hrace haha
        playerIndex = 1 - playerIndex;

        Player currentPlayer = players[playerIndex];

        // Hrac zadava tah
        currentPlayer.NextMove();

        // Check trefy
        // -> Hrac strili znova pokud trefil

        // opakujem dokud jeden hrac nema zadny lode

        bool aimDone = false;
        while (!aimDone)
        {
            (int x, int y) cPos = Console.GetCursorPosition();
            Console.BackgroundColor = ConsoleColor.Red;
            Console.Write("󰩷");
            Console.ResetColor();

            Console.SetCursorPosition(cPos.x, cPos.y);
            // Cekame
            ConsoleKeyInfo input = Console.ReadKey();
            player1.view.RenderFields();

            switch (input.Key)
            {
                case (ConsoleKey.K):
                case (ConsoleKey.UpArrow):
                    Console.SetCursorPosition(cPos.x, cPos.y - 1);
                    break;
                case (ConsoleKey.J):
                case (ConsoleKey.DownArrow):
                    Console.SetCursorPosition(cPos.x, cPos.y + 1);
                    break;
                case (ConsoleKey.H):
                case (ConsoleKey.LeftArrow):
                    Console.SetCursorPosition(cPos.x - 1, cPos.y);
                    break;
                case (ConsoleKey.L):
                case (ConsoleKey.RightArrow):
                    Console.SetCursorPosition(cPos.x + 1, cPos.y);
                    break;
                case (ConsoleKey.Enter):
                    Console.SetCursorPosition(1, 1);
                    Console.Write(playerIndex);
                    aimDone = true;
                    break;
                default:
                    Console.SetCursorPosition(cPos.x, cPos.y);
                    break;

            }
            // hmm
            if (Console.GetCursorPosition().Left <= currentPlayer.view.self_pos.X - 1 ||
                    Console.GetCursorPosition().Left >= currentPlayer.view.self_field.sizeX + player1.view.self_pos.X ||
                    Console.GetCursorPosition().Top <= currentPlayer.view.self_pos.Y - 1 ||
                    Console.GetCursorPosition().Top >= currentPlayer.view.self_field.sizeY + player1.view.self_pos.Y)
                Console.SetCursorPosition(cPos.x, cPos.y);

        }
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

    public String[] availableTiles = { "w", "b", "A" };

    public int sizeX { get; private set; }
    public int sizeY { get; private set; }

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
                field[_x, _y] = new Tile("~", ConsoleColor.White, ConsoleColor.Black);
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

public abstract class Player
{
    private int pts = 0;
    public PlayingField field { get; private set; }
    public PlayerView view { get; private set; }

    public Player(PlayingField _field, PlayerView _view)
    {
        this.field = _field;
        this.view = _view;
    }

    public abstract void NextMove();
}

public class PlayerHuman : Player
{
    public PlayerHuman(PlayingField _field, PlayerView _view) : base(_field, _view) { }

    public override void NextMove()
    {
        (int X, int Y) e_pos = this.view.enemy_pos;
        Console.SetCursorPosition(e_pos.X, e_pos.Y);
    }
}

public class PlayerComputer : Player
{
    public PlayerComputer(PlayingField _field, PlayerView _view) : base(_field, _view) { }

    public override void NextMove()
    {
        (int X, int Y) e_pos = this.view.enemy_pos;
        Console.SetCursorPosition(e_pos.X, e_pos.Y);
    }
}

