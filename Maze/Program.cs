using System.Text;
using System.IO;

namespace Maze
{
    enum TileType
    {
        Wall = 35,
        Floor = 32,
        Finish = 102,
        Spike = 94,
        Coin = 99,
        Key = 107,
        LockedDoor = 100
    }

    enum Direction
    {
        Up,
        Down,
        Left,
        Right,
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
        private bool win = false;
        private bool lose = false;
        private bool endedPlaying = false;
        private bool showGuide = true;
        private ConsoleKey inputKey = ConsoleKey.NoName;
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
            if (win)
                Console.WriteLine("you won!");
            else if (lose)
                Console.WriteLine("you lost...");

            Console.WriteLine("press any button to close the game");
        }

        private void Input()
        {
            inputKey = Console.ReadKey(true).Key;
        }

        private void Update()
        {
            Direction walkDirection = inputKey switch//ЦЕ В ІНПУТ
            {
                ConsoleKey.W => Direction.Up,
                ConsoleKey.S => Direction.Down,
                ConsoleKey.A => Direction.Left,
                ConsoleKey.D => Direction.Right,
                _ => Direction.None
            };

            if (walkDirection != Direction.None)
            {
                PlayerGo(walkDirection);
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

            stringToWrite = stringBuilder.ToString();

            Console.SetCursorPosition(0, 0);
            Console.WriteLine(stringToWrite);
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
                        return (1, 0);
                    break;
                case Direction.Down:
                    if (playerY != tiles.GetLength(0) - 1)
                        return (-1, 0);
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
        private void PlayerGo(Direction direction)//ЗМЕНШИ МЕТОД
        {
            (int Y, int X) delta = DirectionToDelta(direction);
            int dy = delta.Y;
            int dx = delta.X;


            TileType wherePlayerWantToGo = tiles[playerY + dy, playerX + dx];

            switch (wherePlayerWantToGo)
            {
                case TileType.Finish:
                    win = true;
                    endedPlaying = true;
                    return;
                case TileType.Spike:
                    lose = true;
                    endedPlaying = true;
                    return;
            }

            if (wherePlayerWantToGo == TileType.LockedDoor && playerHasKey)
            {
                tiles[playerY + dy, playerX + dx] = TileType.Floor;
                playerHasKey = false;
            }

            bool playerCanGoThere = wherePlayerWantToGo == TileType.Floor || wherePlayerWantToGo == TileType.Coin || (wherePlayerWantToGo == TileType.Key && !playerHasKey);

            if (playerCanGoThere)
            {
                playerX += dx;
                playerY += dy;

                if (wherePlayerWantToGo == TileType.Coin)
                {
                    tiles[playerY, playerX] = TileType.Floor;
                    playerCoins++;
                }

                if (wherePlayerWantToGo == TileType.Key)
                {
                    tiles[playerY, playerX] = TileType.Floor;
                    playerHasKey = true;
                }
            }
        }
    }
}