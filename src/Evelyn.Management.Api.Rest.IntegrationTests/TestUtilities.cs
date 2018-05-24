namespace Evelyn.Management.Api.Rest.IntegrationTests
{
    using System;
    using System.Linq;

    public class TestUtilities
    {
        private static readonly Random Random = new Random();

        public static string CreateKey(uint length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz01234567890_-";

            return new string(Enumerable.Repeat(chars, (int)length).Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }
}
