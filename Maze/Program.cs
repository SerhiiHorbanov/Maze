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
        LockedDoor = 'X'
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
        static Gameplay game = new Gameplay(new TileType[,] { { TileType.Wall, TileType.Floor, TileType.Finish },
                                                          { TileType.Spike, TileType.Coin, TileType.Key },
                                                          { TileType.LockedDoor, TileType.Floor, TileType.Floor },}, 1, 1);

        public int playerCoins = 0;
        public bool playerHasKey = false;
        const string title = "Maze";
        static ConsoleKey inputKey;
        static bool isOnExit = false;
        static string[] titleAnimationFrames = { "M", "MA", "MAZ", "MAZE"};
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
                    game.Update(inputKey);
                    break;
            }
        }


        static void Render()
        {
            StringBuilder stringBuilder = new StringBuilder();
            string stringToWrite;
            for (int y = 0; y < game.tiles.GetLength(0); y++)
            {
                for (int x = 0; x < game.tiles.GetLength(1); x++)
                {
                    char charToAdd = (char)game.tiles[y, x];
                    stringBuilder.Append(charToAdd);
                }
                stringBuilder.Append('\n');
            }

            
            stringBuilder.Remove(game.playerY * (game.lengthY + 1) + game.playerX, 1);
            stringBuilder.Insert(game.playerY * (game.lengthY + 1) + game.playerX, '@');
            stringBuilder.Append($"you have {game.playerCoins} coin");
            stringBuilder.Append(game.playerCoins != 1? "s\n" : "\n");//вибачте але чомусь не можна зробити stringBuilder.Append($"you have {game.playerCoins} coin{game.playerCoins != 1? "s" : ""}\n");

            stringToWrite = stringBuilder.ToString();
            Console.Clear();
            Console.WriteLine(stringToWrite);
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

        static void ReadMapFromFile()
        {

        }
    }

    struct Gameplay
    {
        public TileType[,] tiles;
        public int lengthX { get; private set; }
        public int lengthY { get; private set; }
        public int playerX;
        public int playerY;
        public int playerCoins;
        public bool playerHasKey;

        public Gameplay(TileType[,] tiles, int spawnX = 0, int spawnY = 0, int playerCoins = 0, bool playerHasKey = false)
        {
            this.tiles = tiles;
            playerX = spawnX;
            playerY = spawnY;
            lengthY = tiles.GetLength(0);
            lengthX = tiles.GetLength(1);
            this.playerCoins = playerCoins;
            this.playerHasKey = playerHasKey;
        }

        private void playerGo(Direction direction)
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
                tiles[playerY + dy, playerX + dx] = TileType.Floor;
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

        public void Update(ConsoleKey inputKey)
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
            playerGo(walkDirection);
        }
    }
}