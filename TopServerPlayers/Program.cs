using System;
using System.Collections.Generic;
using System.Linq;

namespace TopServerPlayers
{
    class Program
    {
        static void Main()
        {
            ServerFactory serverFactory = new ServerFactory();
            Server server = serverFactory.Create();

            server.Work();
        }
    }

    class ServerFactory
    {
        private PlayerFactory _playerFactory = new PlayerFactory();

        public Server Create() =>
            new Server(_playerFactory.CreatePlayers());
    }

    class Server
    {
        private List<Player> _players;

        public Server(List<Player> players)
        {
            _players = players;
        }

        public void Work()
        {
            const string SortByLevelCommand = "1";
            const string SortByStrengthCommand = "2";
            const string ExitCommand = "3";

            int sortQuantity = 3;
            bool isWork = true;
            List<Player> players = _players;

            while (isWork)
            {
                Console.WriteLine();
                players.ForEach(player => player.ShowInfo());

                Console.WriteLine($"\n" +
                    $"{SortByLevelCommand} - вывести топ по уровню\n" +
                    $"{SortByStrengthCommand} - вывести топ по силе\n" +
                    $"{ExitCommand} - выйти\n");

                switch (UserUtils.ReadString("Введите команду:"))
                {
                    case SortByLevelCommand:
                        players = _players.OrderByDescending(player => player.Level).Take(sortQuantity).ToList();
                        break;

                    case SortByStrengthCommand:
                        players = _players.OrderByDescending(player => player.Strength).Take(sortQuantity).ToList();
                        break;

                    case ExitCommand:
                        isWork = false;
                        break;

                    default:
                        break;
                }
            }
        }
    }

    class PlayerFactory
    {
        public List<Player> CreatePlayers()
        {
            int playersQuantity = 10;

            int[] levelStats = { 20, 60 };
            int[] strengthStats = { 100, 400 };

            List<Player> players = new List<Player>();

            for (int i = 0; i < playersQuantity; i++)
            {
                string name = CreateName();
                int level = UserUtils.GenerateStat(levelStats);
                int strength = UserUtils.GenerateStat(strengthStats);

                players.Add(new Player(name, level, strength));
            }

            return players;
        }

        private string CreateName()
        {
            string name = UserUtils.GenerateRandomValue(GetNames());
            int maxPrefix = 300;

            name += UserUtils.GenerateInt(maxPrefix);

            return name;
        }

        private List<string> GetNames() =>
            new List<string>
            {
                "Геннадий",
                "Дмитрий",
                "Максим",
                "Александр",
                "Валерий",
                "Михаил"
            };
    }

    class Player
    {
        public Player(string name, int level, int strength)
        {
            Name = name;
            Level = level;
            Strength = strength;
        }

        public string Name { get; }
        public int Level { get; }
        public int Strength { get; }

        public void ShowInfo()
        {
            const int ShortOffset = -5;
            const int LongOffset = -15;

            Console.WriteLine($"{Name,LongOffset} Уровень: {Level,ShortOffset} Сила: {Strength}");
        }
    }

    static class UserUtils
    {
        private static Random s_random = new Random();

        public static string ReadString(string text)
        {
            Console.Write(text);

            return Console.ReadLine();
        }

        public static T GenerateRandomValue<T>(IEnumerable<T> values)
        {
            int index = s_random.Next(values.Count());

            return values.ElementAt(index);
        }

        public static int GenerateStat(int[] stats)
        {
            int maxLength = 2;

            if (stats.Length != maxLength)
            {
                throw new ArgumentException("Массив stats должен содержать ровно 2 элемента.");
            }

            return s_random.Next(stats[0], stats[1] + 1);
        }

        public static int GenerateInt(int max) =>
            s_random.Next(max);
    }
}
