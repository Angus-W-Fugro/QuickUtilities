using System.Collections.ObjectModel;

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
}
