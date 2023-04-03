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
        static Map currentMap = new Map();
        const string title = "Maze";
        static ConsoleKey inputKey;
        static bool isOnExit = false;
        static string[] titleAnimationFrames = { "M", "MA", "MAZ", "MAZE", "MAZEEEEE", "MAZE" };
        static int timeBetweenFrames = 100;
        static GameState gameState = GameState.MainMenu;
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
        TileType[,] tiles;
        int playerX;
        int playerY;
        int playerCoins = 0;
        bool playerHasKey = false;
        public Map(TileType[,] tiles, int spawnX, int spawnY)
        {
            this.tiles = tiles;
            playerX = spawnX;
            playerY = spawnY;
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
                    dy = 1;
                    break;
                case Direction.Down:
                    dy = -1;
                    break;
                case Direction.Left:
                    dx = -1;
                    break;
                case Direction.Right:
                    dx = 1;
                    break;
            }

            wherePlayerWantToGo = tiles[playerX + dx, playerY + dy];
            playerCanGoThere = wherePlayerWantToGo == TileType.Floor || wherePlayerWantToGo == TileType.Coin || (wherePlayerWantToGo == TileType.Key && !playerHasKey);

            if (playerCanGoThere)
            {
                playerX += dx;
                playerY += dy;

                if (wherePlayerWantToGo == TileType.Coin)
                    playerCoins++;

                if (wherePlayerWantToGo == TileType.Key) 
                    playerHasKey = true;
            }
        }
    }
}