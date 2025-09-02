namespace PokemonTextAdventure
{
    using System;
    using System.Threading;

    // Helpt bij het weergeven van dialoog met een 'type-effect' voor de tekst
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
