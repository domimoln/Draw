using System;
using System.Diagnostics.Metrics;
using System.Data.SQLite;

class Program
{
    static string connectionString = "Data Source=rajzok.db;Version=3;";
    static void Main()
    {
        InitializeDatabase();
        // Opciók listája
        string[] menuOpcio = { "Rajzolás", "Szerkesztés", "Mentés", "Kilépés" };
        int aktualisKivalasztas = 0;
        ConsoleKey billentyu;

        RajzoldMenu(menuOpcio, aktualisKivalasztas, null);
        bool enterLenyomva = false;

        do
        {
            if (!enterLenyomva)
            {
                RajzoldMenu(menuOpcio, aktualisKivalasztas, null);
            }
            else
            {
                RajzoldMenu(menuOpcio, aktualisKivalasztas, menuOpcio[aktualisKivalasztas]);
                enterLenyomva = false;
            }

            billentyu = Console.ReadKey(true).Key;

            switch (billentyu)
            {
                case ConsoleKey.UpArrow:
                    if (aktualisKivalasztas > 0)
                        aktualisKivalasztas--;
                    break;

                case ConsoleKey.DownArrow:
                    if (aktualisKivalasztas < menuOpcio.Length - 1)
                        aktualisKivalasztas++;
                    break;

                case ConsoleKey.Enter:
                    if (menuOpcio[aktualisKivalasztas] == "Kilépés")
                    {
                        return;
                    }
                    else if (menuOpcio[aktualisKivalasztas] == "Rajzolás")
                    {
                        Rajzolo_Resz();
                    }

                    else if (menuOpcio[aktualisKivalasztas] == "Szerkesztés")
                    {
                        Szerkesztes();
                    }

                    else if (menuOpcio[aktualisKivalasztas] == "Mentés")
                    {
                        Mentes_Resz();
                    }
                    RajzoldMenu(menuOpcio, aktualisKivalasztas, menuOpcio[aktualisKivalasztas]);
                    enterLenyomva = true;
                    break;
            }
        }
        while (billentyu != ConsoleKey.Escape);
    }

    static void InitializeDatabase()
    {
        using (SQLiteConnection conn = new SQLiteConnection(connectionString))
        {
            conn.Open();
            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Rajz (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    X INT,
                    Y INT,
                    Szin TEXT,
                    RajzNeve TEXT
                )";
            using (SQLiteCommand cmd = new SQLiteCommand(createTableQuery, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }
    }

    static void RajzoldMenu(string[] opciok, int kivalasztottIndex, string kivalasztottOpcio)
    {
        Console.Clear();

        int konzolSzelesseg = Console.WindowWidth;
        int konzolMagassag = Console.WindowHeight;
        int szelesseg = 20;
        int belsoSzelesseg = szelesseg - 2;
        int menuMagassag = opciok.Length * 3;

        int felsoPozicio = (konzolMagassag / 2) - (menuMagassag / 2);
        int balPozicio = (konzolSzelesseg / 2) - (szelesseg / 2);

        for (int i = 0; i < opciok.Length; i++)
        {
            Console.SetCursorPosition(balPozicio, felsoPozicio + i * 3);
            Console.WriteLine("|" + new string('-', belsoSzelesseg) + "|");


            string szoveg = opciok[i].PadLeft((belsoSzelesseg / 2) + (opciok[i].Length / 2)).PadRight(belsoSzelesseg);
            Console.SetCursorPosition(balPozicio, felsoPozicio + 1 + i * 3);

            if (i == kivalasztottIndex)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
            }

            Console.WriteLine("|" + szoveg + "|");
            Console.ResetColor();

            Console.SetCursorPosition(balPozicio, felsoPozicio + 2 + i * 3);
            Console.WriteLine("|" + new string('-', belsoSzelesseg) + "|");
        }
    }

    static void Rajzolo_Resz()
    {
        Console.Clear();
        int kurzorX = 0;
        int kurzorY = 0;
        ConsoleColor aktSzin = ConsoleColor.White;

        Dictionary<(int, int), string> szinezettPoziciok = new Dictionary<(int, int), string>(); //Console színe és a 2 koordináta mentése csak, ha átszínezne egy meglevo koord-ot


        Console.Clear();
        ConsoleKeyInfo gomb_lenyom;

        while (true)
        {
            kiIras();
            gomb_lenyom = Console.ReadKey(true);

            if (gomb_lenyom.Key == ConsoleKey.Escape)
            {
                Mentes(szinezettPoziciok);
                break;
            }
            KezeljeAGombNyom(gomb_lenyom);

        }

        void KezeljeAGombNyom(ConsoleKeyInfo gomb)
        {

            if (gomb_lenyom.Key == ConsoleKey.LeftArrow)
            {
                kurzorX = Math.Max(0, kurzorX - 1);
            }
            else if (gomb_lenyom.Key == ConsoleKey.LeftArrow)
            {
                kurzorX = Math.Max(0, kurzorX - 1);
            }
            else if (gomb_lenyom.Key == ConsoleKey.RightArrow)
            {
                kurzorX = Math.Min(Console.WindowWidth - 1, kurzorX + 1);
            }
            else if (gomb_lenyom.Key == ConsoleKey.UpArrow)
            {
                kurzorY = Math.Max(0, kurzorY - 1);
            }
            else if (gomb_lenyom.Key == ConsoleKey.DownArrow)
            {
                kurzorY = Math.Min(Console.WindowHeight - 1, kurzorY + 1);
            }
            else if (gomb_lenyom.Key == ConsoleKey.Spacebar)
            {
                rajzolas_a_kurzornal();
            }
            else
            {
                if (char.IsDigit(gomb_lenyom.KeyChar))
                {
                    SzinValtas(gomb_lenyom.KeyChar);
                }
            }
        }
        void rajzolas_a_kurzornal()
        {
            Console.SetCursorPosition(kurzorX, kurzorY);
            Console.BackgroundColor = aktSzin;
            Console.Write(" ");
            Console.ResetColor();


            szinezettPoziciok[(kurzorX, kurzorY)] = aktSzin.ToString();
        }
        void SzinValtas(char number)
        {
            int szinIndexe = int.Parse(number.ToString()) % 16;
            aktSzin = (ConsoleColor)szinIndexe;
        }
        void kiIras()
        {
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"Pos: ({kurzorX}, {kurzorY}) | Color: {aktSzin}   ");
            Console.ResetColor();
        }
        void Mentes(Dictionary<(int, int), string> poziciok)
        {
            using (StreamWriter mentes = new StreamWriter("rajz.txt", true))
            {
                foreach (var poz in poziciok)
                {
                    mentes.WriteLine($"{poz.Value};{poz.Key.Item1};{poz.Key.Item2}");
                }
            }
        }
    }
    static void Mentes_Resz()
    {
        Console.Clear();
        Console.Write("Add meg a file nevét amin menteni akarod:");
        string mentesneve = Console.ReadLine();
        if (!string.IsNullOrEmpty(mentesneve))
        {
            string asd = $@".\mentes\{mentesneve}.txt";
            File.Copy("rajz.txt", asd, true);
            Console.WriteLine($"Biztonsági mentés sikeres lett ide: ${asd} emellett az adatbázisba a mentés szintén sikeres");
            File.Delete("rajz.txt");
            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
            {
                return;
            }
        }
        else
        {
            return;
        }
    }

