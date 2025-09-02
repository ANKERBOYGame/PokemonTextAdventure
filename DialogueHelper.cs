namespace PokemonTextAdventure
{
    using System;
    using System.Threading;

    public static class DialogueHelper
    {
        public static void TypeDialogue(string speaker, string text, int speed = 20)
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
