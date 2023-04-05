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
        #region variables
        private static TileType[,] tiles = new TileType[,]
        { { TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Key },
          { TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Wall, TileType.Wall },
          { TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Coin, TileType.Floor },
          { TileType.Floor, TileType.Floor, TileType.Floor, TileType.Floor, TileType.Wall, TileType.LockedDoor },
          { TileType.Floor, TileType.Floor, TileType.Coin, TileType.Floor, TileType.Wall, TileType.Finish },};
        private static int lengthX = tiles.GetLength(1);
        private static int lengthY = tiles.GetLength(0);
        private static int playerX = 0;
        private static int playerY = 0;
        private static int playerCoins;
        private static bool playerHasKey = false;
        private static bool win = false;
        private static bool lose = false;
        private static bool endedPlaying = false;
        private static bool showGuide = true;
        private static ConsoleKey inputKey = ConsoleKey.NoName;
        #endregion

        #region constants and readonlys
        private static readonly string[] titleAnimationFrames = { "", "M", "MA", "MAZ", "MAZE" };
        private const int timeBetweenFrames = 200;
        #endregion

        static void Main(string[] args)
        {
            Start();
            Console.ReadKey();
        }
        static void Start()
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

        static void Input()
        {
            inputKey = Console.ReadKey(true).Key;
        }

        static void Update()
        {
            Direction walkDirection = inputKey switch
            {
                ConsoleKey.W => Direction.Up,
                ConsoleKey.S => Direction.Down,
                ConsoleKey.A => Direction.Left,
                ConsoleKey.D => Direction.Right,
                _ => Direction.None
            };

            if (walkDirection != Direction.None)
            {
                playerGo(walkDirection);
                return;
            }

            if (inputKey == ConsoleKey.H)
                Console.Clear();
                showGuide = !showGuide;

            if (inputKey == ConsoleKey.Q)
                endedPlaying = true;
        }

        static void Render()
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

            stringBuilder.Remove(playerY * (lengthY + 2) + playerX, 1);
            stringBuilder.Insert(playerY * (lengthY + 2) + playerX, '@');

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

        static void SetupConsole()
        {
            Console.CursorVisible = false;
            Console.Title = "Maze";
        }

        static void TitleAnimation()
        {
            foreach (string frame in titleAnimationFrames)
            {
                Console.Clear();
                Console.WriteLine(frame);
                System.Threading.Thread.Sleep(timeBetweenFrames);
            }
        }

        private static void playerGo(Direction direction)
        {
            short dx = 0;
            short dy = 0;
            TileType wherePlayerWantToGo;
            bool playerCanGoThere;

            switch (direction)
            {
                case Direction.Up:
                    if (playerY != 0)
                        dy = -1;
                    break;
                case Direction.Down:
                    if (playerY != lengthY - 1)
                        dy = 1;
                    break;
                case Direction.Left:
                    if (playerX != 0)
                        dx = -1;
                    break;
                case Direction.Right:
                    if (playerX != lengthX - 1)
                        dx = 1;
                    break;
            }

            wherePlayerWantToGo = tiles[playerY + dy, playerX + dx];

            switch (wherePlayerWantToGo)
            {
                case TileType.Finish:
                    win = true;
                    endedPlaying = true;
                    return;
                case TileType.Spike:
                    endedPlaying = true;
                    return;
            }

            if (wherePlayerWantToGo == TileType.LockedDoor && playerHasKey)
            {
                tiles[playerY + dy, playerX + dx] = TileType.Floor;
                playerHasKey = false;
            }

            playerCanGoThere = wherePlayerWantToGo == TileType.Floor || wherePlayerWantToGo == TileType.Coin || (wherePlayerWantToGo == TileType.Key && !playerHasKey);

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