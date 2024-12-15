public enum ViewSide
{
    MY,
    ENEMY,
}

public class PlayerView
{
    public int wX { get; private set; } = 48;
    public int wY { get; private set; } = 24;

    public Tile[,] screen { get; private set; }

    public PlayingField myFieldScreen { get; private set; }
    public (int X, int Y) myFieldPos { get; private set; }

    public PlayingField enemyFieldScreen { get; private set; }
    public (int X, int Y) enemyFieldPos { get; private set; }

    public (int X, int Y) cursorPos { get; private set; }


    public PlayerView(PlayingField fSelf)
    {
        this.myFieldScreen = fSelf;
        this.enemyFieldScreen = new PlayingField(10, 10);
        Console.CursorVisible = false;
        // gg ( ͡° ͜ʖ ͡°)
        if (fSelf.sizeX != this.enemyFieldScreen.sizeX) throw new Exception();
        if (fSelf.sizeY != this.enemyFieldScreen.sizeY) throw new Exception();

        // math math math!!
        int fieldSizeX = fSelf.sizeX + 2; // misto na carecky
        int fieldSizeY = fSelf.sizeY + 2;

        // 3: misto nahore
        // 2: misto na carecky
        // 10: misto na dolni ui
        wY = fieldSizeY + 3 + 2 + 10;
        this.screen = new Tile[wX, wY];

        // playing field
        CreateOutline(0, 0, wX, wY, " "); // taky moznost: '▒'

        int paddingTop = 4;
        // padding l/r (p) = (2/3)*size 
        int paddingLR = (wX - (2 * fieldSizeX)) / 3;
        // padding vevnitr = 2*(size-p)
        int paddingMid = (wX - (2 * fieldSizeX)) - (2 * paddingLR);

        // c# :(
        (int, int) achjoPomoc;
        // Leve pole
        achjoPomoc.Item1 = paddingLR + 1;
        achjoPomoc.Item2 = paddingTop + 1;
        myFieldPos = achjoPomoc;
        CreateOutline(paddingLR, paddingTop, fieldSizeX, fieldSizeY);

        // Prave pole
        achjoPomoc.Item1 = paddingLR + fieldSizeX + paddingMid + 1;
        achjoPomoc.Item2 = paddingTop + 1;
        enemyFieldPos = achjoPomoc;

        CreateOutline(paddingLR + fieldSizeX + paddingMid, paddingTop, fieldSizeX, fieldSizeY);

        // Dolni UI
        CreateOutline(1, wY - 9, wX - 2, 8);

        // Levy nazev
        CreateOutline(paddingLR, 1, fieldSizeX, 3);
        // Pravy nazev
        CreateOutline(paddingLR + fieldSizeX + paddingMid, 1, fieldSizeX, 3);

        WriteText(paddingLR + 2, 2, "Player 1");
        WriteText(paddingLR + paddingMid + fieldSizeX + 2, 2, "Player 2");


    }

    public Tile GetPixel(int x, int y)
    {
        return screen[x, y];
    }

    private void CreateOutline(int x, int y, int w, int h, string fill = " ")
    {
        // fill with ' ' y,x
        Enumerable.Range(y, h).ToList().ForEach(i =>
                Enumerable.Range(x, w).ToList().ForEach(j => screen[j, i].character = fill));

        // horni/dolni line
        Enumerable.Range(x, w).ToList().ForEach(i => screen[i, y].character = "═");
        Enumerable.Range(x, w).ToList().ForEach(i => screen[i, y + h - 1].character = "═");

        Enumerable.Range(y, h).ToList().ForEach(i => screen[x, i].character = "║");
        Enumerable.Range(y, h).ToList().ForEach(i => screen[x + w - 1, i].character = "║");

        // ╔ ╗ ╚  ╝
        screen[x, y].character = "╔";
        screen[x + w - 1, y].character = "╗";
        screen[x, y + h - 1].character = "╚";
        screen[x + w - 1, y + h - 1].character = "╝";
    }

