using System;
using System.Collections.Generic;

namespace PokemonTextAdventure
{
    class Program
    {
        enum Location { PalletTown, Route1, ViridianCity }

        class Move
        {
            public string Name { get; set; }
            public int Power { get; set; }
            public Move(string name, int power) { Name = name; Power = power; }
        }

        class Pokemon
        {
            public string Name { get; set; }
            public int MaxHP { get; set; }
            public int CurrentHP { get; set; }
            public List<Move> Moves { get; set; }
            public Pokemon(string name, int hp, List<Move> moves) { Name = name; MaxHP = hp; CurrentHP = hp; Moves = moves; }
            public bool IsFainted() => CurrentHP <= 0;
        }

        static List<Pokemon> party = new List<Pokemon>();
        static List<Pokemon> storage = new List<Pokemon>();
        static Location currentLocation = Location.PalletTown;
        static string currentGoal = "Talk to Professor Oak and choose your first Pokémon.";
        static bool gameStarted = false;

        // Inventory
        static int pokeBalls = 5;
        static int money = 500;
        static int maxPartySize = 6;

        static void Main()
        {
            Console.Title = "Pokémon Text Adventure";
            ShowWelcome();

            while (true)
            {
                if (!gameStarted)
                {
                    Console.Write("\n> ");
                    string input = Console.ReadLine()?.Trim().ToLower();
                    if (string.IsNullOrEmpty(input)) continue;

                    if (HandleGlobalCommands(input)) continue;
                    HandleStartMenu(input);
                }
                else
                {
                    ShowLocationMenu();
                    Console.Write("\n> ");
                    string input = Console.ReadLine()?.Trim().ToLower();
                    if (string.IsNullOrEmpty(input)) continue;

                    if (HandleGlobalCommands(input)) continue;
                    HandleLocation(input);
                }
            }
        }

        // ---------- Global Commands ----------
        static bool HandleGlobalCommands(string input) => input switch
        {
            "hint" => ShowHint(),
            "goal" or "doel" => ShowGoal(),
            "party" or "pokémon" => ShowParty(),
            "quit" or "exit" => QuitGame(),
            _ => false
        };

        static bool ShowHint()
        {
            Console.WriteLine("\n=== HINT ===");
            if (party.Count == 0) Console.WriteLine("Professor Oak is waiting. Choose your starter!");
            else if (currentLocation == Location.Route1) Console.WriteLine("Wild Pokémon are on Route 1. Be ready!");
            else if (currentLocation == Location.ViridianCity) Console.WriteLine("Visit Pokémon Center or Poké Mart.");
            else Console.WriteLine("Explore your surroundings!");
            Console.WriteLine("============\n");
            return true;
        }

        static bool ShowGoal()
        {
            Console.WriteLine($"\n=== CURRENT GOAL ===\n{currentGoal}\n");
            return true;
        }

