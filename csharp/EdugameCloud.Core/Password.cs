using System;

namespace EdugameCloud.Core
{
    public static class Password
    {
        public static string CreateAlphaNumericRandomPassword(int passwordLength)
        {
            const string AllowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            return CreateRandomPassword(passwordLength, AllowedChars);
        }

        public static string CreateRandomPassword(int passwordLength, string allowedChars)
        {
            var randNum = new Random();
            var chars = new char[passwordLength];
            int allowedCharCount = allowedChars.Length;

            for (int i = 0; i < passwordLength; i++)
            {
                chars[i] = allowedChars[(int)(allowedCharCount * randNum.NextDouble())];
            }

            return new string(chars);
        }

    }

}
