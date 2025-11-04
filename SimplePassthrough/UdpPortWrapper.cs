using System.Net;
using System.Net.Sockets;

namespace SimplePassthrough;

public class UdpPortWrapper : IPortWrapper
{
    private UdpClient _UdpClient;
    private IPEndPoint _Endpoint;

    public UdpPortWrapper(string address, bool listening)
    {
        _Endpoint = IPEndPoint.Parse(address);

        if (listening)
        {
            _UdpClient = new UdpClient(_Endpoint.Port);
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
        _UdpClient.Send(data, data.Length, _Endpoint);
    }
}
