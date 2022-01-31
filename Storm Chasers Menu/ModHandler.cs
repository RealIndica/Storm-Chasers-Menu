using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using System.Reflection;
using UnityEngine;
using System.IO;
using System.Runtime.InteropServices;

namespace Storm_Chasers_Menu
{
    public static class ModHandler
    {
        public static string unmodifiedMD5 = "";
        public static string currentMD5 = "";

        public static void FixGameCompatibility()
        {
            unmodifiedMD5 = GetStoredMD5();
            currentMD5 = GetMD5();

            if (currentMD5 != unmodifiedMD5)
            {
                Application.Quit(0);
            }
            else
            {

            }
        }

        static void printBytes(byte[] bytes)
        {
            MelonLogger.Msg(BitConverter.ToString(bytes));
        }

        public static List<byte> TakeLast(List<byte> source, int N)
        {
            List<byte> temp = source;
            temp.RemoveRange(0, source.Count - N);
            return temp;
        }

        public static List<byte> RemoveLast(List<byte> lst, int N)
        {
            List<byte> temp = lst;
            temp.RemoveRange(lst.Count - N, N);
            return temp;
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        private static string GetStoredMD5()
        {
            List<byte> fileBytes = File.ReadAllBytes(MelonLoader.MelonHandler.ModsDirectory + "\\" + typeof(ModHandler).Assembly.GetName().Name + ".dll").ToList();
            List<byte> md5Bytes = TakeLast(fileBytes, 32);
            return Encoding.ASCII.GetString(md5Bytes.ToArray());
        }

        private static string GetMD5()
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            System.IO.FileStream stream = new System.IO.FileStream(MelonLoader.MelonHandler.ModsDirectory + "\\" + typeof(ModHandler).Assembly.GetName().Name + ".dll", System.IO.FileMode.Open, System.IO.FileAccess.Read);

            List<byte> fBytes = ReadFully(stream).ToList();
            stream.Close();
            fBytes = RemoveLast(fBytes, 32);
            md5.ComputeHash(fBytes.ToArray());

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < md5.Hash.Length; i++)
                sb.Append(md5.Hash[i].ToString("x2"));
            return sb.ToString().ToUpperInvariant();
        }
    }
}
