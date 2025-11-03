using System.Text;

namespace SimplePassthrough
{
    public class PassthroughManager : IDisposable
    {
        private readonly IPortWrapper _IncomingPort;
        private readonly IPortWrapper _OutgoingPort;
        private readonly Action<string> _LogData;

        public PassthroughManager(IPortWrapper incomingPort, IPortWrapper outgoingPort, Action<string> logData) 
        {
            _IncomingPort = incomingPort;
            _OutgoingPort = outgoingPort;
            _LogData = logData;
            _IncomingPort.DataReceived += IncomingPort_DataReceived;
        }

        private void IncomingPort_DataReceived(object? sender, byte[] data)
        {
            var dataString = Encoding.UTF8.GetString(data);

            _LogData(dataString);

            _OutgoingPort.Send(data);
        }

        public void Dispose()
        {
            _IncomingPort.Dispose();
            _OutgoingPort.Dispose();
        }
    }
}
