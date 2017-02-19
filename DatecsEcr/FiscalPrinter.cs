﻿using System;
using DatecsEcr.Protocol;
using DatecsEcr.Protocol.Datecs;
using DatecsEcr.Helper;
using System.Runtime.InteropServices;
using System.Text;
using DatecsEcr.FiscalLowLevel;

namespace DatecsEcr
{
    [Guid("C0482EEF-6640-49BD-A0D3-D0AF47FC8EF3"), ClassInterface(ClassInterfaceType.None), ComSourceInterfaces(typeof(IEvents))]
    public class FiscalPrinter : IEcrClass
    {

        public string s1;
        public string s2;
        public string s3;
        public string s4;
        public string s5;
        public string s6;
        public string s7;
        public string s8;
        public string s9;
        public string s10;
        public short LastError;
        public string LastErrorText;
        private IProtocol _datecsPort;

        public FiscalPrinter()
        {
            s1 = string.Empty;
            s2 = string.Empty;
            s3 = string.Empty;
            s4 = string.Empty;
            s5 = string.Empty;
            s6 = string.Empty;
            s7 = string.Empty;
            s8 = string.Empty;
            s9 = string.Empty;
            s10 = string.Empty;
            LastError = 0;
            LastErrorText = string.Empty;
        }

        public void OpenPort(int portNum, int baudRate)
        {
            _datecsPort = Datecs.GetDatecsPrinterPort(portNum, baudRate);
            if (_datecsPort.PortOpen())
            {
                MHelper.WriteLog("Port COM" + portNum + " is opened");
                _datecsPort.EventHandlersAddRemove(MHelper.AfterDataUpdateHandler,
                    MHelper.ErrorHandler, MHelper.AfterStatusUpdateHandler);
            }
            else
            {
                ErrorPropertiesSet(1, "Невозможно открыть СОМ порт");
                MHelper.WriteLog("Error open port COM" + portNum, LogType.Error);
            }
        }

        public void ClosePort()
        {
            _datecsPort.EventHandlersAddRemove();
            _datecsPort.PortClose();
            MHelper.WriteLog("Port is closed");
        }

        public void SaveSettings()
        {
            _datecsPort.SendCommand(Commands.SaveSettingsToFlash);
            MHelper.WriteLog("SaveSettings()");
        }

        public void SetBarcodeHeight(int value)
        {
            if (value >= 24 && value <= 240)
            {
                _datecsPort.SendCommand(Commands.PrintSettings, "B" + value);
                MHelper.WriteLog("SetBarcodeHeight(int value) value = " + value);
            }
            else
            {
                _datecsPort.SendCommand(Commands.PrintSettings, "B240");
                MHelper.WriteLog("SetBarcodeHeight(int value) incorrect value. Set = 240", LogType.Error);
            }
        }

        public void SetPrintDensity(int value)
        {
            if (value >= 1 && value <= 5)
            {
                _datecsPort.SendCommand(Commands.PrintSettings, "D3");
                MHelper.WriteLog("SetPrintDensity(int value) value = " + value);
            }
            else
            {
                _datecsPort.SendCommand(Commands.PrintSettings, "D" + value);
                MHelper.WriteLog("SetPrintDensity(int value) incorrect value. Set = 3 ", LogType.Error);
            }
        }

        public void EnableAutoOpenDrawer(bool enabled)
        {
            _datecsPort.SendCommand(Commands.PrintSettings, enabled ? "X0" : "X1");
            MHelper.WriteLog("EnableAutoOpenDrawer(bool enabled)  = " + enabled);
        }

        public void SetTaxName(int tax, string name)
        {
            string paymentType;
            switch (tax)
            {
                case 1:
                    paymentType = "I";
                    break;
                case 2:
                    paymentType = "J";
                    break;
                case 3:
                    paymentType = "K";
                    break;
                case 4:
                    paymentType = "L";
                    break;
                default:
                    paymentType = "I";
                    break;
            }
            _datecsPort.SendCommand(Commands.AdditionalPaymentType, paymentType, name);
            MHelper.WriteLog("SetTaxName(int tax, string name). Payment type = " + tax + ". Payment name = " + name);
        }