    static void Szerkesztes()
    {
        Console.Clear();
        string mentesMappa = "./mentes";

        if (!Directory.Exists(mentesMappa))
        {
            Console.WriteLine("A 'mentes' mappa nem található.");
            return;
        }

        var fajlok = Directory.GetFiles(mentesMappa, "*.txt").Select(Path.GetFileNameWithoutExtension).ToList();

        if (fajlok.Count == 0)
        {
            Console.WriteLine("Nincs elérhető fájl a szerkesztéshez.");
            return;
        }

        int aktualisKivalasztas = 0;
        ConsoleKey billentyu;
        do
        {
            Console.Clear();
            Console.WriteLine("Válaszd ki a szerkesztendő fájlt:");

            for (int i = 0; i < fajlok.Count; i++)
            {
                if (i == aktualisKivalasztas)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.WriteLine(fajlok[i]);
                Console.ResetColor();
            }

            billentyu = Console.ReadKey(true).Key;

            switch (billentyu)
            {
                case ConsoleKey.UpArrow:
                    if (aktualisKivalasztas > 0)
                        aktualisKivalasztas--;
                    break;

                case ConsoleKey.DownArrow:
                    if (aktualisKivalasztas < fajlok.Count - 1)
                        aktualisKivalasztas++;
                    break;

                case ConsoleKey.Enter:
                    string szerkhelye = Path.Combine(mentesMappa, fajlok[aktualisKivalasztas] + ".txt");
                    BetoltesEsSzerkesztes(szerkhelye);
                    return;
            }
        } while (billentyu != ConsoleKey.Escape);
    }

