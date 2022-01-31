using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MD5Signer
{
    class Program
    {
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

        private static string GetMD5(string filePath)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            System.IO.FileStream stream = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);

            List<byte> fBytes = ReadFully(stream).ToList();
            stream.Close();

            md5.ComputeHash(fBytes.ToArray());

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < md5.Hash.Length; i++)
                sb.Append(md5.Hash[i].ToString("x2"));
            return sb.ToString().ToUpperInvariant();
        }

        public static void AppendAllBytes(string path, byte[] bytes)
        {
            using (var stream = new FileStream(path, FileMode.Append))
            {
                stream.Write(bytes, 0, bytes.Length);
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Signing Assembly . . .");
            string filePath = args[0];
            string md5 = GetMD5(filePath);
            AppendAllBytes(filePath, Encoding.ASCII.GetBytes(md5));
            Console.WriteLine("Appended MD5 Hash to end of Assembly!");
        }
    }
}
