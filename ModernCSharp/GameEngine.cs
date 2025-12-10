namespace ModernCSharp;

public record PlayerStats(int Health, int Level, bool HasWeapon);

public class GameEngine
{
    public string DetermineAction(PlayerStats player) => player switch
    {
        { Health: <= 0 } => "Game Over",
        { Health: <20, HasWeapon: false } => "Run away",
        { Health: <20, HasWeapon: true } => "Use weapon to defend",
        { Level: >=10, HasWeapon :true } => "Attack with confidence",
        _ => "Explore more area"
    };

    public void PlayScenarios()
    {
        var scenarios = new[]
        {
            new PlayerStats(0, 5, true),
            new PlayerStats(15, 3, false),
            new PlayerStats(15, 3, true),
            new PlayerStats(80, 12, true),
            new PlayerStats(100, 5, false),
            new PlayerStats(50, 7, true)
        };

        Console.WriteLine("\n=== Game Scenarios ===");
        foreach (var player in scenarios)
        {
            Console.WriteLine($"Health: {player.Health}, Level: {player.Level}, Has Weapon: {player.HasWeapon}");
            Console.WriteLine($"Action: {DetermineAction(player)}\n");
        }
    }
}

//In program.cs, to test  this code copy the below code
//using ModernCSharp;

//var game = new GameEngine();
//game.PlayScenarios();
