using System.Collections;
using System.Numerics;
using System.Reflection;
using System.Threading.Channels;
using Raylib_cs;

public enum Direction
{
    Up,
    Down,
    Right,
    Left
}

class Program
{
    const int tileSize = 10;
    const float tileFillPercentage = 0.9f;
    const int tileFillOffset = (int)(tileSize * (1 - tileFillPercentage));
    const int tileFillSize = tileSize - (2 * tileFillOffset);
    const int tileCount = 64;
    const int screenWidth = tileSize * tileCount;
    const int screenHeight = tileSize * tileCount;

    static Queue<Vector2> snake = new Queue<Vector2>(new[]
    {
        new Vector2((float)tileCount / 2f, (float)tileCount / 2f)
    });

    static List<Vector2> food = new List<Vector2>
    {
        new Vector2(5, 8),
        new Vector2(15, 4),
        new Vector2(53, 43),
        new Vector2(23, 48)
    };

    static Direction direction = Direction.Down;
    static Vector2 headPosition = new Vector2((float)tileCount / 2f, (float)tileCount / 2f);

    static bool alive = true;
    static string deathMessage = "";

    static double moveInterval = 0.15;
    static double lastMoveTime = 0;
    static int score = 0;

    static Random random = new Random();

    public static void Main()
    {
        Raylib.InitWindow(screenWidth, screenHeight, "Snake");
        Raylib.SetTargetFPS(60);

        lastMoveTime = Raylib.GetTime();

        score = 0;

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();

            Raylib.ClearBackground(Color.Black);

            if (alive)
            {
                UpdateDirection();

                // Update if interval has passed
                if (Raylib.GetTime() - lastMoveTime > moveInterval) {
                    // Process update
                    Update();
                    lastMoveTime = Raylib.GetTime();
                    // Increase movement speed
                    //moveInterval -= 0.005;
                }
            }

            // Render world
            RenderWorld();

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();

    }

    public static void RenderWorld()
    {
        // Draw food
        foreach (Vector2 pos in food) {
            Rectangle rect = GetRectOfBoardPosition((int)pos.X, (int)pos.Y);
            Raylib.DrawRectangle((int)rect.X, (int)rect.Y, tileFillSize, tileFillSize, Color.Red);
        }

        // Draw snake
        foreach (Vector2 pos in snake) {
            Rectangle rect = GetRectOfBoardPosition((int)pos.X, (int)pos.Y);
            Raylib.DrawRectangle((int)rect.X, (int)rect.Y, tileFillSize, tileFillSize, Color.White);
        }

        // Draw score
        Raylib.DrawText(score.ToString(), screenWidth / 2, (int)(screenHeight * 0.1), tileSize*2, Color.White);
    }

    public static void UpdateDirection()
    {
        if (Raylib.IsKeyDown(KeyboardKey.W))
        {
            direction = Direction.Up;
        }

        if (Raylib.IsKeyDown(KeyboardKey.A))
        {
            direction = Direction.Left;
        }

        if (Raylib.IsKeyDown(KeyboardKey.S))
        {
            direction = Direction.Down;
        }

        if (Raylib.IsKeyDown(KeyboardKey.D))
        {
            direction = Direction.Right;
        }
    }

    public static void Update()
    {
        // Calculate new snake head
        Vector2 newPos = headPosition;
        switch (direction)
        {
            case Direction.Down:
                newPos.Y += 1;
                break;

            case Direction.Up:
                newPos.Y -= 1;
                break;

            case Direction.Right:
                newPos.X += 1;
                break;

            case Direction.Left:
                newPos.X -= 1;
                break;
        }

        // Out of bounds
        if (newPos.X < 0 || newPos.X >= tileCount || newPos.Y < 0 || newPos.Y >= tileCount)
        {
            alive = false;
            deathMessage = "You hit the edge of the world!";
            return;
        }

        // Hit snake
        if (snake.Contains(newPos) && snake.Peek() != newPos)
        {
            alive = false;
            deathMessage = "You hit yourself!";
            return;
        }

        // Eat food
        if (!food.Remove(newPos))
        {
            // Remove last part (skipped if ate) - before the self collision check for fairness.
            snake.Dequeue();
        }
        else
        {
            // Increase score
            score++;

            // Spawn new food until free position
            while (true)
            {
                Vector2 newFoodPos = new Vector2(random.Next(0, tileCount));
                if (!snake.Contains(newFoodPos))
                {
                    food.Add(newFoodPos);
                    break;
                }
            }
        }

        // Add / move head
        snake.Enqueue(newPos);

        // Actually move head position vector
        headPosition = newPos;
    }

    public static Rectangle GetRectOfBoardPosition(int x, int y)
    {
        if (x < 0 || x >= tileCount || y < 0 || x >= tileCount)
        {
            throw new IndexOutOfRangeException("Position outside world.");
        }

        float posX = x * tileSize + tileFillOffset;
        float posY = y * tileSize + tileFillOffset;

        return new Rectangle(posX, posY, tileFillSize, tileFillSize);
    }
}