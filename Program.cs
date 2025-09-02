namespace PokemonTextAdventure
{
    using System;
    using System.Threading;

    class Program
    {
        // Enum die de mogelijke locaties van de speler bijhoudt
        enum PlayerLocation
        {
            PalletTown,
            Route1,
            ViridianCity
        }

        // Huidige doel van de speler
        static string currentGoal = "Talk to Professor Oak and choose your first Pokémon.";
        // Starter Pokémon gekozen door de speler
        static string playerStarter = "";
        // Huidige locatie van de speler
        static PlayerLocation currentLocation = PlayerLocation.PalletTown;

        static void Main(string[] args)
        {
            Console.Title = "Pokémon Text Adventure";
            ShowWelcome(); // Toon welkomsscherm met instructies

            // Hoofdgame loop
            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine()?.Trim().ToLowerInvariant();

                if (string.IsNullOrEmpty(input)) continue;

                // Command-handler
                if (input == "start")
                {
                    StartGame();
                }
                else if (input == "help" || input == "instructions")
                {
                    ShowInstructions();
                }
                else if (input == "goal" || input == "doel")
                {
                    ShowGoal();
                }
                else if (input == "hint")
                {
                    ShowHint();
                }
                else if (input == "explore")
                {
                    Explore();
                }
                else if (input == "back" || input == "go back")
                {
                    GoBack();
                }
                else if (input == "quit" || input == "exit")
                {
                    Console.WriteLine("Goodbye, Trainer!");
                    return; // Sluit het spel
                }
                else
                {
                    Console.WriteLine("Unknown command. Type 'help' for instructions.");
                }
            }
        }

        // Welkomstscherm van het spel
        static void ShowWelcome()
        {
            Console.Clear();
            Console.WriteLine("===========================================");
            Console.WriteLine("        POKÉMON TEXT ADVENTURE");
            Console.WriteLine("===========================================");
            Console.WriteLine("Welcome, Trainer!");
            Console.WriteLine("Type 'start' to begin your Pokémon journey.");
            Console.WriteLine("Type 'help' to see instructions.");
            Console.WriteLine("Type 'goal' to check your current mission.");
            Console.WriteLine("Type 'hint' if you get stuck.");
            Console.WriteLine("Type 'quit' to exit.");
            Console.WriteLine();
        }

        // Toon een lijst met beschikbare commands en uitleg
        static void ShowInstructions()
        {
            Console.Clear();
            Console.WriteLine("\n=== INSTRUCTIONS ===");
            Console.WriteLine("Available Commands:");
            Console.WriteLine(" - start: Begin your adventure");
            Console.WriteLine(" - explore: Look around your current location");
            Console.WriteLine(" - back: Go back to the previous location");
            Console.WriteLine(" - goal / doel: Show your current mission");
            Console.WriteLine(" - hint: Get a useful clue if you are stuck");
            Console.WriteLine(" - help / instructions: Show instructions");
            Console.WriteLine(" - quit: Exit the game");
            Console.WriteLine();
            Console.WriteLine("Gameplay:");
            Console.WriteLine(" • The game will often give you options to choose from.");
            Console.WriteLine(" • Type the option name (for example 'center').");
            Console.WriteLine(" • Use 'goal' to stay on track and 'hint' if you need help.");
            Console.WriteLine("====================\n");
        }

        // Toon het huidige doel van de speler
        static void ShowGoal()
        {
            Console.WriteLine($"\n=== CURRENT GOAL ===\n{currentGoal}\n");
        }

        // Toon hints gebaseerd op de huidige voortgang
        static void ShowHint()
        {
            Console.WriteLine("\n=== HINT ===");

            if (playerStarter == "")
            {
                Console.WriteLine("Professor Oak is waiting for you. Try typing the name of a Pokémon (Bulbasaur, Charmander, or Squirtle).");
            }
            else if (currentLocation == PlayerLocation.Route1)
            {
                Console.WriteLine("Keep moving north on Route 1. Wild Pokémon may appear—be ready!");
            }
            else if (currentLocation == PlayerLocation.ViridianCity)
            {
                Console.WriteLine("You made it to Viridian City! Try exploring the Poké Mart or the Pokémon Center.");
            }
            else
            {
                Console.WriteLine("Explore your surroundings and interact with characters. The path forward will become clear!");
            }

            Console.WriteLine("====================\n");
        }

        // Start de game en begin het verhaal
        static void StartGame()
        {
            Console.Clear();

            TypeDialogue("Professor Oak", "\"Welcome to the world of Pokémon!\"");
            Thread.Sleep(500);
            TypeDialogue("Professor Oak", "\"My name is Oak, but people call me the Pokémon Professor!\"");
            Thread.Sleep(500);
            Console.WriteLine();

            TypeDialogue("Narrator", "You find yourself in Professor Oak’s lab in Pallet Town.");
            TypeDialogue("Narrator", "On a table, three Poké Balls rest in gleaming red-and-white.");
            Console.WriteLine();

            ChooseStarter(); // Laat speler een starter Pokémon kiezen
        }

        // Laat de speler zijn starter Pokémon kiezen
        static void ChooseStarter()
        {
            Console.WriteLine();
            TypeDialogue("Professor Oak", "\"Now, choose your first Pokémon to begin your journey!\"");

            while (playerStarter == "")
            {
                Console.WriteLine("\nChoose your starter:");
                Console.WriteLine("1. Bulbasaur");
                Console.WriteLine("2. Charmander");
                Console.WriteLine("3. Squirtle");
                Console.Write("> ");
                var input = Console.ReadLine()?.Trim().ToLowerInvariant();

                switch (input)
                {
                    case "1":
                    case "bulbasaur":
                        playerStarter = "Bulbasaur";
                        break;
                    case "2":
                    case "charmander":
                        playerStarter = "Charmander";
                        break;
                    case "3":
                    case "squirtle":
                        playerStarter = "Squirtle";
                        break;
                    default:
                        Console.WriteLine("Please choose 1, 2, or 3.");
                        break;
                }
            }

            Console.Clear();
            TypeDialogue("Narrator", $"You pick up {playerStarter}’s Poké Ball. A new friendship begins!");
            TypeDialogue("Professor Oak", $"\"Excellent choice! Take care of {playerStarter}, and your journey will be filled with adventure.\"");

            currentGoal = "Head north to Route 1 and begin your journey to Viridian City.";
            currentLocation = PlayerLocation.Route1;

            Console.WriteLine("\nType 'explore' to travel to Route 1.");
        }

        // Handle voor exploratie op de huidige locatie
        static void Explore()
        {
            switch (currentLocation)
            {
                case PlayerLocation.PalletTown:
                    Console.WriteLine("You look around Professor Oak’s lab. The three Poké Balls are waiting for you.");
                    break;
                case PlayerLocation.Route1:
                    Route1(); // Route 1 encounter
                    break;
                case PlayerLocation.ViridianCity:
                    ViridianCity(); // Viridian City interacties
                    break;
            }
        }

        // Laat de speler teruggaan naar de vorige locatie
        static void GoBack()
        {
            if (currentLocation == PlayerLocation.ViridianCity)
            {
                currentLocation = PlayerLocation.Route1;
                Console.WriteLine("You walk back from Viridian City towards Route 1.");
            }
            else if (currentLocation == PlayerLocation.Route1)
            {
                currentLocation = PlayerLocation.PalletTown;
                Console.WriteLine("You walk back south to Pallet Town.");
            }
            else
            {
                Console.WriteLine("You’re already at the starting point!");
            }
        }

        // Route 1 encounter met wild Pokémon
        static void Route1()
        {
            Console.Clear();
            TypeDialogue("Narrator", "You walk along Route 1. The tall grass rustles nearby...");
            Thread.Sleep(500);
            TypeDialogue("Narrator", "A wild Pidgey appears!");

            Console.WriteLine("\nWhat do you do?");
            Console.WriteLine("1. Fight");
            Console.WriteLine("2. Run");

            bool resolved = false;
            while (!resolved)
            {
                Console.Write("> ");
                var input = Console.ReadLine()?.Trim().ToLowerInvariant();

                if (input == "1" || input == "fight")
                {
                    TypeDialogue("Narrator", $"You send out {playerStarter}! After a short battle, the wild Pidgey flees.");
                    resolved = true;
                }
                else if (input == "2" || input == "run")
                {
                    TypeDialogue("Narrator", "You dash away from the tall grass. The Pidgey loses interest.");
                    resolved = true;
                }
                else
                {
                    Console.WriteLine("Choose 1 or 2.");
                }
            }

            Console.WriteLine();
            TypeDialogue("Narrator", "After your encounter, you continue walking and soon see the gates of Viridian City ahead.");
            currentLocation = PlayerLocation.ViridianCity;
            currentGoal = "Explore Viridian City and visit the Pokémon Center or Poké Mart.";

            Console.WriteLine("\nType 'explore' to enter Viridian City or 'back' to return to Pallet Town.");
        }

        // Interacties in Viridian City
        static void ViridianCity()
        {
            Console.Clear();
            TypeDialogue("Narrator", "You arrive in Viridian City. The bustling town is alive with trainers and shops.");

            Console.WriteLine("\nWhere do you want to go?");
            Console.WriteLine("1. Pokémon Center");
            Console.WriteLine("2. Poké Mart");

            bool chosen = false;
            while (!chosen)
            {
                Console.Write("> ");
                var input = Console.ReadLine()?.Trim().ToLowerInvariant();

                switch (input)
                {
                    case "1":
                    case "center":
                        TypeDialogue("Nurse Joy", "\"Welcome to the Pokémon Center! Your Pokémon are fighting fit!\"");
                        chosen = true;
                        break;

                    case "2":
                    case "mart":
                        TypeDialogue("Shopkeeper", "\"Hello! We sell Potions and Poké Balls. Stock up for your journey!\"");
                        chosen = true;
                        break;

                    default:
                        Console.WriteLine("Please choose 1 or 2.");
                        break;
                }
            }

            Console.WriteLine();
            currentGoal = "Prepare for your next journey beyond Viridian City!";
            Console.WriteLine("\nType 'back' to return to Route 1.");
        }

        // Typing effect voor dialoog
        static void TypeDialogue(string speaker, string text, int speed = 20)
        {
            Console.Write(speaker + ": ");
            foreach (char c in text)
            {
                Console.Write(c);
                Thread.Sleep(speed);
            }
            Console.WriteLine();
        }
    }
}
