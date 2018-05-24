namespace Evelyn.Core.Tests
{
    using System;
    using System.Linq;
    using System.Text;

    public class TestUtilities
    {
        private static readonly Random Random = new Random();

        public static string CreateKey(uint length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz01234567890_-";

            return new string(Enumerable.Repeat(chars, (int)length).Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        public static string CreateString(uint length)
        {
            var stringBuilder = new StringBuilder();
            while (length > 0)
            {
                stringBuilder.Append(length % 10);
                length--;
            }

            return stringBuilder.ToString();
        }
    }
}
