namespace SimplePassthrough
{
    public class PassthroughManager : IDisposable
    {
        private readonly IPortWrapper _IncomingPort;
        private readonly IPortWrapper _OutgoingPort;

        public PassthroughManager(IPortWrapper incomingPort, IPortWrapper outgoingPort) 
        {
            _IncomingPort = incomingPort;
            _OutgoingPort = outgoingPort;

            _IncomingPort.DataReceived += IncomingPort_DataReceived;
        }

        private void IncomingPort_DataReceived(object? sender, byte[] data)
        {
            _OutgoingPort.Send(data);
        }

        public void Dispose()
        {
            _IncomingPort.Dispose();
            _OutgoingPort.Dispose();
        }
    }
}
