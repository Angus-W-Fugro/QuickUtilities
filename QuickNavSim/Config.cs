using System.Diagnostics;
using System.Text.Json;

namespace QuickNavSim;

public record Config(int UpdatesPerSecond, int ReceivePortNumber, int TransmitPortNumber, bool PrefixTime, Coordinate StartCoordinate, Motion Motion, Jitter Jitter)
{
    private static readonly string _AppDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
    private static readonly string _AppName = "QuickNavSim";
    private static readonly string _QuickNavSimFolder = Path.Combine(_AppDataFolder, _AppName);
    private static readonly string _FileName = "config.json";
    private static readonly string _FilePath = Path.Combine(_QuickNavSimFolder, _FileName);

    public static Config Default => new Config(1, 7468, 7467, true, Coordinate.Default, Motion.Default, Jitter.Default);

    public void Save()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(this, options);

        Directory.CreateDirectory(_QuickNavSimFolder); // Create the directory if it doesn't exist

        File.WriteAllText(_FilePath, json);
    }

    public static Config LoadOrDefault()
    {
        var config = TryLoad();

        if (config is null)
        {
            config = Default;
        }

        config.Save();

        return config;
    }

    public static Config? TryLoad()
    {
        if (!File.Exists(_FilePath))
        {
            return null;
        }

        var json = File.ReadAllText(_FilePath);

        try
        {
            var config = JsonSerializer.Deserialize<Config>(json);

            if (config is null || config.StartCoordinate is null || config.Motion is null || config.Jitter is null)
            {
                return null;
            }

            return config;
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Failed to load config.json, reverting to default values");
            return null;
        }
    }

    public override string ToString()
    {
        var path = Path.GetFullPath(_FilePath);

        var mapping = Coordinate.Mapping;

        if (PrefixTime)
        {
            mapping = $"DateTime,{mapping}";
        }

        return $"Loaded from: {path} (press \"o\" to open, \"r\" to reload)\nTransmitting on port: {TransmitPortNumber}\nReceiving on port: {ReceivePortNumber}\nMapping: {mapping}\nStarting position: {StartCoordinate}\nMotion: {Motion}\nJitter: {Jitter}";
    }

    public static void OpenConfigFile()
    {
        var processInfo = new ProcessStartInfo
        {
            UseShellExecute = true,
            FileName = _FilePath,
        };

        var process = new Process
        {
            StartInfo = processInfo
        };

        process.Start();
    }
}

public record Coordinate(double Easting, double Northing, double Depth, double KP, double Heading, double Pitch, double Roll)
{
    public static Coordinate Default => new Coordinate(311926.07, 7784141.86, 200.0, 90.000, 0.0, 0.0, 0.0);

    public static string Mapping => "Easting,Northing,Depth,KP,Heading,Pitch,Roll";

    public override string ToString()
    {
        return $"{Easting:F2},{Northing:F2},{Depth:F1},{KP:F3},{Heading:F1},{Pitch:F1},{Roll:F1}";
    }
}

public record Motion(double Easting, double Northing, double Depth, double KP, double Heading, double Pitch, double Roll)
{
    public static Motion Default => new Motion(0.5, 0.5, 0.0, 0.001, 0, 0, 0);

    public override string ToString()
    {
        return $"{Easting:+0.####},{Northing:+0.####},{Depth:+0.####},{KP:+0.####},{Heading:+0.#},{Pitch:+0.#},{Roll:+0.#}";
    }
}

public record Jitter(double Easting, double Northing, double Depth, double KP, double Heading, double Pitch, double Roll)
{
    public static Jitter Default => new Jitter(0.1, 0.1, 0.1, 0.0001, 1.0, 1.0, 1.0);

    public override string ToString()
    {
        return $"±{Easting},±{Northing},±{Depth},±{KP},±{Heading},±{Pitch},±{Roll}";
    }
}
