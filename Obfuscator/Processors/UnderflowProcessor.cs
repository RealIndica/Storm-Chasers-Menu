using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System.Collections.Generic;
using System.Linq;
using ObfuscatorBase.Interfaces;
using ObfuscatorBase.Utils;

namespace ObfuscatorBase.Processors
{
    internal class UnderflowProcessor : IProcessor
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
                UnderflowHelper.Execute(method);
                ObfuscateResult.UnderflowCount++;
            }
        }
    }
}