using System;

namespace DatecsEcr.Protocol.Datecs
{

    public enum ConnectionType
    { 
        Rs232,
        Socket
    }

    public class DataUpdatedEventArgs
    {
        public byte[] Command { get; private set; }
        public byte[] DataToHost { get; private set; }
        public byte[] DataToPrinter { get; private set; }
        public byte[] Status { get; private set; }
        public string[] DataStringArray { get; private set;}

        public DataUpdatedEventArgs(byte[] command, byte[] data, byte[] toPrinter, byte[] status, string[] dataStringArray = null)
        {
            Command = command;
            DataToHost = data;
            DataToPrinter = toPrinter;
            Status = status;
            DataStringArray = dataStringArray;
        }
    }

    public class ErrorMessagesEventArgs
    {
        public string ErrorMessages { get; private set; }
        public int ErrorCode { get; private set;}

        public ErrorMessagesEventArgs(string error, int errNum = 0)
        {
            ErrorMessages = error;
            ErrorCode = errNum;
        }
    }

    public class GetSynException : Exception
    {
        public string MessageSynException { get; private set; }
        public GetSynException()
        {
            MessageSynException = "Ошибка получения SYN. Перезагрузите регистратор.";
        }
    }

    public class GetNakException : Exception
    {
        public string MessageGetNakException { get; private set; }
        public GetNakException()
        {
            MessageGetNakException = "Последняя команда некорректна. Получен ответ NAK";
        }
    }

    public class AnswerPackageLenException : Exception
    {
        public string MessageAnswerPackageLenException { get; private set; }
        public AnswerPackageLenException()
        {
            MessageAnswerPackageLenException = "Длина ответного пакета не корректна.";
        }
    }

    public enum Commands
    {
        ClearDisplay = 33,
        DisplayText = 35,
        EthernetSettings = 36,
        OpenNonFiscalReceipt = 38,
        CloseNonFiscalReceipt = 39,
        SaveSettingsToFlash = 41,
        NonFiscalPrint = 42,
        PrintSettings = 43,
        PaperTransport = 44,
        PaperCut = 45,
        ShiftDuration = 46,
        DisplayTextUpper = 47,
        OpenFiscalReceipt = 48,
        TaxChangeHistory = 50,
        SubTotalDiscAllow = 51,
        SalesRegister = 52,
        SumTotal = 53,
        FiscalTextPrint = 54,
        PaymentAndCloseRecipt = 55,
        CloseFiscalReceipt = 56,
        CancelFiscalReceipt = 57,
        ArticlesSale = 58,
        GroupTaxDiscount = 59,
        SetDateTime = 61,
        ReceiveDateTime = 62,
        DateTimeDisplay = 63,
        LastFiscalClosureInfo = 64,
        DaylyTurnoverInfo = 65,
        StornoSumInfo = 67,
        FiscalMemoryCapasity = 68,
        DailyReport = 69,
        SericeInOut = 70,
        PrintDiagnostic = 71,
        FiscalizationPersonalization = 72,
        PeriodReportByNumberLong = 73,
        PrinterStatus = 74,
        FiscalTransactionStatus = 76,
        PeriodReportByDateShort = 79,
        Sound = 80,
        TaxSet = 83,
        OpenReturnReceipt = 85,
        AdditionalPaymentType = 87,
        BarQrCodePrint = 88,
        DiagnosticInfo = 90,
        SerialNumberSet = 91,
        FiscalNumberSet = 92,
        DemarcationLinePrint = 93,
        PeriodReportByDateLong = 94,
        PeriodReportByNumberShort = 95,
        CurrentTax = 97,
        PersonalNumberSet = 98,
        CurrentPersonalNumber = 99,
        DisplayFreeText = 100,
        OperatorPasswdSet = 101,
        OperatorNameSet = 102,
        CurrentReceiptInfo = 103,
        OperatorsReport = 105,
        CashDrawerOpen = 106,
        ProgramArticles = 107,
        ReceiptCopy = 109,
        AdditionalInfo = 110,
        ArticlesReport = 111,
        OperatorsInfo = 112,
        LastDocument = 113,
        ReadWriteLogo = 115,
        FiscalMemoryRead = 116,
        AdminPasswdSet = 118,
        OperatorPasswdClear = 119,
        ReceiveDataStatus = 122,
        ControlTape = 126,
        RamReset = 128,
        FmErase = 130,
        FmFormatting = 131,
        PrintDisable = 133,
        FiscalMemoryWrite = 135, // 135,адрес,количество байт(32), байты
        ControlTapeFormatting = 136
    };

}