    private void WriteText(int x, int y, string text)
    {
        char[] charStr = text.Replace('\n', ' ').Trim().ToArray();
        for (int i = 0; i < charStr.Length; i++)
        {
            this.screen[x + i, y].character = charStr[i].ToString();
        }
    }

    public void WriteTile(int x, int y, Tile t)
    {
        screen[x, y] = t;
    }

    // result of spaghetti code :))
    public void UpdateField(PlayingField reload)
    {
        this.myFieldScreen = reload;
    }
}

public static class Renderer
{

    public static void RenderAll(PlayerView v)
    {
        for (int i = 0; i < v.wY; i++)
        {
            for (int j = 0; j < v.wX; j++)
            {
                Console.Write(v.screen[j, i].character);
            }
            Console.WriteLine();
        }

    }

    public static void RenderFields(PlayerView v)
    {
        for (int i = 0; i < v.myFieldScreen.sizeY; i++)
        {
            Console.SetCursorPosition(v.myFieldPos.X, v.myFieldPos.Y + i);
            for (int j = 0; j < v.myFieldScreen.sizeX; j++)
            {
                Tile t = v.myFieldScreen.GetTile(j, i);
                Console.ForegroundColor = t.fg;
                Console.BackgroundColor = t.bg;
                Console.Write(t.character);
            }
        }

        for (int i = 0; i < v.enemyFieldScreen.sizeY; i++)
        {
            Console.SetCursorPosition(v.enemyFieldPos.X, v.enemyFieldPos.Y + i);
            for (int j = 0; j < v.enemyFieldScreen.sizeX; j++)
            {
                Tile t = v.enemyFieldScreen.GetTile(j, i);
                Console.ForegroundColor = t.fg;
                Console.BackgroundColor = t.bg;
                Console.Write(t.character);
            }
        }
        Console.ResetColor();
    }

    public static bool RenderCursor(PlayerView v, int x, int y, ViewSide side, Tile cursorAppearance)
    {
        (int X, int Y) fPos;

        if (side == ViewSide.MY)
        {
            // out of bounds
            if ((x > v.myFieldScreen.sizeX) ||
                (y > v.myFieldScreen.sizeY) ||
                (x < 0) ||
                (y < 0))
            {
                return false;
            }

            fPos = (v.myFieldPos.X, v.myFieldPos.Y);
        }
        else if (side == ViewSide.ENEMY)
        {
            // out of bounds
            if ((x > v.enemyFieldScreen.sizeX) ||
                (y > v.enemyFieldScreen.sizeY) ||
                (x < 0) ||
                (y < 0))
            {
                return false;
            }

            fPos = (v.enemyFieldPos.X, v.enemyFieldPos.Y);
        }
        else
        {
            return false;
        }

        Renderer.RenderFields(v);
        Console.SetCursorPosition(fPos.X + x, fPos.Y + y);
        Console.BackgroundColor = cursorAppearance.bg;
        Console.ForegroundColor = cursorAppearance.fg;
        Console.Write(cursorAppearance.character);
        Console.ResetColor();

        return true;
    }

    public static bool RenderBoatPlacement(PlayerView v, int x, int y, Boat boat)
    {
        // pomoc
        int a = 0, b = 0;
        if (boat.rotation == Rotation.LEFT)
            a = 1;
        if (boat.rotation == Rotation.DOWN)
            b = 1;

        for (int i = 0; i < boat.size; i++)
        {
            Console.SetCursorPosition(v.myFieldPos.X + x + (a * i), v.myFieldPos.Y + y + (b * i));
            Console.BackgroundColor = boat.appearance.bg;
            Console.ForegroundColor = boat.appearance.fg;

            // out of bounds
            if ((x + (a * i)) > 9 || (y + (b * i) > 9))
            {
                return false;
            }
            Console.Write(boat.appearance.character);
            Console.ResetColor();
        }
        return true;
    }
}
