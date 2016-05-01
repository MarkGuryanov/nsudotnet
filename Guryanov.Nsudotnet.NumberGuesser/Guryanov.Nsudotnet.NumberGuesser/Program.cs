using System;

namespace Guryanov.Nsudotnet.NumberGuesser
{
    class Game
    {
        private static readonly string[] Taunts =
        {
            "{0}, где та женщина, что тебя рожала?! Почему её до сих пор не пристрелили?!",
            "{0}, ты поинтересуйся у своих родителей: может быть, тебя маленького роняли вниз головой?",
            "Нет, {0}, мне больно смотреть на это. Ты кусок недоразумения.",
            "Вот смотрю я на твои старания, {0}, и понимаю, что ты -- плод скрещивания двух обмазанных говном горилл.",
            "\"Дорогой {0}, нам стыдно за тебя.\" (с) твои мама и папа.",
            // ASCII-art here
            "  .-\'---`-.\r\n,\'          `.\r\n|             \\\r\n|              \\\r\n\\           _  \\\r\n,\\  _ " +
            "   ,\'-,/-)\\\r\n( * \\ \\,\' ,\' ,\'-)\r\n `._,)     -\',-\')\r\n   \\/         \'\'/\r\n    )        / /" +
            "\r\n   /       ,\'-\'"
        };

        private static int _minimumNumber;
        public static int MinimumNumber
        {
            get
            {
                return _minimumNumber;
            }
            set
            {
                if (value <= _maximumNumber) _minimumNumber = value;
            }
        }

        private static int _maximumNumber;
        public static int MaximumNumber
        {
            get
            {
                return _maximumNumber;
            }
            set
            {
                if (value >= _minimumNumber) _maximumNumber = value;
            }
        }

        public static uint HistorySize { get; set; }

        public static uint TauntPeriod { get; set; }

        
        private readonly string _name;
        private readonly int _guessedNumber;
        private uint _numberOfGuesses;
        private readonly Tuple<int, bool>[] _history;

        private readonly Random _rng;
        private readonly DateTime _startTime;

        public Game(string name)
        {
            MinimumNumber = 0;
            MaximumNumber = 100;
            HistorySize = 1000;
            TauntPeriod = 4;

            _name = name;
            _numberOfGuesses = 0;
            _history = new Tuple<int,bool>[HistorySize];

            _rng = new Random();
            _guessedNumber = _rng.Next(MinimumNumber, MaximumNumber + 1);
            _startTime = DateTime.Now;
            Console.WriteLine("Загадано число от {0} до {1}. Попробуй его угадать.", 
                MinimumNumber, MaximumNumber);
        }

        public void Run()
        {
            while (_numberOfGuesses < HistorySize)
            {
                string userInput = Console.ReadLine();
                if (userInput == "q")
                {
                    Console.WriteLine("Уже уходишь?");
                    if (_numberOfGuesses >= TauntPeriod)
                    {
                        Console.WriteLine("Ну тогда я извиняюсь за сказанное ранее.");
                        Console.WriteLine("Ты же понимаешь, что не я так считаю, а мой программист.");
                    }
                    Console.WriteLine("Пока.");
                    return;
                }
                int userGuess;
                if (!int.TryParse(userInput, out userGuess))
                {
                    Console.WriteLine("От тебя просили ввести целое число. Будь любезен, введи целое число.");
                    Console.WriteLine("Ты вообще знаешь что за целые числа? Штуки такие, из циферок состоят.");
                    Console.WriteLine("Их ещё в школе проходят.");
                    continue;
                }
                if (userGuess == _guessedNumber)
                {
                    // intended typo
                    Console.WriteLine("Моё увожение, {0}! Ты сделал это!", _name);

                    Console.WriteLine("А вот и твои \"успехи\":");
                    for (int i = 0; i < _numberOfGuesses; i++)
                    {
                        var entry = _history[i];
                        Console.WriteLine("{0}\t{1}", entry.Item1, (entry.Item2 ? '<' : '>'));
                    }
                    Console.WriteLine("{0}\t==", _guessedNumber);
                    int floorMinutes = (int) (DateTime.Now - _startTime).TotalMinutes;
                    Console.WriteLine("Потрачено минут: {0}", 
                        (floorMinutes == 0 ? "менее минуты" : floorMinutes.ToString()));

                    Console.WriteLine("А теперь иди и покажи это своей маме.");
                    return;
                }
                _history[_numberOfGuesses] = new Tuple<int, bool>(userGuess, _guessedNumber < userGuess);
                ++_numberOfGuesses;
                if (_numberOfGuesses % TauntPeriod == 0)
                    Console.WriteLine(Taunts[_rng.Next(Taunts.Length)], _name);
                Console.WriteLine("Загаданное мною число {0} введённого тобой. Попробуй ещё раз.", 
                    (_guessedNumber < userGuess ? "меньше" : "больше"));
            }
            Console.WriteLine("{0}. Ты настолько тупой, что даже не можешь угадать число от {1} до {2}!", 
                _name, MinimumNumber, MaximumNumber);
        }
    }

    class Program
    {
        static void Main()
        {
            Console.Write("Введите своё имя: ");
            string name = Console.ReadLine();
            Game game = new Game(name);
            game.Run();
            Console.WriteLine("Нажмите любую клавишу, чтобы выйти.");
            Console.ReadKey(false);
        }
    }
}