        public void SetHeaderFooter(int line, string text)
        {
            _datecsPort.SendCommand(Commands.PrintSettings, line - 1 + text);
            MHelper.WriteLog("SetHeaderFooter(int line, string text) " + (line - 1) + " set");
        }

        public void GetHeader(int line)
        {
            _datecsPort.SendCommand(Commands.PrintSettings, "I" + (line - 1));
            s1 = MHelper.GetStringFromByteArray(_datecsPort.DataToHost);
            MHelper.WriteLog("GetHeader(int line) - " + (line - 1) + " - " +  s1);
        }

        public void EnableCutCheck(bool enabled)
        {
            _datecsPort.SendCommand(Commands.PrintSettings, enabled ? "C1" : "C0");
            MHelper.WriteLog("EnableCutCheck(bool enabled) is - " + enabled);
        }

        public void EnableSmallFont(bool enabled)
        {
            _datecsPort.SendCommand(Commands.PrintSettings, enabled ? "F1" : "F0");
            MHelper.WriteLog("EnableSmallFont(bool enabled) is - " + enabled);
        }

        public void EnableLogo(bool enabled)
        {
            _datecsPort.SendCommand(Commands.PrintSettings, enabled ? "L0" : "L1");
            MHelper.WriteLog("EnableLogo(bool enable) is " + enabled);
        }

        public void SetDateTime(string date, string time)
        {
            _datecsPort.SendCommand(Commands.SetDateTime, date + " " + time);
            MHelper.WriteLog("SetDateTime(string date, string time) " + date + " " + time);
        }

        public void Fiscalise(string passwd, string serial, string taxnum, int taxNumType)
        {
            _datecsPort.SendCommand(Commands.FiscalizationPersonalization, passwd, serial, taxnum, taxNumType.ToString());
            MHelper.WriteLog("Fiscalise(string passwd, string serial, string taxnum, int taxNumType)." + serial + " " +
                             taxnum + " " + taxNumType);
        }

        public void SetMulDecCurRF(string passwd, int dec, string enableTax, double taxA, double taxB, double taxC, double taxD)
        {
            _datecsPort.SendCommand(Commands.TaxSet, passwd, dec.ToString(), enableTax, taxA.ToString(), taxB.ToString(),
                taxC.ToString(), taxD.ToString());
            MHelper.WriteLog("SetMulDecCurRF(string " + passwd + "," + "int " + dec + ",string" + enableTax + ", double" +
                             taxA + ", double" + taxB + ", double" + taxC + ", double" + taxD);
        }

        public void SetTaxType(int type)
        {
        }

        public void SetSerialNumber(string serial)
        {
            _datecsPort.SendCommand(Commands.SerialNumberSet, "2", serial);
            MHelper.WriteLog("SetSerialNumber(string serial) is " + serial);
        }

        public void SetFiscalNumber(string fNumber)
        {
            _datecsPort.SendCommand(Commands.FiscalNumberSet, fNumber);
            MHelper.WriteLog("SetFiscalNumber(string fNumber) is " + fNumber);
        }

        public void SetTaxNumber(string taxNum, int type)
        {
            _datecsPort.SendCommand(Commands.PersonalNumberSet, taxNum, type.ToString());
            MHelper.WriteLog("SetTaxNumber(string taxNum, int type) is " + taxNum);
        }

        public void SetOperatorPassword(int opNum, string oldPasswd, string newPasswd)
        {
            _datecsPort.SendCommand(Commands.OperatorPasswdSet, opNum.ToString(), oldPasswd, newPasswd);
            MHelper.WriteLog("SetOperatorPassword(int opNum, string oldPasswd, string newPasswd). " + opNum + " " +
                             oldPasswd + " " + newPasswd);
        }

        public void SetOperatorName(int opNum, string passwd, string name)
        {
            _datecsPort.SendCommand(Commands.OperatorNameSet, opNum.ToString(), passwd, name);
            MHelper.WriteLog("SetOperatorName(int opNum, string passwd, string name). " + opNum + " " +
                             passwd + " " + name);
        }

