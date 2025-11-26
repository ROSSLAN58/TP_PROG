using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Formats.Tar;

namespace ConsoleApp1
{
    internal class Program
    {

        const int Fer = 0;
        const int Silex = 1;
        const int Bois = 2;
        const int Argile = 3;
        const int Herbe = 4;
        const int Sable = 5;
        const int Feu = 6;
        const int Hache = 7;
        const int Vitre = 8;
        const int Planche = 9;
        const int Brique = 10;
        const int Isolant = 11;
        const int Maison = 12;


        public static void AfficherMap( List<string> map, int joueurX, int joueurY, int radius)
        {
            for (int i = 0; i < (radius+10)*2; i++)
            {
                Console.Write("*");
            }

            Console.WriteLine("**");
            for (int i = joueurY - radius; i < joueurY + radius; i++)
            {
                Console.Write("*");
                for (int j = joueurX - (radius+10); j < joueurX + (radius + 10); j++)
                {
                    
                    if (i < 0 || j < 0)
                    {
                        Console.Write(" ");
                    }
                    else
                    {
                        if(i == joueurY && j == joueurX)
                            Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(map[i][j]);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                Console.WriteLine("*");
            }

            for (int i = 0; i < (radius + 10) * 2; i++)
            {
                Console.Write("*");
            }

            Console.WriteLine("**");


        }


        public static void CreeMap(ref List<string> map)
        {
            

            map.Add("X               ");
            map.Add("                ");
            map.Add("                ");
            map.Add("                ");
            map.Add("                ");
        }


        public static void AfficherCredits(ref string partie)
        {
            Console.WriteLine("****************************************");
            Console.WriteLine("*               MonCraft               *");
            Console.WriteLine("****************************************");
            Console.WriteLine("*Tout drois reserve.                   *");
            Console.WriteLine("****************************************");
            Console.WriteLine("*Rosslan Kabisov, Dilane Giresse Pokam *");
            Console.WriteLine("****************************************");
            Console.WriteLine("");
            Console.WriteLine("*Appuyer une touche pour sortire...");
            _ = Console.ReadKey();
            partie = "menu";
        }


        public static void AfficherLobbyPrincipale(ref string partie, ref int[] inventair, ref List<string> map, ref int actionRestant, ref int posX, ref int posY, ref int posRX, ref int posRY)
        {

            while (true) {

                Console.WriteLine("****************************");
                Console.WriteLine("* 1 - Jouer                *");
                Console.WriteLine("****************************");
                Console.WriteLine("* 2 - Charger une partie   *");
                Console.WriteLine("****************************");
                Console.WriteLine("* 3 - Sauvegarde la partie *");
                Console.WriteLine("****************************");
                Console.WriteLine("* 4 - Crédits              *");
                Console.WriteLine("****************************");

                string action = Console.ReadKey().KeyChar.ToString();
                if (action == "1")
                {
                    Console.Clear();
                    for (int i = 0; i < 3; i++)
                    {
                        Console.Write("Loading");
                        for (int j = 0; j < 3; j++)
                        {
                            Thread.Sleep(200);
                            Console.Write(".");
                        }
                        Console.Clear();
                    }

                    partie = "en jeux";
                    break;
                }
                else if (action == "2")
                {
                    

                    Console.Clear();
                    if (File.Exists("data.txt"))
                    {
                        StreamReader sr = new StreamReader("data.txt");
                        inventair = System.Text.Json.JsonSerializer.Deserialize<int[]>(sr.ReadLine());
                        map = System.Text.Json.JsonSerializer.Deserialize<List<string>>(sr.ReadLine());
                        actionRestant = System.Text.Json.JsonSerializer.Deserialize<int>(sr.ReadLine());
                        posX = System.Text.Json.JsonSerializer.Deserialize<int>(sr.ReadLine());
                        posY = System.Text.Json.JsonSerializer.Deserialize<int>(sr.ReadLine());
                        posRX = System.Text.Json.JsonSerializer.Deserialize<int>(sr.ReadLine());
                        posRY = System.Text.Json.JsonSerializer.Deserialize<int>(sr.ReadLine());
                        sr.Close();
                    }
                   

                    partie = "en jeux";

                }
                else if (action == "3")
                {

                    

                    StreamWriter sw = new StreamWriter("data.txt");

                    string inventaireJSON = System.Text.Json.JsonSerializer.Serialize(inventair);
                    sw.WriteLine(inventaireJSON);

                    string mapJSON = System.Text.Json.JsonSerializer.Serialize(map);
                    sw.WriteLine(mapJSON);

                    string actionRestantJSON = System.Text.Json.JsonSerializer.Serialize(actionRestant);
                    sw.WriteLine(actionRestantJSON);

                    string posXJSON = System.Text.Json.JsonSerializer.Serialize(posX);
                    sw.WriteLine(posXJSON);

                    string posYJSON = System.Text.Json.JsonSerializer.Serialize(posY);
                    sw.WriteLine(posYJSON);

                    string posRXJSON = System.Text.Json.JsonSerializer.Serialize(posRX);
                    sw.WriteLine(posRXJSON);

                    string posRYJSON = System.Text.Json.JsonSerializer.Serialize(posRY);
                    sw.WriteLine(posRYJSON);


                    sw.Close();

                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Saved");
                    Console.ForegroundColor = ConsoleColor.White;

                }
                else if (action == "4")
                {
                    partie = "credits";
                    break;
                }
                else
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Option invalide.");
                    Console.ForegroundColor = ConsoleColor.White;

                }

            }

        }

        static void Main(string[] args)
        {

            string partie = "menu";

            bool maisonConstruite = false;

            //map
            List<string> map = new List<string>();
            CreeMap(ref map);
            char[] biomsSymboles = { '/', '≈', '#', '~', '.', '*' };
            string[] biomsNoms = { "Montagne", "Rivière", "Forêt", "Marais", "Prairie", "Désert" };

            //joueur
            const int Radius = 5;
            int joueurX = 0;
            int joueurY = 0;
            int joueurXReel = 0;
            int joueurYReel = 0;
            string action;
            int actionsReastant = 500;
            string biom = "";
            //inventaire
            int[] inventaire = new int[13];

            //game loop
            while (!maisonConstruite && actionsReastant > 0)
            {

                if (partie == "en jeux")
                {

                    while (true)
                    {

                        AfficherMap(map, joueurX, joueurY, Radius);
                        Console.WriteLine();
                        Console.WriteLine($"X : {joueurXReel}, Y : {joueurYReel}");
                        Console.WriteLine($"Actions restant : {actionsReastant}");
                        Console.WriteLine($"Biom : {biom}");
                        Console.WriteLine("Avancer: w a s d, Menu: esc, Collecter: c, Inventaire: i, Fabrication: f");

                        action = Console.ReadKey().KeyChar.ToString();

                        if (action == "w")
                        {
                            AvancerNord(ref map, ref joueurYReel, ref joueurY, ref joueurX, Radius);
                            break;
                        }
                        else if (action == "s")
                        {
                            AvancerSud(ref map, ref joueurYReel, ref joueurY, ref joueurX, Radius);
                            break;
                        }
                        else if (action == "a")
                        {
                            AvancerEst(ref map, ref joueurXReel, ref joueurX, ref joueurY, Radius);
                            break;
                        }
                        else if (action == "d")
                        {
                            AvancerOuest(ref map, ref joueurXReel, ref joueurX, ref joueurY, Radius);
                            break;
                        }
                        else if (action == "\u001B")
                        {
                            partie = "menu";
                            break;
                        }
                        else if (action == "c")
                        {
                            if (Collecter(ref inventaire, joueurX, joueurY, biom))
                            {
                                break;
                            }

                            Console.Clear();
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Biom pas collectable.");
                            Console.ForegroundColor = ConsoleColor.White;

                        }
                        else if (action == "i")
                        {
                            partie = "inv";
                            break;
                        }
                        else if (action == "f")
                        {
                            if (joueurXReel == 0 && joueurYReel == 0)
                            {
                                partie = "fabrication";
                                break;
                            }
                            
                                Console.Clear();
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("La fabrication est assecible aux coordones [0,0]");
                                Console.ForegroundColor = ConsoleColor.White;
                            
                        }
                        else
                        {
                            Console.Clear();
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Option invalide.");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                        GenererBiom(ref map, ref biom, biomsSymboles, biomsNoms, joueurX, joueurY);
                    actionsReastant--;

                }
                else if (partie == "menu")
                { 
                    AfficherLobbyPrincipale(ref partie, ref inventaire, ref map, ref actionsReastant, ref  joueurX, ref  joueurY, ref  joueurXReel, ref  joueurYReel);
                }
                else if (partie == "credits")
                {
                    AfficherCredits(ref partie);
                }
                else if (partie == "inv")
                {
                    AfficherInventaire(inventaire, ref partie);
                }
                else if (partie == "fabrication")
                {
                    AfficherFabrication(joueurXReel, joueurYReel, ref inventaire, ref partie, ref maisonConstruite);
                }

                    Console.Clear();
            }

            if (actionsReastant <= 0)
            {
                Console.WriteLine("Tu est mort.");
            }
            else
            {
                Console.WriteLine("Bravo!");
            }
      
        }


        public static void AfficherInventaire(int[] inventaire, ref string partie)
        { 
        Console.Clear();
            Console.WriteLine($"Fer :     {inventaire[Fer]}");
            Console.WriteLine($"Silex :   {inventaire[1]}");
            Console.WriteLine($"Bois :    {inventaire[2]}");
            Console.WriteLine($"Argile :  {inventaire[3]}");
            Console.WriteLine($"Herbe :   {inventaire[4]}");
            Console.WriteLine($"Sable :   {inventaire[5]}");
            Console.WriteLine($"Feu :     {inventaire[6]}");
            Console.WriteLine($"Hache :   {inventaire[7]}");
            Console.WriteLine($"Vitre :   {inventaire[8]}");
            Console.WriteLine($"Planche : {inventaire[9]}");
            Console.WriteLine($"Brique :  {inventaire[10]}");
            Console.WriteLine($"Isolant : {inventaire[11]}");
            Console.WriteLine($"Maison :  {inventaire[12]}");

            Console.WriteLine(" ");
            Console.WriteLine("Appuyer une touche pour sortire...");
            _ = Console.ReadKey();
            partie = "en jeux";
        
        }

        public static void AfficherFabrication(int joueurReelX, int joueurReelY, ref int[] inventaire, ref string partie, ref bool maison)
        {

            bool sortire = false;

                while (!sortire)
                {
                while (true)
                {

                    Console.WriteLine($"Fer :     {inventaire[Fer]}");
                    Console.WriteLine($"Silex :   {inventaire[1]}");
                    Console.WriteLine($"Bois :    {inventaire[2]}");
                    Console.WriteLine($"Argile :  {inventaire[3]}");
                    Console.WriteLine($"Herbe :   {inventaire[4]}");
                    Console.WriteLine($"Sable :   {inventaire[5]}");
                    Console.WriteLine($"Feu :     {inventaire[6]}");
                    Console.WriteLine($"Hache :   {inventaire[7]}");
                    Console.WriteLine($"Vitre :   {inventaire[8]}");
                    Console.WriteLine($"Planche : {inventaire[9]}");
                    Console.WriteLine($"Brique :  {inventaire[10]}");
                    Console.WriteLine($"Isolant : {inventaire[11]}");
                    Console.WriteLine($"Maison :  {inventaire[12]}");

                    Console.WriteLine(" ");

                    Console.WriteLine("1 - Feu : 1 bois, 1 silex");
                    Console.WriteLine("2 - Hache : 1 bois, 1 fer");
                    Console.WriteLine("3 - Vitre : 1 sable, 1 feu");
                    Console.WriteLine("4 - Planche : 1 bois, 1 hache");
                    Console.WriteLine("5 - Brique : 1 argile, 1 feu");
                    Console.WriteLine("6 - Isolant : 3 herbe");
                    Console.WriteLine("7 - Maison : 4 planche, 4 brique, 4 isolant, 2 vitre");
                    Console.WriteLine("8 - Sortire");

                    string action = Console.ReadKey().KeyChar.ToString();


                    if (action == "8")
                    {
                        sortire = true;
                        break;
                    }
                    else if (action == "7")
                    {
                        if (inventaire[Planche] >= 4 && inventaire[Brique] >= 4 && inventaire[Isolant] >= 4 && inventaire[Vitre] >= 2)
                        {
                            inventaire[Planche] = inventaire[Planche] - 4;
                            inventaire[Brique] = inventaire[Brique] - 4;
                            inventaire[Isolant] = inventaire[Isolant] - 4;
                            inventaire[Vitre] = inventaire[Vitre] - 2;
                            inventaire[Maison]++;
                            maison = true;
                        }
                        break;
                    }
                    else if (action == "6")
                    {
                        if (inventaire[Herbe] >= 3)
                        {
                            inventaire[Herbe] = inventaire[Herbe] - 3;
                            inventaire[Isolant]++;
                        }
                        break;
                    }
                    else if (action == "5")
                    {
                        if (inventaire[Argile] >= 1 && inventaire[Feu] >= 1)
                        {
                            inventaire[Argile] = inventaire[Argile] - 1;
                            inventaire[Feu] = inventaire[Feu] - 1;
                            inventaire[Brique]++;
                        }
                        break;
                    }

                    else if (action == "4")
                    {
                        if (inventaire[Bois] >= 1 && inventaire[Hache] >= 1)
                        {
                            inventaire[Bois] = inventaire[Bois] - 1;
                            inventaire[Hache] = inventaire[Hache] - 1;
                            inventaire[Planche]++;
                        }
                        break;
                    }
                    else if (action == "3")
                    {
                        if (inventaire[Sable] >= 1 && inventaire[Feu] >= 1)
                        {
                            inventaire[Sable] = inventaire[Sable] - 1;
                            inventaire[Feu] = inventaire[Feu] - 1;
                            inventaire[Vitre]++;
                        }
                        break;
                    }
                    else if (action == "2")
                    {
                        if (inventaire[Bois] >= 1 && inventaire[Fer] >= 1)
                        {
                            inventaire[Bois] = inventaire[Bois] - 1;
                            inventaire[Fer] = inventaire[Fer] - 1;
                            inventaire[Hache]++;
                        }
                        break;
                    }
                    else if (action == "1")
                    {
                        if (inventaire[Bois] >= 1 && inventaire[Silex] >= 1)
                        {
                            inventaire[Bois] = inventaire[Bois] - 1;
                            inventaire[Silex] = inventaire[Silex] - 1;
                            inventaire[Feu]++;
                        }
                        break;
                    }
                    else
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Option invalide.");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }

                    Console.Clear();


                }

                partie = "en jeux";

        }



        public static bool Collecter(ref int[] inventaire, int joueurX, int joueurY, string biom)
        {

            switch (biom)
            { 
            
            case "Montagne":
                    inventaire[Fer]++;
                break;

            case "Rivière":
                inventaire[Silex]++;
                break;

            case "Forêt":
                inventaire[Bois]++;
                break;

            case "Marais":
                inventaire[Argile]++;
                break;

            case "Prairie":
                inventaire[Herbe]++;
                break;

            case "Désert":
                inventaire[Sable]++;
                break;


            default:
                return false;
                break;


            }
            return true;

        }


        public static void GenererBiom(ref List<string> map, ref string biom, char[] biomsSymboles, string[] biomsNoms, int joueurX, int joueurY)
        {

            if (map[joueurY][joueurX] == ' ')
            {

                Random random = new Random();
                int biomGenere = random.Next(0, 6);
                biom = biomsNoms[biomGenere];

                string nouvelleline = "";
                for (int i = 0; i < map[0].Length; i++)
                {

                    if (i == joueurX)
                    {
                        nouvelleline = nouvelleline + biomsSymboles[biomGenere];
                    }
                    else
                    {
                        nouvelleline = nouvelleline + map[joueurY][i];

                    }

                }
                map[joueurY] = nouvelleline;
            }
            else
            {
                switch (map[joueurY][joueurX])
                {
                    case '#':
                        biom = "Forêt";
                        break;
                    case '/':
                        biom = "Montagne";
                        break;
                    case '≈':
                        biom = "Rivière";
                        break;
                    case '~':
                        biom = "Marais";
                        break;
                    case '.':
                        biom = "Prairie";
                        break;
                    case '*':
                        biom = "Désert";
                        break;
                    default:
                        biom = "Maison";
                        break;
                }
            }
        }
        public static void AvancerNord(ref List<string> map, ref int joueurYReel, ref int joueurY, ref int joueurX, int radius)
        {
            joueurYReel--;
            if (joueurY - radius <= 0)
            {
                string line = "";
                for (int i = 0; i < map[0].Length; i++)
                {
                    line = line + " ";
                }
                List<string> nouvelleMap = new List<string>();
                nouvelleMap.Add(line);
                for (int j = 0; j < map.Count; j++)
                {
                    nouvelleMap.Add(map[j]);
                }
                map = nouvelleMap;   
            }
            else
            { 
                joueurY--;
            }
        }
        public static void AvancerSud(ref List<string> map, ref int joueurYReel, ref int joueurY, ref int joueurX, int radius)
        {
            joueurYReel++;
            joueurY++;
            if (joueurY + radius > map.Count)
            {
                string line = "";
                for (int i = 0; i < map[0].Length; i++)
                {
                    line = line + " ";
                }
                map.Add(line);
            }
        }
        public static void AvancerEst(ref List<string> map, ref int joueurXReel, ref int joueurX, ref int joueurY, int radius)
        {
            joueurXReel--;
            if (joueurX - (radius + 10) <= 0)
            {
                for (int i = 0; i < map.Count; i++)
                {
                    map[i] = " " + map[i];
                }
            }
            else
            {
                joueurX--;
            }
        }
        public static void AvancerOuest(ref List<string> map, ref int joueurXReel, ref int joueurX, ref int joueurY, int radius)
        {
            joueurXReel++;
            joueurX++;
            if (joueurX + (radius + 10) > map[0].Length)
            {
                for (int i = 0; i < map.Count; i++)
                {
                    map[i] = map[i] + " ";
                }      
            }
        }
    }
}
