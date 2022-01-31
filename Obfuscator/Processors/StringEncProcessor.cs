using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System.Collections.Generic;
using System.Linq;
using ObfuscatorBase.Interfaces;
using ObfuscatorBase.Utils;

namespace ObfuscatorBase.Processors
{
    internal class StringEncProcessor : IProcessor
    {
        public void Process(AssemblyDef assembly)
        {
            if (!Analyzer.CanObfuscateMembers(assembly))
                return;

            foreach (ModuleDef module in assembly.Modules)
            {
                StringEncryption.EncryptStrings(module);
            }
        }
    }
}