        public void ClearOperator(int opNum, string passwd)
        {
            _datecsPort.SendCommand(Commands.PersonalNumberSet, opNum.ToString(), passwd);
            MHelper.WriteLog("ClearOperator(int opNum, string passwd) " + opNum);
        }

        public void SetAdminPassword(string oldPass, string newPass)
        {
            _datecsPort.SendCommand(Commands.AdminPasswdSet, oldPass, newPass);
            MHelper.WriteLog("SetAdminPassword(string oldPass, string newPass). " + oldPass + " " + newPass);
        }

        public void ClearOperatorPassword(int opNum, string admPasswd)
        {
            _datecsPort.SendCommand(Commands.AdminPasswdSet, opNum.ToString(), admPasswd);
            MHelper.WriteLog("ClearOperatorPassword(int opNum, string admPasswd). " + opNum);
        }

        public void GetArticlesInfo()
        {
            _datecsPort.SendCommand(Commands.ProgramArticles, "I");
            MHelper.WriteLog("GetArticlesInfo() ");
        }

        public void SetArticle(int artNum, int taxGrp, int grp, double price, string passwd, string name)
        {
            char taxName = MHelper.GetTaxNameFromNumber(taxGrp);
            _datecsPort.SendCommand(Commands.ProgramArticles, "P" + taxName + artNum,  grp.ToString(), price.ToString(), passwd, name);
            MHelper.WriteLog("SetArticle(int artNum, int taxGrp, int grp, double price, string passwd, string name)");
        }

        public void DelArticle(string passwd, int artNum)
        {
            _datecsPort.SendCommand(Commands.ProgramArticles, "D" + (artNum == 0 ? "A," + passwd : artNum + "," + passwd));
            MHelper.WriteLog("DelArticle(string passwd, int artNum) " + artNum);
        }

        public void GetArticle(int artNum)
        {
            _datecsPort.SendCommand(Commands.ProgramArticles, "R" + artNum);
            MHelper.WriteLog("GetArticle(int artNum) " + artNum);
        }

        public void GetFirstArticle()
        {
            _datecsPort.SendCommand(Commands.ProgramArticles, "F");
            MHelper.WriteLog("GetFirstArticle()");
        }

        public void GetNextArticle()
        {
            _datecsPort.SendCommand(Commands.ProgramArticles, "N");
            MHelper.WriteLog("GetNextArticle()");
        }

        public void ChangeArticlePrice(string passwd, int artNum, double price)
        {
            _datecsPort.SendCommand(Commands.ProgramArticles, "C" + artNum, price.ToString(), passwd);
            MHelper.WriteLog("ChangeArticlePrice(string passwd, int artNum, double price) is " + artNum + " . Price is " + price);
        }

        public void GetFirstFreeArticle()
        {
            _datecsPort.SendCommand(Commands.ProgramArticles, "X");
            MHelper.WriteLog("GetFirstFreeArticle()");
        }

        public void GetLastFreeArticle()
        {
            _datecsPort.SendCommand(Commands.ProgramArticles, "x");
            MHelper.WriteLog("GetLastFreeArticle()");
        }

        public void OpenNonfiscalReceipt()
        {
            _datecsPort.SendCommand(Commands.OpenNonFiscalReceipt);
            MHelper.WriteLog("OpenNonfiscalReceipt()");
        }

        public void PrintNonfiscalText(string text)
        {
            _datecsPort.SendCommand(Commands.NonFiscalPrint, text);
            MHelper.WriteLog("PrintNonfiscalText(string text) - " + text);
        }

        public void CloseNonFiscalReceipt()
        {
            _datecsPort.SendCommand(Commands.CloseNonFiscalReceipt);
            MHelper.WriteLog("CloseNnFiscalReceipt()");
        }

        public void OpenFiscalReceipt(int operNum, string passwd, int cashDeskNum)
        {
            _datecsPort.SendCommand(Commands.OpenFiscalReceipt, operNum.ToString(), passwd, cashDeskNum.ToString(), "I");
            MHelper.WriteLog("OpenFiscalReceipt(int operNum, string passwd, int cashDeskNum)");
        }

