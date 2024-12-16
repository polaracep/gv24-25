namespace Lode;

class Program
{
    // i3-msg exec ' urxvt -lsp -1 -letsp 12 -fn "xft:DejaVuSansM Nerd Font Mono:size=18"'
    static void Main(string[] args)
    {
        Console.Clear();
        Console.CursorVisible = true;

        PlayingField fieldPlayer = new PlayingField(10, 10);
        PlayingField fieldComputer = new PlayingField(10, 10);

        PlayerHuman humanPlayer = new PlayerHuman(fieldPlayer, "Hrac 1", "Hrac PC");
        PlayerComputer pcPlayer = new PlayerComputer(fieldComputer, "Hrac PC", "Hrac 1");

        Boat[] playerBoats = {
            new SimpleBoat(),
            new SimpleBoat(),
            new LongerBoat(),
            new LongestBoat(),
        };

        Game game = new Game(pcPlayer, humanPlayer);
        Renderer.RenderAll(humanPlayer.view);
        Renderer.RenderFields(humanPlayer.view);

        game.Setup(playerBoats);

        while (game.GameLoop()) ;

        // Game done
        Console.CursorVisible = true;
        Console.ResetColor();
    }
}
