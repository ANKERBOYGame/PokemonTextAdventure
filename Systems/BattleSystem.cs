using System;
using System.Linq;
using PokemonTextAdventure.Models;

namespace PokemonTextAdventure.Systems
{
    public static class BattleSystem
    {
        public static void Battle(Pokemon wild)
        {
            int activeIndex = 0; // which party Pokémon is active
            var playerPokemon = Game.Party[activeIndex];

            Console.WriteLine("=== BATTLE START ===");
            while (!wild.IsFainted() && Game.Party.Exists(p => !p.IsFainted()))
            {
                Console.Clear();

                playerPokemon = Game.Party[activeIndex];
                Console.WriteLine($"\n{playerPokemon.Name} HP: {playerPokemon.CurrentHP}/{playerPokemon.MaxHP}");
                Console.WriteLine($"{wild.Name} HP: {wild.CurrentHP}/{wild.MaxHP}");
                Console.WriteLine($"Poké Balls: {Game.PokeBalls}");

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

                    if (move.Power > 0) Dialogue.TypeDialogue("Narrator", $"{playerPokemon.Name} uses {move.Name}!");
                    else Dialogue.TypeDialogue("Narrator", $"{playerPokemon.Name} used {move.Name}, but it had no effect!");

                    wild.CurrentHP -= move.Power;
                    if (wild.CurrentHP < 0) wild.CurrentHP = 0;

                    if (wild.IsFainted()) { Dialogue.TypeDialogue("Narrator", $"The wild {wild.Name} fainted!"); break; }
                }
                else if (action == "2" || action == "poké ball" || action == "throw")
                {
                    if (!Game.UsePokeball())
                    {
                        Console.WriteLine("You have no Poké Balls left!");
                        continue;
                    }

                    Console.WriteLine($"You throw a Poké Ball! ({Game.PokeBalls} left)");

                    if (TryCatchPokemon(wild))
                    {
                        Console.WriteLine($"Gotcha! {wild.Name} was caught!");
                        if (Game.Party.Count < Game.MaxPartySize)
                        {
                            Game.AddPokemon(wild);
                            Console.WriteLine($"{wild.Name} was added to your party!");
                        }
                        else
                        {
                            Game.AddToStorage(wild);
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
                    for (int i = 0; i < Game.Party.Count; i++)
                    {
                        var p = Game.Party[i];
                        Console.WriteLine($"{i + 1}. {p.Name} - HP: {p.CurrentHP}/{p.MaxHP}" + (i == activeIndex ? " (active)" : ""));
                    }
                    Console.WriteLine("Choose a Pokémon to switch to:");
                    string input = Console.ReadLine()?.Trim();
                    if (int.TryParse(input, out int choice) && choice > 0 && choice <= Game.Party.Count)
                    {
                        if (choice - 1 == activeIndex) Console.WriteLine($"{Game.Party[choice - 1].Name} is already active!");
                        else if (Game.Party[choice - 1].IsFainted()) Console.WriteLine("You cannot switch to a fainted Pokémon!");
                        else
                        {
                            activeIndex = choice - 1;
                            Console.WriteLine($"You switched to {Game.Party[activeIndex].Name}!");
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
                    Dialogue.TypeDialogue("Narrator", $"The wild {wild.Name} uses {enemyMove.Name}!");
                    playerPokemon.CurrentHP -= enemyMove.Power;
                    if (playerPokemon.CurrentHP < 0) playerPokemon.CurrentHP = 0;

                    if (playerPokemon.IsFainted())
                    {
                        Dialogue.TypeDialogue("Narrator", $"{playerPokemon.Name} fainted!");

                        // Force switch if you still have Pokémon left
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
    }
}