        public void RegistrItem(int artNum, double quantity, double percDisc, double sumDisc)
        {
            if (percDisc == 0 && sumDisc == 0)
            {
                _datecsPort.SendCommand(Commands.SalesRegister, "+" + artNum + "*" + quantity);
                MHelper.WriteLog("RegistrItem(int artNum, double quantity, double percDisc, double sumDisc). PLU -  " +
                                 artNum + ". Quantity - " + quantity);
            }
            else if (percDisc != 0 && sumDisc == 0)
            {
                _datecsPort.SendCommand(Commands.SalesRegister, "+" + artNum + "*" + quantity, percDisc.ToString());
                MHelper.WriteLog("RegistrItem(int artNum, double quantity, double percDisc, double sumDisc). PLU -  " +
                                 artNum + ". Quantity - " + quantity + ". Perc discount - " + percDisc);
            }
            else if (percDisc == 0 && sumDisc != 0)
            {
                _datecsPort.SendCommand(Commands.SalesRegister, "+" + artNum + "*" + quantity + ";" + sumDisc);
                MHelper.WriteLog("RegistrItem(int artNum, double quantity, double percDisc, double sumDisc). PLU -  " +
                                 artNum + ". Quantity - " + quantity + ". Sum discount - " + sumDisc);
            }
            else
            {
                MHelper.WriteLog("Error add item sum discount and abs discount not 0.00", LogType.Error);
            }
        }

        public void RegistrItemEx(int artNum, double quantity, double price, double percDisc, double sumDisc)
        {
            if (percDisc == 0 && sumDisc == 0)
            {
                _datecsPort.SendCommand(Commands.SalesRegister, "+" + artNum + "*" + quantity + "#" + price);
                MHelper.WriteLog("RegistrItemEx(int artNum, double quantity, double price, double percDisc, double sumDisc). PLU -  " +
                                 artNum + ". Quantity - " + quantity);
            }
            else if (percDisc != 0 && sumDisc == 0)
            {
                _datecsPort.SendCommand(Commands.SalesRegister, "+" + artNum + "*" + quantity + "#" + price, percDisc.ToString());
                MHelper.WriteLog("RegistrItemEx(int artNum, double quantity, double price, double percDisc, double sumDisc). PLU -  " +
                                 artNum + ". Quantity - " + quantity + ". Perc discount - " + percDisc);
            }
            else if (percDisc == 0 && sumDisc != 0)
            {
                _datecsPort.SendCommand(Commands.SalesRegister, "+" + artNum + "*" + quantity + "#" + price + ";" + sumDisc);
                MHelper.WriteLog("RegistrItemEx(int artNum, double quantity, double price, double percDisc, double sumDisc). PLU -  " +
                                 artNum + ". Quantity - " + quantity + ". Sum discount - " + sumDisc);
            }
            else
            {
                MHelper.WriteLog("Error add item sum discount and abs discount not 0.00", LogType.Error);
            }
        }

        public void RegistrAndDisplayItem(int artNum, double quantity, double percDisc, double sumDisc)
        {
            if (percDisc == 0 && sumDisc == 0)
            {
                _datecsPort.SendCommand(Commands.ArticlesSale, "+" + artNum + "*" + quantity);
                MHelper.WriteLog("RegistrAndDisplayItem(int artNum, double quantity, double percDisc, double sumDisc). PLU -  " +
                                 artNum + ". Quantity - " + quantity);
            }
            else if (percDisc != 0 && sumDisc == 0)
            {
                _datecsPort.SendCommand(Commands.ArticlesSale, "+" + artNum + "*" + quantity, percDisc.ToString());
                MHelper.WriteLog("RegistrAndDisplayItem(int artNum, double quantity, double percDisc, double sumDisc). PLU -  " +
                                 artNum + ". Quantity - " + quantity + ". Perc discount - " + percDisc);
            }
            else if (percDisc == 0 && sumDisc != 0)
            {
                _datecsPort.SendCommand(Commands.ArticlesSale, "+" + artNum + "*" + quantity + ";" + sumDisc);
                MHelper.WriteLog("RegistrAndDisplayItem(int artNum, double quantity, double percDisc, double sumDisc). PLU -  " +
                                 artNum + ". Quantity - " + quantity + ". Sum discount - " + sumDisc);
            }
            else
            {
                MHelper.WriteLog("Error add item sum discount and abs discount not 0.00", LogType.Error);
            }
        }