        static bool ShowParty()
        {
            Console.WriteLine("\n=== YOUR POKÉMON PARTY ===");
            if (party.Count == 0)
            {
                Console.WriteLine("You don’t have any Pokémon yet!");
            }
            else
            {
                for (int i = 0; i < party.Count; i++)
                {
                    var p = party[i];
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
                gameStarted = true;
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
            Console.WriteLine("Type 'help' for instructions.");
            Console.WriteLine("You can also type 'hint', 'goal', 'party', or 'quit' anytime.");
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
            TypeDialogue("Professor Oak", "\"Welcome to the world of Pokémon!\"");
            TypeDialogue("Professor Oak", "\"I'm Professor Oak!\"");
            TypeDialogue("Narrator", "You're in Professor Oak’s lab. Three Poké Balls sit on the table.");
            currentGoal = "Choose your first Pokémon.";
            ChooseStarter();
        }

        static void ChooseStarter()
        {
            var starters = new Dictionary<string, Pokemon>()
            {
                ["bulbasaur"] = new Pokemon("Bulbasaur", 45, new List<Move> { new Move("Tackle", 10), new Move("Growl", 0) }),
                ["charmander"] = new Pokemon("Charmander", 39, new List<Move> { new Move("Scratch", 10), new Move("Growl", 0) }),
                ["squirtle"] = new Pokemon("Squirtle", 44, new List<Move> { new Move("Tackle", 10), new Move("Tail Whip", 0) })
            };

            while (party.Count == 0)
            {
                Console.WriteLine("\nChoose your starter Pokémon:");
                Console.WriteLine("1. Bulbasaur\n2. Charmander\n3. Squirtle\n> ");
                string input = Console.ReadLine()?.Trim().ToLower();
                if (input == "1") input = "bulbasaur";
                else if (input == "2") input = "charmander";
                else if (input == "3") input = "squirtle";

                if (starters.ContainsKey(input))
                {
                    var chosen = starters[input];
                    party.Add(chosen);
                    Console.Clear();
                    TypeDialogue("Narrator", $"You pick up {chosen.Name}’s Poké Ball. A new friendship begins!");
                    TypeDialogue("Professor Oak", $"\"Excellent choice! Take care of {chosen.Name}!\"");
                    currentGoal = "Head north to Route 1.";
                    break;
                }
                else
                {
                    Console.WriteLine("Choose 1, 2, 3, or type the Pokémon's name.");
                }
            }
        }

        // ---------- Location Menu ----------
        static void ShowLocationMenu()
        {
            switch (currentLocation)
            {
                case Location.PalletTown:
                    Console.WriteLine("\nWhat do you want to do?");
                    Console.WriteLine("1. Explore Lab");
                    Console.WriteLine("2. Go north to Route 1");
                    break;
                case Location.Route1:
                    Console.WriteLine("\nWhat do you want to do?");
                    Console.WriteLine("1. Move forward / Encounter Pokémon");
                    Console.WriteLine("2. Go back to Pallet Town");
                    break;
                case Location.ViridianCity:
                    Console.WriteLine("\nWhere do you want to go?");
                    Console.WriteLine("1. Pokémon Center");
                    Console.WriteLine("2. Poké Mart");
                    Console.WriteLine("3. Go back to Route 1");
                    break;
            }
        }

        static void HandleLocation(string input)
        {
            Console.Clear();
            switch (currentLocation)
            {
                case Location.PalletTown:
                    if (input == "1" || input == "explore lab")
                        TypeDialogue("Narrator", "You look around the lab. Your starter Pokémon awaits.");
                    else if (input == "2" || input == "go north" || input == "route 1")
                        MoveTo(Location.Route1, "You leave Pallet Town and start walking north along Route 1.");
                    else Console.WriteLine("Type 1 to explore lab, 2 to go north.");
                    break;

                case Location.Route1:
                    if (input == "1" || input == "move forward" || input == "encounter") EncounterWildPokemon();
                    else if (input == "2" || input == "go back") MoveTo(Location.PalletTown, "You walk back south to Pallet Town.");
                    else Console.WriteLine("Type 1 to move forward, 2 to go back.");
                    break;

                case Location.ViridianCity:
                    if (input == "1" || input == "center" || input == "pokémon center") PokemonCenter();
                    else if (input == "2" || input == "mart" || input == "poké mart") PokeMart();
                    else if (input == "3" || input == "go back") MoveTo(Location.Route1, "You leave Viridian City and return to Route 1.");
                    else Console.WriteLine("Type 1 for Pokémon Center, 2 for Poké Mart, 3 to go back.");
                    break;
            }
        }

        static void MoveTo(Location loc, string message)
        {
            TypeDialogue("Narrator", message);
            currentLocation = loc;
            if (loc == Location.Route1) currentGoal = "Move forward and encounter Pokémon!";
            else if (loc == Location.ViridianCity) currentGoal = "Explore Viridian City and visit Pokémon Center or Poké Mart.";
        }

        // ---------- Pokémon Center ----------
        static void PokemonCenter()
        {
            Console.Clear();
            HealPokemon();
            Console.WriteLine("\nWhat do you want to do?");
            Console.WriteLine("1. Deposit Pokémon");
            Console.WriteLine("2. Withdraw Pokémon");
            Console.WriteLine("3. Leave Pokémon Center");
            Console.Write("> ");
            string input = Console.ReadLine()?.Trim().ToLower();

            if (input == "1") DepositPokemon();
            else if (input == "2") WithdrawPokemon();
            else if (input == "3") return;
            else Console.WriteLine("Invalid option.");
        }

        static void HealPokemon()
        {
            foreach (var p in party)
                p.CurrentHP = p.MaxHP;
            TypeDialogue("Nurse Joy", "\"Welcome! Your Pokémon are fighting fit!\"");
        }

        static void DepositPokemon()
        {
            if (party.Count <= 1)
            {
                Console.WriteLine("You must keep at least one Pokémon in your party.");
                return;
            }

            ShowParty();
            Console.WriteLine("Choose a Pokémon number to deposit:");
            string input = Console.ReadLine()?.Trim();
            if (int.TryParse(input, out int choice) && choice > 0 && choice <= party.Count)
            {
                var selected = party[choice - 1];
                storage.Add(selected);
                party.RemoveAt(choice - 1);
                Console.WriteLine($"{selected.Name} was deposited into storage!");
            }
            else Console.WriteLine("Invalid choice.");
        }

        static void WithdrawPokemon()
        {
            if (storage.Count == 0)
            {
                Console.WriteLine("No Pokémon in storage.");
                return;
            }

            Console.WriteLine("\n=== STORAGE BOX ===");
            for (int i = 0; i < storage.Count; i++)
                Console.WriteLine($"{i + 1}. {storage[i].Name} - HP: {storage[i].CurrentHP}/{storage[i].MaxHP}");
            Console.WriteLine("===================");
            Console.WriteLine("Choose a Pokémon number to withdraw:");
            string input = Console.ReadLine()?.Trim();
            if (int.TryParse(input, out int choice) && choice > 0 && choice <= storage.Count)
            {
                var selected = storage[choice - 1];
                party.Add(selected);
                storage.RemoveAt(choice - 1);
                Console.WriteLine($"{selected.Name} was withdrawn and added to your party!");
            }
            else Console.WriteLine("Invalid choice.");
        }

        // ---------- Poké Mart ----------
        static void PokeMart()
        {
            Console.Clear();
            TypeDialogue("Shopkeeper", "\"Welcome to the Poké Mart!\"");
            Console.WriteLine($"You have {money} Pokédollars and {pokeBalls} Poké Balls.");
            Console.WriteLine("\n1. Buy Poké Ball (200₽)");
            Console.WriteLine("2. Leave");
            Console.Write("> ");
            string input = Console.ReadLine()?.Trim().ToLower();

            if (input == "1" || input == "buy")
            {
                if (money >= 200)
                {
                    money -= 200;
                    pokeBalls++;
                    Console.WriteLine("You bought a Poké Ball!");
                    Console.WriteLine($"Now you have {pokeBalls} Poké Balls.");
                }
                else Console.WriteLine("Not enough money!");
            }
            else if (input == "2" || input == "leave")
            {
                Console.WriteLine("You leave the Poké Mart.");
            }
            else Console.WriteLine("Invalid option.");
        }

        // ---------- Battles ----------
        static void EncounterWildPokemon()
        {
            TypeDialogue("Narrator", "The tall grass rustles...");
            var wild = new Pokemon("Pidgey", 40, new List<Move> { new Move("Tackle", 8) });
            TypeDialogue("Narrator", $"A wild {wild.Name} appears!");
            Battle(wild);

            if (!party[0].IsFainted()) MoveTo(Location.ViridianCity, "After the encounter, you continue and soon see Viridian City ahead.");
            else { Console.WriteLine("Game Over! Your Pokémon fainted."); Environment.Exit(0); }
        }

        static void Battle(Pokemon wild)
        {
            int activeIndex = 0; // which party Pokémon is active
            var playerPokemon = party[activeIndex];

            Console.WriteLine("=== BATTLE START ===");
            while (!wild.IsFainted() && party.Exists(p => !p.IsFainted()))
            {
                Console.Clear();

                playerPokemon = party[activeIndex];
                Console.WriteLine($"\n{playerPokemon.Name} HP: {playerPokemon.CurrentHP}/{playerPokemon.MaxHP}");
                Console.WriteLine($"{wild.Name} HP: {wild.CurrentHP}/{wild.MaxHP}");
                Console.WriteLine($"Poké Balls: {pokeBalls}");

                Console.WriteLine("\nWhat do you want to do?");
                Console.WriteLine("1. Fight");
                Console.WriteLine("2. Throw Poké Ball");
                Console.WriteLine("3. Pokémon (switch)");
                Console.WriteLine("4. Run");
                Console.Write("> ");
                string action = Console.ReadLine()?.Trim().ToLower();

                if (action == "1" || action == "fight")
                {
                    Console.WriteLine("\nChoose a move:");
                    for (int i = 0; i < playerPokemon.Moves.Count; i++)
                        Console.WriteLine($"{i + 1}. {playerPokemon.Moves[i].Name}");
                    Console.Write("> ");
                    string input = Console.ReadLine()?.Trim().ToLower();

                    Move move = ParseMove(input, playerPokemon);
                    if (move == null) { Console.WriteLine("Invalid move."); continue; }

                    if (move.Power > 0) TypeDialogue("Narrator", $"{playerPokemon.Name} uses {move.Name}!");
                    else TypeDialogue("Narrator", $"{playerPokemon.Name} used {move.Name}, but it had no effect!");

                    wild.CurrentHP -= move.Power;
                    if (wild.CurrentHP < 0) wild.CurrentHP = 0;

                    if (wild.IsFainted()) { TypeDialogue("Narrator", $"The wild {wild.Name} fainted!"); break; }
                }
                else if (action == "2" || action == "poké ball" || action == "throw")
                {
                    if (pokeBalls <= 0)
                    {
                        Console.WriteLine("You have no Poké Balls left!");
                        continue;
                    }

                    pokeBalls--;
                    Console.WriteLine($"You throw a Poké Ball! ({pokeBalls} left)");

                    if (TryCatchPokemon(wild))
                    {
                        Console.WriteLine($"Gotcha! {wild.Name} was caught!");
                        if (party.Count < maxPartySize)
                        {
                            party.Add(wild);
                            Console.WriteLine($"{wild.Name} was added to your party!");
                        }
                        else
                        {
                            storage.Add(wild);
                            Console.WriteLine($"{wild.Name} was sent to storage!");
                        }
                        return;
                    }
                    else
                    {
                        Console.WriteLine($"{wild.Name} broke free!");
                    }
                }
                else if (action == "3" || action == "pokémon" || action == "switch")
                {
                    Console.WriteLine("\n=== YOUR PARTY ===");
                    for (int i = 0; i < party.Count; i++)
                    {
                        var p = party[i];
                        Console.WriteLine($"{i + 1}. {p.Name} - HP: {p.CurrentHP}/{p.MaxHP}" + (i == activeIndex ? " (active)" : ""));
                    }
                    Console.WriteLine("Choose a Pokémon to switch to:");
                    string input = Console.ReadLine()?.Trim();
                    if (int.TryParse(input, out int choice) && choice > 0 && choice <= party.Count)
                    {
                        if (choice - 1 == activeIndex) Console.WriteLine($"{party[choice - 1].Name} is already active!");
                        else if (party[choice - 1].IsFainted()) Console.WriteLine("You cannot switch to a fainted Pokémon!");
                        else
                        {
                            activeIndex = choice - 1;
                            Console.WriteLine($"You switched to {party[activeIndex].Name}!");
                        }
                    }
                    else Console.WriteLine("Invalid choice.");
                }
                else if (action == "4" || action == "run" || action == "escape")
                {
                    Random rng = new Random();
                    if (rng.NextDouble() < 0.75) // 75% chance to escape
                    {
                        Console.WriteLine("You successfully ran away!");
                        return; // end battle
                    }
                    else
                    {
                        Console.WriteLine("You couldn’t escape!");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid option. Choose 1, 2, 3, or 4.");
                    continue;
                }

                // Enemy's turn (if wild still alive)
                if (!wild.IsFainted())
                {
                    var enemyMove = wild.Moves[0];
                    TypeDialogue("Narrator", $"The wild {wild.Name} uses {enemyMove.Name}!");
                    playerPokemon.CurrentHP -= enemyMove.Power;
                    if (playerPokemon.CurrentHP < 0) playerPokemon.CurrentHP = 0;

                    if (playerPokemon.IsFainted())
                    {
                        TypeDialogue("Narrator", $"{playerPokemon.Name} fainted!");

                        // Force switch if you still have Pokémon left
                        if (party.Exists(p => !p.IsFainted()))
                        {
                            Console.WriteLine("Choose a new Pokémon:");
                            while (true)
                            {
                                for (int i = 0; i < party.Count; i++)
                                {
                                    var p = party[i];
                                    Console.WriteLine($"{i + 1}. {p.Name} - HP: {p.CurrentHP}/{p.MaxHP}");
                                }
                                string input = Console.ReadLine()?.Trim();
                                if (int.TryParse(input, out int choice) && choice > 0 && choice <= party.Count && !party[choice - 1].IsFainted())
                                {
                                    activeIndex = choice - 1;
                                    Console.WriteLine($"You sent out {party[activeIndex].Name}!");
                                    break;
                                }
                                else Console.WriteLine("Invalid choice. Pick a healthy Pokémon.");
                            }
                        }
                    }
                }
            }
            Console.WriteLine("=== BATTLE END ===");
        }

        static bool TryCatchPokemon(Pokemon wild)
        {
            double hpFactor = 1.0 - ((double)wild.CurrentHP / wild.MaxHP);
            double baseCatchRate = 0.25;
            double catchChance = baseCatchRate + hpFactor * 0.5;

            Random rng = new Random();
            return rng.NextDouble() < catchChance;
        }

        static Move ParseMove(string input, Pokemon p)
        {
            if (int.TryParse(input, out int idx) && idx > 0 && idx <= p.Moves.Count)
                return p.Moves[idx - 1];
            foreach (var m in p.Moves)
                if (m.Name.ToLower() == input) return m;
            return null;
        }

        // ---------- Dialogue ----------
        static void TypeDialogue(string speaker, string text, int speed = 20, int pause = 800)
        {
            Console.Write(speaker + ": ");
            foreach (char c in text)
            {
                Console.Write(c);
                System.Threading.Thread.Sleep(speed);
            }
            Console.WriteLine();

            // Add pause after the whole line
            System.Threading.Thread.Sleep(pause);
        }
    }
}
