using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using ObfuscatorBase.Interfaces;
using ObfuscatorBase.Utils;
using dnlib.DotNet.Emit;

namespace ObfuscatorBase.Processors
{
    internal class ILProcessor : IProcessor
    {
        public void Process(AssemblyDef assembly)
        {
            if (!Analyzer.CanObfuscateMembers(assembly))
                return;

            foreach (ModuleDef module in assembly.Modules)
            {
                Process(module);
            }
        }

        public void Process(ModuleDef module)
        {
            if (Analyzer.CanObfuscate(module))
                module.Name = Randomization.GetRandomGlitchString(128, 496);

            if (!Analyzer.CanObfuscateMembers(module))
                return;

            foreach (TypeDef type in module.Types)
            {
                Process(type);
            }
        }

        private void Process(TypeDef type)
        {
            if (Analyzer.CanObfuscate(type))
            {
                if (type.IsRuntimeSpecialName || type.IsGlobalModuleType)
                    return;

                type.Name = Randomization.GetRandomGlitchString(128, 496);
                type.Namespace = Randomization.GetRandomGlitchString(128, 496);
            }

            if (!Analyzer.CanObfuscateMembers(type))
                return;

            if (type.HasNestedTypes)
            {
                foreach (TypeDef typex in type.NestedTypes)
                {
                    Process(typex);
                }
            }

            foreach (MethodDef method in type.Methods)
            {
                if (ILHelper.isPatchableMethod(method))
                {
                    ObfuscateResult.C2ILCount++;
                    List<Instruction> originalInstructions = new List<Instruction>();
                    foreach (Instruction i in method.Body.Instructions)
                    {
                        originalInstructions.Add(i);
                        i.OpCode = OpCodes.Nop;
                    }

                    Instruction[] newStructs = ILHelper.toILArray(originalInstructions.ToArray(), method.Module);

                    foreach (Instruction n in newStructs)
                    {
                        method.Body.Instructions.Add(n);
                    }
                }
            }
        }
    }
}
