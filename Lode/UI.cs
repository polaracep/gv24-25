public class PlayerView
{
    public int wX { get; private set; } = 48;
    public int wY { get; private set; } = 24;

    private Tile[,] screen;

    public PlayerView(PlayingField f_self, PlayingField f_enemy)
    {
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
        f_self.screenPosX = paddingLR + 1;
        f_self.screenPosY = paddingTop + 1;
        CreateOutline(paddingLR, paddingTop, fieldSizeX, fieldSizeY);
        // Prave pole
        f_enemy.screenPosX = paddingLR + fieldSizeX + paddingMid + 1;
        f_enemy.screenPosY = paddingTop + 1;
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

    public void RenderField(PlayingField field)
    {
        Console.BackgroundColor = ConsoleColor.Blue;
        Console.ForegroundColor = ConsoleColor.White;
        for (int i = 0; i < field.sizeY; i++)
        {
            Console.SetCursorPosition(field.screenPosX, field.screenPosY + i);
            for (int j = 0; j < field.sizeX; j++)
            {
                Console.Write(field.GetTile(j, i).character);
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
