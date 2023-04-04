using System.Text;

namespace Maze
{
    enum TileType
    {
        Wall,
        Floor,
        Finish,
        Spike,
        Coin,
        Key,
        LockedDoor,
        OpenedDoor
    }

    enum GameState
    {
        MainMenu,
        Settings,
        InGame,
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
        static Map currentMap = new Map(new TileType[,] { { TileType.Wall, TileType.Floor, TileType.Finish },
                                                          { TileType.Spike, TileType.Coin, TileType.Key },
                                                          { TileType.LockedDoor, TileType.Floor, TileType.Floor },}, 1, 1);
        const string title = "Maze";
        static ConsoleKey inputKey;
        static bool isOnExit = false;
        static string[] titleAnimationFrames = { "M", "MA", "MAZ", "MAZE", "MAZEEEEE", "MAZE" };
        static int timeBetweenFrames = 100;
        static GameState gameState = GameState.InGame;
        static void Main(string[] args)
        {
            Start();
        }
        static void Start()
        {
            SetupConsole();
            TitleAnimation();
            while (!isOnExit)
            {
                Timing();
                Input();
                Update();
                Render();
            }
        }

        static void Timing()
        {
            System.Threading.Thread.Sleep(timeBetweenFrames);
        }

        static void Input()
        {
            inputKey = ConsoleKey.NoName;

            if (Console.KeyAvailable)
            {
                inputKey = Console.ReadKey(true).Key;
            }
        }

        static void Update()
        {
            switch (gameState)
            {
                case GameState.InGame:
                    inGameUpdate();
                    break;
            }
        }

        static void inGameUpdate()
        {
            bool playerGoes = inputKey == ConsoleKey.W || inputKey == ConsoleKey.S || inputKey == ConsoleKey.A || inputKey == ConsoleKey.D;

            if (!playerGoes)
                return;

            Direction walkDirection = inputKey switch
            {
                ConsoleKey.W => Direction.Up,
                ConsoleKey.S => Direction.Down,
                ConsoleKey.A => Direction.Left,
                ConsoleKey.D => Direction.Right,
                _ => Direction.None
            };
            currentMap.playerGo(walkDirection);
        }

        static void Render()
        {
            StringBuilder stringBuilder = new StringBuilder();
            string stringToWrite;
            for (int y = 0; y < currentMap.tiles.GetLength(0); y++)
            {
                for (int x = 0; x < currentMap.tiles.GetLength(1); x++)
                {
                    char charToAdd = currentMap.tiles[y, x] switch
                    {
                        TileType.Wall => '#',
                        TileType.Floor => '.',
                        TileType.Finish => 'F',
                        TileType.Spike => '^',
                        TileType.Coin => 'c',
                        TileType.Key => 'k',
                        TileType.LockedDoor => 'H',
                        TileType.OpenedDoor => 'N',
                    };
                    stringBuilder.Append(charToAdd);
                }
                stringBuilder.Append('\n');
            }

            
            stringBuilder.Remove(currentMap.playerY * (currentMap.lengthY + 1) + currentMap.playerX, 1);
            stringBuilder.Insert(currentMap.playerY * (currentMap.lengthY + 1) + currentMap.playerX, '@');
            stringToWrite = stringBuilder.ToString();
            Console.Clear();
            Console.WriteLine(stringToWrite);
            Console.WriteLine(currentMap.playerCoins);
        }

        static void SetupConsole()
        {
            Console.CursorVisible = false;
            Console.Title = title;
            Console.SetWindowSize(50, 25);
            Console.SetBufferSize(50, 25);
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

    }

    struct Map
    {
        public TileType[,] tiles;
        public int lengthX;
        public int lengthY;
        public int playerX;
        public int playerY;
        public int playerCoins = 0;
        public bool playerHasKey = false;

        public Map(TileType[,] tiles, int spawnX, int spawnY)
        {
            this.tiles = tiles;
            playerX = spawnX;
            playerY = spawnY;
            lengthY = tiles.GetLength(0);
            lengthX = tiles.GetLength(1);
        }

        public void playerGo(Direction direction)
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

            if (wherePlayerWantToGo == TileType.LockedDoor && playerHasKey)
            {
                tiles[playerY + dy, playerX + dx] = TileType.OpenedDoor;
            }

            playerCanGoThere = wherePlayerWantToGo == TileType.Floor || wherePlayerWantToGo == TileType.Coin || wherePlayerWantToGo == TileType.OpenedDoor || (wherePlayerWantToGo == TileType.Key && !playerHasKey);

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