using System.Numerics;
using Raylib_cs;

namespace Snake;

enum Screen
{
    MainMenu,
    Game
}

class Program
{
    internal const int tileSize = 10;
    internal const float tileFillPercentage = 0.9f;
    internal const int tileFillOffset = (int)(tileSize * (1 - tileFillPercentage));
    internal const int tileFillSize = tileSize - (2 * tileFillOffset);
    internal const int tileCount = 64;
    internal const int screenWidth = tileSize * tileCount;
    internal const int screenHeight = tileSize * tileCount;

    public static Screen screen = Screen.MainMenu;

    private static Game game;

    public static void Main()
    {
        Raylib.InitWindow(screenWidth, screenHeight, "Snake");
        Raylib.SetTargetFPS(60);

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();

            switch (screen)
            {
                case Screen.MainMenu:
                    MainMenu();
                    break;
                
                case Screen.Game:
                    game.Frame(0.0);
                    break;
            }

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();

    }

    public static void ResetGame()
    {
        game = new Game();
    }

    public static int btnWidth = tileSize * 32;
    public static int btnHeight = tileSize * 12;
    static Rectangle startBtn = new Rectangle((int)(screenWidth * 0.5 - btnWidth * 0.5), (int)(screenWidth * 0.4 - btnHeight * 0.5), btnWidth, btnHeight);
    static Rectangle quitBtn = new Rectangle((int)(screenWidth * 0.5 - btnWidth * 0.5), (int)(screenWidth * 0.6 - btnHeight * 0.5), btnWidth, btnHeight);
    static void MainMenu()
    {
        Raylib.ClearBackground(Color.Black);

        DrawTextCentered("Snake;", new Vector2(screenWidth * 0.5f, screenHeight * 0.2f), tileSize * 4, Color.White);

        int startTextSize = tileSize * 4;
        int quitTextSize = tileSize * 4;

        Vector2 mouse = Raylib.GetMousePosition();

        if (Raylib.CheckCollisionPointRec(mouse, startBtn))
        {
            startTextSize = tileSize * 6;

            if (Raylib.IsMouseButtonReleased(MouseButton.Left))
            {   
                game = new Game();
                screen = Screen.Game;
            }
        }
        
        if (Raylib.CheckCollisionPointRec(mouse, quitBtn))
        {
            quitTextSize = tileSize * 6;

            if (Raylib.IsMouseButtonReleased(MouseButton.Left))
            {
                Environment.Exit(0);
            }
        }

        Raylib.DrawRectangleLinesEx(startBtn, 5f, Color.White);
        DrawTextCentered("Start", new Vector2(startBtn.X + (int)(startBtn.Width * 0.5), startBtn.Y + (int)(startBtn.Height * 0.5)), startTextSize, Color.White);

        Raylib.DrawRectangleLinesEx(quitBtn, 5f, Color.White);
        DrawTextCentered("Quit", new Vector2(quitBtn.X + (int)(startBtn.Width * 0.5), quitBtn.Y + (int)(startBtn.Height * 0.5)), quitTextSize, Color.White);
    }

    // Draws text centered on `center` instead of Raylib.DrawText's top-left anchor.
    internal static void DrawTextCentered(string text, Vector2 center, int fontSize, Color color)
    {
        int textWidth = Raylib.MeasureText(text, fontSize);
        int x = (int)center.X - textWidth / 2;
        int y = (int)center.Y - fontSize / 2;

        Raylib.DrawText(text, x, y, fontSize, color);
    }
}