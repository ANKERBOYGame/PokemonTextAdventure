using System;

namespace PokemonTextAdventure.Systems
{
    public static class Dialogue
    {
        // Typ-effect voor dialoog om het spel levendiger te maken
        public static void TypeDialogue(string speaker, string text, int speed = 20, int pause = 800)
        {
            Console.Write(speaker + ": ");
            foreach (char c in text)
            {
                Console.Write(c);
                System.Threading.Thread.Sleep(speed); // kleine vertraging per letter voor effect
            }
            Console.WriteLine();

            System.Threading.Thread.Sleep(pause); 
        }
    }
}
