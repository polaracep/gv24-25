public class UI
{
    public int wX { get; private set; } = 48;
    public int wY { get; private set; } = 24;

    private char[,] screen;

    public UI(Player p1, Player p2)
    {
        // gg ( ͡° ͜ʖ ͡°)
        if (p1.field.sizeX != p2.field.sizeX) throw new Exception();
        if (p1.field.sizeY != p2.field.sizeY) throw new Exception();

        // math math math!!
        int fieldSizeX = p1.field.sizeX + 2; // misto na carecky
        int fieldSizeY = p1.field.sizeY;

        // 3: misto nahore
        // 2: misto na carecky
        // 10: misto na dolni ui
        wY = fieldSizeY + 3 + 2 + 10;
        this.screen = new char[wX, wY];

        // playing field
        CreateOutline(0, 0, wX, wY, 'x'); // taky moznost: '▒'

        int paddingTop = 4;
        // padding l/r (p) = (2/3)*size 
        int paddingLR = (wX - (2 * fieldSizeX)) / 3;
        // padding vevnitr = 2*(size-p)
        int paddingMid = (wX - (2 * fieldSizeX)) - (2 * paddingLR);

        // Leve pole
        CreateOutline(paddingLR, paddingTop, fieldSizeX, fieldSizeY);
        // Prave pole
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

    public void Render()
    {
        for (int i = 0; i < wY; i++)
        {
            for (int j = 0; j < wX; j++)
            {
                Console.Write(this.screen[j, i]); // Print each element
            }
            Console.WriteLine(); // Move to the next line after each row
        }

    }

    private void CreateOutline(int x, int y, int w, int h, char fill = ' ')
    {
        // fill with ' ' y,x
        Enumerable.Range(y, h).ToList().ForEach(i =>
                Enumerable.Range(x, w).ToList().ForEach(j => screen[j, i] = fill));

        // horni/dolni line
        Enumerable.Range(x, w).ToList().ForEach(i => screen[i, y] = '═');
        Enumerable.Range(x, w).ToList().ForEach(i => screen[i, y + h - 1] = '═');

        Enumerable.Range(y, h).ToList().ForEach(i => screen[x, i] = '║');
        Enumerable.Range(y, h).ToList().ForEach(i => screen[x + w - 1, i] = '║');

        // ╔ ╗ ╚  ╝
        screen[x, y] = '╔';
        screen[x + w - 1, y] = '╗';
        screen[x, y + h - 1] = '╚';
        screen[x + w - 1, y + h - 1] = '╝';
    }

    private void WriteText(int x, int y, string text)
    {
        char[] charStr = text.Replace('\n', ' ').Trim().ToArray();
        for (int i = 0; i < charStr.Length; i++)
        {
            this.screen[x + i, y] = charStr[i];
        }
    }
}
