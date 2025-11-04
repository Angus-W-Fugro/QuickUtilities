using System.IO.Ports;

namespace SimplePassthrough;

public interface IPortWrapper : IDisposable
{
    event EventHandler<byte[]> DataReceived;
    void Send(byte[] data);
}

public class SerialPortWrapper : IPortWrapper
{
    private readonly SerialPort _SerialPort;

    public SerialPortWrapper(string port)
    {
        _SerialPort = new SerialPort(port);
        _SerialPort.Open();

        _SerialPort.DataReceived += _SerialPort_DataReceived;

#if DEBUG
        StartFCPPoll();
#endif
    }

    private void _SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        var bytesAvailable = _SerialPort.BytesToRead;
        var buffer = new byte[bytesAvailable];
        _SerialPort.Read(buffer, 0, bytesAvailable);
        DataReceived?.Invoke(this, buffer);
    }

    public event EventHandler<byte[]> DataReceived = null!;

    public void Dispose()
    {
        _SerialPort.DataReceived -= _SerialPort_DataReceived;
        _SerialPort.Dispose();
    }

    public void Send(byte[] data)
    {
        _SerialPort.Write(data, 0, data.Length);
    }

    private void StartFCPPoll()
    {
        Task.Run(async () =>
        {
            var pollCommand = "#01\r";

            while (true && _SerialPort.IsOpen)
            {
                _SerialPort.Write(pollCommand);

                await Task.Delay(1000);
            }
        });
    }
}