        public void RegistrAndDisplayItemEx(int artNum, double quantity, double price, double percDisc, double sumDisc)
        {
            if (percDisc == 0 && sumDisc == 0)
            {
                _datecsPort.SendCommand(Commands.SalesRegister, "+" + artNum + "*" + quantity + "#" + price);
                MHelper.WriteLog("RegistrAndDisplayItemEx(int artNum, double quantity, double price, double percDisc, double sumDisc). PLU -  " +
                                 artNum + ". Quantity - " + quantity);
            }
            else if (percDisc != 0 && sumDisc == 0)
            {
                _datecsPort.SendCommand(Commands.SalesRegister, "+" + artNum + "*" + quantity + "#" + price, percDisc.ToString());
                MHelper.WriteLog("RegistrAndDisplayItemEx(int artNum, double quantity, double price, double percDisc, double sumDisc). PLU -  " +
                                 artNum + ". Quantity - " + quantity + ". Perc discount - " + percDisc);
            }
            else if (percDisc == 0 && sumDisc != 0)
            {
                _datecsPort.SendCommand(Commands.SalesRegister, "+" + artNum + "*" + quantity + "#" + price + ";" + sumDisc);
                MHelper.WriteLog("RegistrAndDisplayItemEx(int artNum, double quantity, double price, double percDisc, double sumDisc). PLU -  " +
                                 artNum + ". Quantity - " + quantity + ". Sum discount - " + sumDisc);
            }
            else
            {
                MHelper.WriteLog("Error add item sum discount and abs discount not 0.00", LogType.Error);
            }
        }

        public void PrintFiscalText(string text)
        {
            _datecsPort.SendCommand(Commands.FiscalTextPrint, text);
            MHelper.WriteLog("PrintFiscalText(string text) -  " + text);
        }

        public void SubTotal(double percDisc, double sumDisc)
        {
            if(percDisc != 0 && sumDisc != 0)
                return;
            _datecsPort.SendCommand(Commands.SubTotalDiscAllow,
                "11" + (percDisc != 0 ? "," + percDisc : "") + (sumDisc != 0 ? ";" + sumDisc : ""));
            MHelper.WriteLog("SubTotal(double percDisc, double sumDisc). Percent - " + percDisc + ". Sum - " + sumDisc);
        }

        public void Total(string text, int payMode, double sum)
        {
            char payName = MHelper.GetPayNameFromNumber(payMode);
            _datecsPort.SendCommand(Commands.SumTotal, text + '\t' + (payMode == 1 ? payName + sum : payName));
            MHelper.WriteLog("Total(string text, int payMode, double sum). Sum " + sum + ". Payment type - " + payName + ". Text - " + text);
        }

        public void TotalEx(string text, int payMode, double sum)
        {
            char payName = MHelper.GetPayNameFromNumber(payMode);
            _datecsPort.SendCommand(Commands.PaymentAndCloseRecipt, text + '\t' + (payMode == 1 ? payName + sum : payName));
            MHelper.WriteLog("TotalEx(string text, int payMode, double sum). Sum " + sum + ". Payment type - " + payName + ". Text - " + text);
        }

        public void PrintBarCode(int type, string text)
        {
            _datecsPort.SendCommand(Commands.BarQrCodePrint, type.ToString(), text);
            MHelper.WriteLog("PrintBarCode(int type, string text). Type - " + type + ". Text - " + text);
        }

        public void PrintLine(int type)
        {
            _datecsPort.SendCommand(Commands.DemarcationLinePrint, type.ToString());
            MHelper.WriteLog("PrintLine(int type). Type - " + type);
        }

        public void CloseFiscalReceipt()
        {
            _datecsPort.SendCommand(Commands.CloseFiscalReceipt);
            MHelper.WriteLog("CloseFiscalReceipt()");
        }

