using System.IO;
using System.Text.Json;

namespace SimplePassthrough;

public class Config
{
    private ConfigRecord _Record = new ConfigRecord(
        IncomingPortType: PortType.COM,
        IncomingPortAddress: string.Empty,
        OutgoingPortType: PortType.UDP,
        OutgoingPortAddress: string.Empty
    );

    public Config()
    {
        if (File.Exists("config.json"))
        {
            var json = File.ReadAllText("config.json");
            var record = JsonSerializer.Deserialize<ConfigRecord>(json);

            if (record != null)
            {
                _Record = record;
                return;
            }
        }

        Save();
    }

    private void Save()
    {
        var json = JsonSerializer.Serialize(_Record);
        File.WriteAllText("config.json", json);
    }

    public PortType IncomingPortType
    {
        get => _Record.IncomingPortType;
        set
        {
            _Record = _Record with { IncomingPortType = value };
            Save();
        }
    }

    public string IncomingPortAddress
    {
        get => _Record.IncomingPortAddress;
        set
        {
            _Record = _Record with { IncomingPortAddress = value };
            Save();
        }
    }

    public PortType OutgoingPortType
    {
        get => _Record.OutgoingPortType;
        set
        {
            _Record = _Record with { OutgoingPortType = value };
            Save();
        }
    }

    public string OutgoingPortAddress
    {
        get => _Record.OutgoingPortAddress;
        set
        {
            _Record = _Record with { OutgoingPortAddress = value };
            Save();
        }
    }
}

public record ConfigRecord(PortType IncomingPortType, string IncomingPortAddress, PortType OutgoingPortType, string OutgoingPortAddress);
