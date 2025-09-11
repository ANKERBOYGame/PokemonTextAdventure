using System.Collections.Generic;

namespace PokemonTextAdventure.Models
{
    public class Pokemon
    {
        // Publiek zodat andere delen van het spel de Pokémon kunnen gebruiken
        public string Name { get; set; }
        public int MaxHP { get; set; }
        public int CurrentHP { get; set; }
        public List<Move> Moves { get; set; }

        // Lege constructor zodat we een Pokémon later makkelijk kunnen aanmaken
        public Pokemon() { }

        // Constructor om een Pokémon direct met naam, HP en moves te maken
        public Pokemon(string name, int hp, List<Move> moves)
        {
            Name = name;
            MaxHP = hp;
            CurrentHP = hp;
            Moves = moves;
        }

        // Controleer of de Pokémon bewusteloos is
        public bool IsFainted() => CurrentHP <= 0;
    }
}
