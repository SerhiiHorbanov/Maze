using System.Text;
using System.IO;

namespace Maze
{
    enum TileType
    {
        Wall = '#',
        Floor = ' ',
        Finish = 'F',
        Spike = '^',
        Coin = 'C',
        Key = 'K',
        LockedDoor = 'D'
    }

    enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        None
    }

    enum GameEndResult
    {
        Win,
        Lose,
        None
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Maze maze = new Maze();
            maze.Start();
            Console.ReadKey();
        }
    }

    class Maze
    {
        #region variables
        private TileType[,] tiles = new TileType[,]
        { { TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Key },
          { TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Wall, TileType.Wall },
          { TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Coin, TileType.Floor },
          { TileType.Floor, TileType.Spike, TileType.Floor, TileType.Floor, TileType.Wall, TileType.LockedDoor },
          { TileType.Floor, TileType.Floor, TileType.Coin, TileType.Floor, TileType.Wall, TileType.Finish },};
        private int playerX = 0;
        private int playerY = 0;
        private int playerCoins;
        private bool playerHasKey = false;
        private bool endedPlaying = false;
        private bool showGuide = true;
        private GameEndResult gameEndResult = GameEndResult.None;
        private ConsoleKey inputKey = ConsoleKey.NoName;
        Direction walkDirection = Direction.None;
        #endregion

        #region constants and readonlys
        private readonly string[] titleAnimationFrames = { "", "M", "MA", "MAZ", "MAZE" };
        private const int timeBetweenFrames = 200;
        #endregion

        public void Start()
        {
            SetupConsole();
            TitleAnimation();

            while (!endedPlaying)
            {
                Render();
                Input();
                Update();
            }

            Console.Clear();

            if (gameEndResult == GameEndResult.Win)
                Console.WriteLine("you won!");
            else if (gameEndResult == GameEndResult.Lose)
                Console.WriteLine("you lost...");

            Console.WriteLine("press any button to close the game");
        }

        private void Input()
        {
            inputKey = Console.ReadKey(true).Key;

            walkDirection = inputKey switch
            {
                ConsoleKey.W => Direction.Up,
                ConsoleKey.S => Direction.Down,
                ConsoleKey.A => Direction.Left,
                ConsoleKey.D => Direction.Right,
                _ => Direction.None
            };
        }

        private void Update()
        {

            if (walkDirection != Direction.None)
            {
                PlayerTryGo(walkDirection);
                return;
            }

            if (inputKey == ConsoleKey.H)
                Console.Clear();
            showGuide = !showGuide;

            if (inputKey == ConsoleKey.Q)
                endedPlaying = true;
        }

        private void Render()
        {
            StringBuilder stringBuilder = new StringBuilder();
            string stringToWrite;

            RenderMap(stringBuilder);

            RenderInfoUI(stringBuilder);

            stringToWrite = stringBuilder.ToString();

            Console.SetCursorPosition(0, 0);
            Console.WriteLine(stringToWrite);
        }

        private void RenderMap(StringBuilder stringBuilder)
        {
            for (int y = 0; y < tiles.GetLength(0); y++)
            {
                for (int x = 0; x < tiles.GetLength(1); x++)
                {
                    char charToAdd = (char)tiles[y, x];
                    stringBuilder.Append(charToAdd);
                }
                stringBuilder.Append('\n');
            }

            stringBuilder.Remove(playerY * (tiles.GetLength(0) + 2) + playerX, 1);
            stringBuilder.Insert(playerY * (tiles.GetLength(0) + 2) + playerX, '@');
        }

        private void RenderInfoUI(StringBuilder stringBuilder)
        {
            stringBuilder.Append($"you have {playerCoins} coin");
            if (playerCoins != 1)
                stringBuilder.Append("s");
            else
                stringBuilder.Append(" ");

            if (playerHasKey)
                stringBuilder.Append($"\nyou have key");
            else
                stringBuilder.Append($"\n            ");

            stringBuilder.Append("\n");//вибачте але чомусь не можна зробити stringBuilder.Append($"you have {game.playerCoins} coin{game.playerCoins != 1? "s" : ""}\n");

            if (showGuide)
                stringBuilder.Append($"\"@\" is you.\n\"#\" is wall. you can't walk on walls ):\n\" \" is floor. you can walk on floor (:\n\"F\" is finish. when you on \"F\" it you win\n\"^\" is spike. when you step on it you lose\n\"C\" is coin collecting coins give you nothing but i guess it's cool\n\"K\" is key. you can have only 1 key\n\"D\" is door. you need key to open it. when you open it, it disappears\npress H to hide this text and guide text");
        }

        private void SetupConsole()
        {
            Console.CursorVisible = false;
            Console.Title = "Maze";
        }

        private void TitleAnimation()
        {
            foreach (string frame in titleAnimationFrames)
            {
                Console.Clear();
                Console.WriteLine(frame);
                System.Threading.Thread.Sleep(timeBetweenFrames);
            }
        }

        private (int, int) DirectionToDelta(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    if (playerY != 0)
                        return (-1, 0);
                    break;
                case Direction.Down:
                    if (playerY != tiles.GetLength(0) - 1)
                        return (1, 0);
                    break;
                case Direction.Left:
                    if (playerX != 0)
                        return (0, -1);
                    break;
                case Direction.Right:
                    if (playerX != tiles.GetLength(1) - 1)
                        return (0, 1);
                    break;
            }
            return (0, 0);
        }
        private void PlayerTryGo(Direction direction)
        {
            (int dy, int dx) = DirectionToDelta(direction);

            TileType wherePlayerWantsToGo = tiles[playerY + dy, playerX + dx];

            switch (wherePlayerWantsToGo)
            {
                case TileType.Finish:
                    gameEndResult = GameEndResult.Win;
                    endedPlaying = true;
                    return;
                case TileType.Spike:
                    gameEndResult = GameEndResult.Lose;
                    endedPlaying = true;
                    return;
            }

            if (wherePlayerWantsToGo == TileType.LockedDoor && playerHasKey)
            {
                tiles[playerY + dy, playerX + dx] = TileType.Floor;
                playerHasKey = false;
            }

            bool playerCanGo = wherePlayerWantsToGo == TileType.Floor || wherePlayerWantsToGo == TileType.Coin || (wherePlayerWantsToGo == TileType.Key && !playerHasKey);

            if (playerCanGo)
                PlayerGo(dx, dy);
        }

        void PlayerGo(int dx, int dy)
        {
            TileType wherePlayerWantsToGo = tiles[playerY + dy, playerX + dx];
            playerX += dx;
            playerY += dy;

            if (wherePlayerWantsToGo == TileType.Coin)
            {
                tiles[playerY, playerX] = TileType.Floor;
                playerCoins++;
            }

            if (wherePlayerWantsToGo == TileType.Key)
            {
                tiles[playerY, playerX] = TileType.Floor;
                playerHasKey = true;
            }
        }
    }
}