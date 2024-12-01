using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MyTutor.Usefull
{
    public static class CommonMethods
    {
        private readonly static Random _rand = new Random();
        public static string CreatePassword(this int length)
        {
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string number = "1234567890";
            const string special = "!@#$%^&*_-=+";

            // Get cryptographically random sequence of bytes
            var bytes = new byte[length];
            new RNGCryptoServiceProvider().GetBytes(bytes);

            // Build up a string using random bytes and character classes
            var res = new StringBuilder();
            foreach (byte b in bytes)
            {
                // Randomly select a character class for each byte
                switch (_rand.Next(4))
                {
                    // In each case use mod to project byte b to the correct range
                    case 0:
                        res.Append(lower[b % lower.Count()]);
                        break;
                    case 1:
                        res.Append(upper[b % upper.Count()]);
                        break;
                    case 2:
                        res.Append(number[b % number.Count()]);
                        break;
                    case 3:
                        res.Append(special[b % special.Count()]);
                        break;
                }
            }
            return res.ToString();
        }
        public static string RandomString(this int size)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return builder.ToString();
        }
    }
}
