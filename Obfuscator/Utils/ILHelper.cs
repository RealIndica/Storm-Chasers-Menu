using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace ObfuscatorBase.Utils
{
    internal static class ILHelper
    {
        public static Instruction[] toILArray(Instruction[] originalInstructions, ModuleDef mod)
        {
            List<Instruction> newStructs = new List<Instruction>();

            newStructs.Add(new Instruction(OpCodes.Ldc_I4, originalInstructions.Length));
            newStructs.Add(OpCodes.Newarr.ToInstruction(mod.CorLibTypes.Object));

            int count = 0;

            foreach (Instruction i in originalInstructions)
            {
                newStructs.Add(new Instruction(OpCodes.Dup));
                newStructs.Add(new Instruction(OpCodes.Ldc_I4, count));
                newStructs.Add(OpCodes.Ldsfld.ToInstruction(i)); 
                newStructs.Add(new Instruction(OpCodes.Stelem_Ref));
                count++;
            }
            
            newStructs.Add(new Instruction(OpCodes.Pop));
            newStructs.Add(new Instruction(OpCodes.Ret));

            return newStructs.ToArray();
        }

        public static bool isPatchableMethod(MethodDef func)
        {
            if (func.HasBody)
            {
                return true;
            } else
            {
                return false;
            }
        }
    }
}
