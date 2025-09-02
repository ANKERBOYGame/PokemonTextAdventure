namespace PokemonTextAdventure
{
    public static class PlayerState
    {
        public static string Starter { get; set; } = "";
        public static string CurrentGoal { get; set; } = "Talk to Professor Oak and choose your first Pokémon.";
        public static string CurrentLocation { get; set; } = "lab";
        public static bool OnRoute1 { get; set; } = false;
        public static bool ReachedCity { get; set; } = false;
    }
}
