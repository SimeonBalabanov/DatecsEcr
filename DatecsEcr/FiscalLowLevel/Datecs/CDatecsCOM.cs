using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using DatecsEcr.Helper;
using DatecsEcr.Protocol.Datecs;

namespace DatecsEcr.FiscalLowLevel.Datecs
{
    public class CDatecsCom : IPrinterPort, IDisposable
    {
        public event ErrorMessagesHandler ErrorOccurrence;

        public byte[] PortAnswer { get; private set; }
        public string ErrorMessage { get; private set; }

        private SerialPort _port;

        private const byte Syn = 0x16;
        private const byte Nak = 0x15;
        private const byte Terminator = 0x03;
        private const byte NullByte = 0x00;
        private const int GetSynExceptionCount = 2000;
        private const int SynRecevingStart = 20;
        private const byte PackageLenghtWithoutCmdDataEnq = 0x1A;

        public CDatecsCom(int port, int baudRate)
        {
            _port = new SerialPort("COM" + port, baudRate, Parity.None, 8, StopBits.One)
            {
                Encoding = Encoding.Default,
                WriteTimeout = 1000,
                ReadTimeout = 1000
            };
        }

        public void ClearSubscribers()
        {
            ErrorOccurrence = null;
        }

        public bool PortOpen()
        {
            try
            {
                _port.Open();
                ErrorOccurenceEventGenerated(string.Empty, 0);
            }
            catch (UnauthorizedAccessException ex)
            {
                ErrorOccurenceEventGenerated(ex.Message, 1);
                return false;
            }
            catch (InvalidOperationException ex)
            {
                ErrorOccurenceEventGenerated(ex.Message, 1);
                return false;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                ErrorOccurenceEventGenerated(ex.Message, 1);
                return false;
            }
            catch (ArgumentException ex)
            {
                ErrorOccurenceEventGenerated(ex.Message, 1);
                return false;
            }
            catch (IOException ex)
            {
                ErrorOccurenceEventGenerated(ex.Message, 1);
                return false;
            }
            return true;
        }

        public bool IsOpened()
        {
            return _port.IsOpen;
        }

        public void PortClose()
        {
            _port.Close();
            Dispose();
        }

        public void Dispose()
        {
            _port.Dispose();
        }
        
        public bool PortReadWrite(byte[] command)
        {
            return WritePort(command) && ReadPort();
        }

        public bool WritePort(byte[] command)
        {
            try
            {
                _port.Write(command, 0, command.Length);
            }
            catch (TimeoutException ex)
            {
                ErrorOccurenceEventGenerated(ex.Message, 100);
                return false;
            }
            catch (InvalidOperationException ex)
            {
                ErrorOccurenceEventGenerated(ex.Message, 1);
                return false;
            }
            catch (ArgumentNullException ex)
            {
                ErrorOccurenceEventGenerated(ex.Message, 1);
                return false;
            }
            return true;
        }

        private bool ReadPort()
        {
            string str = string.Empty;
            char temp = (char)Syn;

            if (!ReadAnswerFirstByte(ref temp))
            {
                return false;
            }
            str += temp;
            str += _port.ReadExisting();
            byte[] ans = MHelper.GetByteArrayFromString(str);
            while (ans[ans.Length - 1] != Terminator)
            {
                str += _port.ReadExisting();
                ans = MHelper.GetByteArrayFromString(str);
            }
            if (!AnswerLenghtCount(ans)) return false;
            PortAnswer = ans;
            return true;
        }

        private bool ReadAnswerFirstByte(ref char result)
        {
            int tempSyn = 0;
                try
                {
                    while (result == Syn)
                    {
                        result = (char)_port.ReadByte();
                        
                        if (tempSyn > GetSynExceptionCount)
                        {
                            throw new GetSynException();
                        }
                        if (tempSyn > SynRecevingStart)
                        {
                            ErrorOccurenceEventGenerated("Received Syn");
                        }
                        if (result == Nak)
                        {
                            throw new GetNakException();
                        }
                        if (result == NullByte)
                        {
                            throw new AnswerPackageLenException();
                        }
                        tempSyn++;
                    }
                }
                catch (TimeoutException ex)
                {
                    ErrorOccurenceEventGenerated(ex.Message, 100);
                    return false;
                }
                catch (InvalidOperationException ex)
                {
                    ErrorOccurenceEventGenerated(ex.Message, 1);
                    return false;
                }
                catch (GetSynException ex)
                {
                    ErrorOccurenceEventGenerated(ex.MessageSynException, 1);
                    return false;
                }
                catch (GetNakException ex)
                {
                    ErrorOccurenceEventGenerated(ex.MessageGetNakException, 1);
                    return false;
                }
                catch (AnswerPackageLenException ex)
                {
                    ErrorOccurenceEventGenerated(ex.MessageAnswerPackageLenException, 1);
                    return false;
                }
                return true;
        }

        private bool AnswerLenghtCount(byte[] package)
        {
            try
            {
                if (package[1] != package.Length + PackageLenghtWithoutCmdDataEnq)
                {
                    throw new AnswerPackageLenException();
                }
                return true;
            }
            catch (AnswerPackageLenException ex)
            {
                ErrorOccurenceEventGenerated(ex.MessageAnswerPackageLenException);
                return false;
            }
        }

        private void ErrorOccurenceEventGenerated(string error, int errNum = 0)
        {
            ErrorMessage = error;
            if (ErrorOccurrence != null)
            {
                ErrorOccurrence(this, new ErrorMessagesEventArgs(error, errNum));
            }
        }

        ~CDatecsCom()
        {
            Dispose();
            _port = null;
        }
        
    }
}
