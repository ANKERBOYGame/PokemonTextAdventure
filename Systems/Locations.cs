using System;
using System.Collections.Generic;
using PokemonTextAdventure.Models;

namespace PokemonTextAdventure.Systems
{
    public static class Locations
    {
        public static bool ShowHint()
        {
            Console.WriteLine("\n=== HINT ===");
            if (Game.Party.Count == 0) Console.WriteLine("Professor Oak is waiting. Choose your starter!");
            else if (Game.CurrentLocation == Game.Location.Route1) Console.WriteLine("Wild Pokémon are on Route 1. Be ready!");
            else if (Game.CurrentLocation == Game.Location.ViridianCity) Console.WriteLine("Visit Pokémon Center or Poké Mart.");
            else Console.WriteLine("Explore your surroundings!");
            Console.WriteLine("============\n");
            return true;
        }

        public static void ShowLocationMenu(Game.Location loc)
        {
            switch (loc)
            {
                case Game.Location.PalletTown:
                    Console.WriteLine("\nWhat do you want to do?");
                    Console.WriteLine("1. Explore Lab");
                    Console.WriteLine("2. Go north to Route 1");
                    break;
                case Game.Location.Route1:
                    Console.WriteLine("\nWhat do you want to do?");
                    Console.WriteLine("1. Move forward / Encounter Pokémon");
                    Console.WriteLine("2. Go back to Pallet Town");
                    break;
                case Game.Location.ViridianCity:
                    Console.WriteLine("\nWhere do you want to go?");
                    Console.WriteLine("1. Pokémon Center");
                    Console.WriteLine("2. Poké Mart");
                    Console.WriteLine("3. Go back to Route 1");
                    break;
            }
        }

        public static void HandleLocation(string input)
        {
            Console.Clear();
            switch (Game.CurrentLocation)
            {
                case Game.Location.PalletTown:
                    if (input == "1" || input == "explore lab")
                        Dialogue.TypeDialogue("Narrator", "You look around the lab. Your starter Pokémon awaits.");
                    else if (input == "2" || input == "go north" || input == "route 1")
                        MoveTo(Game.Location.Route1, "You leave Pallet Town and start walking north along Route 1.");
                    else Console.WriteLine("Type 1 to explore lab, 2 to go north.");
                    break;

                case Game.Location.Route1:
                    if (input == "1" || input == "move forward" || input == "encounter") EncounterWildPokemon();
                    else if (input == "2" || input == "go back") MoveTo(Game.Location.PalletTown, "You walk back south to Pallet Town.");
                    else Console.WriteLine("Type 1 to move forward, 2 to go back.");
                    break;

                case Game.Location.ViridianCity:
                    if (input == "1" || input == "center" || input == "pokémon center") PokemonCenter();
                    else if (input == "2" || input == "mart" || input == "poké mart") PokeMart();
                    else if (input == "3" || input == "go back") MoveTo(Game.Location.Route1, "You leave Viridian City and return to Route 1.");
                    else Console.WriteLine("Type 1 for Pokémon Center, 2 for Poké Mart, 3 to go back.");
                    break;
            }
        }

