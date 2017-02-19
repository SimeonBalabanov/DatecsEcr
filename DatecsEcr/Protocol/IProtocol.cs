using System;
using DatecsEcr.FiscalLowLevel;
using DatecsEcr.Protocol.Datecs;

namespace DatecsEcr.Protocol
{
    public delegate void DataUpdateHandler(object sender, DataUpdatedEventArgs e);
    public delegate void StatusErrorHandler(object sender, ErrorMessagesEventArgs e);

    public interface IProtocol
    {
          event DataUpdateHandler DataUpdated;
          event StatusErrorHandler StatusErrorOccured;
          void EventHandlersAddRemove(DataUpdateHandler methodData = null,
                    ErrorMessagesHandler methodErrors = null, StatusErrorHandler methodStatus = null);

          byte[] CommandToPrinter { get; }
          byte[] Status { get; }
          byte[] DataToHost { get; }
          byte[] DataToPrinter { get; }

          string StatusErrorMessage { get; }  
          string Password { get; }

          bool IsOpened();
          bool PortOpen();
          void PortClose();

          byte[] SendCommand(Enum command, params string[] data);
          byte[] CommandVisualization(string comm, string data);
          byte[] ExecuteCommand(string comm, string data);
    }
}
