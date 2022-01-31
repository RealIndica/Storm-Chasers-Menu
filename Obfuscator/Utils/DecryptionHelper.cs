using System;
using System.Text;

namespace ObfuscatorBase.Utils
{
    internal static class DecryptionHelper
    {
        /// <summary>
        /// Method for decrypt string with Base64.
        /// </summary>
        /// <param name="dataEnc">Input encode string</param>
        /// <returns>Plain string</returns>
        public static string LoadString(string input)
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