using System.Collections.Generic;

namespace PokemonTextAdventure.Models
{
    public class Pokemon
    {
        public string Name { get; set; }
        public int MaxHP { get; set; }
        public int CurrentHP { get; set; }
        public List<Move> Moves { get; set; }

        public Pokemon() { }  // ← nodig voor deserialisatie

        public Pokemon(string name, int hp, List<Move> moves)
        {
            Name = name;
            MaxHP = hp;
            CurrentHP = hp;
            Moves = moves;
        }

        public bool IsFainted() => CurrentHP <= 0;
    }
}
