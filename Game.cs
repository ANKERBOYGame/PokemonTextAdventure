using PokemonTextAdventure.Models;
using PokemonTextAdventure.Systems;
using System;
using System.Collections.Generic;

namespace PokemonTextAdventure
{
    public static class Game
    {
        public enum Location { PalletTown, Route1, ViridianCity, Route2 }

        // Static gekozen zodat er maar één spelstatus is, makkelijk bereikbaar vanuit overal.
        public static List<Pokemon> Party { get; private set; } = new List<Pokemon>();
        public static List<Pokemon> Storage { get; private set; } = new List<Pokemon>();

        // Locatie en doel centraal in Game opgeslagen i.p.v. per locatie-object,
        // omdat er altijd maar één actieve locatie/goal tegelijk is.
        public static Location CurrentLocation { get; private set; } = Location.PalletTown;
        public static string CurrentGoal { get; private set; } = "Talk to Professor Oak and choose your first Pokémon.";
        public static bool GameStarted { get; private set; } = false;

        // Inventory simpel gehouden als ints voor toegankelijkheid; geen complex systeem nodig.
        public static int PokeBalls { get; private set; } = 5;
        public static int Money { get; private set; } = 500;
        public const int MaxPartySize = 6;

        public static void Start()
        {
            ShowWelcome();

            // Eindeloze lus gebruikt zodat de game blijft draaien tot de speler zelf afsluit.
            while (true)
            {
                if (!GameStarted)
                {
                    Console.Write("\n> ");
                    string input = Console.ReadLine()?.Trim().ToLower();
                    if (string.IsNullOrEmpty(input)) continue;

                    // Eerst globale commando’s checken, zodat deze overal in het spel werken.
                    if (HandleGlobalCommands(input)) continue;
                    HandleStartMenu(input);
                }
                else
                {
                    // Locaties krijgen hun eigen verantwoordelijkheid voor logica,
                    // zodat Game niet te groot/chaotisch wordt.
                    Locations.ShowLocationMenu(CurrentLocation);
                    Console.Write("\n> ");
                    string input = Console.ReadLine()?.Trim().ToLower();
                    if (string.IsNullOrEmpty(input)) continue;

                    if (HandleGlobalCommands(input)) continue;
                    Locations.HandleLocation(input);
                }
            }
        }


        // ---------- Global Commands ----------
        static bool HandleGlobalCommands(string input) => input switch
        {
            "hint" => Locations.ShowHint(),
            "goal" or "doel" => ShowGoal(),
            "party" or "pokémon" => ShowParty(),
            "save" => SaveGame(),
            "load" => LoadGame(),
            "quit" or "exit" => QuitGame(),
            _ => false
        };

        static bool SaveGame() { SaveSystem.Save(); return true; }
        static bool LoadGame() { SaveSystem.Load(); return true; }

        static bool ShowGoal()
        {
            Console.WriteLine($"\n=== CURRENT GOAL ===\n{CurrentGoal}\n");
            return true;
        }

        static bool ShowParty()
        {
            Console.WriteLine("\n=== YOUR POKÉMON PARTY ===");
            if (Party.Count == 0)
            {
                Console.WriteLine("You don’t have any Pokémon yet!");
            }
            else
            {
                for (int i = 0; i < Party.Count; i++)
                {
                    var p = Party[i];
                    Console.WriteLine($"{i + 1}. {p.Name} - HP: {p.CurrentHP}/{p.MaxHP}");
                    Console.WriteLine("   Moves: " + string.Join(", ", p.Moves.ConvertAll(m => m.Name)));
                }
            }
            Console.WriteLine("==========================\n");
            return true;
        }

        static bool QuitGame()
        {
            Console.WriteLine("Goodbye, Trainer!");
            Environment.Exit(0);
            return true;
        }

        // ---------- Start Menu ----------
        static void HandleStartMenu(string input)
        {
            if (input == "start" || input == "1")
            {
                StartGame();
                GameStarted = true;
            }
            else if (input == "help" || input == "instructions" || input == "2")
            {
                ShowInstructions();
            }
            else
            {
                Console.WriteLine("Invalid input. Type 'start' or 'help'.");
            }
        }

        static void ShowWelcome()
        {
            Console.Clear();
            Console.WriteLine("===============================");
            Console.WriteLine("     POKÉMON TEXT ADVENTURE    ");
            Console.WriteLine("===============================");
            Console.WriteLine("Welcome, Trainer!");
            Console.WriteLine("Type 'start' to begin your journey.");
            Console.WriteLine("Type 'load' to load your last saved game.");
            Console.WriteLine("Type 'help' for instructions.");
            Console.WriteLine("You can also type 'hint', 'goal', 'party', 'save' or 'quit' anytime.");
        }

        static void ShowInstructions()
        {
            Console.Clear();
            Console.WriteLine("\n=== INSTRUCTIONS ===");
            Console.WriteLine("• Type numbers or names for choices.");
            Console.WriteLine("• Battles use HP and moves.");
            Console.WriteLine("• You can catch Pokémon with Poké Balls.");
            Console.WriteLine("• Buy more Poké Balls at the Poké Mart.");
            Console.WriteLine("• Global commands: hint, goal, party, quit");
            Console.WriteLine("====================\n");
        }

        static void StartGame()
        {
            Console.Clear();
            Dialogue.TypeDialogue("Professor Oak", "\"Welcome to the world of Pokémon!\"");
            Dialogue.TypeDialogue("Professor Oak", "\"I'm Professor Oak!\"");
            Dialogue.TypeDialogue("Narrator", "You're in Professor Oak’s lab. Three Poké Balls sit on the table.");
            CurrentGoal = "Choose your first Pokémon.";
            Locations.ChooseStarter();
        }

        public static void UpdateGoal(string goal) => CurrentGoal = goal;
        public static void UpdateLocation(Location loc) => CurrentLocation = loc;
        public static void AddPokemon(Pokemon p) => Party.Add(p);
        public static void AddToStorage(Pokemon p) => Storage.Add(p);
        public static void RemovePokemon(Pokemon p) => Party.Remove(p);
        public static void AddPokeball() => PokeBalls++;
        public static bool UsePokeball()
        {
            if (PokeBalls <= 0) return false;
            PokeBalls--;
            return true;
        }
        public static bool SpendMoney(int amount)
        {
            if (Money < amount) return false;
            Money -= amount;
            return true;
        }

        // ----------------- Save/Load setters -----------------
        public static void SetMoney(int amount) => Money = amount;
        public static void SetPokeBalls(int amount) => PokeBalls = amount;
        public static void SetGameStarted(bool started) => GameStarted = started;
    }
}
