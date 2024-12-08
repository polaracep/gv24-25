namespace Lode;

class Program
{
    // i3-msg exec ' urxvt -lsp -1 -letsp 12 -fn "xft:DejaVuSansM Nerd Font Mono:size=18"'
    static void Main(string[] args)
    {
        Console.Clear();
        Console.CursorVisible = true;
        bool isPlaying = true;

        PlayingField fieldPlayer = new PlayingField(10, 10);
        PlayingField fieldComputer = new PlayingField(10, 10);

        PlayerView uiPlayer = new PlayerView(fieldPlayer);
        PlayerView uiPc = new PlayerView(fieldComputer);

        PlayerHuman userPlayer = new PlayerHuman(fieldPlayer, uiPlayer);
        PlayerComputer pc = new PlayerComputer(fieldComputer, uiPc);

        fieldPlayer.SetTile(new Tile("A", ConsoleColor.Red, ConsoleColor.Blue), 3, 3);
        //fieldPlayer.PrintField();

        fieldComputer.SetTile(new Tile("b", ConsoleColor.Red, ConsoleColor.Blue), 1, 1);
        //fieldComputer.PrintField();

        Renderer.RenderAll(uiPlayer);
        Renderer.RenderFields(uiPlayer);

        Game game = new Game(userPlayer, pc);

        while (isPlaying)
        {
            game.GameLoop();
        }
    }
}
