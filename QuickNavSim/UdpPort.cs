using System.Net.Sockets;
using System.Text;
using System.Net;

namespace QuickNavSim;

public class UdpPort : IDisposable
{
    private readonly UdpClient _Client;
    private readonly IPEndPoint _TransmitEndPoint;

    private bool _Receiving = false;
    private DateTime _LastReceived = DateTime.MinValue;

    public UdpPort(int portNumber, bool isTransmitter = false)
    {
        if (isTransmitter)
        {
            _Client = new UdpClient();
        }
        else
        {
            _Client = new UdpClient(portNumber);
        }

        _TransmitEndPoint = new IPEndPoint(IPAddress.Loopback, portNumber);
    }

    public event EventHandler<DataEventArgs>? Received;

    public event EventHandler<ErrorEventArgs>? Error;

    public void StartReceiving()
    {
        _Receiving = true;

        Task.Run(async () =>
        {
            while (_Receiving)
            {
                try
                {
                    var result = await _Client.ReceiveAsync();
                    byte[] data = result.Buffer;
                    string message = Encoding.ASCII.GetString(data);
                    _LastReceived = DateTime.Now;
                    Received?.Invoke(this, new DataEventArgs(message));
                }
                catch (ObjectDisposedException) // Thrown when _Client is disposed while awaiting ReceiveAsync
                {
                    _Receiving = false;
                }
                catch (Exception ex)
                {
                    Error?.Invoke(this, new ErrorEventArgs(ex));
                }
            }
        });
    }

    public void Send(string message)
    {
        byte[] data = Encoding.ASCII.GetBytes(message);
        _Client.SendAsync(data, data.Length, _TransmitEndPoint);
    }

    public double TimeSinceLastData()
    {
        return (DateTime.Now - _LastReceived).TotalSeconds;
    }

    public void Dispose()
    {
        _Receiving = false;
        _Client.Dispose();
    }
}

public class DataEventArgs : EventArgs
{
    public DataEventArgs(string data)
    {
        Data = data;
        Time = DateTime.UtcNow;
    }

    public DateTime Time { get; set; }

    public string Data { get; set; } = string.Empty;
}
