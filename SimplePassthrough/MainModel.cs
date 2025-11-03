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
    private PortType _IncomingPortType = PortType.COM;
    private string _IncomingPortAddress = "COM1";
    private PortType _OutgoingPortType = PortType.UDP;
    private string _OutgoingPortAddress = "8080";
    private bool _IsRunning;
    private PassthroughManager? _PassthroughManager;

    public ObservableCollection<PortType> AvailablePortTypes { get; } =
    [
        PortType.COM,
        PortType.UDP
    ];

    public PortType IncomingPortType
    {
        get => _IncomingPortType;
        set
        {
            _IncomingPortType = value;
            NotifyPropertyChanged();
        }
    }

    public string IncomingPortAddress
    {
        get => _IncomingPortAddress;
        set
        {
            _IncomingPortAddress = value;
            NotifyPropertyChanged();
        }
    }

    public PortType OutgoingPortType
    {
        get => _OutgoingPortType;
        set
        {
            _OutgoingPortType = value;
            NotifyPropertyChanged();
        }
    }

    public string OutgoingPortAddress
    {
        get => _OutgoingPortAddress;
        set
        {
            _OutgoingPortAddress = value;
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

    private void StartRunning()
    {
        try
        {
            var incomingPort = CreatePort(IncomingPortType, IncomingPortAddress, true);
            var outgoingPort = CreatePort(OutgoingPortType, OutgoingPortAddress, false);

            _PassthroughManager = new PassthroughManager(incomingPort, outgoingPort);
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
