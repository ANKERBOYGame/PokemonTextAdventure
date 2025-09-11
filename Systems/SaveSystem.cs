using PokemonTextAdventure;
using PokemonTextAdventure.Models;
using System.Text.Json;

public static class SaveSystem
{
    private static readonly string SaveFile = "savegame.json";

    // SaveData is een aparte klasse zodat alleen de relevante state
    // geserialiseerd wordt i.p.v. de hele Game class (die veel static logica bevat).
    public class SaveData
    {
        public Game.Location CurrentLocation { get; set; }
        public string CurrentGoal { get; set; }
        public int Money { get; set; }
        public int PokeBalls { get; set; }
        public List<Pokemon> Party { get; set; }
        public List<Pokemon> Storage { get; set; }
    }

    public static void Save()
    {
        // Direct de properties uit Game ophalen i.p.v. Game zelf serializen,
        // omdat Game statisch is en ook methodes bevat die niet in save-bestand horen.
        var data = new SaveData
        {
            CurrentLocation = Game.CurrentLocation,
            CurrentGoal = Game.CurrentGoal,
            Money = Game.Money,
            PokeBalls = Game.PokeBalls,
            Party = Game.Party,
            Storage = Game.Storage
        };

        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(SaveFile, json);
        Console.WriteLine("Game saved!");
    }

    public static void Load()
    {
        if (!File.Exists(SaveFile))
        {
            Console.WriteLine("No save file found!");
            return;
        }

        var json = File.ReadAllText(SaveFile);
        var data = JsonSerializer.Deserialize<SaveData>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (data == null)
        {
            Console.WriteLine("Failed to load save data.");
            return;
        }

        // State terugzetten via setters i.p.v. direct properties te overschrijven
        // zodat de interne game-logica consistent blijft.
        Game.UpdateLocation(data.CurrentLocation);
        Game.UpdateGoal(data.CurrentGoal);
        Game.SetMoney(data.Money);
        Game.SetPokeBalls(data.PokeBalls);

        // Clear+AddRange gebruikt i.p.v. lijst vervangen,
        // omdat de Game class de originele lijstobjecten bewaart.
        Game.Party.Clear();
        if (data.Party != null) Game.Party.AddRange(data.Party);

        Game.Storage.Clear();
        if (data.Storage != null) Game.Storage.AddRange(data.Storage);

        Game.SetGameStarted(true);
        Console.WriteLine("Game loaded!");
    }
}