namespace Lode;

class Program
{
    // i3-msg exec ' urxvt -lsp -1 -letsp 12 -fn "xft:DejaVuSansM Nerd Font Mono:size=18"'
    static void Main(string[] args)
    {
        Console.Clear();
        Console.CursorVisible = true;

        PlayingField fieldPlayer = new PlayingField(10, 10, Game.WATER_TILE);
        PlayingField fieldComputer = new PlayingField(10, 10, Game.WATER_TILE);

        PlayerHuman humanPlayer = new PlayerHuman(fieldPlayer, "Hrac 1", "Hrac PC");
        PlayerComputerEasy pcPlayer = new PlayerComputerEasy(fieldComputer, "Hrac PC", "Hrac 1");

        Boat[] playerBoats = {
            new SimpleBoat(),
            new SimpleBoat(),
            new LongerBoat(),
            new LongestBoat(),
        };

        Game game = new Game(pcPlayer, humanPlayer);
        Renderer.RenderUI(humanPlayer.view);
        Renderer.RenderFields(humanPlayer.view);

        Weapon[] gear = {
            new BoringWeapon(-1),
            new BigWeapon(3),
            new LineWeapon(2),
            new VeryBigWeapon(1)
        };

        game.Setup(playerBoats, gear);

        while (game.GameLoop()) ;

        // Game done
        Console.CursorVisible = true;
        Console.ResetColor();
    }
}
