using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace CuiLib.Extensions
{
    /// <summary>
    /// 文字列の拡張を表します。
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 文字同士を比較します。
        /// </summary>
        /// <param name="value">文字</param>
        /// <param name="other">比較対象の文字</param>
        /// <param name="comparisonType">比較方法</param>
        /// <returns>比較結果</returns>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/>が無効な値</exception>
        public static int CompareTo(char value, char other, StringComparison comparisonType)
        {
            return string.Compare(value.ToString(), other.ToString(), comparisonType);
        }

        /// <summary>
        /// 文字同士を比較します。
        /// </summary>
        /// <param name="value">文字</param>
        /// <param name="other">比較対象の文字</param>
        /// <param name="comparisonType">比較方法</param>
        /// <returns><paramref name="value"/>と<paramref name="other"/>が等しい場合はtrue，それ以外でfalse</returns>
        /// <exception cref="ArgumentException"><paramref name="comparisonType"/>が無効な値</exception>
        public static bool Equals(char value, char other, StringComparison comparisonType)
        {
            string.Compare("A", "B", comparisonType);
            return string.Equals(value.ToString(), other.ToString(), comparisonType);
        }

        /// <summary>
        /// 文字列の置換を纏めて行います。
        /// </summary>
        /// <param name="value">置換する文字列</param>
        /// <param name="from">置換前の文字一覧</param>
        /// <param name="to">置換後の文字</param>
        /// <returns>置換後の文字列</returns>
        /// <exception cref="ArgumentNullException"><paramref name="from"/>がnull</exception>
        public static ReadOnlySpan<char> ReplaceAll(this ReadOnlySpan<char> value, IEnumerable<char> from, char to)
        {
            if (value.Length == 0) return ReadOnlySpan<char>.Empty;

            HashSet<char> set = from.ToHashSet();
            var array = new char[value.Length];
            for (int i = 0; i < value.Length; i++) array[i] = set.Contains(value[i]) ? to : value[i];

            return array.AsSpan();
        }

        /// <summary>
        /// 文字列の置換を纏めて行います。
        /// </summary>
        /// <param name="value">置換する文字列</param>
        /// <param name="map">置換情報を表すマップ</param>
        /// <returns>置換後の文字列</returns>
        /// <exception cref="ArgumentNullException"><paramref name="map"/>がnull</exception>
        public static ReadOnlySpan<char> ReplaceAll(this ReadOnlySpan<char> value, IDictionary<char, char> map)
        {
            ArgumentNullException.ThrowIfNull(map);

            if (value.Length == 0) return [];

            var array = new char[value.Length];
            for (int i = 0; i < value.Length; i++) array[i] = map.TryGetValue(value[i], out char to) ? to : value[i];

            return array.AsSpan();
        }

        /// <summary>
        /// エスケープを考慮して文字列を分割します。
        /// </summary>
        /// <param name="value">分割する文字列</param>
        /// <param name="separator">分割に利用する部分</param>
        /// <returns>分割後の文字列一覧</returns>
        public static string[] EscapedSplit(this ReadOnlySpan<char> value, char separator)
        {
            return EscapedSplitPrivate(value, MemoryMarshal.CreateSpan(ref separator, 1));
        }

        /// <summary>
        /// エスケープを考慮して文字列を分割します。
        /// </summary>
        /// <param name="value">分割する文字列</param>
        /// <param name="separator">分割に利用する部分</param>
        /// <returns>分割後の文字列一覧</returns>
        public static string[] EscapedSplit(this ReadOnlySpan<char> value, string? separator)
        {
            return EscapedSplitPrivate(value, separator.AsSpan());
        }

        /// <summary>
        /// エスケープを考慮して文字列を分割します。
        /// </summary>
        /// <param name="value">分割する文字列</param>
        /// <param name="separator">分割に利用する部分</param>
        /// <returns>分割後の文字列一覧</returns>
        private static string[] EscapedSplitPrivate(ReadOnlySpan<char> value, ReadOnlySpan<char> separator)
        {
            if (value.Length < separator.Length) return Array.Empty<string>();
            if (value.Length == separator.Length)
            {
                if (value.Equals(separator, StringComparison.Ordinal)) return Array.Empty<string>();
                return new[] { value.ToString() };
            }

            var list = new List<string>();

            int start = 0;
            int index = 0;
            while (index + separator.Length < value.Length)
            {
                // "Hoge" や 'Fuga' を判定
                if (index == start)
                {
                    char current = value[index];
                    // エスケープ判定
                    if (current is '"' or '\'')
                    {
                        int nextEscapeIndex = value.SliceOrDefault(index + 1).IndexOf(current.ToString(), StringComparison.Ordinal);
                        if (nextEscapeIndex != -1) nextEscapeIndex += index + 1;
                        // エスケープ文字で囲まれているかどうか
                        // 右側にエスケープ文字があるなら nextEscapeIndex + 1 以降に separator があるはず
                        if (nextEscapeIndex != -1 || value.SliceOrDefault(nextEscapeIndex + 1, separator.Length, separator).Equals(separator, StringComparison.Ordinal))
                        {
                            list.Add(value[(start + 1)..nextEscapeIndex].ToString());
                            index = nextEscapeIndex + separator.Length + 1;
                            start = index;
                            continue;
                        }
                    }
                }
                // 区切り文字の開始点かどうか判定
                if (value.Slice(index, separator.Length).Equals(separator, StringComparison.Ordinal))
                {
                    list.Add(value[start..index].ToString());
                    index += separator.Length;
                    start = index;
                    continue;
                }
                index++;
            }
            // 最後のグループを処理していない場合
            if (start < value.Length) list.Add(value[start..].ToString());

            return list.ToArray();
        }
    }
}
