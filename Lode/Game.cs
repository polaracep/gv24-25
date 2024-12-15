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

    public bool Setup()
    {
        // player1.PlaceBoats();

        return true;
    }

    public bool GameLoop()
    {
        // zmena hrace 
        playerIndex = 1 - playerIndex;

        Player currentPlayer = players[playerIndex];
        Player enemyPlayer = players[1 - playerIndex];

        (int X, int Y) shot = (0, 0);
        while (true)
        {
            // Hrac zadava tah
            currentPlayer.NextMove(shot);

            // Check trefy
            // -> Hrac strili znova pokud trefil
            Tile uncoveredTile = enemyPlayer.myField.GetTile(shot.X, shot.Y);
            if (uncoveredTile.character != "~")
            {
                // hit!
                currentPlayer.view.enemyFieldScreen.SetTile(new Tile("", ConsoleColor.Red, ConsoleColor.Blue), shot.X, shot.Y);
                Renderer.RenderFields(currentPlayer.view);

                // hmph
                Thread.Sleep(1000);
                currentPlayer.view.enemyFieldScreen.SetTile(uncoveredTile, shot.X, shot.Y);
                Renderer.RenderFields(currentPlayer.view);
            }
            else
            {
                break;
            }
        }

        // opakujem dokud jeden hrac nema zadny lode
        return true;
    }

    public void PlaceBoats()
    {

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
                field[_x, _y] = new Tile("~", ConsoleColor.White, ConsoleColor.Blue);
        // ~ ; 󰞍 ; 󰼮
    }

    public void SetTile(Tile tile, int x, int y)
    {
        if (x >= sizeX || y >= sizeY)
            throw new Exception("Tile out of bounds");

        // if (!availableTiles.Contains(tile))
        // throw new Exception("Unknown tile");

        field[x, y] = tile;
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
                Console.ForegroundColor = this.field[i, j].fg;
                Console.BackgroundColor = this.field[i, j].bg;
                // Console.Write(this.field[i, j].character + " "); // Print each element
                Console.Write(this.field[i, j].fg + " "); // Print each element
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
    public ConsoleColor fg { get; set; }
    public ConsoleColor bg { get; set; }
}

public abstract class Player
{
    private int pts = 0;
    // my field je hracovo pole
    public PlayingField myField { get; protected set; }

    // ui, pres ktere hrac hraje 
    public PlayerView view { get; protected set; }

    public Boat[] boats { get; protected set; }

    public Player(PlayingField _myField, PlayerView _view)
    {
        this.myField = _myField;
        this.view = _view;
        this.boats = [];
    }

    public abstract void PlaceBoats(Boat[] placedBoats);

    public abstract (int X, int Y) NextMove((int X, int Y) cPos);
}

public class PlayerHuman : Player
{
    public PlayerHuman(PlayingField _myField, PlayerView _view) : base(_myField, _view) { }

    public override (int X, int Y) NextMove((int X, int Y) cPos)
    {
        // set cursor to enemy window pos 
        bool aimDone = false;
        while (!aimDone)
        {
            if (Renderer.RenderCursor(this.view, cPos.X, cPos.Y, ViewSide.ENEMY, new Tile("󰩷", ConsoleColor.White, ConsoleColor.Red)) == false)
            {
                throw new Exception("bing bong");
            }
            // Cekame
            ConsoleKeyInfo input = Console.ReadKey(true);

            switch (input.Key)
            {
                case (ConsoleKey.K):
                case (ConsoleKey.UpArrow):
                    cPos.Y -= 1;
                    break;
                case (ConsoleKey.J):
                case (ConsoleKey.DownArrow):
                    cPos.Y += 1;
                    break;
                case (ConsoleKey.H):
                case (ConsoleKey.LeftArrow):
                    cPos.X -= 1;
                    break;
                case (ConsoleKey.L):
                case (ConsoleKey.RightArrow):
                    cPos.X += 1;
                    break;
                case (ConsoleKey.Enter):
                    aimDone = true;
                    break;
                default:
                    break;
            }
            if (cPos.X >= this.myField.sizeX)
                cPos.X = 0;
            if (cPos.X < 0)
                cPos.X = this.myField.sizeX - 1;

            if (cPos.Y >= this.myField.sizeY)
                cPos.Y = 0;
            if (cPos.Y < 0)
                cPos.Y = this.myField.sizeY - 1;

        }
        return cPos;
    }

