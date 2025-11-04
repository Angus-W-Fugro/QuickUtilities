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
    private IPortWrapper? _IncomingPort;
    private IPortWrapper? _OutgoingPort;
    private string? _PassedData;
    private ObservableCollection<string> _ConnectedCOMPorts = [];

    public MainModel()
    {
        CheckCOMPortLoop();
    }

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
            NotifyPropertyChanged(nameof(IncomingPortPlaceholder));
            IncomingPortAddress = string.Empty;
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
            NotifyPropertyChanged(nameof(OutgoingPortPlaceholder));
            OutgoingPortAddress = string.Empty;
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

    public string OutgoingPortPlaceholder => OutgoingPortType == PortType.COM ? "e.g. COM1" : "e.g. 127.0.0.1:8080";

    public string IncomingPortPlaceholder => IncomingPortType == PortType.COM ? "e.g. COM1" : "e.g. 127.0.0.1:8080";

    public string ToggleButtonLabel => IsRunning ? "Open" : "Closed";

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
            StopRunning();
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

    public ObservableCollection<string> ConnectedCOMPorts
    {
        get => _ConnectedCOMPorts;
        set
        {
            _ConnectedCOMPorts = value;
            NotifyPropertyChanged();
        }
    }

    private void CheckCOMPortLoop()
    {
        Task.Run(async () =>
        {
            while (true)
            {
                var ports = System.IO.Ports.SerialPort.GetPortNames();

                Application.Current?.Dispatcher.Invoke(() =>
                {
                    ConnectedCOMPorts = new ObservableCollection<string>(ports);
                });

                await Task.Delay(1000);
            }
        });
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
        _PassthroughManager?.Dispose();
        _IncomingPort?.Dispose();
        _OutgoingPort?.Dispose();

        try
        {
            _IncomingPort = CreatePort(IncomingPortType, IncomingPortAddress, true);
            _OutgoingPort = CreatePort(OutgoingPortType, OutgoingPortAddress, false);

            _PassthroughManager = new PassthroughManager(_IncomingPort, _OutgoingPort, LogData);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to start passthrough: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void StopRunning()
    {
        _PassthroughManager?.Dispose();
        _PassthroughManager = null;
        PassedData = null;
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
