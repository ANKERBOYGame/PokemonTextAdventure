using PokemonTextAdventure.Models;
using System.Collections.Generic;

namespace Text_based_adventure.Models
{
    // Data container voor opslaan/laden
    public class SaveData
    {
        public List<Pokemon> Party { get; set; }
        public List<Pokemon> Storage { get; set; }
        public int Money { get; set; }
        public int PokeBalls { get; set; }
        public string CurrentLocation { get; set; }
        public string CurrentGoal { get; set; }
    }
}
