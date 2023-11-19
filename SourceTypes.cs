using System;

namespace Sourcerer
{
    abstract class ASource
    {
        public abstract SourceInfo[] SourceInf { get; }
        public abstract string Type { get; }
        public abstract string Format();
        public virtual void GetInfo()
        {
            int chosenInfo = -1;
            int currentInfo = 0;
            while (true)
            {
                do
                {
                    Console.Clear();
                    Console.WriteLine(Type);
                    for (int i = 0; i < SourceInf.Length; i++)
                    {
                        if (i == currentInfo)
                        {
                            CH.PrintChosen($"[{i}]\t [{SourceInf[i].Type}] ");
                        }
                        else
                        {
                            CH.PrintNormal($"[{i}]\t" + $"  {SourceInf[i].Type}");
                        }

                        if (SourceInf[i].InfoGotten)
                        {
                            CH.PrintCorrect('\t' + SourceInf[i].Info);
                        }
                        else
                        {
                            CH.PrintIncorrect(SourceInf[i].Info == "" ? "\t{INFORMATION KRÄVS}" : SourceInf[i].Info);
                        }
                    }
                    if (currentInfo == SourceInf.Length)
                    {
                        CH.PrintChosen($"[{SourceInf.Length}]\t [KLAR] ");
                    }
                    else
                    {
                        CH.PrintNormal($"[{SourceInf.Length}]\t  KLAR");
                    }

                    CH.KeyHandler(Console.ReadKey().Key, SourceInf.Length+1, ref chosenInfo, ref currentInfo);
                } while (chosenInfo == -1);

                if (chosenInfo < SourceInf.Length)
                {
                    SourceInf[chosenInfo].GetInfo();
                }
                else
                {
                    bool allGotten = true;
                    foreach (SourceInfo inf in SourceInf)
                    {
                        if (!inf.InfoGotten) allGotten = false;
                    }

                    if (allGotten) //Chosen info has to be "done" option and all required info is gotten; we can exit
                    {
                        break;
                    }
                    CH.PrintIncorrect("ALL NÖDVÄNDIG INFORMATION ÄR INTE SPECIFIERAD");
                    Console.ReadKey();
                }
                chosenInfo = -1;
            }

            infoGotten = true;
        }

        //protected string title = "";
        //public string Title { get => title; protected set => title = value; }

        //public string Date = "";
        //public string Edition = "";
        //public string Publisher = "";
        //public string PubLocation = "";
        //public string URL = "";
        //public string DownloadDate = "";
        //public bool Editor = false;

        protected bool infoGotten = false;
    }

    class Bok : ASource
    {
        public override string Type { get => "Bok/E-Bok"; }

        //GetAuthors();
        //GetEditor();
        //GetDate("år");
        //GetTitle();
        //GetEdition();
        //GetPublisher();
        //GetPubLocation();
        //GetEBook();

        readonly Authors authors = new(true);
        readonly Editor editor = new(false);
        readonly Date date = new(true, "år");
        readonly Title title = new(true);
        readonly Edition edition = new(false);
        readonly Publisher publisher = new(true);
        readonly PubLocation pubLocation = new(true);
        readonly Copyright copyright = new(false);
        readonly URL uRL = new(false);
        readonly DownloadDate downloadDate = new(false);

        public override SourceInfo[] SourceInf => new SourceInfo[]{
            authors, editor, date, title, edition, publisher, pubLocation, copyright, uRL, downloadDate
        };

        public override string Format()
        {
            while (!infoGotten) GetInfo();
            string format = "";
            format += authors;
            format += editor;
            format += $". K{title}K. ";
            if (edition.Info != "") format += edition + ". uppl. ";
            format += pubLocation + ": ";
            format += publisher + ", ";
            format += date + ". ";
            if (uRL.Info == "" || downloadDate.Info == "") format += copyright;
            else format += uRL + " " + downloadDate + ". ";
            return format + '\n';
        }

        //"Författarens efternamn, förnamnsinitial(er)., Författarens efternamn, förnamnsinitial(er). & Författarens efternamn, förnamnsinitial(er). (Utgivningsår). Titel. Upplaga (om ej 1:a upplaga). Förlag."; 
        //En, två/flera författare, ibland redaktör, E-bok, Kapitel/del i bok
    }

    class Tidskrift : ASource
    {
        public override string Type { get => "Tidskrift"; }

        //void GetWebPaper()
        //{
        //    IsEPaper = false;
        //    Console.WriteLine("Websida? (y/n): ");
        //    string? webpage = Console.ReadLine();
        //    if (webpage != null)
        //    {
        //        IsEPaper = YesAnswer(webpage);
        //    }
        //    if (!IsEPaper) return;

        //    GetURL();
        //    GetDownloadDate();
        //}

        readonly Authors authors = new(true);
        readonly Title title = new(true);
        readonly Date date = new(true, "yyyy-MM-dd");
        readonly Publisher publisher = new(true);
        readonly URL uRL = new(false);
        readonly DownloadDate downloadDate = new(false);

