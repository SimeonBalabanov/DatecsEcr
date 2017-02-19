using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DatecsEcr.Protocol;
using DatecsEcr.Protocol.Datecs;

namespace DatecsEcr.Helper
{
    public enum LogType
    {
        Error,
        Normal
    }

    public enum FiscalizationType
    {
        Fiscalization,
        Refiscalization
    }

    public enum PersonalNumberType
    { 
        PN,
        ID
    }

    public enum PaperWidth
    { 
        Wide,
        Narrow        
    }

    public enum FontSize
    { 
        Normal,
        WithoutSpace,
        Small
    }

    public enum PrintContrast
    { 
        VeryLight = 1,
        Light,
        Normal,
        Dark,
        VeryDark
    }

    public enum DailyReportType
    { 
        ZReport,
        XReport = 2
    };

    public enum NetworkConfig
    {
        StaticIP,
        DHCP,
        EkvAddress,
        MACAddress,
        GRPSConfig,
        TestServer = 9
    }

    public enum BrCdType
    { 
        EAN8 = 1,
        EAN13,
        Code128,
        ITF,
        ITFwCRC,
        Q
    }

    public enum DemarcLineType
    { 
        Type1 = 1,
        Type2,
        Type3
    }

    public enum Country
    { 
        Ukraine = 2
    }

    public enum DocTypeForPrintKlef
    {
        AllReceipt,
        FiscalReceipt,
        ServiceReceipt,
        ServiceOperations,
        XReport,
        ZReport
    }

    public enum FontForKlefPrint
    {
        Small,
        Normal
    }

    public class MHelper
    {
        protected static IProtocol Protocol { get; set; }


        public static string[] SplitBySeparator(byte[] array)
        {
            return GetStringFromByteArray(array).Split(',');
        }

        public static string IntToHexString(int number)
        {
            //Convert.ToString(result, 16); 
            return string.Format("{0:X}", number);
        }

        public static byte[] GetByteArrayFromString(string str)
        {
            return Encoding.Default.GetBytes(str);
        }

        public static string BitConverterToString(byte[] array)
        {
            return BitConverter.ToString(array);
        }

        public static void StatusControlsUpdateHadler(DataUpdateHandler method = null)
        {
            Protocol.DataUpdated += method;
        }

        public static byte StringToByte(string str)
        {
            return Convert.ToByte(str);
        }

        public static byte[] ByteListToArray(List<byte> list)
        {
            return list.ToArray();
        }

        public static string GetStringFromByteArray(byte[] array)
        {
            return Encoding.Default.GetString(array);
            //return Encoding.UTF8.GetString(array);
        }

        public static string GetStringFromByteArray(byte[] array, int index, int count)
        {
            return Encoding.Default.GetString(array, index, count);
        }

        public static string StringUnion(params string[] args)
        {
            return args.Aggregate(String.Empty, (current, t) => current + t);
        }

