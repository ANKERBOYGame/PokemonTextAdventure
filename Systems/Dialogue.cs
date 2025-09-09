using System;

namespace PokemonTextAdventure.Systems
{
    public static class Dialogue
    {
        public static void TypeDialogue(string speaker, string text, int speed = 20, int pause = 800)
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