    public override void PlaceBoats(Boat[] placeableBoats)
    {
        boats = placeableBoats;

        foreach (Boat boat in boats)
        {
            (int X, int Y) cPos = (0, 0);
            Rotation rotation = Rotation.LEFT;

            // achjo
            bool placementDone = false;
            bool placeable = false;
            while (!placementDone)
            {
                boat.rotation = rotation;
                boat.startPos = cPos;

                Renderer.RenderFields(this.view);
                if (Renderer.RenderBoatPlacement(this.view, cPos.X, cPos.Y, boat) == false)
                {
                    if (Renderer.RenderCursor(this.view, cPos.X, cPos.Y, ViewSide.MY,
                                new Tile("", ConsoleColor.White, ConsoleColor.Red)) == false)
                        throw new Exception("bing bong");

                    placeable = false;
                }
                else
                {
                    int _a = 0, _b = 0;
                    if (boat.rotation == Rotation.LEFT)
                        _a = 1;
                    if (boat.rotation == Rotation.DOWN)
                        _b = 1;

                    placeable = true;

                    for (int i = 0; i < boat.size; i++)
                    {
                        if (this.myField.GetTile(boat.startPos.X + (_a * i), boat.startPos.Y + (_b * i)).character != "~")
                            placeable = false;
                    }
                }

                // Cekame
                ConsoleKeyInfo input = Console.ReadKey(true);

                switch (input.Key)
                {
                    case (ConsoleKey.K):
                    case (ConsoleKey.UpArrow):
                        cPos.Y -= 1;
                        break;
                    case (ConsoleKey.J):
                    case (ConsoleKey.DownArrow):
                        cPos.Y += 1;
                        break;
                    case (ConsoleKey.H):
                    case (ConsoleKey.LeftArrow):
                        cPos.X -= 1;
                        break;
                    case (ConsoleKey.L):
                    case (ConsoleKey.RightArrow):
                        cPos.X += 1;
                        break;
                    case (ConsoleKey.Enter):
                        if (placeable == true)
                            placementDone = true;
                        else
                            Console.Beep();
                        break;
                    case (ConsoleKey.R):
                        if (rotation == Rotation.DOWN)
                            rotation = Rotation.LEFT;
                        else
                            rotation = Rotation.DOWN;
                        break;
                    default:
                        break;
                }
                if (cPos.X >= this.myField.sizeX)
                    cPos.X = 0;
                if (cPos.X < 0)
                    cPos.X = this.myField.sizeX - 1;

                if (cPos.Y >= this.myField.sizeY)
                    cPos.Y = 0;
                if (cPos.Y < 0)
                    cPos.Y = this.myField.sizeY - 1;


            }

            int a = 0, b = 0;
            if (boat.rotation == Rotation.LEFT)
                a = 1;
            if (boat.rotation == Rotation.DOWN)
                b = 1;

            for (int i = 0; i < boat.size; i++)
            {
                this.myField.SetTile(boat.appearance, (boat.startPos.X + (a * i)), boat.startPos.Y + (b * i));
            }
        }
    }
}


public class PlayerComputer : Player
{
    public PlayerComputer(PlayingField _myField, PlayerView _view) : base(_myField, _view) { }

    public override (int X, int Y) NextMove((int X, int Y) cPos)
    {
        return (0, 0);
    }

    public override void PlaceBoats(Boat[] placedBoats) { }
}

