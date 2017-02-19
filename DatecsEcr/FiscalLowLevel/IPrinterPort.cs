

using DatecsEcr.Protocol.Datecs;

namespace DatecsEcr.FiscalLowLevel
{
    public delegate void ErrorMessagesHandler(object sender, ErrorMessagesEventArgs e);

    public interface IPrinterPort
    {
        event ErrorMessagesHandler ErrorOccurrence;

        byte[] PortAnswer { get; }

        bool PortReadWrite(byte[] comm);

        bool PortOpen();

        void PortClose();

        bool IsOpened();

        void ClearSubscribers();

        string ErrorMessage { get; }
    }
}
