public class PlayerView
{
    public int wX { get; private set; } = 48;
    public int wY { get; private set; } = 24;

    private Tile[,] screen;

    public PlayingField self_field { get; private set; }
    public (int X, int Y) self_pos;

    public PlayingField enemy_field { get; private set; }
    public (int X, int Y) enemy_pos;

    public PlayerView(PlayingField f_self, PlayingField f_enemy)
    {
        this.self_field = f_self;
        this.enemy_field = f_enemy;
        Console.CursorVisible = false;
        // gg ( ͡° ͜ʖ ͡°)
        if (f_self.sizeX != f_enemy.sizeX) throw new Exception();
        if (f_self.sizeY != f_enemy.sizeY) throw new Exception();

        // math math math!!
        int fieldSizeX = f_self.sizeX + 2; // misto na carecky
        int fieldSizeY = f_self.sizeY + 2;

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

        // Leve pole
        self_pos.X = paddingLR + 1;
        self_pos.Y = paddingTop + 1;
        CreateOutline(paddingLR, paddingTop, fieldSizeX, fieldSizeY);

        // Prave pole
        enemy_pos.X = paddingLR + fieldSizeX + paddingMid + 1;
        enemy_pos.Y = paddingTop + 1;

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

    public void RenderAll()
    {
        for (int i = 0; i < wY; i++)
        {
            for (int j = 0; j < wX; j++)
            {
                Console.Write(this.screen[j, i].character);
            }
            Console.WriteLine();
        }

    }

    public void RenderFields()
    {
        for (int i = 0; i < this.self_field.sizeY; i++)
        {
            Console.SetCursorPosition(this.self_pos.X, this.self_pos.Y + i);
            for (int j = 0; j < this.self_field.sizeX; j++)
            {
                Tile t = this.self_field.GetTile(j, i);
                Console.ForegroundColor = t.fg;
                Console.BackgroundColor = t.bg;
                Console.Write(t.character);
            }
        }

        for (int i = 0; i < this.enemy_field.sizeY; i++)
        {
            Console.SetCursorPosition(this.enemy_pos.X, this.enemy_pos.Y + i);
            for (int j = 0; j < this.enemy_field.sizeX; j++)
            {
                Tile t = this.enemy_field.GetTile(j, i);
                Console.ForegroundColor = t.fg;
                Console.BackgroundColor = t.bg;
                Console.Write(t.character);
            }
        }
        Console.ResetColor();
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

}
