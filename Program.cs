global using ConsoleHelpers;
global using CH = ConsoleHelpers.ConsoleHandler;
using System.Reflection;

namespace Sourcerer
{
    internal class Program
    {
        static string Version
        {
            get
            {
                Assembly? a = Assembly.GetEntryAssembly();
                if (a != null)
                {
                    Version? v = a.GetName().Version;
                    if (v != null)
                        return "Version: " + v;
                }
                return "Failed to retrive version";
            }
        }

        static string Help => "\n" + Version + " help\n\n" +
                            "[command] \t \t[command help]\n" +
                            "-h/--help \t-\tDisplay help menu\n" +
                            "--version \t-\tDisplay Sourcerer version\n" +
                            "-o <path> \t-\tSpecify path to output file, default is /SourcererOut.txt\n";
        static int Main(string[] args)
        {
            Console.Title = "Sourcerer";

            Console.CancelKeyPress += delegate (object? sender, ConsoleCancelEventArgs e) {
                e.Cancel = true;
                CH.RevertConsoleUI();
                throw new ConsoleReturnException();
            };

            string? path = null;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--help" || args[i] == "-h")
                {
                    Console.WriteLine(Help);
                    return 0;
                }
                if (args[i] == "--version")
                {
                    Console.WriteLine(Version);
                    return 0;
                }
                if (args[i] == "-o")
                {
                    path = args[++i];
                }
            }

            string format;
            try
            {
                ASource source = SourceHandler.PickSource();
                format = source.Format();
            }
            catch (ConsoleReturnException) { return -1073741510; }

            CH.RevertConsoleUI();

            if (args.Length == 0 || path == null || path == "")
            {
                while (path == null)
                {
                    Console.WriteLine("Specify source path/file name");
                    path = Console.ReadLine();
                }
                path = path.Trim();
                if (path == "") path = "SourcererOut.txt";
            }

            File.AppendAllText(path, format);
            Console.WriteLine(format);
            Console.ReadKey();

            //if (args.Length == 0) return;
            //string file = File.ReadAllText(args[0]);
            //List<string> lines = file.Split(' ').ToList();

            //for (int i = 0; i < lines.Count; i++)
            //{
            //    lines[i] = lines[i].Trim();
            //    if (lines[i].StartsWith("http") || lines[i].Length == 0)
            //    {
            //        lines.RemoveAt(i);
            //        i--;
            //        continue;
            //    }
            //    lines[i] = $"        \"§:{lines[i]}\",";
            //}

            //File.WriteAllLines("dump.txt", lines.ToArray());
            return 0;
        }
    }
}