namespace QuickNavSim;

public class Manager : IDisposable
{
    private Config _Config;
    private readonly UdpPort _Receiver;
    private readonly UdpPort _Transmitter;
    private readonly Timer _Timer;
    private readonly TimeSpan _TimerInterval;
    private readonly ConsoleText _LastTransmitted = new();
    private readonly ConsoleText _LastReceived = new();
    private readonly Random _Random = new();

    private readonly int _ConsoleDisplayLine; // Initial config displays lines of text, update the fields after this line

    private bool Running;
    private Coordinate _CurrentPosition;

    public static void Run()
    {
        var config = Config.LoadOrDefault();

        var manager = new Manager(config);

        while (manager.Running)
        {
            var userInput = Console.ReadKey(); // Keeps the program running

            if (userInput.Key == ConsoleKey.O)
            {
                Config.OpenConfigFile();
            }

            if (userInput.Key == ConsoleKey.R)
            {
                manager.Dispose();
                config = Config.LoadOrDefault(); // Reload the config
                manager = new Manager(config); // Create a new instance with the updated config
            }
        }
    }

    public Manager(Config config)
    {
        Running = true;
        _Config = config;
        _CurrentPosition = config.StartCoordinate;

        _Receiver = new UdpPort(_Config.ReceivePortNumber);

        _Receiver.Received += Receiver_Received;
        _Receiver.Error += Receiver_Error;

        _Transmitter = new UdpPort(_Config.TransmitPortNumber, isTransmitter: true);

        Console.SetCursorPosition(0, 0);
        Console.WriteLine(_Config);
        Console.WriteLine("---");
        _ConsoleDisplayLine = Console.CursorTop;
        Display();

        var updatesPerSecond = Math.Max(1, _Config.UpdatesPerSecond);

        _TimerInterval = TimeSpan.FromSeconds(1.0 / updatesPerSecond);

        _Timer = new Timer(OnTimerTick, null, TimeSpan.Zero, _TimerInterval);

        _Receiver.StartReceiving();
    }

    private void Receiver_Received(object? sender, DataEventArgs e)
    {
        _LastReceived.Text = e.Data;
    }

    private void Receiver_Error(object? sender, ErrorEventArgs e)
    {
        _LastReceived.Text = $"ERROR: {e.GetException().Message}";
    }

    private void OnTimerTick(object? state)
    {
        string data = _CurrentPosition.ToString();

        if (_Config.PrefixTime)
        {
            string timeData = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
            data = $"{timeData},{data}";
        }

        _Transmitter.Send(data);
        _LastTransmitted.Text = data;

        UpdatePosition();

        Display();
    }

    private void UpdatePosition()
    {
        double easting = _CurrentPosition.Easting;
        double northing = _CurrentPosition.Northing;
        double depth = _CurrentPosition.Depth;
        double kp = _CurrentPosition.KP;
        double heading = _CurrentPosition.Heading;
        double pitch = _CurrentPosition.Pitch;
        double roll = _CurrentPosition.Roll;

        easting += _Config.Motion.Easting;
        northing += _Config.Motion.Northing;
        depth += _Config.Motion.Depth;
        kp += _Config.Motion.KP;
        heading += _Config.Motion.Heading;
        pitch += _Config.Motion.Pitch;
        roll += _Config.Motion.Roll;

        easting += RandomValue(_Config.Jitter.Easting);
        northing += RandomValue(_Config.Jitter.Northing);
        depth += RandomValue(_Config.Jitter.Depth);
        kp += RandomValue(_Config.Jitter.KP);
        heading += RandomValue(_Config.Jitter.Heading);
        pitch += RandomValue(_Config.Jitter.Pitch);
        roll += RandomValue(_Config.Jitter.Roll);

        heading = (heading + 360) % 360;
        pitch = (pitch + 360) % 360;
        roll = (roll + 360) % 360;

        _CurrentPosition = new Coordinate(easting, northing, depth, kp, heading, pitch, roll);
    }

    private double RandomValue(double max)
    {
        if (max == 0)
        {
            return 0;
        }

        return (_Random.NextDouble() - 0.5) * 2 * max;
    }

    private void Display()
    {
        string display = $"Tx: {_LastTransmitted}\nRx: {_LastReceived}";

        Console.SetCursorPosition(0, _ConsoleDisplayLine);
        Console.WriteLine(display);
    }

    public void Dispose()
    {
        Running = false;

        _Receiver.Dispose();
        _Transmitter.Dispose();
        _Timer.Dispose();
    }
}
