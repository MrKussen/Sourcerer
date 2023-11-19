namespace Sourcerer
{
    abstract class SourceInfo
    {
        protected bool requiredField;
        protected bool infoGotten = false;
        public bool InfoGotten { get => !requiredField || infoGotten; protected set => infoGotten = value; }

        public SourceInfo(bool isRequired) => requiredField = isRequired;

        public abstract string Type { get; }
        protected abstract string AskInfo();
        protected string info = "";
        public virtual string Info { get => info; protected set => info = value; }
        public virtual void GetInfo()
        {
            Info = AskInfo();
            InfoGotten = Info != "";
        }

        public override string ToString() => Info;

        private static string BaseQuestion(string question)
        {
            string? ans = null;
            CH.PrintNormal(question);
            while (ans == null) ans = Console.ReadLine();
            ans = ans.Trim();
            return ans;
        }

        protected static string Question(string question)
        {
            CH.RevertConsoleUI();
            string ans = BaseQuestion(question);
            CH.SetupConsoleUI();

            return ans;
        }

        protected static string Question(string question, string elseValue)
        {
            string ans = Question(question);
            return ans == "" ? elseValue : ans;
        }

        protected static bool YesNoQuestion(string question)
        {
            string ans = Question(question);
            ans = ans.Trim().ToLower();
            return ans == "y" || ans == "yes" || ans == "ja" || ans == "j" || ans == "true";
        }

        protected static string QuestionNoUISet(string question)
        {
            return BaseQuestion(question);
        }
    }

    class Authors : SourceInfo
    {
        public Authors(bool isRequired) : base(isRequired) { }

        public override string Type { get => "Författare"; }
        protected override string AskInfo()
        {
            CH.RevertConsoleUI();
            List<Name> ath = new();
            Console.WriteLine("Namn i alfabetisk ordning (/redaktör sist): ");
            for (int i = 0; true; i++)
            {
                string authorF;
                do
                {
                    authorF = QuestionNoUISet("Författare " + (i + 1) + ":s förnamn: ");
                } while (i == 0 && authorF == "");
                if (authorF == "") break;

                string authorL;
                do
                {
                    authorL = QuestionNoUISet("Författare " + (i + 1) + ":s efternamn: ");
                } while (i == 0 && authorL == "");
                if (authorL == "") break;

                ath.Add(new(authorF, authorL));
            }
            authors = ath;
            CH.SetupConsoleUI();
            return "";
        }
        protected List<Name> authors = new();
        public override string Info
        {
            get
            {
                string ath = "";
                for (int i = 0; i < authors.Count; i++)
                {
                    ath += authors[i].LastNames + ", " + authors[i].FirstNames;
                    if (i == authors.Count - 2 && i >= 0) ath += " och ";
                    else if (i < authors.Count - 2) ath += "; ";
                }
                return ath;
            }
        }
    }

    class Date : SourceInfo
    {
        public override string Type { get => "Datum"; }
        readonly string format = "";
        public Date(bool isRequired, string format) : base(isRequired)
        {
            this.format = format;
        }

        protected override string AskInfo()
        {
            string ans = Question($"Utgivnings-/publikationsdatum ({format}): ", "u.å");
            return ans.ToLower().Contains("u.å") || ans.ToLower().Contains("u. å") ? "[u.å.]" : ans;
        }
    }

    class Title : SourceInfo
    {
        public Title(bool isRequired) : base(isRequired) { }

        public override string Type { get => "Titel"; }
        protected override string AskInfo()
        {
            return Question("Titel: ");
        }
    }

    class Edition : SourceInfo
    {
        public Edition(bool isRequired) : base(isRequired) { }

        public override string Type { get => "Utgåva"; }
        protected override string AskInfo()
        {
            string ans = Question("Utgåva (endast siffra): ");
            if (int.TryParse(ans, out int i) && ans != "")
                if (i > 0) return ans + ". uppl";
            return "";
        }
    }

    class Publisher : SourceInfo
    {
        public Publisher(bool isRequired) : base(isRequired) { }

        public override string Type { get => "Förlag/Publicerare"; }
        protected override string AskInfo()
        {
            return Question("Vem har publicerat texten; Förlag/Tidning/Organisation/etc: ");
        }
    }

    class PubLocation : SourceInfo
    {
        public PubLocation(bool isRequired) : base(isRequired) { }

        public override string Type { get => "Förlagsort/ort"; }
        protected override string AskInfo()
        {
            return Question("Förlagsort: ");
        }
    }

    class Editor : SourceInfo
    {
        public Editor(bool isRequired) : base(isRequired) { }

        public override string Type { get => "Redaktör"; }
        bool hasEditor;
        public override string Info { get => hasEditor ? " (red.)" : ""; }
        protected override string AskInfo()
        {
            hasEditor = YesNoQuestion("Har redaktör (för samlingar och antologier - y/n): ");
            return "";
        }
    }

    class URL : SourceInfo
    {
        public URL(bool isRequired) : base(isRequired) { }

        public override string Type { get => "URL"; }
        protected override string AskInfo()
        {
            return Question("URL: ");
        }
    }

    class DownloadDate : SourceInfo
    {
        public DownloadDate(bool isRequired) : base(isRequired) { }

        public override string Type { get => "Hämtdatum"; }
        protected override string AskInfo()
        {
            return "(Hämtad " + Question("Hämtdatum (yyyy-MM-dd): ") + ")";
        }
    }

    class Copyright : SourceInfo
    {
        public Copyright(bool isRequired) : base(isRequired) { }

        public override string Type { get => "Upphovsrätt"; }
        bool hasCopyright;
        public override string Info { get => hasCopyright ? "E-Bok. " : ""; }
        protected override string AskInfo()
        {
            hasCopyright = YesNoQuestion("Har upphovsrätt? [E-Bok] (y/n): ");
            return "";
        }
    }

    class AuthorOrg : SourceInfo
    {
        public AuthorOrg(bool isRequired) : base(isRequired) { }

        public override string Type { get => "Author/Organisation"; }
        protected override string AskInfo()
        {
            if (YesNoQuestion("Author is person(s)? (y/n)"))
            {
                Authors ath = new(true);
                ath.GetInfo();
                return ath.Info;
            }
            Publisher pub = new(true);
            pub.GetInfo();
            return pub.Info;
        }
    }

    class Website : SourceInfo
    {
        public Website(bool isRequired) : base(isRequired) { }

        public override string Type { get => "Webplats/websidas ägare"; }
        protected override string AskInfo()
        {
            return Question("Webplats titel/ägare: ");
        }
    }

    /// <summary>
    /// Special/disposable question that will be used only for one source type
    /// </summary>
    class SpecQuestion : SourceInfo
    {
        string question;
        string title;
        public SpecQuestion(bool isRequired, string title, string question) : base(isRequired) { this.title = title; this.question = question; }

        public override string Type { get => title; }
        protected override string AskInfo()
        {
            return Question(question);
        }
    }
}
