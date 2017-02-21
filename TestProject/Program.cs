using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatecsEcr;
using System.Diagnostics;
using System.Reflection;

namespace TestProject
{
    class Program
    {
        static void Main(string[] args)
        {
            //ProcessStartInfo reg = new ProcessStartInfo();
            //reg.UseShellExecute = false;
            //reg.FileName = "cmd";
            //reg.Arguments = @"/k c:\sn.exe -k c:\DatecsEcr.snk";
            //Process.Start(reg);
            //reg.Arguments = @"/k c:\RegAsm.exe /codebase c:\DatecsEcr.dll";
            //Process.Start(reg);
            //reg.Arguments = @"/k c:\RegAsm.exe /tlb c:\DatecsEcr.dll";
            //Process.Start(reg);
            //reg.Arguments = @"/k c:\RegAsm.exe c:\DatecsEcr.dll";
            //Process.Start(reg);

            FiscalPrinter ecr = new FiscalPrinter();
            ecr.OpenPort(1, 115200);
            
            ecr.OpenFiscalReceipt(1, "0000", 1);
            ecr.RegistrItem(10, 1.00, 0.00, 0.00);
            ecr.AbsDiscTax(6, 3.00);
            ecr.TotalEx("", 1, 100);
            //ecr.GetDateTime();
            Console.WriteLine(ecr.s1);
            //  ecr.PrintDiagnosticInfo();
            ecr.ClosePort();
         
            Console.ReadKey();
        }
    }
}
