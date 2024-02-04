using System;
using System.Collections.Generic;
using System.Linq;

namespace TopServerPlayers
{
    internal class Program
    {
        static void Main()
        {
            Console.CursorVisible = false;

            int playersQuantity = 10;

            PlayersFabrik playersFabrik = new PlayersFabrik();
            List<Player> players = playersFabrik.CreatePlayers(playersQuantity);

            ActionBuilder actionBuilder = new ActionBuilder(players);
            Menu menu = new MainMenu(actionBuilder.GiveActions());

            menu.Work();
        }
    }

    class Player
    {
        public Player(string name, int level, int strength)
        {
            Name = name;
            Level = level;
            Strength = strength;
        }

        public string Name { get; private set; }
        public int Level { get; private set; }
        public int Strength { get; private set; }

    }

    class PlayersFabrik
    {
        private List<string> _names;

        private int[] _levelStats = { 20, 60 };
        private int[] _strengthStats = { 100, 400 };

        private Random _random = new Random();

        public PlayersFabrik() =>
            FillNames();

        public List<Player> CreatePlayers(int playersQuantity)
        {
            List<Player> players = new List<Player>();

            for (int i = 0; i < playersQuantity; i++)
            {
                string name;
                int maxNameNumber = 300;

                if (i >= _names.Count)
                {
                    int index = i % _names.Count;
                    name = _names[index] + _random.Next(maxNameNumber + 1);
                }
                else
                {
                    name = _names[i];
                }

                int level = _random.Next(_levelStats[0], _levelStats[1]);
                int strength = _random.Next(_strengthStats[0], _strengthStats[1]);

                players.Add(new Player(name, level, strength));
            }

            return players;
        }

        private void FillNames() =>
            _names = new List<string>
            {
                "Геннадий",
                "Дмитрий",
                "Максим",
                "Александр",
                "Валерий",
                "Михаил"
            };
    }

    class MainMenu : Menu
    {
        private Dictionary<string, Action> _actions = new Dictionary<string, Action>();

        public MainMenu(Dictionary<string, Action> actions)
        {
            _actions = actions;
            _actions.Add("Выход", Exit);
            _items = _actions.Keys.ToArray();
        }

        protected override void ConfirmActionSelection()
        {
            base.ConfirmActionSelection();

            _actions[_items[_itemIndex]].Invoke();
        }
    }

    abstract class Menu
    {
        private const ConsoleKey MoveSelectionUp = ConsoleKey.UpArrow;
        private const ConsoleKey MoveSelectionDown = ConsoleKey.DownArrow;
        private const ConsoleKey ConfirmSelection = ConsoleKey.Enter;

        private ConsoleColor _backgroundColor = ConsoleColor.White;
        private ConsoleColor _foregroundColor = ConsoleColor.Black;

        protected string[] _items;
        protected int _itemIndex = 0;

        private bool _isRunning;

        public void Work()
        {
            _isRunning = true;

            while (_isRunning)
            {
                DrawItems();

                ReadKey();
            }
        }

        protected virtual void ConfirmActionSelection() =>
            EraseText();

        protected void Exit() =>
            _isRunning = false;

        private void SetItemIndex(int index)
        {
            int lastIndex = _items.Length - 1;

            if (index > lastIndex)
                index = lastIndex;

            if (index < 0)
                index = 0;

            _itemIndex = index;
        }

        private void ReadKey()
        {
            switch (Console.ReadKey(true).Key)
            {
                case MoveSelectionDown:
                    SetItemIndex(_itemIndex + 1);
                    break;

                case MoveSelectionUp:
                    SetItemIndex(_itemIndex - 1);
                    break;

                case ConfirmSelection:
                    ConfirmActionSelection();
                    break;
            }
        }

        private void DrawItems()
        {
            Console.SetCursorPosition(0, 0);

            for (int i = 0; i < _items.Length; i++)
                if (i == _itemIndex)
                    WriteColoredText(_items[i]);
                else
                    Console.WriteLine(_items[i]);
        }

        private void WriteColoredText(string text)
        {
            Console.ForegroundColor = _foregroundColor;
            Console.BackgroundColor = _backgroundColor;

            Console.WriteLine(text);

            Console.ResetColor();
        }

        private void EraseText()
        {
            int spaceLineSize = 60;
            char space = ' ';

            Console.SetCursorPosition(0, 0);

            for (int i = 0; i < _items.Length; i++)
                Console.WriteLine(new string(space, spaceLineSize));
        }
    }

    class ActionBuilder
    {
        private int _topQuantity = 3;
        private int _cursorPositionYIndent = 2;
        private List<Player> _players;

        public ActionBuilder(List<Player> players)
        {
            _players = players;

            WriteAllPlayers();
        }

        public Dictionary<string, Action> GiveActions() =>
            new Dictionary<string, Action>
            {
                { "Вывести всех игроков", WriteAllPlayers },
                { $"Вывести топ {_topQuantity} игроков по уровню", WriteTopLevelPlayers },
                { $"Вывести топ {_topQuantity} игроков по силе", WriteTopStrengthPlayers }
            };

        private void WritePlayers(IEnumerable<Player> players)
        {
            int spaceLineSize = 40;
            char space = ' ';

            Console.SetCursorPosition(0, GiveActions().Count + _cursorPositionYIndent);

            for (int i = 0; i < _players.Count; i++)
                Console.WriteLine(new string(space, spaceLineSize));

            Console.SetCursorPosition(0, GiveActions().Count + _cursorPositionYIndent);

            foreach (Player player in players)
                Console.WriteLine($"{player.Name}, уровень - {player.Level}, сила - {player.Strength}");
        }

        private void WriteAllPlayers() =>
            WritePlayers(_players);

        private void WriteTopStrengthPlayers() =>
            WritePlayers(_players.OrderByDescending(player => player.Strength).Take(_topQuantity));

        private void WriteTopLevelPlayers() =>
            WritePlayers(_players.OrderByDescending(player => player.Level).Take(_topQuantity));
    }
}
