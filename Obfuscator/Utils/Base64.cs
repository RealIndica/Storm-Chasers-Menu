using ObfuscatorBase.Interfaces;
using System;
using System.Text;

namespace ObfuscatorBase.Utils
{
    public class Base64 : ICrypto
    {
        /// <summary>
        /// Method for encrypt string with Base64. 
        /// </summary>
        /// <param name="dataPlain">Input plain string</param>
        /// <returns>Encode string</returns>
        public string Encrypt(string input)
        {
            uint key = (uint)2391823912;
            try
            {
                StringBuilder szInputStringBuild = new StringBuilder(input);
                StringBuilder szOutStringBuild = new StringBuilder(input.Length);
                char Textch;
                for (int iCount = 0; iCount < input.Length; iCount++)
                {
                    Textch = szInputStringBuild[iCount];
                    Textch = (char)(Textch ^ key);
                    szOutStringBuild.Append(Textch);
                }
                return szOutStringBuild.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}