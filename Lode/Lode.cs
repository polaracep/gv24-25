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
        // PlayerComputer pc = new PlayerComputer(fieldComputer, uiPc);
        PlayerHuman pc = new PlayerHuman(fieldComputer, uiPc);

        Boat[] playerBoats = {
            new SimpleBoat((0, 0), Rotation.DOWN),
            new SimpleBoat((0, 0), Rotation.DOWN),
            new LongerBoat((0, 0), Rotation.DOWN),
            new LongestBoat((0, 0), Rotation.DOWN),
        };

        Boat[] pcBoats = {
            new SimpleBoat((0, 0), Rotation.DOWN),
        };
        // pc.PlaceBoats(playerBoats);

        // uiPlayer.UpdateField(fieldPlayer);

        Game game = new Game(pc, userPlayer);
        Renderer.RenderAll(uiPlayer);
        Renderer.RenderFields(uiPlayer);

        game.Setup();
        userPlayer.PlaceBoats(playerBoats);
        pc.PlaceBoats(pcBoats);

        while (isPlaying)
        {
            game.GameLoop();
        }
    }
}
