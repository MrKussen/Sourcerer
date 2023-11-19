namespace Sourcerer
{
    static class SourceHandler
    {
        private static readonly ASource[] sources = { new Bok(), new Tidskrift(), new Websida(), new Uppslagsverk(), new Avhandling(), new Rapport(), new TVProgram() };

        public static ASource PickSource()
        {
            Pick:
            CH.SetupConsoleUI();
            int currentSource = 0;
            int chosenSource = -1;
            do
            {
                Console.Clear();
                CH.PrintNormal("[ ]\t [Källa] ");
                for (int i = 0; i < sources.Length; i++)
                {
                    if (i == currentSource)
                    {
                        CH.PrintChosen($"[{i}]\t [{sources[i].Type}]");
                    }
                    else
                    {
                        CH.PrintNormal($"[{i}]\t" + $"  {sources[i].Type}");
                    }
                }

                CH.KeyHandler(Console.ReadKey().Key, sources.Length, ref chosenSource, ref currentSource);
            } while (chosenSource == -1);

            try
            {
                sources[chosenSource].GetInfo();
            }
            catch (ConsoleReturnException) { goto Pick; }
            return sources[chosenSource];
        }
    }

    class Name
    {
        public string FirstNames { get; }
        public string LastNames { get; }
        public Name(string firstNames, string lastNames)
        {
            FirstNames = firstNames.Trim();
            LastNames = lastNames.Trim();
        }

        public static string InitialsOf(string name)
        {
            string[] names = name.Trim().Split(' ');
            string initials = "";
            foreach (string n in names)
            {
                initials += n[0] + ".";
            }
            return initials;
        }

        public string FirstInitials()
        {
            return InitialsOf(FirstNames);
        }

        public string Initials()
        {
            return InitialsOf(FirstNames) + InitialsOf(LastNames);
        }
    }
}