namespace Lode;

class Program
{
    static void Main(string[] args)
    {
        bool isPlaying = false;

        PlayingField fieldPlayer = new PlayingField(10, 10);
        PlayerHuman player = new PlayerHuman(fieldPlayer);

        PlayingField fieldComputer = new PlayingField(10, 10);
        PlayerComputer pc = new PlayerComputer(fieldComputer);

        fieldPlayer.SetTile('b', 3, 3);
        //fieldPlayer.PrintField();

        fieldComputer.SetTile('b', 6, 6);
        //fieldComputer.PrintField();

        UI ui = new UI(player, pc);
        //ui.CreateUI();
        ui.Render();

        while (isPlaying)
        {
            Game.GameLoop();
        }
    }
}
