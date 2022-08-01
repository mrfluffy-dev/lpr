using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



    internal class Write
    {
         public void writer()
         {
        
         Console.WriteLine("Please save the notepad file in the file you use for program");
         System.Diagnostics.Process process = new System.Diagnostics.Process();
         System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
         startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
         startInfo.FileName = "cmd.exe";
         startInfo.Arguments = "/C notepad.exe";
         process.StartInfo = startInfo;
         process.Start();



         }




    }

