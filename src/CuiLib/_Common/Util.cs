using System.Text;

namespace CuiLib
{
    /// <summary>
    /// 共通処理を実装します。
    /// </summary>
    internal static class Util
    {
        /// <summary>
        /// 特殊文字を置換します。
        /// </summary>
        /// <param name="value">置換する文字列</param>
        /// <returns>置換後の文字列</returns>
        internal static string ReplaceSpecialCharacters(this string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;

            var builder = new StringBuilder();
            foreach (char current in value)
            {
                switch (current)
                {
                    case '\\': builder.Append(@"\\"); break;
                    case '\a': builder.Append(@"\a"); break;
                    case '\b': builder.Append(@"\b"); break;
                    case '\f': builder.Append(@"\f"); break;
                    case '\n': builder.Append(@"\n"); break;
                    case '\r': builder.Append(@"\r"); break;
                    case '\t': builder.Append(@"\t"); break;
                    case '\v': builder.Append(@"\v"); break;
                    default: builder.Append(current); break;
                }
            }
            return builder.ToString();
        }
    }
}