        public static void ChooseStarter()
        {
            var starters = new Dictionary<string, Pokemon>()
            {
                ["bulbasaur"] = new Pokemon("Bulbasaur", 45, new List<Move> { new Move("Tackle", 10), new Move("Growl", 0) }),
                ["charmander"] = new Pokemon("Charmander", 39, new List<Move> { new Move("Scratch", 10), new Move("Growl", 0) }),
                ["squirtle"] = new Pokemon("Squirtle", 44, new List<Move> { new Move("Tackle", 10), new Move("Tail Whip", 0) })
            };

            while (Game.Party.Count == 0)
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
                    Game.AddPokemon(chosen);
                    Console.Clear();
                    Dialogue.TypeDialogue("Narrator", $"You pick up {chosen.Name}’s Poké Ball. A new friendship begins!");
                    Dialogue.TypeDialogue("Professor Oak", $"\"Excellent choice! Take care of {chosen.Name}!\"");
                    Game.UpdateGoal("Head north to Route 1.");
                    break;
                }
                else
                {
                    Console.WriteLine("Choose 1, 2, 3, or type the Pokémon's name.");
                }
            }
        }

        static void MoveTo(Game.Location loc, string message)
        {
            Dialogue.TypeDialogue("Narrator", message);
            Game.UpdateLocation(loc);
            if (loc == Game.Location.Route1) Game.UpdateGoal("Move forward and encounter Pokémon!");
            else if (loc == Game.Location.ViridianCity) Game.UpdateGoal("Explore Viridian City and visit Pokémon Center or Poké Mart.");
        }

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
            foreach (var p in Game.Party)
                p.CurrentHP = p.MaxHP;
            Dialogue.TypeDialogue("Nurse Joy", "\"Welcome! Your Pokémon are fighting fit!\"");
        }

        static void DepositPokemon()
        {
            if (Game.Party.Count <= 1)
            {
                Console.WriteLine("You must keep at least one Pokémon in your party.");
                return;
            }

            Console.WriteLine("\n=== YOUR PARTY ===");
            for (int i = 0; i < Game.Party.Count; i++)
                Console.WriteLine($"{i + 1}. {Game.Party[i].Name} - HP: {Game.Party[i].CurrentHP}/{Game.Party[i].MaxHP}");
            Console.WriteLine("Choose a Pokémon number to deposit:");
            string input = Console.ReadLine()?.Trim();
            if (int.TryParse(input, out int choice) && choice > 0 && choice <= Game.Party.Count)
            {
                var selected = Game.Party[choice - 1];
                Game.AddToStorage(selected);
                Game.RemovePokemon(selected);
                Console.WriteLine($"{selected.Name} was deposited into storage!");
            }
            else Console.WriteLine("Invalid choice.");
        }

        static void WithdrawPokemon()
        {
            if (Game.Storage.Count == 0)
            {
                Console.WriteLine("No Pokémon in storage.");
                return;
            }

            Console.WriteLine("\n=== STORAGE BOX ===");
            for (int i = 0; i < Game.Storage.Count; i++)
                Console.WriteLine($"{i + 1}. {Game.Storage[i].Name} - HP: {Game.Storage[i].CurrentHP}/{Game.Storage[i].MaxHP}");
            Console.WriteLine("===================");
            Console.WriteLine("Choose a Pokémon number to withdraw:");
            string input = Console.ReadLine()?.Trim();
            if (int.TryParse(input, out int choice) && choice > 0 && choice <= Game.Storage.Count)
            {
                var selected = Game.Storage[choice - 1];
                Game.AddPokemon(selected);
                Game.Storage.Remove(selected);
                Console.WriteLine($"{selected.Name} was withdrawn and added to your party!");
            }
            else Console.WriteLine("Invalid choice.");
        }

        static void PokeMart()
        {
            Console.Clear();
            Dialogue.TypeDialogue("Shopkeeper", "\"Welcome to the Poké Mart!\"");
            Console.WriteLine($"You have {Game.Money} Pokédollars and {Game.PokeBalls} Poké Balls.");
            Console.WriteLine("\n1. Buy Poké Ball (200₽)");
            Console.WriteLine("2. Leave");
            Console.Write("> ");
            string input = Console.ReadLine()?.Trim().ToLower();

            if (input == "1" || input == "buy")
            {
                if (Game.SpendMoney(200))
                {
                    Game.AddPokeball();
                    Console.WriteLine("You bought a Poké Ball!");
                    Console.WriteLine($"Now you have {Game.PokeBalls} Poké Balls.");
                }
                else Console.WriteLine("Not enough money!");
            }
            else if (input == "2" || input == "leave")
            {
                Console.WriteLine("You leave the Poké Mart.");
            }
            else Console.WriteLine("Invalid option.");

        }

        static void EncounterWildPokemon()
        {
            Dialogue.TypeDialogue("Narrator", "The tall grass rustles...");

            // Lijst van mogelijke Pokémon op Route 1
            List<Pokemon> Route1Wild = new List<Pokemon>
            {
                new Pokemon("Pidgey", 40, new List<Move> { new Move("Tackle", 8) }),
                new Pokemon("Rattata", 30, new List<Move> { new Move("Tackle", 10) }),
                new Pokemon("Caterpie", 35, new List<Move> { new Move("Tackle", 5) }),
                new Pokemon("Weedle", 35, new List<Move> { new Move("Poison Sting", 5) })
            };

            // Kies willekeurig een Pokémon
            Random rng = new Random();
            var wildTemplate = Route1Wild[rng.Next(Route1Wild.Count)];
            var wild = new Pokemon(wildTemplate.Name, wildTemplate.MaxHP, wildTemplate.Moves);

            Dialogue.TypeDialogue("Narrator", $"A wild {wild.Name} appears!");
            BattleSystem.Battle(wild);

            // Controleer of de speler nog een Pokémon heeft die niet is uitgeschakeld
            if (!Game.Party[0].IsFainted())
                MoveTo(Game.Location.ViridianCity, "After the encounter, you continue and soon see Viridian City ahead.");
            else
            {
                Console.WriteLine("Game Over! Your Pokémon fainted.");
                Environment.Exit(0);
            }
        }

    }
}