    static void BetoltesEsSzerkesztes(string szerkhelye)
    {
        if (!File.Exists(szerkhelye))
        {
            Console.WriteLine("A fájl nem található.");
            return;
        }

        Dictionary<(int, int), string> szinezettPoziciok = new Dictionary<(int, int), string>();

        using (StreamReader reader = new StreamReader(szerkhelye))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var parts = line.Split(';');
                if (parts.Length == 3)
                {
                    string szin = parts[0];
                    int x = int.Parse(parts[1]);
                    int y = int.Parse(parts[2]);
                    szinezettPoziciok[(x, y)] = szin;
                }
            }
        }

        Console.Clear();
        foreach (var poz in szinezettPoziciok)
        {
            ConsoleColor szin = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), poz.Value);
            Console.SetCursorPosition(poz.Key.Item1, poz.Key.Item2);
            Console.BackgroundColor = szin;
            Console.Write(" ");
        }
        Console.ResetColor();

        int kurzorX = 0;
        int kurzorY = 0;
        ConsoleColor aktSzin = ConsoleColor.White;
        ConsoleKeyInfo gomb_lenyom;


        while (true)
        {
            kiIras(kurzorX, kurzorY, aktSzin);
            gomb_lenyom = Console.ReadKey(true);

            if (gomb_lenyom.Key == ConsoleKey.Escape)
            {
                Mentes(szinezettPoziciok, szerkhelye);
                break;
            }

            KezeljeAGombNyom(gomb_lenyom, ref kurzorX, ref kurzorY, ref aktSzin, szinezettPoziciok);
        }
    }

    static void KezeljeAGombNyom(ConsoleKeyInfo gomb, ref int kurzorX, ref int kurzorY, ref ConsoleColor aktSzin, Dictionary<(int, int), string> szinezettPoziciok)
    {
        if (gomb.Key == ConsoleKey.LeftArrow)
        {
            kurzorX = Math.Max(0, kurzorX - 1);
        }
        else if (gomb.Key == ConsoleKey.RightArrow)
        {
            kurzorX = Math.Min(Console.WindowWidth - 1, kurzorX + 1);
        }
        else if (gomb.Key == ConsoleKey.UpArrow)
        {
            kurzorY = Math.Max(0, kurzorY - 1);
        }
        else if (gomb.Key == ConsoleKey.DownArrow)
        {
            kurzorY = Math.Min(Console.WindowHeight - 1, kurzorY + 1);
        }
        else if (gomb.Key == ConsoleKey.Spacebar)
        {
            rajzolas_a_kurzornal(kurzorX, kurzorY, aktSzin, szinezettPoziciok);
        }
        else
        {
            if (char.IsDigit(gomb.KeyChar))
            {
                SzinValtas(gomb.KeyChar, ref aktSzin);
            }
        }
    }

    static void rajzolas_a_kurzornal(int kurzorX, int kurzorY, ConsoleColor aktSzin, Dictionary<(int, int), string> szinezettPoziciok)
    {
        Console.SetCursorPosition(kurzorX, kurzorY);
        Console.BackgroundColor = aktSzin;
        Console.Write(" ");
        Console.ResetColor();

        szinezettPoziciok[(kurzorX, kurzorY)] = aktSzin.ToString();
    }

    static void SzinValtas(char number, ref ConsoleColor aktSzin)
    {
        int szinIndexe = int.Parse(number.ToString()) % 16;
        aktSzin = (ConsoleColor)szinIndexe;
    }

    static void kiIras(int kurzorX, int kurzorY, ConsoleColor aktSzin)
    {
        Console.SetCursorPosition(0, Console.WindowHeight - 1);
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($"Pos: ({kurzorX}, {kurzorY}) | Color: {aktSzin}   ");
        Console.ResetColor();
    }

    static void Mentes(Dictionary<(int, int), string> poziciok, string rajzNeve)
    {
        using (SQLiteConnection conn = new SQLiteConnection(connectionString))
        {
            conn.Open();
            string deleteQuery = "DELETE FROM Rajz WHERE RajzNeve = @rajzNeve";
            using (SQLiteCommand cmd = new SQLiteCommand(deleteQuery, conn))
            {
                cmd.Parameters.AddWithValue("@rajzNeve", rajzNeve);
                cmd.ExecuteNonQuery();
            }
            string insertQuery = "INSERT INTO Rajz (X, Y, Szin, RajzNeve) VALUES (@x, @y, @szin, @rajzNeve)";
            foreach (var poz in poziciok)
            {
                using (SQLiteCommand cmd = new SQLiteCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@x", poz.Key.Item1);
                    cmd.Parameters.AddWithValue("@y", poz.Key.Item2);
                    cmd.Parameters.AddWithValue("@szin", poz.Value);
                    cmd.Parameters.AddWithValue("@rajzNeve", rajzNeve);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        Console.WriteLine("Mentés sikeres az adatbázisba.");
    }
    static Dictionary<(int, int), string> Betoltes(string rajzNeve)
    {
        Dictionary<(int, int), string> szinezettPoziciok = new Dictionary<(int, int), string>();

        using (SQLiteConnection conn = new SQLiteConnection(connectionString))
        {
            conn.Open();
            string selectQuery = "SELECT X, Y, Szin FROM Rajz WHERE RajzNeve = @rajzNeve";
            using (SQLiteCommand cmd = new SQLiteCommand(selectQuery, conn))
            {
                cmd.Parameters.AddWithValue("@rajzNeve", rajzNeve);
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int x = reader.GetInt32(0);
                        int y = reader.GetInt32(1);
                        string szin = reader.GetString(2);
                        szinezettPoziciok[(x, y)] = szin;
                    }
                }
            }
        }

        return szinezettPoziciok;
    }

    static List<string> ListazRajzNeveket()
    {
        List<string> rajzNevek = new List<string>();

        using (SQLiteConnection conn = new SQLiteConnection(connectionString))
        {
            conn.Open();
            string selectQuery = "SELECT DISTINCT RajzNeve FROM Rajz";
            using (SQLiteCommand cmd = new SQLiteCommand(selectQuery, conn))
            {
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rajzNevek.Add(reader.GetString(0));
                    }
                }
            }
        }

        return rajzNevek;
    }

}
