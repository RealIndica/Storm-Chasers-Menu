using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using System.IO;
using ObfuscatorBase;

namespace Obfuscator
{
    class Program
    {
        static void Main(string[] args)
        {
            string PREtarget = args[0];
            string POSTtarget = Path.GetDirectoryName(PREtarget) + "\\FINAL\\" + Path.GetFileName(PREtarget);

            Console.WriteLine("Obfuscating Assembly . . .");
            ObfuscationProcessor proc = new ObfuscationProcessor();
            proc.Load(PREtarget);
            proc.Process();
            proc.Save(POSTtarget);
            proc.Unload();
            Console.WriteLine("Successfully obfuscated the target assembly!");
        }
    }
}