        public static byte[] StringToByteArray(string hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

        public static string AnswerFromPrinterToStringWithEnter()
        {
            string str = string.Empty;
            for (int i = 0; i < Protocol.DataToHost.Length; i++)
            {
                byte temp = Protocol.DataToHost[i];
                str += temp == 44 ? Environment.NewLine : GetStringFromByteArray(Protocol.DataToHost, i, 1);
            }
            return str;
        }

        public static string UlongToStringWithDecPoint(ulong num)
        {
            if (num.ToString().Length > 1)
            {
                return num.ToString().Insert(num.ToString().Length - 2, ".");
            }
            return num.ToString().Length == 1 ? num.ToString().Insert(num.ToString().Length - 1, ".") : num.ToString();
        }

        public static string GetShortStringFromDateTime(DateTime date)
        {
            return date.ToString().Replace(".", "").Replace("-", "").Replace(":", "").Replace(" ", "").Replace(date.Year.ToString(), date.Year.ToString().Substring(2));
        }

        public static string GetShortStringFromStringDate(string date)
        {
            return date.Replace(".", "").Replace("-", "").Replace(":", "").Replace(" ", "");
        }

        public static string DateTimeToPrinterFormat(DateTime date)
        {
            string result;
            try
            {
                result = date.AddMinutes(-3).ToString().Replace(date.Year.ToString(), date.Year.ToString().Substring(2)).Replace(".", "-");
            }
            catch (ArgumentOutOfRangeException)
            {
                return new DateTime(2001, 01, 01).ToString();
            }
            return result;
        }

        public static byte[] ArrayReverse(byte[] array)
        {
            List<byte> temp = new List<byte>();
            if (array.Length > 1)
            {
                for (int i = array.Length - 1; i >= 0; i--)
                {
                    temp.Add(array[i]);
                }
            }
            else
            {
                temp.Add(array[0]);
            }
            return temp.ToArray();
        }

        public static uint GetIntFromByteArray(byte[] arr)
        {
            return BitConverter.ToUInt32(arr, 0);
        }

        public static ushort GetShortFromByteArray(byte[] arr)
        {
            return BitConverter.ToUInt16(arr, 0);
        }

        public static void WriteLog(string text, LogType type = LogType.Normal)
        {
            StreamWriter SW;
            FileStream FS;
            try
            {
                string filename_log = AppDomain.CurrentDomain.BaseDirectory + "datecs_dll.log";
                string filename_critical_log = AppDomain.CurrentDomain.BaseDirectory + "datecs_dll_err.log";

                switch (type)
                {
                    case LogType.Normal:
                        FS = new FileStream(filename_log, FileMode.Append);
                        break;
                    case LogType.Error:
                        FS = new FileStream(filename_critical_log, FileMode.Append);
                        System.Windows.Forms.MessageBox.Show(text);
                        break;
                    default:
                        FS = new FileStream(filename_log, FileMode.Append);
                        break;
                }
                SW = new StreamWriter(FS, Encoding.GetEncoding("utf-8"));
                SW.WriteLine(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + " === " + text);
                SW.Flush();
                SW.Close();
                FS.Close();
            }
            catch (Exception e)
            {
                string filename_critical_log = AppDomain.CurrentDomain.BaseDirectory + "datecs_dll_err.log";
                FS = new FileStream(filename_critical_log, FileMode.Append);
                SW = new StreamWriter(FS, Encoding.GetEncoding("utf-8"));
                SW.WriteLine(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + " === " + e.Message);
                SW.Flush();
                SW.Close();
                FS.Close();
            }
        }

        public static void ErrorHandler(object sender, ErrorMessagesEventArgs e)
        {
            WriteLog(e.ErrorMessages, LogType.Error);
        }

        public static void AfterDataUpdateHandler(object sender, DataUpdatedEventArgs e)
        {
            WriteLog("Command: " + e.Command[3] + ". Data: " +
                     Encoding.Default.GetString(e.DataToPrinter));
        }

        public static void AfterStatusUpdateHandler(object sender, ErrorMessagesEventArgs e)
        {
            WriteLog(e.ErrorMessages, LogType.Error);
        }

        public static char GetTaxNameFromNumber(int taxGrp)
        {
            char taxName = char.MinValue;
            switch (taxGrp)
            {
                case 1:
                    taxName = 'А';
                    break;
                case 2:
                    taxName = 'Б';
                    break;
                case 3:
                    taxName = 'В';
                    break;
                case 4:
                    taxName = 'Г';
                    break;
                case 5:
                    taxName = 'Д';
                    break;
                case 6:
                    taxName = 'М';
                    break;
                case 7:
                    taxName = 'Н';
                    break;
            }
            return taxName;
        }

        public static char GetPayNameFromNumber(int number)
        {
            char payName = char.MinValue;
            switch (number)
            {
                case 1:
                    payName = 'P';
                    break;
                case 2:
                    payName = 'N';
                    break;
                case 3:
                    payName = 'C';
                    break;
                case 4:
                    payName = 'D';
                    break;
                case 5:
                    payName = 'I';
                    break;
                case 6:
                    payName = 'J';
                    break;
                case 7:
                    payName = 'K';
                    break;
                case 8:
                    payName = 'L';
                    break;
            }
            return payName;
        }
    }


}
