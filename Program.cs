using Raylib_cs;
using Game;

class Program
{
    public static void Main()
    {
        Raylib.InitWindow(screenWidth, screenHeight, "Snake");
        Raylib.SetTargetFPS(60);

        lastMoveTime = Raylib.GetTime();

        score = 0;

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();



            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();

    }

}