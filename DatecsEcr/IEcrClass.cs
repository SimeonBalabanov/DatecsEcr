using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace DatecsEcr
{
    [Guid("01EEEFE0-2383-4F4F-B55F-BE94A35EB352")]
    internal interface IEcrClass
    {

        // initialization
        void OpenPort(int portNum, int baudRate);
        void ClosePort();
        void SaveSettings();
        void SetBarcodeHeight(int value);
        void SetPrintDensity(int value);
        void EnableAutoOpenDrawer(bool enabled);
        void SetTaxName(int tax, string name);
        void SetHeaderFooter(int line, string text);
        void GetHeader(int line);
        void EnableCutCheck(bool enabled);
        void EnableSmallFont(bool enabled);
        void EnableLogo(bool enable);
        void SetDateTime(string date, string time);
        void Fiscalise(string passwd, string serial, string taxnum, int taxNumType);
        void SetMulDecCurRF(string passwd, int dec, string enableTax, double taxA, double taxB, double taxC, double taxD);
        void SetTaxType(int type);
        void SetSerialNumber(string serial);
        void SetFiscalNumber(string fNumber);
        void SetTaxNumber(string taxNum, int type);
        void SetOperatorPassword(int opNum, string oldPasswd, string newPasswd);
        void SetOperatorName(int opNum, string passwd, string name);
        void ClearOperator(int opNum, string passwd);
        void SetAdminPassword(string oldPass, string newPass);
        void ClearOperatorPassword(int opNum, string admPasswd);

        //atricles programming and get articles info
        void GetArticlesInfo();
        void SetArticle(int artNum, int taxGrp, int grp, double price, string passwd, string name);
        void DelArticle(string passwd, int artNum);
        void GetArticle(int artNum);
        void GetFirstArticle();
        void GetNextArticle();
        void ChangeArticlePrice(string passwd, int artNum, double price);
        void GetFirstFreeArticle();
        void GetLastFreeArticle();

        //sale
        void OpenNonfiscalReceipt();
        void PrintNonfiscalText(string text);
        void CloseNonFiscalReceipt();
        void OpenFiscalReceipt(int operNum, string passwd, int cashDeskNum);
        void RegistrItem(int artNum, double quantity, double percDisc, double sumDisc);
        void RegistrItemEx(int artNum, double quantity, double price, double percDisc, double sumDisc);
        void RegistrAndDisplayItem(int artNum, double quantity, double percDisc, double sumDisc);
        void RegistrAndDisplayItemEx(int artNum, double quantity, double price, double percDisc, double sumDisc);
        void PrintFiscalText(string text);
        void SubTotal(double percDisc, double sumDisc);
        void Total(string text, int payMode, double sum);
        void TotalEx(string text, int payMode, double sum);
        void PrintBarCode(int type, string text);
        void PrintLine(int type);
        void CloseFiscalReceipt();
        void CancelReceipt();
        void OpenReturnReceipt(int operNum, string passwd, int cashDescNum);
        void MakeReceiptCopy(int count);
        void PrintNullCheck();
        void AbsDiscGrp(int group, double disc);
        void PerDiscGrp(int group, double disc);
        void AbsDiscTax(int taxGroup, double disc);
        void PerDiscTax(int grup, double disc);


        //reports
        void XReport(string passwd);
        void ZReport(string passwd);
        void PrintTaxReport(string passwd, string dateFrom, string dateTo);
        void PrintRepByNumFull(string passwd, int fromNum, int toNum);
        void PrintRepByDateFull(string passwd, string fromDate, string toDate);
        void PrintRepByNum(string passwd, int fromNum, int toNum);
        void PrintRepByDate(string passwd, string fromDate, string toDate);
        void PrintRepByArt(string passwd, int type);
        void PrintRepByOperator(string passwd);

        //receive info from printer to software
        void GetDateTime();
        void LastFiscalClosure(int param);
        void GetCurrentSums(int param);
        void GetCorectSums();
        void GetFreeClosures();
        void GetStatus(bool wait);
        void GetFiscalClosureStatus(bool current);
        void GetDiagnosticInfo(bool calcCrc);
        void GetCurrentTaxRates();
        void GetTaxNumber();
        void GetReceiptInfo();
        void GetDayInfo();
        void GetOperatorInfo(int opNum);
        void GetLastReceiptNum();
        int isFiscalized();
        void GetSmenLen();
        void GetLastClosureDate();
        

        //printer commands
        void AdvancePaper(int lines);
        void AdvancePaperEx(int lines, int type);
        void CutReceipt();

        //display commands
        void ClearDisplay();
        void DisplayTextLL(string text);
        void DisplayTextUL(string text);
        void DisplayDateTime();
        void DisplayFreeText(string text);

        //debug 
        void ShowError(bool show);

        //others
        void OpenDrawer();
        void OpenDrawerEx(int mSec);
        void InOut(double sum);
        void PrintDiagnosticInfo();
        void Sound();
        void SoundEx(int hZ, int mSec);
        void GetLastDPAExchangeTime();

    }
}
