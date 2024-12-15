namespace Tenrec.Utils
{
    public static class StringHelper
    {
        public static string CodeableNickname(string nickname)
        {
            return nickname.Replace(" ", "_");
        }
        public static string IndexedString(string str, int index)
        {
            return string.Concat(System.Linq.Enumerable.Repeat("    ", index)) + str;
        }
    }
}
