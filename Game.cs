using System.Numerics;
using Raylib_cs;

namespace Snake;
public enum Direction
{
    Up,
    Down,
    Right,
    Left
}

public class Game
{
    Queue<Vector2> snake = new Queue<Vector2>(new[]
    {
        new Vector2((float)Program.tileCount / 2f, (float)Program.tileCount / 2f)
    });

    List<Vector2> food = new List<Vector2>
    {
        new Vector2(5, 8),
        new Vector2(15, 4),
        new Vector2(53, 43),
        new Vector2(23, 48)
    };

    Direction direction = Direction.Down;
    Vector2 headPosition = new Vector2((float)Program.tileCount / 2f, (float)Program.tileCount / 2f);

    bool alive = true;
    string deathMessage = "";

    double moveInterval = 0.15;
    double lastMoveTime = 0;
    int score = 0;
    int level = 0;
    
    Random random = new Random();

    public Game()
    {
        lastMoveTime = Raylib.GetTime();
    }

    public void Frame(double dt)
    {
        Raylib.ClearBackground(Color.Black);

        // Render world
        RenderWorld();

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
        else
        {
            RenderDeadScreen();
        }
    }
    static Rectangle tryAgainBtn = new Rectangle((int)(Program.screenWidth * 0.5 - Program.btnWidth * 0.5), (int)(Program.screenWidth * 0.4 - Program.btnHeight * 0.5), Program.btnWidth, Program.btnHeight);
    static Rectangle backBtn = new Rectangle((int)(Program.screenWidth * 0.5 - Program.btnWidth * 0.5), (int)(Program.screenWidth * 0.6 - Program.btnHeight * 0.5), Program.btnWidth, Program.btnHeight);

    private void RenderDeadScreen()
    {
        Program.DrawTextCentered($"You died:", new Vector2(Program.screenWidth * 0.5f, Program.screenHeight * 0.1f), Program.tileSize * 4, Color.White);
        Program.DrawTextCentered(deathMessage, new Vector2(Program.screenWidth * 0.5f, Program.screenHeight * 0.2f), Program.tileSize * 4, Color.White);

        int startTextSize = Program.tileSize * 4;
        int quitTextSize = Program.tileSize * 4;

        Vector2 mouse = Raylib.GetMousePosition();

        if (Raylib.CheckCollisionPointRec(mouse, tryAgainBtn))
        {
            startTextSize = Program.tileSize * 6;

            if (Raylib.IsMouseButtonReleased(MouseButton.Left))
            {
                Program.ResetGame();
            }
        }
        
        if (Raylib.CheckCollisionPointRec(mouse, backBtn))
        {
            quitTextSize = Program.tileSize * 6;

            if (Raylib.IsMouseButtonReleased(MouseButton.Left))
            {
                Program.screen = Screen.MainMenu;
            }
        }

        Raylib.DrawRectangleLinesEx(tryAgainBtn, 5f, Color.White);
        Program.DrawTextCentered("Try Again", new Vector2(tryAgainBtn.X + (int)(tryAgainBtn.Width * 0.5), tryAgainBtn.Y + (int)(tryAgainBtn.Height * 0.5)), startTextSize, Color.White);

        Raylib.DrawRectangleLinesEx(backBtn, 5f, Color.White);
        Program.DrawTextCentered("Back", new Vector2(backBtn.X + (int)(tryAgainBtn.Width * 0.5), backBtn.Y + (int)(tryAgainBtn.Height * 0.5)), quitTextSize, Color.White);
    }

    private void RenderWorld()
    {
        // Draw food
        foreach (Vector2 pos in food) {
            Rectangle rect = GetRectOfBoardPosition((int)pos.X, (int)pos.Y);
            Raylib.DrawRectangle((int)rect.X, (int)rect.Y, Program.tileFillSize, Program.tileFillSize, Color.Red);
        }

        // Draw snake
        foreach (Vector2 pos in snake) {
            Rectangle rect = GetRectOfBoardPosition((int)pos.X, (int)pos.Y);
            Raylib.DrawRectangle((int)rect.X, (int)rect.Y, Program.tileFillSize, Program.tileFillSize, Color.White);
        }

        // Draw score
        Vector2 scorePos = new(Program.screenWidth / 2f, Program.screenHeight * 0.1f);
        Program.DrawTextCentered(score.ToString(), scorePos, Program.tileSize * 2, Color.White);
    }

    private void UpdateDirection()
    {
        if (Raylib.IsKeyDown(KeyboardKey.W))
        {
            if (direction == Direction.Down) { return; }
            direction = Direction.Up;
        }

        if (Raylib.IsKeyDown(KeyboardKey.A))
        {
            if (direction == Direction.Right) { return; }
            direction = Direction.Left;
        }

        if (Raylib.IsKeyDown(KeyboardKey.S))
        {
            if (direction == Direction.Up) { return; }
            direction = Direction.Down;
        }

        if (Raylib.IsKeyDown(KeyboardKey.D))
        {
            if (direction == Direction.Left) { return; }
            direction = Direction.Right;
        }
    }

    private void Update()
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
        if (newPos.X < 0 || newPos.X >= Program.tileCount || newPos.Y < 0 || newPos.Y >= Program.tileCount)
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
                Vector2 newFoodPos = new Vector2(random.Next(0, Program.tileCount));
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

    private Rectangle GetRectOfBoardPosition(int x, int y)
    {
        if (x < 0 || x >= Program.tileCount || y < 0 || x >= Program.tileCount)
        {
            throw new IndexOutOfRangeException("Position outside world.");
        }

        float posX = x * Program.tileSize + Program.tileFillOffset;
        float posY = y * Program.tileSize + Program.tileFillOffset;

        return new Rectangle(posX, posY, Program.tileFillSize, Program.tileFillSize);
    }
}