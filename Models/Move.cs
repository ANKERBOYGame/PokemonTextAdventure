namespace PokemonTextAdventure.Models
{
    public class Move
    {
        public string Name { get; set; }
        public int Power { get; set; }

        public Move(string name, int power)
        {
            Name = name;
            Power = power;
        }
    }
}
