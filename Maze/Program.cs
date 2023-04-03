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

    internal class Program
    {
        const string title = "Maze";
        static ConsoleKey inputKey;
        static bool isOnExit = false;
        static string[] titleAnimationFrames = { "M", "MA", "MAZ", "MAZE", "MAZEEEEE", "MAZE" };
        static int timeBetweenFrames = 100;
        static GameState gameState = GameState.MainMenu;
        static int X;
        static int Y;
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
            short dx = 0;
            short dy = 0;
            switch (inputKey)
            {
                case ConsoleKey.W:
                    dy = 1;
                    break;
                case ConsoleKey.S:
                    dy = -1;
                    break;
                case ConsoleKey.A:
                    dx = -1;
                    break;
                case ConsoleKey.D:
                    dx = 1;
                    break;
            }
            
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
    struct Tile
    {
        TileType type = TileType.Floor;
        public Tile(TileType type)
        {
            this.type = type;
        }
    }

    struct Map
    {
        Tile[,] tiles;
        int spawnX;
        int spawnY;
    }
}