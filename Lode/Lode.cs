﻿namespace Lode;

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

        PlayerView uiPlayer = new PlayerView(fieldPlayer, fieldComputer);
        PlayerView uiPc = new PlayerView(fieldComputer, fieldPlayer);

        PlayerHuman userPlayer = new PlayerHuman(fieldPlayer, uiPlayer);
        PlayerComputer pc = new PlayerComputer(fieldComputer, uiPc);

        fieldPlayer.SetTile("A", 3, 3);
        //fieldPlayer.PrintField();

        fieldComputer.SetTile("b", 6, 6);
        //fieldComputer.PrintField();

        //ui.CreateUI();
        uiPlayer.RenderAll();
        uiPlayer.RenderFields();

        Game game = new Game(userPlayer, pc);
        Console.SetCursorPosition(userPlayer.view.self_pos.X, userPlayer.view.self_pos.Y);


        while (isPlaying)
        {

            game.GameLoop();
        }
    }
}
