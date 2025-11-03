using System.Net;
using System.Net.Sockets;

namespace SimplePassthrough;

public class UdpPortWrapper : IPortWrapper
{
    private UdpClient _UdpClient;
    private IPEndPoint _Address;

    public UdpPortWrapper(string portNumber, bool listening)
    {
        var port = int.Parse(portNumber);
        _Address = new IPEndPoint(IPAddress.Loopback, port);

        if (listening)
        {
            _UdpClient = new UdpClient(port);
            StartListening();
        }
        else
        {
            _UdpClient = new UdpClient();
        }
    }

    private void StartListening()
    {
        Task.Run(async () =>
        {
            try
            {
                while (true)
                {
                    var result = await _UdpClient.ReceiveAsync();
                    DataReceived?.Invoke(this, result.Buffer);
                }
            }
            catch (ObjectDisposedException)
            {
                // Socket has been closed, exit the loop
            }
        });
    }

    public event EventHandler<byte[]> DataReceived = null!;

    public void Dispose()
    {
        _UdpClient.Dispose();
    }

    public void Send(byte[] data)
    {
        _UdpClient.Send(data, data.Length, _Address);
    }
}
