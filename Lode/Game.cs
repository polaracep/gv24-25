public class Game
{
    private Player player1;
    private Player player2;

    public static Player[] players { get; private set; } = [];
    public static int playerIndex { get; private set; }

    private int maxPoints = 0;

    public Game(Player player_1, Player player_2)
    {
        this.player1 = player_1;
        this.player2 = player_2;
        players = [player_1, player_2];
    }


    public static Tile WATER_TILE = new Tile("~", ConsoleColor.White, ConsoleColor.Blue);

    public bool Setup(Boat[] availableBoats)
    {
        foreach (Boat boat in availableBoats)
            maxPoints += boat.size;

        player1.PlaceBoats(availableBoats);
        player2.PlaceBoats(availableBoats);

        return true;
    }

    public bool GameLoop()
    {
        // zmena hrace 
        Player enemyPlayer = players[playerIndex];

        playerIndex = 1 - playerIndex;

        Player currentPlayer = players[playerIndex];

        // Ted hraje druhy hrac!

        PlayerView shownView = currentPlayer.view;

        if (typeof(PlayerHuman) == currentPlayer.GetType())
        {
            Renderer.RenderAll(currentPlayer.view);
        }
        (int X, int Y) shot = (0, 0);
        while (true)
        {
            // Hrac zadava tah
            shot = currentPlayer.NextMove(shot);

            // Check trefy
            // -> Hrac strili znova pokud trefil
            Tile uncoveredTile = enemyPlayer.myField.GetTile(shot.X, shot.Y);

            // Uz odkryto
            if (!currentPlayer.view.enemyFieldScreen.GetTile(shot.X, shot.Y).Equals(WATER_TILE))
            {
                if (currentPlayer.GetType() == typeof(PlayerHuman))
                    Renderer.RenderCursor(currentPlayer.view, shot.X, shot.Y,
                            ViewSide.ENEMY,
                            new Tile(Icons.WRONG, ConsoleColor.Red, ConsoleColor.White));
                Thread.Sleep(1000);
            }
            // Hit!
            else if (!uncoveredTile.Equals(Game.WATER_TILE))
            {

                // hmph
                if (currentPlayer.GetType() == typeof(PlayerHuman))
                {
                    currentPlayer.view.enemyFieldScreen.SetTile(
                        new Tile(Icons.FIRE, ConsoleColor.Red, ConsoleColor.Blue), shot.X, shot.Y);
                    enemyPlayer.view.myFieldScreen.SetTile(
                        new Tile(Icons.FIRE, ConsoleColor.DarkRed, ConsoleColor.DarkBlue), shot.X, shot.Y);

                    Renderer.RenderFields(currentPlayer.view);
                    Thread.Sleep(1000);
                    currentPlayer.view.enemyFieldScreen.SetTile(uncoveredTile, shot.X, shot.Y);
                    Renderer.RenderFields(currentPlayer.view);
                }
                currentPlayer.view.enemyFieldScreen.SetTile(uncoveredTile, shot.X, shot.Y);

                currentPlayer.pts += 1;

                if (currentPlayer.pts == maxPoints)
                {
                    currentPlayer.Win();
                    return false;
                }
            }
            else
            {
                currentPlayer.view.enemyFieldScreen.SetTile(new Tile(" ", ConsoleColor.White, ConsoleColor.DarkBlue), shot.X, shot.Y);
                enemyPlayer.view.myFieldScreen.SetTile(new Tile(" ", ConsoleColor.White, ConsoleColor.DarkBlue), shot.X, shot.Y);
                break;
            }
        }

        // opakujem dokud jeden hrac nema zadny lode
        return true;
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
    public String name { get; protected set; }
    public int pts = 0;
    // my field je hracovo pole
    public PlayingField myField { get; protected set; }

    // ui, pres ktere hrac hraje 
    public PlayerView view { get; protected set; }

    public Boat[] boats { get; protected set; }

    // gg jmena jsou cooked
    public Player(PlayingField myField, String name, String nameEnemy)
    {
        this.myField = myField;
        this.boats = [];
        if (name.Length < 8)
            this.name = name;
        else
            throw new Exception("Moc dlouhy jmeno");

        this.view = new PlayerView(myField, name, nameEnemy);

    }

    public abstract void PlaceBoats(Boat[] placedBoats);

    public abstract (int X, int Y) NextMove((int X, int Y) cPos);

    public abstract void Win();
}

public class PlayerHuman : Player
{
    public PlayerHuman(PlayingField myField, String name, String nameEnemy) : base(myField, name, nameEnemy) { }

    public override (int X, int Y) NextMove((int X, int Y) cPos)
    {
        // set cursor to enemy window pos 
        bool aimDone = false;
        while (!aimDone)
        {
            if (Renderer.RenderCursor(this.view, cPos.X, cPos.Y, ViewSide.ENEMY, new Tile(Icons.CURSOR_SHOOT, ConsoleColor.White, ConsoleColor.Red)) == false)
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
                case (ConsoleKey.V):
                    Renderer.RenderAll(Game.players[1 - Game.playerIndex].view);
                    Renderer.RenderFields(Game.players[1 - Game.playerIndex].view);
                    Console.ReadKey();
                    Renderer.RenderAll(Game.players[Game.playerIndex].view);
                    Renderer.RenderFields(Game.players[Game.playerIndex].view);
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
        Renderer.RenderAll(this.view);
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
                    Renderer.RenderCursor(this.view, cPos.X, cPos.Y, ViewSide.MY,
                                new Tile(Icons.WRONG, ConsoleColor.White, ConsoleColor.Red));
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
                        if (!this.myField.GetTile(boat.startPos.X + (_a * i), boat.startPos.Y + (_b * i)).Equals(Game.WATER_TILE))
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

    public override void Win()
    {
        this.view.ShowWinScreen(this.name);
        Renderer.RenderAll(this.view);
    }
}

public class PlayerComputer : Player
{
    public PlayerComputer(PlayingField myField, String name, String nameEnemy) : base(myField, name, nameEnemy) { }

    public override (int X, int Y) NextMove((int X, int Y) cPos)
    {
        Renderer.RenderAll(this.view);
        Renderer.RenderFields(this.view);
        // Thread.Sleep(500);
        Random r = new Random();
        return (r.Next(0, 10), r.Next(0, 10));
    }

    public override void PlaceBoats(Boat[] placeableBoats)
    {
        boats = placeableBoats;

        Random rand = new Random();
        foreach (Boat boat in boats)
        {
            while (true)
            {
                boat.startPos = (rand.Next(0, 10), rand.Next(0, 10));
                boat.rotation = rand.Next(0, 2) == 0 ? Rotation.LEFT : Rotation.DOWN;

                if (Renderer.RenderBoatPlacement(this.view, boat.startPos.X, boat.startPos.Y, boat))
                    break;
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

    public override void Win()
    {
        this.view.ShowWinScreen(this.name);
        Renderer.RenderAll(this.view);
    }
}