        public void CancelReceipt()
        {
            _datecsPort.SendCommand(Commands.CancelFiscalReceipt);
            MHelper.WriteLog("CancelReceipt()");
        }

        public void OpenReturnReceipt(int operNum, string passwd, int cashDescNum)
        {
            _datecsPort.SendCommand(Commands.OpenReturnReceipt, operNum.ToString(), passwd, cashDescNum.ToString(), "I");
            MHelper.WriteLog("OpenReturnReceipt(int operNum, string passwd, int cashDeskNum)");
        }

        public void MakeReceiptCopy(int count)
        {
            _datecsPort.SendCommand(Commands.ReceiptCopy, (count < 0 || count > 2) ? "1" : count.ToString());
            MHelper.WriteLog("MakeReceiptCopy(int count) - " + count);
        }

        public void PrintNullCheck()
        {
            OpenFiscalReceipt(1, "0000", 1);
            PrintFiscalText("НУЛЕВОЙ ЧЕК");
            Total("", 1, 0.00);
            CloseFiscalReceipt();
            MHelper.WriteLog("PrintNullCheck()");
        }

        public void AbsDiscGrp(int group, double disc)
        {
            _datecsPort.SendCommand(Commands.GroupTaxDiscount, "G" + group, "11" + ";" + disc);
            MHelper.WriteLog("AbsDiscGrp(int group, double disc). By group - " + group + ". Value - " + disc);
        }

        public void PerDiscGrp(int group, double disc)
        {
            _datecsPort.SendCommand(Commands.GroupTaxDiscount, "G" + group, "11", disc.ToString());
            MHelper.WriteLog("PercDiscGrp(int group, double disc). By group - " + group + ". Value - " + disc);
        }

        public void AbsDiscTax(int taxGrp, double disc)
        {
            char taxName = MHelper.GetTaxNameFromNumber(taxGrp);
            _datecsPort.SendCommand(Commands.GroupTaxDiscount, "T" + taxName, "11" + ";" + disc);
            MHelper.WriteLog("AbsDiscTax(int group, double disc). By tax group - " + taxName + ". Value - " + disc);
        }

        public void PerDiscTax(int taxGrp, double disc)
        {
            char taxName = MHelper.GetTaxNameFromNumber(taxGrp);
            _datecsPort.SendCommand(Commands.GroupTaxDiscount, "T" + taxName, "11", disc.ToString());
            MHelper.WriteLog("PerDiscTax(int group, double disc). By tax group - " + taxName + ". Value - " + disc);
        }

        public void XReport(string passwd)
        {
            _datecsPort.SendCommand(Commands.DailyReport, passwd, "2");
            MHelper.WriteLog("XReport(string passwd)");
        }

        public void ZReport(string passwd)
        {
            _datecsPort.SendCommand(Commands.DailyReport, passwd, "0");
            MHelper.WriteLog("ZReport(string passwd)");
        }

        public void PrintTaxReport(string passwd, string dateFrom, string dateTo)
        {
            _datecsPort.SendCommand(Commands.TaxChangeHistory, passwd, dateFrom, dateTo);
            MHelper.WriteLog("PrintTaxReport(string passwd, string dateFrom, string dateTo)");
        }

        public void PrintRepByNumFull(string passwd, int fromNum, int toNum)
        {
            _datecsPort.SendCommand(Commands.PeriodReportByNumberLong, passwd, fromNum.ToString(), toNum.ToString());
            MHelper.WriteLog("PrintRepByNumFull(string passwd, int fromNum, int toNum)");
        }

        public void PrintRepByDateFull(string passwd, string fromDate, string toDate)
        {
            _datecsPort.SendCommand(Commands.PeriodReportByDateLong, passwd, fromDate, toDate);
            MHelper.WriteLog("PrintRepByDateFull(string passwd, string fromDate, string toDate)");
        }

