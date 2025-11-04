using System.Net;
using System.Net.Sockets;

namespace SimplePassthrough;

public class UdpPortWrapper : IPortWrapper
{
    private UdpClient _UdpClient;
    private IPEndPoint _Endpoint;
    private CancellationTokenSource _CancelTokenSource;

    public UdpPortWrapper(string address, bool listening)
    {
        _CancelTokenSource = new CancellationTokenSource();
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
            var cancelToken = _CancelTokenSource.Token;

            try
            {
                while (true)
                {
                    var result = await _UdpClient.ReceiveAsync(cancelToken);
                    DataReceived?.Invoke(this, result.Buffer);
                }
            }
            catch (Exception)
            {
                if (cancelToken.IsCancellationRequested)
                {
                    return;
                }
                else
                {
                    throw;
                }
            }
        });
    }

    public event EventHandler<byte[]> DataReceived = null!;

    public void Dispose()
    {
        _CancelTokenSource.Cancel();

        _UdpClient.Dispose();
    }

    public void Send(byte[] data)
    {
        _UdpClient.Send(data, data.Length, _Endpoint);
    }
}
