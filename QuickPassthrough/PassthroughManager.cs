using System.IO.Ports;
using System.Net;
using System.Net.Sockets;

namespace QuickPassthrough;

public class PassthroughManager : IDisposable
{
    private readonly SerialPort _SerialPort;
    private readonly UdpClient _UdpClient;
    private byte[] _Buffer = [];

    public PassthroughManager(string incomingCOMPort, int outgoingUDPPort)
    {
        _SerialPort = new SerialPort(incomingCOMPort);
        _SerialPort.Open();

        _UdpClient = new UdpClient();
        var address = new IPEndPoint(IPAddress.Any, outgoingUDPPort);
        _UdpClient.Connect(address);

        _SerialPort.DataReceived += SerialPort_DataReceived;
    }

    public void Dispose()
    {
        _SerialPort.DataReceived -= SerialPort_DataReceived;

        _SerialPort.Dispose();
        _UdpClient.Dispose();
    }

    private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        var bytesAvailable = _SerialPort.BytesToRead;

        if (_Buffer.Length < bytesAvailable)
        {
            Array.Resize(ref _Buffer, bytesAvailable);
        }

        _SerialPort.Read(_Buffer, 0, bytesAvailable);

        _UdpClient.Send(_Buffer, bytesAvailable);
    }
}