        public void PrintRepByNum(string passwd, int fromNum, int toNum)
        {
            _datecsPort.SendCommand(Commands.PeriodReportByNumberShort, passwd, fromNum.ToString(), toNum.ToString());
            MHelper.WriteLog("PrintRepByNum(string passwd, int fromNum, int toNum)");
        }

        public void PrintRepByDate(string passwd, string fromDate, string toDate)
        {
            _datecsPort.SendCommand(Commands.PeriodReportByDateShort, passwd, fromDate, toDate);
            MHelper.WriteLog("PrintRepByDate(string passwd, string fromDate, string toDate)");
        }

        public void PrintRepByArt(string passwd, int type)
        {
            string reportType = string.Empty;
            switch (type)
            {
                case 1:
                    reportType = "S";
                    break;
                case 2:
                    reportType = "P";
                    break;
                case 3:
                    reportType = "G";
                    break;
            }
            _datecsPort.SendCommand(Commands.ArticlesReport, passwd, reportType);
            MHelper.WriteLog("PrintRepByArt(string passwd, int type)");

        }

        public void PrintRepByOperator(string passwd)
        {
            _datecsPort.SendCommand(Commands.OperatorsReport, passwd);
            
            MHelper.WriteLog("PrintRepByOperator(string passwd)");
        }

        public void GetDateTime()
        {
            s1 = Encoding.Default.GetString(_datecsPort.SendCommand(Commands.ReceiveDateTime));
            MHelper.WriteLog("GetDateTime()" + (string.IsNullOrEmpty(s1) ? "" : s1));
        }

        public void LastFiscalClosure(int param)
        {
            _datecsPort.SendCommand(Commands.LastFiscalClosureInfo, param > 1 || param < 0 ? "0" : param.ToString());
            MHelper.WriteLog("LastFiscalClosure(int param). Param - " + param);
        }

        public void GetCurrentSums(int param)
        {
            _datecsPort.SendCommand(Commands.DaylyTurnoverInfo, param > 3 || param < 0 ? "0" : param.ToString());
            MHelper.WriteLog("GetCurrentSums(int param). Param - " + param);
        }

        public void GetCorectSums()
        {
            _datecsPort.SendCommand(Commands.StornoSumInfo);
            MHelper.WriteLog("GetCorectSums()");
        }

        public void GetFreeClosures()
        {
            _datecsPort.SendCommand(Commands.FiscalMemoryCapasity);
            MHelper.WriteLog("GetFreeClosures()");
        }

        public void GetStatus(bool wait)
        {
            _datecsPort.SendCommand(Commands.PrinterStatus, wait ? "W" : "X");
            MHelper.WriteLog("GetStatus(bool wait) Wait for print buffer clean? -" + wait);
        }

        public void GetFiscalClosureStatus(bool current)
        {
            _datecsPort.SendCommand(Commands.FiscalTransactionStatus, current ? "T" : "");
            MHelper.WriteLog("GetFiscalClosureStatus(bool current)");
        }

        public void GetDiagnosticInfo(bool calcCrc)
        {
            _datecsPort.SendCommand(Commands.DiagnosticInfo, calcCrc ? "1" : "");
            MHelper.WriteLog("GetDiagnosticInfo(bool calcCrc). Crc calculate ? -" + calcCrc);
        }

        public void GetCurrentTaxRates()
        {
            _datecsPort.SendCommand(Commands.CurrentTax);
            MHelper.WriteLog("GetCurrentTaxRates()");
        }

        public void GetTaxNumber()
        {
            _datecsPort.SendCommand(Commands.CurrentPersonalNumber);
            MHelper.WriteLog("GetTaxNumber()");
        }

        public void GetReceiptInfo()
        {
            _datecsPort.SendCommand(Commands.CurrentReceiptInfo);
            MHelper.WriteLog("GetReceiptInfo()");
        }

        public void GetDayInfo()
        {
            _datecsPort.SendCommand(Commands.AdditionalInfo);
            MHelper.WriteLog("GetDayInfo()");
        }

        public void GetOperatorInfo(int opNum)
        {
            _datecsPort.SendCommand(Commands.OperatorsInfo, opNum > 16 || opNum < 1 ? "1" : opNum.ToString());
            MHelper.WriteLog("GetOperatorInfo(int opNum). Operator number - " + opNum);
        }

