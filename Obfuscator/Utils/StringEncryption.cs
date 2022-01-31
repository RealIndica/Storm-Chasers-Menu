using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using ObfuscatorBase.Interfaces;
using ObfuscatorBase.Utils;
using System.Reflection;

namespace ObfuscatorBase.Utils
{
    internal static class StringEncryption
    {
        private static MethodDef InjectMethod(ModuleDef module, string methodName)
        {
            ModuleDefMD typeModule = ModuleDefMD.Load(typeof(DecryptionHelper).Module);
            TypeDef typeDef = typeModule.ResolveTypeDef(MDToken.ToRID(typeof(DecryptionHelper).MetadataToken));
            IEnumerable<IDnlibDef> members = InjectHelper.Inject(typeDef, module.GlobalType, module);
            MethodDef injectedMethodDef = (MethodDef)members.Single(method => method.Name == methodName);

            foreach (MethodDef md in module.GlobalType.Methods)
            {
                if (md.Name == ".ctor")
                {
                    module.GlobalType.Remove(md);
                    break;
                }
            }

            return injectedMethodDef;
        }

        public static void DoEncrypt(string inPath, string outPath)
        {
            ModuleDef module = ModuleDefMD.Load(inPath);

            module = EncryptStrings(module);

            module.Write(outPath);
        }

        public static void EncryptionWorker(TypeDef type, ICrypto crypto, MethodDef decryptMethod)
        {
            if (type.HasNestedTypes)
            {
                foreach (TypeDef nested in type.NestedTypes)
                {
                    EncryptionWorker(nested, crypto, decryptMethod);
                }
            }

            foreach (MethodDef method in type.Methods)
            {
                if (!method.HasBody)
                    continue;
                if (method == decryptMethod)
                    continue;

                method.Body.KeepOldMaxStack = true;

                for (int i = 0; i < method.Body.Instructions.Count; i++)
                {
                    if (method.Body.Instructions[i].OpCode == OpCodes.Ldstr)    // String
                    {
                        string oldString = method.Body.Instructions[i].Operand.ToString();  //Original String

                        method.Body.Instructions[i].Operand = crypto.Encrypt(oldString);
                        method.Body.Instructions.Insert(i + 1, new Instruction(OpCodes.Call, decryptMethod));
                        ObfuscateResult.StringEncryptCount++;
                    }
                }

                method.Body.SimplifyBranches();
                method.Body.OptimizeBranches();
            }      
        }

        public static ModuleDef EncryptStrings(ModuleDef inModule)
        {
            ModuleDef module = inModule;

            ICrypto crypto = new Base64();

            MethodDef decryptMethod = InjectMethod(module, "LoadString");

            foreach (TypeDef type in module.Types)
            {
                if (type.IsGlobalModuleType || type.Name == "Resources" || type.Name == "Settings")
                    continue;

                EncryptionWorker(type, crypto, decryptMethod);

            }

            return module;
        }
    }
}
