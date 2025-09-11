namespace PokemonTextAdventure.Models
{
    public class Move
    {
        // Publiek zodat gevechten de aanval kunnen gebruiken
        public string Name { get; set; }
        public int Power { get; set; }

        // Lege constructor zodat we een move later makkelijk kunnen aanmaken
        public Move() { }

        // Constructor om een move direct met naam en kracht te maken
        public Move(string name, int power) { Name = name; Power = power; }
    }
}
