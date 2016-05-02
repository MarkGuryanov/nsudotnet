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

        private int _minimumNumber;
        public int MinimumNumber
        {
            get { return _minimumNumber; }
            set
            {
                if (value <= _maximumNumber) _minimumNumber = value;
            }
        }

        private int _maximumNumber;
        public int MaximumNumber
        {
            get { return _maximumNumber; }
            set
            {
                if (value >= _minimumNumber) _maximumNumber = value;
            }
        }

        private uint _historySize;
        public uint HistorySize
        {
            get { return _historySize; }
            set
            {
                if (value > 0)
                {
                    _historySize = value;
                    _guessHistory = new int[_historySize];
                }
            }
        }

        private int _tauntPeriod;
        public int TauntPeriod
        {
            get { return _tauntPeriod; }
            set
            {
                if (value > 0) _tauntPeriod = value;
            }
        }
        
        private readonly string _userName;
        private int[] _guessHistory;
        private Random _rng;

        public Game(string userName)
        {
            _userName = userName;
            _rng = new Random();
            
            MinimumNumber = 0;
            MaximumNumber = 100;
            HistorySize = 1000;
            TauntPeriod = 4;
        }

        public void Run()
        {
            int guessedNumber = _rng.Next(MinimumNumber, MaximumNumber + 1);
            DateTime startTime = DateTime.Now;
            Console.WriteLine("Загадано число от {0} до {1}. Попробуй его угадать.",
                MinimumNumber, MaximumNumber);

            int numberOfGuesses = 0;
            while (numberOfGuesses < HistorySize)
            {
                string userInput = Console.ReadLine();
                if (userInput == "q")
                {
                    Console.WriteLine("Уже уходишь?");
                    if (numberOfGuesses >= TauntPeriod)
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
                if (userGuess == guessedNumber)
                {
                    // intended typo
                    Console.WriteLine("Моё увожение, {0}! Ты сделал это!", _userName);

                    Console.WriteLine("А вот и твои \"успехи\":");
                    for (int i = 0; i < numberOfGuesses; i++)
                    {
                        int guess = _guessHistory[i];
                        Console.WriteLine("{0}\t{1}", guess,(guess < guessedNumber ? '<' : '>'));
                    }
                    Console.WriteLine("{0}\t==", guessedNumber);
                    int floorMinutes = (int) (DateTime.Now - startTime).TotalMinutes;
                    Console.WriteLine("Потрачено минут: {0}", 
                        (floorMinutes == 0 ? "менее минуты" : floorMinutes.ToString()));

                    Console.WriteLine("А теперь иди и покажи это своей маме.");
                    return;
                }
                _guessHistory[numberOfGuesses] = userGuess;
                ++numberOfGuesses;
                if (numberOfGuesses % TauntPeriod == 0)
                    Console.WriteLine(Taunts[_rng.Next(Taunts.Length)], _userName);
                Console.WriteLine("Загаданное мною число {0} введённого тобой. Попробуй ещё раз.", 
                    (guessedNumber < userGuess ? "меньше" : "больше"));
            }
            Console.WriteLine("{0}. Ты настолько тупой, что даже не можешь угадать число от {1} до {2}!", 
                _userName, MinimumNumber, MaximumNumber);
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
