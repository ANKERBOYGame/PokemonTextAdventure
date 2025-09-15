using System;
using System.Linq;
using PokemonTextAdventure.Models;

namespace PokemonTextAdventure.Systems
{
    public static class BattleSystem
    {
        public static void Battle(Pokemon opponent, bool isTrainerBattle = false)
        {
            int activeIndex = 0;
            var playerPokemon = Game.Party[activeIndex];
            Random rng = new Random();

            Console.WriteLine("=== BATTLE START ===");
            while (!opponent.IsFainted() && Game.Party.Exists(p => !p.IsFainted()))
            {
                Console.Clear();

                playerPokemon = Game.Party[activeIndex];
                Console.WriteLine($"\n{playerPokemon.Name} HP: {playerPokemon.CurrentHP}/{playerPokemon.MaxHP}");
                Console.WriteLine($"{opponent.Name} HP: {opponent.CurrentHP}/{opponent.MaxHP}");
                Console.WriteLine($"Poké Balls: {Game.PokeBalls}");

                Console.WriteLine("\nWhat do you want to do?");
                Console.WriteLine("1. Fight");
                Console.WriteLine("2. Throw Poké Ball");
                Console.WriteLine("3. Pokémon (switch)");
                Console.WriteLine("4. Run");
                Console.Write("> ");
                string action = Console.ReadLine()?.Trim().ToLower();

                bool playerTurnValid = true;

                // ---------------------------
                // Fight
                // ---------------------------
                if (action == "1" || action == "fight")
                {
                    Console.WriteLine("\nChoose a move:");
                    for (int i = 0; i < playerPokemon.Moves.Count; i++)
                        Console.WriteLine($"{i + 1}. {playerPokemon.Moves[i].Name}");
                    Console.Write("> ");
                    string input = Console.ReadLine()?.Trim().ToLower();

                    Move move = ParseMove(input, playerPokemon);
                    if (move == null)
                    {
                        Console.WriteLine("Invalid move. Press Enter to continue...");
                        Console.ReadLine();
                        playerTurnValid = false;
                    }
                    else
                    {
                        if (move.Power > 0)
                            Dialogue.TypeDialogue("Narrator", $"{playerPokemon.Name} uses {move.Name}!");
                        else
                            Dialogue.TypeDialogue("Narrator", $"{playerPokemon.Name} used {move.Name}, but it had no effect!");

                        opponent.CurrentHP -= move.Power;
                        if (opponent.CurrentHP < 0) opponent.CurrentHP = 0;

                        if (opponent.IsFainted())
                        {
                            Dialogue.TypeDialogue("Narrator", $"{opponent.Name} fainted!");
                            break;
                        }
                    }
                }
                // ---------------------------
                // Poké Ball
                // ---------------------------
                else if (action == "2" || action == "poké ball" || action == "throw")
                {
                    if (isTrainerBattle)
                    {
                        Console.WriteLine("You can't use Poké Balls on a trainer's Pokémon!");
                        Console.WriteLine("Press Enter to continue...");
                        Console.ReadLine();
                        playerTurnValid = false;
                    }
                    else
                    {
                        if (!Game.UsePokeball())
                        {
                            Console.WriteLine("You have no Poké Balls left!");
                            Console.WriteLine("Press Enter to continue...");
                            Console.ReadLine();
                            playerTurnValid = false;
                        }
                        else
                        {
                            Console.WriteLine($"You throw a Poké Ball! ({Game.PokeBalls} left)");

                            if (TryCatchPokemon(opponent))
                            {
                                Console.WriteLine($"Gotcha! {opponent.Name} was caught!");
                                if (Game.Party.Count < Game.MaxPartySize)
                                {
                                    Game.AddPokemon(opponent);
                                    Console.WriteLine($"{opponent.Name} was added to your party!");
                                }
                                else
                                {
                                    Game.AddToStorage(opponent);
                                    Console.WriteLine($"{opponent.Name} was sent to storage!");
                                }
                                return;
                            }
                            else
                            {
                                Console.WriteLine($"{opponent.Name} broke free!");
                                // failed catch still counts as a valid turn
                            }
                        }
                    }
                }
                // ---------------------------
                // Switch Pokémon
                // ---------------------------
                else if (action == "3" || action == "pokémon" || action == "switch")
                {
                    Console.WriteLine("\n=== YOUR PARTY ===");
                    for (int i = 0; i < Game.Party.Count; i++)
                    {
                        var p = Game.Party[i];
                        Console.WriteLine($"{i + 1}. {p.Name} - HP: {p.CurrentHP}/{p.MaxHP}" + (i == activeIndex ? " (active)" : ""));
                    }
                    Console.WriteLine("Choose a Pokémon to switch to:");
                    string input = Console.ReadLine()?.Trim();
                    if (int.TryParse(input, out int choice) && choice > 0 && choice <= Game.Party.Count)
                    {
                        if (choice - 1 == activeIndex)
                        {
                            Console.WriteLine($"{Game.Party[choice - 1].Name} is already active!");
                            Console.WriteLine("Press Enter to continue...");
                            Console.ReadLine();
                            playerTurnValid = false;
                        }
                        else if (Game.Party[choice - 1].IsFainted())
                        {
                            Console.WriteLine("You cannot switch to a fainted Pokémon!");
                            Console.WriteLine("Press Enter to continue...");
                            Console.ReadLine();
                            playerTurnValid = false;
                        }
                        else
                        {
                            activeIndex = choice - 1;
                            Console.WriteLine($"You switched to {Game.Party[activeIndex].Name}!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice.");
                        Console.WriteLine("Press Enter to continue...");
                        Console.ReadLine();
                        playerTurnValid = false;
                    }
                }
                // ---------------------------
                // Run
                // ---------------------------
                else if (action == "4" || action == "run" || action == "escape")
                {
                    if (isTrainerBattle)
                    {
                        Console.WriteLine("You can't run from a trainer battle!");
                        Console.WriteLine("Press Enter to continue...");
                        Console.ReadLine();
                        playerTurnValid = false;
                    }
                    else
                    {
                        if (rng.NextDouble() < 0.75)
                        {
                            Console.WriteLine("You successfully ran away!");
                            return;
                        }
                        else
                        {
                            Console.WriteLine("You couldn't escape!");
                        }
                    }
                }
                // ---------------------------
                // Invalid Option
                // ---------------------------
                else
                {
                    Console.WriteLine("Invalid option. Choose 1, 2, 3, or 4.");
                    Console.WriteLine("Press Enter to continue...");
                    Console.ReadLine();
                    playerTurnValid = false;
                }

                // ---------------------------
                // Opponent's turn
                // ---------------------------
                if (playerTurnValid && !opponent.IsFainted())
                {
                    var enemyMove = opponent.Moves[0];
                    Dialogue.TypeDialogue("Narrator", $"The {opponent.Name} uses {enemyMove.Name}!");
                    playerPokemon.CurrentHP -= enemyMove.Power;
                    if (playerPokemon.CurrentHP < 0) playerPokemon.CurrentHP = 0;

                    if (playerPokemon.IsFainted())
                    {
                        Dialogue.TypeDialogue("Narrator", $"{playerPokemon.Name} fainted!");

                        if (Game.Party.Exists(p => !p.IsFainted()))
                        {
                            Console.WriteLine("Choose a new Pokémon:");
                            while (true)
                            {
                                for (int i = 0; i < Game.Party.Count; i++)
                                {
                                    var p = Game.Party[i];
                                    Console.WriteLine($"{i + 1}. {p.Name} - HP: {p.CurrentHP}/{p.MaxHP}");
                                }
                                string input = Console.ReadLine()?.Trim();
                                if (int.TryParse(input, out int choice) && choice > 0 && choice <= Game.Party.Count && !Game.Party[choice - 1].IsFainted())
                                {
                                    activeIndex = choice - 1;
                                    Console.WriteLine($"You sent out {Game.Party[activeIndex].Name}!");
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("Invalid choice. Pick a healthy Pokémon.");
                                }
                            }
                        }
                    }
                }
            }
            Console.WriteLine("=== BATTLE END ===");
        }

        // ---------------------------
        // Helper methods
        // ---------------------------
        static bool TryCatchPokemon(Pokemon wild)
        {
            double baseRate = 0.2;
            double hpFactor = 1.0 - ((double)wild.CurrentHP / wild.MaxHP);

            Random rng = new Random();
            double randomFactor = rng.NextDouble() * 0.1;

            double catchChance = baseRate + hpFactor * 0.5 + randomFactor;
            catchChance = Math.Min(catchChance, 0.95);
            catchChance = Math.Max(catchChance, 0.05);

            return new Random().NextDouble() < catchChance;
        }

        static Move ParseMove(string input, Pokemon p)
        {
            if (int.TryParse(input, out int idx) && idx > 0 && idx <= p.Moves.Count)
                return p.Moves[idx - 1];
            foreach (var m in p.Moves)
                if (m.Name.ToLower() == input) return m;
            return null;
        }
    }
}
