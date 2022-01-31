using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ObfuscatorBase.Interfaces;
using ObfuscatorBase.Utils;

namespace ObfuscatorBase.Utils
{
    internal static class UnderflowHelper
    {

        public static void Execute(MethodDef def)
        {
            if (def != null && !def.HasBody)
            {
                return;
            }
            CilBody body = def.Body;
            Instruction target = body.Instructions[0];
            Instruction item = Instruction.Create(OpCodes.Br_S, target);
            Instruction instruction3 = Instruction.Create(OpCodes.Pop);
            Random random = new Random();
            Instruction instruction4;
            switch (random.Next(0, 5))
            {
                case 0:
                    instruction4 = Instruction.Create(OpCodes.Ldnull);
                    break;
                case 1:
                    instruction4 = Instruction.Create(OpCodes.Ldc_I4_0);
                    break;
                case 2:
                    instruction4 = Instruction.Create(OpCodes.Ldstr, "calli");
                    break;
                case 3:
                    instruction4 = Instruction.Create(OpCodes.Ldc_I8, (uint)random.Next());
                    break;
                default:
                    instruction4 = Instruction.Create(OpCodes.Ldc_I8, (long)random.Next());
                    break;
            }
            body.Instructions.Insert(0, instruction4);
            body.Instructions.Insert(1, instruction3);
            body.Instructions.Insert(2, item);
            foreach (ExceptionHandler handler in body.ExceptionHandlers)
            {
                if (handler.TryStart == target)
                {
                    handler.TryStart = item;
                }
                else if (handler.HandlerStart == target)
                {
                    handler.HandlerStart = item;
                }
                else if (handler.FilterStart == target)
                {
                    handler.FilterStart = item;
                }
            }
        }
    }
}
