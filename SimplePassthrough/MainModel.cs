using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace SimplePassthrough;

public enum PortType
{
    COM,
    UDP
}

public class MainModel : Model
{
    private Config _Config = new Config();
    private bool _IsRunning;
    private PassthroughManager? _PassthroughManager;
    private string? _PassedData;

    public ObservableCollection<PortType> AvailablePortTypes { get; } =
    [
        PortType.COM,
        PortType.UDP
    ];

    public PortType IncomingPortType
    {
        get => _Config.IncomingPortType;
        set
        {
            _Config.IncomingPortType = value;
            NotifyPropertyChanged();
        }
    }

    public string IncomingPortAddress
    {
        get => _Config.IncomingPortAddress;
        set
        {
            _Config.IncomingPortAddress = value;
            NotifyPropertyChanged();
        }
    }

    public PortType OutgoingPortType
    {
        get => _Config.OutgoingPortType;
        set
        {
            _Config.OutgoingPortType = value;
            NotifyPropertyChanged();
        }
    }

    public string OutgoingPortAddress
    {
        get => _Config.OutgoingPortAddress;
        set
        {
            _Config.OutgoingPortAddress = value;
            NotifyPropertyChanged();
        }
    }

    public string ToggleButtonLabel => IsRunning ? "Stop" : "Start";

    public bool IsRunning
    {
        get => _IsRunning;
        set
        {
            _IsRunning = value;
            NotifyPropertyChanged();
            NotifyPropertyChanged(nameof(ToggleButtonLabel));
        }
    }

    public ICommand ToggleRunCommand => new Command(ToggleRun);

    private void ToggleRun()
    {
        if (IsRunning)
        {
            _PassthroughManager?.Dispose();
            IsRunning = false;
        }
        else
        {
            StartRunning();

            if (_PassthroughManager is not null)
            {
                IsRunning = true;
            }
        }
    }

    public string? PassedData
    {
        get => _PassedData;
        set
        {
            _PassedData = value;
            NotifyPropertyChanged();
        }
    }

    public void LogData(string data)
    {
        Application.Current?.Dispatcher.Invoke(() =>
        {
            PassedData = data;
        });
    }

    private void StartRunning()
    {
        try
        {
            var incomingPort = CreatePort(IncomingPortType, IncomingPortAddress, true);
            var outgoingPort = CreatePort(OutgoingPortType, OutgoingPortAddress, false);

            _PassthroughManager = new PassthroughManager(incomingPort, outgoingPort, LogData);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to start passthrough: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private IPortWrapper CreatePort(PortType portType, string address, bool listening)
    {
        switch (portType)
        {
            case PortType.COM:
                return new SerialPortWrapper(address);
            case PortType.UDP:
                return new UdpPortWrapper(address, listening);
            default:
                throw new NotSupportedException($"Port type {portType} is not supported.");
        }
    }
}
