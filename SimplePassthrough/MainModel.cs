using System.Collections.ObjectModel;

namespace SimplePassthrough;

public enum PortType
{
    COM,
    UDP
}

public class MainModel : Model
{
    private PortType _incomingPortType = PortType.COM;
    private string _incomingPortAddress = "COM1";
    private PortType _outgoingPortType = PortType.COM;
    private string _outgoingPortAddress = "COM2";

    public ObservableCollection<PortType> AvailablePortTypes { get; } = new()
    {
        PortType.COM,
        PortType.UDP
    };

    public PortType IncomingPortType
    {
        get => _incomingPortType;
        set
        {
            if (_incomingPortType != value)
            {
                _incomingPortType = value;
                NotifyPropertyChanged();
                // Set default address based on port type
                IncomingPortAddress = value == PortType.COM ? "COM1" : "8080";
            }
        }
    }

    public string IncomingPortAddress
    {
        get => _incomingPortAddress;
        set
        {
            if (_incomingPortAddress != value)
            {
                _incomingPortAddress = value;
                NotifyPropertyChanged();
            }
        }
    }

    public PortType OutgoingPortType
    {
        get => _outgoingPortType;
        set
        {
            if (_outgoingPortType != value)
            {
                _outgoingPortType = value;
                NotifyPropertyChanged();
                // Set default address based on port type
                OutgoingPortAddress = value == PortType.COM ? "COM2" : "8081";
            }
        }
    }

    public string OutgoingPortAddress
    {
        get => _outgoingPortAddress;
        set
        {
            if (_outgoingPortAddress != value)
            {
                _outgoingPortAddress = value;
                NotifyPropertyChanged();
            }
        }
    }
}