        public void GetLastReceiptNum()
        {
            _datecsPort.SendCommand(Commands.LastDocument);
            MHelper.WriteLog("GetLastReceiptNum()");
        }

        public int isFiscalized()
        {
            return 0;
        }

        public void GetSmenLen()
        {
            _datecsPort.SendCommand(Commands.ShiftDuration);
            MHelper.WriteLog("GetSmenLen()");
        }

        public void GetLastClosureDate()
        {
            _datecsPort.SendCommand(Commands.LastFiscalClosureInfo, "0");
            MHelper.WriteLog("GetLastClosureDate()");
        }

        public void AdvancePaper(int lines)
        {
            _datecsPort.SendCommand(Commands.PaperTransport, lines < 1 || lines > 99 ? "1" : lines.ToString());
            MHelper.WriteLog("AdvancePaper(int lines) - " + lines);
        }

        public void AdvancePaperEx(int lines, int type)
        {
        }

        public void CutReceipt()
        {
            _datecsPort.SendCommand(Commands.PaperCut);
            MHelper.WriteLog("CutReceipt()");
        }

        public void ClearDisplay()
        {
            _datecsPort.SendCommand(Commands.ClearDisplay);
            MHelper.WriteLog("ClearDisplay()");
        }

        public void DisplayTextLL(string text)
        {
            _datecsPort.SendCommand(Commands.DisplayText, text);
            MHelper.WriteLog("DisplayTextLL(string text) - " + text); 
        }

        public void DisplayTextUL(string text)
        {
            _datecsPort.SendCommand(Commands.DisplayTextUpper, text);
            MHelper.WriteLog("DisplayTextUL(string text) - " + text);
        }

        public void DisplayDateTime()
        {
            _datecsPort.SendCommand(Commands.DateTimeDisplay);
            MHelper.WriteLog("DisplayDateTime()");
        }

        public void DisplayFreeText(string text)
        {
            _datecsPort.SendCommand(Commands.DisplayFreeText, text);
            MHelper.WriteLog("DisplayFreeText(string text) - " + text); 
        }

        public void ShowError(bool show)
        {
            //
        }

        public void OpenDrawer()
        {
            _datecsPort.SendCommand(Commands.CashDrawerOpen, "30");
            MHelper.WriteLog("OpenDrawer()");
        }

        public void OpenDrawerEx(int mSec)
        {
            _datecsPort.SendCommand(Commands.CashDrawerOpen, mSec < 5 || mSec > 150 ? "30" : mSec.ToString());
            MHelper.WriteLog("OpenDrawerEx(int mSec) - " + mSec);
        }

        public void InOut(double sum)
        {
            _datecsPort.SendCommand(Commands.SericeInOut, sum.ToString());
            MHelper.WriteLog("InOut(double sum) = " + sum);
        }

        public void PrintDiagnosticInfo()
        {
            _datecsPort.SendCommand(Commands.PrintDiagnostic);
            MHelper.WriteLog("PrintDiagnosticInfo()");
        }

        public void Sound()
        {
            _datecsPort.SendCommand(Commands.Sound);
            MHelper.WriteLog("Sound()");
        }

        public void SoundEx(int hZ, int mSec)
        {
            _datecsPort.SendCommand(Commands.Sound, hZ < 100 || hZ > 5000 ? "2000" : hZ.ToString(), mSec < 50 || mSec > 2000 ? "300" : mSec.ToString());
            MHelper.WriteLog("SoundEx(int hZ, int mSec). Hz - " + hZ + ". mSec - " + mSec);
        }

        public void GetLastDPAExchangeTime()
        {
            _datecsPort.SendCommand(Commands.ReceiveDataStatus);
            MHelper.WriteLog("GetLastDPAExchangeTime()");
        }

        private void ErrorPropertiesSet(short errNum, string errText)
        {
            LastError = errNum;
            LastErrorText = errText;
        }

        public static void MessageBoxShow(string mes)
        {
            System.Windows.Forms.MessageBox.Show(mes);
        }
    }
}