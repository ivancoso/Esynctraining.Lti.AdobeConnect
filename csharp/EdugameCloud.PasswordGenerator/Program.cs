using System;
using System.Security.Cryptography;
using System.Text;

namespace EdugameCloud.PasswordGenerator
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            string password = "changepass";
            string hash = BitConverter.ToString(GenerateHash(password)).Replace("-", string.Empty).ToUpper(); ;
        }


        private static byte[] GenerateHash(string plainString)
        {
            SHA256 hash = SHA256.Create();

            try
            {
                return hash.ComputeHash(Encoding.UTF8.GetBytes(plainString));
            }
            finally
            {
                hash.Dispose();
            }
        }

    }

}
