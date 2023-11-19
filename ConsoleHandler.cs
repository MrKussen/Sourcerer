namespace ConsoleHelpers
{
    static class ConsoleHandler
    {
        static ConsoleColor foreStart = ConsoleColor.White;
        static ConsoleColor backStart = ConsoleColor.Black;

        public static bool isUI;

        public static void PrintChosen(string text)
        {
            PrintColoured(text, ConsoleColor.White);
        }

        public static void PrintNormal(string text)
        {
            Console.WriteLine(text);
        }

        public static void PrintColoured(string text, ConsoleColor colour)
        {
            ConsoleColor backPrev = Console.BackgroundColor;
            Console.BackgroundColor = colour;
            Console.WriteLine(text);
            Console.BackgroundColor = backPrev;
        }

        public static void PrintIncorrect(string text)
        {
            PrintColoured(text, ConsoleColor.Red);
        }

        public static void PrintCorrect(string text)
        {
            PrintColoured(text, ConsoleColor.Green);
        }

        public static void SetupConsoleUI()
        {
            if (!isUI)
            {
                foreStart = Console.ForegroundColor;
                backStart = Console.BackgroundColor;
            }
            //Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Blue;
            isUI = true;
            Console.Clear();
        }

        public static void RevertConsoleUI()
        {
            Console.ForegroundColor = foreStart;
            Console.BackgroundColor = backStart;
            //Console.CursorVisible = true;
            isUI = false;
            Console.Clear();
        }

        public static void KeyHandler(ConsoleKey key, int optionCount, ref int chosenSource, ref int currentSource)
        {
            switch (key)
            {
                case ConsoleKey.UpArrow or ConsoleKey.W:
                    if (currentSource > 0) currentSource--;
                    else currentSource = optionCount - 1;
                    break;
                case ConsoleKey.DownArrow or ConsoleKey.S:
                    if (currentSource < optionCount - 1) currentSource++;
                    else currentSource = 0;
                    break;
                case ConsoleKey.Enter or ConsoleKey.Spacebar:
                    chosenSource = currentSource;
                    break;
                case >= ConsoleKey.NumPad0 and <= ConsoleKey.NumPad9:
                    chosenSource = key - ConsoleKey.NumPad0;
                    break;
                case >= ConsoleKey.D0 and <= ConsoleKey.D9:
                    chosenSource = key - ConsoleKey.D0;
                    break;
                case >= ConsoleKey.Escape:
                    throw new ConsoleReturnException();
                default:
                    break;
            }
        }
    }

    [Serializable]
    public class ConsoleReturnException : Exception
    {
        public ConsoleReturnException()
            : base("Console was cancelled")
        { }

        public ConsoleReturnException(string message)
            : base(message)
        { }

        public ConsoleReturnException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}