namespace Tenrec.Utils
{
    /// <summary>
    /// Contains helper classes for string manipulation required in Tenrec project.
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Replaces all spaces in the string with '_'
        /// </summary>
        /// <param name="nickname">source string</param>
        /// <returns>source string without any spaces (all spaces replaced with '_')</returns>
        public static string CodeableNickname(string nickname)
        {
            return nickname.Replace(" ", "_");
        }
        /// <summary>
        /// Adds identation based on provided index. index can start from 0 (no identation).
        /// </summary>
        /// <param name="str">source string</param>
        /// <param name="index">level of identation desired.</param>
        /// <returns>An identated string</returns>
        public static string IndexedString(string str, int index)
        {
            return string.Concat(System.Linq.Enumerable.Repeat("    ", index)) + str;
        }
    }
}