        public override SourceInfo[] SourceInf
        {
            get => new SourceInfo[] {
            authors, title, date, publisher, uRL, downloadDate
            }; /*err GetWebPaper()*/
        }

        public override string Format()
        {
            return authors +
            $". {title}. " +
            $"K{publisher}K. " +
            date +
            ((uRL.Info != "" && downloadDate.Info != "") ? $". {uRL} {downloadDate}. " : ". ") + "\n";
        }

    //Tidningsartikel, vetenskaplig tidsskrift, [Tidskrift på webben]
}

    class Websida : ASource
    {
        public override string Type { get => "Övrig websida"; }
        public string Website = "";
        public bool WrittenByPerson = false;

        readonly AuthorOrg authorOrg = new(true);
        readonly Title title = new(true);
        readonly Website website = new(true);
        readonly Date date = new(true, "år (senast uppdaterad)");
        readonly URL uRL = new(true);
        readonly DownloadDate downloadDate = new(true);

        public override SourceInfo[] SourceInf => new SourceInfo[] {
            authorOrg, title, website, date, uRL, downloadDate
        };

        public override string Format()
        {
            return authorOrg +
            $". {title}. " +
            $"K{website}K. " +
            date +
            $". {uRL} {downloadDate}.\n";
        }
        //[Tidskrift på webben], [E-Bok], Webbsidor, bloggar eller tweetar, elektroniskt uppslagsverk, 
    }

    class Uppslagsverk : ASource
    {
        public override string Type { get => "(Elektroniskt) Uppslagsverk"; }

        readonly Authors authors = new(false);
        readonly Title title = new(true);
        readonly Publisher publisher = new(true);
        readonly Date date = new(true, "år (utgivet)");
        readonly URL uRL = new(true);
        readonly DownloadDate downloadDate = new(true);

        public override SourceInfo[] SourceInf => new SourceInfo[] {
            authors, title, publisher, date, uRL, downloadDate
        };

        public override string Format()
        {
            string format = "";
            if (authors.Info != "") format += authors.Info;
            else format += $"K{publisher}K";
            format += $". {title}. ";
            if (authors.Info != "") format += $"K{publisher}K. ";
            format += date +
            $". {uRL} {downloadDate}.\n";
            return format;
        }
    }

    class Avhandling : ASource
    {
        public override string Type { get => "Avhandling"; }

        readonly Authors authors = new(true);
        readonly Title title = new(true);
        readonly Date date = new(true, "år");
        readonly SpecQuestion universitet = new(true, "Universitet", "Universitet: ");
        readonly SpecQuestion avhandlingstyp = new(true, "Avhandlingstyp" , "Typ av avhandling (ex \"diss.\" för doktorsavhandling och \"lic.-avh.\" för licentiatavhandling): ");

        public override SourceInfo[] SourceInf => new SourceInfo[] {
            authors, title, date, universitet, avhandlingstyp
        };

        public override string Format()
        {
            return
            authors.Info + 
            $". K{title}K. " + 
            avhandlingstyp + ", " +
            universitet + ", " +
            date + ". \n";
        }
    }

    class Rapport : ASource
    {
        public override string Type { get => "Rapport"; }

        readonly AuthorOrg authOrg = new(true);
        readonly Date date = new(true, "Utgivningsår");
        readonly Title title = new(true);
        readonly Publisher publisher = new(true);
        readonly PubLocation pubLocation = new(true);
        readonly URL uRL = new(true);
        readonly DownloadDate downloadDate = new(true);

        public override SourceInfo[] SourceInf => new SourceInfo[]{
            authOrg, date, title, publisher, pubLocation, uRL, downloadDate
        };

        public override string Format()
        {
            while (!infoGotten) GetInfo();
            return authOrg +
            $". K{title}K. " +
            pubLocation + ": " +
            publisher + ", " + 
            date + ". " +
            uRL + " " + downloadDate + ". \n";
        }
    }

    class TVProgram : ASource
    {
        public override string Type { get => "TV-Program"; }

        readonly AuthorOrg authOrg = new(false);
        readonly Date date = new(true, "år");
        readonly Title title = new(true);
        readonly Publisher publisher = new(true);
        readonly PubLocation pubLocation = new(false);
        readonly URL uRL = new(false);
        readonly SpecQuestion program = new(true, "TV-Program", "Vilket TV-Program: ");

        public override SourceInfo[] SourceInf => new SourceInfo[]{
            authOrg, date, program, title, publisher, pubLocation, uRL
        };

        public override string Format()
        {
            return $"{authOrg}. {program} - {title} (TV-Program). {pubLocation}: {publisher}. {date}. {uRL + (uRL.Info == "" ? "" : ". ")}";
        }
        //Lindsjö, Lars. UR Samtiden - Hur kan utåtagerande barn bemötas? (TV-program). Stockholm: Sveriges utbildningsradio. 2011. http://uraccess.se/
    }
}
