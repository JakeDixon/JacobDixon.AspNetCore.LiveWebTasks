using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace JacobDixon.AspNetCore.LiveWebTasks.Extensions
{
    /// <summary>
    /// Extensions for the string class adding MatchesBlob and IsNullOrEmpty.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Compares the string against a given glob pattern.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="glob">The glob pattern to match, where "*" means any sequence of characters, and "?" means any single character.</param>
        /// <returns><c>true</c> if the string matches the given glob pattern; otherwise <c>false</c>.</returns>
        public static bool MatchesGlob(this string str, string glob)
        {
            return new Regex(
                "^" + Regex.Escape(glob).Replace(@"\*", ".*").Replace(@"\?", ".") + "$",
                RegexOptions.IgnoreCase | RegexOptions.Singleline
            ).IsMatch(str);
        }

        /// <summary>
        /// Checks if a string is null or empty.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns><c>true</c> if the string is null or empty; otherwise <c>false</c></returns>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// Compares a string against an <see cref="IEnumerable{T}"/> of globs.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="globs">The enumerable collection of globs to test the string against.</param>
        /// <returns><c>true</c> if the string matches any of the globs, otherwise <c>false</c>.</returns>
        public static bool MatchesAnyGlob(this string str, IEnumerable<string> globs)
        {
            foreach (var glob in globs)
            {
                if (str.MatchesGlob(glob))
                    return true;
            }

            return false;
        }
    }
}