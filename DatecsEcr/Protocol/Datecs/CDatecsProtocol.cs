using System;
using System.Collections.Generic;
using System.Linq;
using DatecsEcr.FiscalLowLevel;
using DatecsEcr.FiscalLowLevel.Datecs;
using DatecsEcr.Helper;


namespace DatecsEcr.Protocol.Datecs
{

    public class Datecs : IProtocol
    {
        public event DataUpdateHandler DataUpdated;
        public event StatusErrorHandler StatusErrorOccured;

        public byte[] DataToHost { get; set; }

        public byte[] CommandToPrinter { get; set; }
        public byte[] DataToPrinter { get; private set; }
        public string StatusErrorMessage { get; private set; }
        public byte[] PortAnswer { get; set; }
        public byte[] Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                CheckStatus();
            }
        }
        public string Password
        {
            get
            {
                return @"000000";
            }
        }

        private readonly IPrinterPort _port;
        private byte[] _status;
        private bool StatusErrorFlag { get; set; }
        private static string Separator 
        {
            get
            {
                return @",";
            }
        }
        private const byte Preamb = 0x01;
        private const byte Enquiry = 0x05;
        private const byte Postamb = 0x03;
        private const byte MinSeq = 0x20;
        private const byte MaxSeq = 0x7F;
        private readonly byte[] _emptyByteArray = { 0x30, 0x30, 0x30, 0x30, 0x30, 0x30 };
        private const byte EndOfTransmission = 0x04;
        private byte _seq;
        private readonly Random _nextValue = new Random();

        public static IProtocol GetDatecsPrinterPort(int portName, int baudRate = 0)
        {
            return new Datecs(new CDatecsCom(portName, baudRate));
        }

        public void EventHandlersAddRemove(DataUpdateHandler methodData,
                    ErrorMessagesHandler methodErrors, StatusErrorHandler methodStatus)
        {
            if (methodData == null)
            {
                DataUpdated = null;
            }
            else
            {
                DataUpdated += methodData;
            }
            if (methodErrors == null)
            {
                _port.ClearSubscribers();
            }
            else
            {
                _port.ErrorOccurrence += methodErrors;
            }
            if (methodStatus == null)
            {
                StatusErrorOccured = null;
            }
            else
            {
                StatusErrorOccured += methodStatus;
            }
        }

        public bool PortOpen()
        {
            return _port.PortOpen();
        }

        public void PortClose()
        {
            _port.PortClose();
        }

        public bool IsOpened()
        {
            return _port.IsOpened();
        }

        public byte[] SendCommand(Enum command, params string[] data)
        {
            string dataToPrinter = string.Empty;
            for (int i = 0; i < data.Length; i++)
            {
                dataToPrinter += i == data.Length - 1 ? data[i] : data[i] + Separator;
            }
            return PortReadWrite(CommandFormation(((int)(Commands)command).ToString(), dataToPrinter))
                                                ? DataToHost : _emptyByteArray;
        }

        public byte[] CommandVisualization(string command, string data)
        {
            return CommandFormation(command, data);
        }

        public byte[] ExecuteCommand(string command, string data)
        {
            if (command == string.Empty) { command = ((int)Commands.DiagnosticInfo).ToString(); data = string.Empty; }
            PortReadWrite(CommandFormation(command, data));
            return DataToHost;
        }

        private Datecs(IPrinterPort port)
        {
            _port = port;
            _seq = (byte)_nextValue.Next(MinSeq, MaxSeq);
            Status = _emptyByteArray;
            DataToHost = _emptyByteArray;
            PortAnswer = _emptyByteArray;
            DataToPrinter = _emptyByteArray;
        }

        private void CheckStatus()
        {
            for (int i = 0; i < 6; i++)
            {
               
                if (i == 0)
                {
                    byte byteTemp = _status[i];
                    byteTemp = (byte)(byteTemp >> 5);
                    if ((byte)(byteTemp & 0x01) == 1)
                    {
                        StatusErrorMessage += "Общая ошибка (или ошибки с #). " + Environment.NewLine;
                        StatusErrorFlag = true;
                        StatusErrorOccurenceEventGenerate("Общая ошибка (или ошибки с #). ");
                    }
                    byteTemp = _status[i];
                    byteTemp = (byte)(byteTemp >> 4);
                    if ((byte)(byteTemp & 0x01) == 1)
                    {
                        StatusErrorMessage += "# Неправильный SAM. " + Environment.NewLine;
                        StatusErrorFlag = true;
                        StatusErrorOccurenceEventGenerate("# Неправильный SAM. ", 38);
                    }
                    byteTemp = _status[i];
                    if ((byte)(byteTemp & 0x01) == 1)
                    {
                        StatusErrorMessage += "# Синтаксическая ошибка. " + Environment.NewLine;
                        StatusErrorFlag = true;
                        StatusErrorOccurenceEventGenerate("# Синтаксическая ошибка. ", 36);
                    }
                    byteTemp = _status[i];
                    byteTemp = (byte)(byteTemp >> 1);
                    if ((byte)(byteTemp & 0x01) == 1)
                    {
                        StatusErrorMessage += "# Недопустимая команда. " + Environment.NewLine;
                        StatusErrorFlag = true;
                        StatusErrorOccurenceEventGenerate("# Недопустимая команда. ", 37);
                       
                    }
                    byteTemp = _status[i];
                    byteTemp = (byte)(byteTemp >> 2);
                    if ((byte)(byteTemp & 0x01) == 1)
                    {
                        StatusErrorMessage += "Не установлены дата/время. " + Environment.NewLine;
                        StatusErrorFlag = true;
                        StatusErrorOccurenceEventGenerate("Не установлены дата/время. ", 10);
                    } 
                }
                if (i == 1)
                {
                    byte byteTemp = _status[i];
                    byteTemp = (byte)(byteTemp >> 5);
                    if ((byte)(byteTemp & 0x01) == 1)
                    {
                        StatusErrorMessage += "Открыта крышка принтера. " + Environment.NewLine;
                        StatusErrorFlag = true;
                        StatusErrorOccurenceEventGenerate("Открыта крышка принтера. ");
                    }
                    byteTemp = _status[i];
                    byteTemp = (byte)(byteTemp >> 4);
                    if ((byte)(byteTemp & 0x01) == 1)
                    {
                        StatusErrorMessage += "# Ошибка SAM. " + Environment.NewLine;
                        StatusErrorFlag = true;
                        StatusErrorOccurenceEventGenerate("# Ошибка SAM. ");
                    }
                    byteTemp = _status[i];
                    byteTemp = (byte)(byteTemp >> 2);
                    if ((byte)(byteTemp & 0x01) == 1)
                    {
                        StatusErrorMessage += "# Аварийное обнуление RAM. " + Environment.NewLine;
                        StatusErrorFlag = true;
                        StatusErrorOccurenceEventGenerate("# Аварийное обнуление RAM. ");
                    }
                    byteTemp = _status[i];
                    byteTemp = (byte)(byteTemp >> 1);
                    if ((byte)(byteTemp & 0x01) == 1)
                    {
                        StatusErrorMessage += "# Команда не может быть выполнена. " + Environment.NewLine;
                        StatusErrorFlag = true;
                        StatusErrorOccurenceEventGenerate("# Команда не может быть выполнена. ", 40);
                    } 
                }
                if (i == 2)
                {
                    byte byteTemp = _status[i];
                    byteTemp = (byte)(byteTemp >> 2);
                    if ((byte)(byteTemp & 0x01) == 1)
                    {
                        StatusErrorMessage += "КЛЕФ заполнена(блокировка). " + Environment.NewLine;
                        StatusErrorFlag = true;
                        StatusErrorOccurenceEventGenerate("КЛЕФ заполнена(блокировка). ");
                    }
                    byteTemp = _status[i];
                    if ((byte)(byteTemp & 0x01) == 1)
                    {
                        StatusErrorMessage += "# Закончилась лента. " + Environment.NewLine;
                        StatusErrorFlag = true;
                        StatusErrorOccurenceEventGenerate("# Закончилась лента. ", 12);
                    }
                }
                if (i == 4)
                {
                    byte byteTemp = _status[i];
                    byteTemp = (byte)(byteTemp >> 5);
                    if ((byte)(byteTemp & 0x01) == 1)
                    {
                        StatusErrorMessage += "Ошибки с пометкой *. " + Environment.NewLine;
                        StatusErrorFlag = true;
                        StatusErrorOccurenceEventGenerate("Ошибки с пометкой *. ");
                    }
                    byteTemp = _status[i];
                    byteTemp = (byte)(byteTemp >> 4);
                    if ((byte)(byteTemp & 0x01) == 1)
                    {
                        StatusErrorMessage += "* Заполнение фискальной памяти. " + Environment.NewLine;
                        StatusErrorFlag = true;
                        StatusErrorOccurenceEventGenerate("* Заполнение фискальной памяти. ");
                    }
                    byteTemp = _status[i];
                    byteTemp = (byte)(byteTemp >> 1);
                    if ((byte)(byteTemp & 0x01) == 1)
                    {
                        StatusErrorMessage += "Неработоспособная ФП. " + Environment.NewLine;
                        StatusErrorFlag = true;
                        StatusErrorOccurenceEventGenerate("Неработоспособная ФП. ");
                    }
                    byteTemp = _status[i];
                    if ((byte)(byteTemp & 0x01) == 1)
                    {
                        StatusErrorMessage += "* Ошибки в ФП. " + Environment.NewLine;
                        StatusErrorFlag = true;
                        StatusErrorOccurenceEventGenerate("* Ошибки в ФП. ");
                    }
                }
                if (i == 5)
                {
                    byte byteTemp = _status[i];
                    byteTemp = (byte)(byteTemp >> 2);
                    if ((byte)(byteTemp & 0x01) == 1)
                    {
                        StatusErrorMessage += "Последняя запись в ФП неудачна. " + Environment.NewLine;
                        StatusErrorFlag = true;
                        StatusErrorOccurenceEventGenerate("Последняя запись в ФП неудачна. ");
                    }
                    byteTemp = _status[i];
                    if ((byte)(byteTemp & 0x01) == 1)
                    {
                        StatusErrorMessage += "* ФП в режиме READONLY. " + Environment.NewLine;
                        StatusErrorFlag = true;
                        StatusErrorOccurenceEventGenerate("* ФП в режиме READONLY. ");
                    }
                }
            }
            StatusErrorFlag = false;
            StatusErrorMessage = string.Empty;
        }

        private bool PortReadWrite(byte[] comm)
        {
            if (_port != null && _port.PortReadWrite(comm))
            {
                PortAnswer = _port.PortAnswer;
                DataUpdateEventGenerate();
                return true;
            }
            StatusErrorOccurenceEventGenerate("Error");
            return false;
        }

        private void StatusErrorOccurenceEventGenerate(string errorMesText, int errorCode = 0)
        {
            if (StatusErrorOccured != null && _port == null)
            {
                StatusErrorOccured(this, new ErrorMessagesEventArgs("Порт не существует."));
            }
            if (StatusErrorOccured != null)
            {
                StatusErrorOccured(this, new ErrorMessagesEventArgs(errorMesText, errorCode));
            }
        }

        private void DataUpdateEventGenerate()
        {
            RefreshData();
            if (DataUpdated != null)
            {
                DataUpdated(this, new DataUpdatedEventArgs(CommandToPrinter, DataToHost, DataToPrinter, Status, GetStringArrayFromDataPrinter(DataToHost)));
            }
        }

        private string[] GetStringArrayFromDataPrinter(byte[] dataToHost)
        {
            return MHelper.SplitBySeparator(dataToHost);
        }

        private static byte[] CsCount(byte[] array)
        {
            byte[] checkSum = new byte[4];
            ushort bcc = 0x00;
            foreach (byte item in array.Where(item => item != Preamb))
            {
                if (item == Enquiry) { bcc += item; break; }
                bcc += item;
            }
            for (int j = 0; j <= 3; j++)
            {
                int val = bcc & 0x0F;
                val += 0x30;
                bcc = (ushort)(bcc >> 4);
                checkSum[3 - j] = (byte)val;
            }
            return checkSum;
        }

        private byte[] CommandFormation(string command, string data)
        {
            DataToPrinter = MHelper.GetByteArrayFromString(data);
            _seq++;
            if (_seq == MaxSeq) { _seq = MinSeq; }
            List<byte> commToPrinter = new List<byte>();
            byte leng = (byte)(0x04 + 0x20 + data.Length);
            commToPrinter.Add(Preamb);
            commToPrinter.Add(leng);
            commToPrinter.Add(_seq);
            commToPrinter.Add(MHelper.StringToByte(command));
            byte[] tempByte = MHelper.GetByteArrayFromString(data);
            commToPrinter.AddRange(tempByte);
            commToPrinter.Add(Enquiry);
            tempByte = CsCount(MHelper.ByteListToArray(commToPrinter));
            commToPrinter.AddRange(tempByte);
            commToPrinter.Add(Postamb);
            CommandToPrinter = MHelper.ByteListToArray(commToPrinter);

            return CommandToPrinter;
        }

        private static byte[] CurrentStatus(byte[] array)
        {
            List<byte> status = new List<byte>();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == EndOfTransmission)
                {
                    for (int j = i + 1; j < i + 7; j++)
                    {
                        status.Add(array[j]);
                    }
                    break;
                }
            }
            return status.ToArray();
        }

        private static byte[] DataFromPrinter(byte[] array)
        {
            List<byte> data = new List<byte>();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == Preamb)
                {
                    for (int j = i + 4; j < array.Length; j++)
                    {
                        if (array[j] != EndOfTransmission) { data.Add(array[j]); }
                        else { break; }
                    }
                    break;
                }
            }
            return data.ToArray();
        }

        private void RefreshData()
        {
            if (PortAnswer != null)
            {
                DataToHost = DataFromPrinter(PortAnswer);
                Status = CurrentStatus(PortAnswer);
            }
            else
            {
                DataToHost = MHelper.GetByteArrayFromString("F,F,F,F,F,F");
                Status = _emptyByteArray;
            }
        }

    }
}
