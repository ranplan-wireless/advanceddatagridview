using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Zuby.ADGV
{
    /// <summary>
    /// Escapes and parses column names used inside DataView RowFilter/Sort expressions.
    /// The .NET Framework expression parser ends a "[name]" reference at the first ']',
    /// so a column name that itself contains ']' must be written as "\]" inside the
    /// brackets — e.g. column "WiFi RSSI [dBm]" is referenced as "[WiFi RSSI [dBm\]]".
    /// A name containing a backslash directly before ']' cannot be represented.
    /// </summary>
    public static class ColumnNameEscaper
    {
        private const char EscapeChar = '\\';
        private const char ClosingBracket = ']';

        /// <summary>Matches a bracketed column reference, honouring "\]" escapes.</summary>
        private static readonly Regex ColumnTokenRegex = new Regex(@"\[(?:\\.|[^\]])+]", RegexOptions.Compiled);

        public static string Escape(string columnName)
        {
            return columnName?.Replace(ClosingBracket.ToString(), EscapeChar.ToString() + ClosingBracket);
        }

        public static string Unescape(string escapedName)
        {
            if (string.IsNullOrEmpty(escapedName) || escapedName.IndexOf(EscapeChar) < 0)
                return escapedName;

            return Regex.Replace(escapedName, @"\\(.)", "$1");
        }

        /// <summary>Returns the real (unescaped) column names of every "[name]" reference in an expression.</summary>
        public static IList<string> ParseColumnTokens(string expression)
        {
            if (string.IsNullOrEmpty(expression))
                return new List<string>();

            return ColumnTokenRegex.Matches(expression)
                .OfType<Match>()
                .Select(m => Unescape(m.Value.Substring(1, m.Value.Length - 2)))
                .ToList();
        }

        /// <summary>Formats a per-column expression template, escaping the substituted column name.</summary>
        public static string FormatColumnExpression(string format, string columnName)
        {
            return string.Format(format, Escape(columnName));
        }
    }
}
