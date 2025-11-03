using System.Text.Json;

namespace QuickPassthrough;

internal class Program
{
    private const string ConfigFileName = "config.json";

    private static void Main(string[] args)
    {
        try
        {
            Run();
        }
        catch (Exception ex)
        {
            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"ERROR: {ex.Message}");
            Console.ForegroundColor = defaultColor;
        }
        finally
        {
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }

    private static void Run()
    {
        Console.WriteLine("Starting QuickPassthrough...\n\n");

        if (!File.Exists(ConfigFileName))
        {
            Console.WriteLine($"Config file '{ConfigFileName}' not found. Default file will be created. Run the app again to use it.");
            var defaultConfig = new Config("COM1", 8080);
            var defaultConfigJson = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigFileName, defaultConfigJson);
            OpenFile(ConfigFileName);
            return;
        }

        var fullPath = Path.GetFullPath(ConfigFileName);
        Console.WriteLine($"Loading config from: {fullPath}");

        var configJson = File.ReadAllText(ConfigFileName);
        var config = JsonSerializer.Deserialize<Config>(configJson);

        if (config is null)
        {
            throw new InvalidOperationException("Failed to parse config.json");
        }

        Console.WriteLine("---");
        Console.WriteLine($" IN: {config.IncomingCOMPort}");
        Console.WriteLine($"OUT: UDP {config.OutgoingUDPPort}");
        Console.WriteLine("---");

        var passthroughManager = new PassthroughManager(config.IncomingCOMPort, config.OutgoingUDPPort);

        Console.WriteLine("QuickPassthrough is running. Press any key to exit...");
        Console.ReadKey();
    }

    private static void OpenFile(string filePath)
    {
        var process = new System.Diagnostics.Process();
        process.StartInfo.FileName = "notepad.exe";
        process.StartInfo.Arguments = filePath;
        process.StartInfo.UseShellExecute = true;
        process.Start();
    }
}

public record Config(string IncomingCOMPort, int OutgoingUDPPort);