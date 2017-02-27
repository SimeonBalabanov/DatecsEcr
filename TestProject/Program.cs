﻿using System;
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
           // ecr.OpenPort(2, 115200);
            ecr.OpenPort(1, 115200);

            ecr.GetArticlesInfo();
            ecr.GetStatus(true);
            ecr.GetDateTime();
            ecr.GetCurrentSums(1);
            ecr.GetCurrentTaxRates();
            ecr.GetFreeClosures();
            ecr.GetLastDPAExchangeTime();
            ecr.GetTaxNumber();
            //ecr.GetDateTime();
            Console.WriteLine(ecr.s1);
            //  ecr.PrintDiagnosticInfo();
            ecr.ClosePort();
         
            Console.ReadKey();
        }
    }
}
