using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CuiLib.Data
{
    /// <summary>
    /// <see cref="ValueRange"/>のコレクションを表します。
    /// </summary>
    [Serializable]
    public class ValueRangeCollection : IList<ValueRange>, IReadOnlyList<ValueRange>, IList, IFormattable
#if NET7_0_OR_GREATER
        , ISpanParsable<ValueRangeCollection>
#endif
#if NET6_0_OR_GREATER
        , ISpanFormattable
#endif
    {
        private const string Separator = ",";

        private readonly List<ValueRange> ranges;

        /// <summary>
        /// 内部配列の容量を取得または設定します。
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">設定しようとした値が<see cref="Count"/>未満</exception>
        public int Capacity
        {
            get => ranges.Capacity;
            set => ranges.Capacity = value;
        }

        /// <summary>
        /// <see cref="ValueRangeCollection"/>の新しいインスタンスを初期化します。
        /// </summary>
        public ValueRangeCollection()
        {
            ranges = [];
        }

        /// <summary>
        /// <see cref="ValueRangeCollection"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="capacity">初期容量</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/>が0未満</exception>
        public ValueRangeCollection(int capacity)
        {
            ranges = new List<ValueRange>(capacity);
        }

        /// <summary>
        /// <see cref="ValueRangeCollection"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="source">予め格納する要素を持つコレクション</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>が<see langword="null"/></exception>
        public ValueRangeCollection(IEnumerable<ValueRange> source)
        {
            ranges = new List<ValueRange>(source);
        }

        /// <summary>
        /// <see cref="ValueRangeCollection"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="source">予め格納する要素を持つコレクション</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>が<see langword="null"/></exception>
        public ValueRangeCollection(IEnumerable<int> source)
        {
            ThrowHelpers.ThrowIfNull(source);

            ranges = new List<ValueRange>(IterateRanges(source));
        }

        /// <summary>
        /// <see cref="ValueRangeCollection"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="ranges">格納する<see cref="ValueRange"/>のリスト</param>
        private ValueRangeCollection(List<ValueRange> ranges)
        {
            this.ranges = ranges;
        }

        /// <summary>
        /// 数値のコレクションから<see cref="ValueRange"/>を組み立てて列挙します。
        /// </summary>
        /// <param name="source">読み込むコレクション</param>
        /// <returns><paramref name="source"/>の数値を結合した<see cref="ValueRange"/>のコレクション</returns>
        private static IEnumerable<ValueRange> IterateRanges(IEnumerable<int> source)
        {
            if (source is ValueRange vrStruct)
            {
                yield return vrStruct;
                yield break;
            }

            using IEnumerator<int> enumerator = source.GetEnumerator();
            int? startValue = null;
            int? prevValue = null;

            while (enumerator.MoveNext())
            {
                int currentValue = enumerator.Current;
                if (startValue.HasValue)
                {
                    int prevValueActual = prevValue!.Value;
                    if (currentValue == prevValueActual + 1) prevValue = currentValue;
                    else
                    {
                        yield return new ValueRange(startValue.Value, prevValue.Value);
                        startValue = currentValue;
                        prevValue = currentValue;
                    }
                }
                else
                {
                    startValue = currentValue;
                    prevValue = currentValue;
                }
            }

            if (startValue.HasValue) yield return new ValueRange(startValue.Value, prevValue!.Value);
        }

        /// <summary>
        /// 文字列から変換します。
        /// </summary>
        /// <param name="s">変換する文字列</param>
        /// <returns><paramref name="s"/>の変換結果</returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/>が<see langword="null"/></exception>
        /// <exception cref="FormatException">フォーマットが無効</exception>
        /// <exception cref="OverflowException">値がオーバーフローしている</exception>
        public static ValueRangeCollection Parse(string s) => Parse(s, null);

        /// <summary>
        /// 文字列から変換します。
        /// </summary>
        /// <param name="s">変換する文字列</param>
        /// <param name="provider">カルチャ依存のフォーマット情報を表すオブジェクト</param>
        /// <returns><paramref name="s"/>の変換結果</returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/>が<see langword="null"/></exception>
        /// <exception cref="FormatException">フォーマットが無効</exception>
        /// <exception cref="OverflowException">値がオーバーフローしている</exception>
        public static ValueRangeCollection Parse(string s, IFormatProvider? provider)
        {
            ThrowHelpers.ThrowIfNull(s);

            return ParseCore(s.AsSpan(), provider);
        }

        /// <summary>
        /// 文字列から変換します。
        /// </summary>
        /// <param name="s">変換する文字列</param>
        /// <returns><paramref name="s"/>の変換結果</returns>
        /// <exception cref="FormatException">フォーマットが無効</exception>
        /// <exception cref="OverflowException">値がオーバーフローしている</exception>
        public static ValueRangeCollection Parse(ReadOnlySpan<char> s) => Parse(s, null);

        /// <summary>
        /// 文字列から変換します。
        /// </summary>
        /// <param name="s">変換する文字列</param>
        /// <param name="provider">カルチャ依存のフォーマット情報を表すオブジェクト</param>
        /// <returns><paramref name="s"/>の変換結果</returns>
        /// <exception cref="FormatException">フォーマットが無効</exception>
        /// <exception cref="OverflowException">値がオーバーフローしている</exception>
        public static ValueRangeCollection Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
        {
            return ParseCore(s, provider);
        }

        /// <summary>
        /// 文字列から変換します。
        /// </summary>
        /// <param name="s">変換する文字列</param>
        /// <param name="provider">カルチャ依存のフォーマット情報を表すオブジェクト</param>
        /// <returns><paramref name="s"/>の変換結果</returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/>が<see langword="null"/></exception>
        /// <exception cref="FormatException">フォーマットが無効</exception>
        /// <exception cref="OverflowException">値がオーバーフローしている</exception>
        private static ValueRangeCollection ParseCore(ReadOnlySpan<char> s, IFormatProvider? provider)
        {
            s = s.Trim();

            if (s.Length == 0) return [];

            var list = new List<ValueRange>();
            int separatorIndex = s.IndexOf(Separator.AsSpan(), StringComparison.Ordinal);

            while (separatorIndex >= 0)
            {
                if (separatorIndex == s.Length - 1) throw new FormatException();
                list.Add(ValueRange.Parse(s[..separatorIndex], provider));

                s = s[(separatorIndex + 1)..];
                separatorIndex = s.IndexOf(Separator.AsSpan(), StringComparison.Ordinal);
            }
            list.Add(ValueRange.Parse(s, provider));

            return new ValueRangeCollection(list);
        }

        /// <summary>
        /// 文字列から変換します。
        /// </summary>
        /// <param name="s">変換する文字列</param>
        /// <param name="result"><paramref name="s"/>の変換結果，変換に失敗した場合は既定値</param>
        /// <returns><paramref name="result"/>の変換に成功した場合は<see langword="true"/>，それ以外で<see langword="false"/></returns>
        public static bool TryParse([NotNullWhen(true)] string? s, [NotNullWhen(true)] out ValueRangeCollection? result)
        {
            if (s is null)
            {
                result = default;
                return false;
            }

            return TryParse(s, null, out result);
        }

        /// <summary>
        /// 文字列から変換します。
        /// </summary>
        /// <param name="s">変換する文字列</param>
        /// <param name="provider">カルチャ依存のフォーマット情報を表すオブジェクト</param>
        /// <param name="result"><paramref name="s"/>の変換結果，変換に失敗した場合は既定値</param>
        /// <returns><paramref name="result"/>の変換に成功した場合は<see langword="true"/>，それ以外で<see langword="false"/></returns>
        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [NotNullWhen(true)] out ValueRangeCollection? result)
        {
            if (s is null)
            {
                result = default;
                return false;
            }

            return TryParseCore(s.AsSpan(), provider, out result);
        }

        /// <summary>
        /// 文字列から変換します。
        /// </summary>
        /// <param name="s">変換する文字列</param>
        /// <param name="result"><paramref name="s"/>の変換結果，変換に失敗した場合は既定値</param>
        /// <returns><paramref name="result"/>の変換に成功した場合は<see langword="true"/>，それ以外で<see langword="false"/></returns>
        public static bool TryParse(ReadOnlySpan<char> s, [NotNullWhen(true)] out ValueRangeCollection? result)
        {
            return TryParse(s, null, out result);
        }

        /// <summary>
        /// 文字列から変換します。
        /// </summary>
        /// <param name="s">変換する文字列</param>
        /// <param name="provider">カルチャ依存のフォーマット情報を表すオブジェクト</param>
        /// <param name="result"><paramref name="s"/>の変換結果，変換に失敗した場合は既定値</param>
        /// <returns><paramref name="result"/>の変換に成功した場合は<see langword="true"/>，それ以外で<see langword="false"/></returns>
        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [NotNullWhen(true)] out ValueRangeCollection? result)
        {
            return TryParseCore(s, provider, out result);
        }

        /// <summary>
        /// 文字列から変換します。
        /// </summary>
        /// <param name="s">変換する文字列</param>
        /// <param name="provider">カルチャ依存のフォーマット情報を表すオブジェクト</param>
        /// <param name="result"><paramref name="s"/>の変換結果，変換に失敗した場合は既定値</param>
        /// <returns><paramref name="result"/>の変換に成功した場合は<see langword="true"/>，それ以外で<see langword="false"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryParseCore(ReadOnlySpan<char> s, IFormatProvider? provider, [NotNullWhen(true)] out ValueRangeCollection? result)
        {
            if (s.Length == 0)
            {
                result = [];
                return true;
            }

            var list = new List<ValueRange>();
            int separatorIndex = s.IndexOf(Separator.AsSpan(), StringComparison.Ordinal);

            while (separatorIndex >= 0)
            {
                if (separatorIndex == s.Length - 1 || !ValueRange.TryParse(s[..separatorIndex], provider, out ValueRange current))
                {
                    result = default;
                    return false;
                }
                list.Add(current);

                s = s[(separatorIndex + 1)..];
                separatorIndex = s.IndexOf(Separator.AsSpan(), StringComparison.Ordinal);
            }
            if (!ValueRange.TryParse(s, provider, out ValueRange last))
            {
                result = default;
                return false;
            }
            list.Add(last);
            result = new ValueRangeCollection(list);

            return true;
        }

        #region Collection Interface Implementation

        /// <summary>
        /// <see cref="ValueRangeCollection"/>に格納されている要素数を取得します。
        /// </summary>
        public int Count => ranges.Count;

        /// <inheritdoc/>
        public ValueRange this[int index]
        {
            get => ranges[index];
            set => ranges[index] = value;
        }

        /// <summary>
        /// 末尾に範囲を追加します。
        /// </summary>
        /// <param name="range">追加する範囲</param>
        public void Add(ValueRange range) => ranges.Add(range);

        /// <summary>
        /// 全ての要素を削除します。
        /// </summary>
        public void Clear() => ranges.Clear();

        /// <summary>
        /// 対象の範囲が格納されているかどうかを検証します。
        /// </summary>
        /// <param name="range">検索する範囲</param>
        /// <returns><paramref name="range"/>が格納されていたら<see langword="true"/>，それ以外で<see langword="false"/></returns>
        public bool Contains(ValueRange range) => ranges.Contains(range);

        /// <summary>
        /// 配列に要素をコピーします。
        /// </summary>
        /// <param name="array">コピー先の配列</param>
        /// <param name="arrayIndex"><paramref name="array"/>におけるコピー開始インデックス</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/>が0未満</exception>
        public void CopyTo(ValueRange[] array, int arrayIndex) => ranges.CopyTo(array, arrayIndex);

        /// <inheritdoc/>
        public IEnumerator<ValueRange> GetEnumerator() => ranges.GetEnumerator();

        /// <summary>
        /// 対象の範囲のうち先頭のもののインデックスを取得します。
        /// </summary>
        /// <param name="range">検索する要素</param>
        /// <returns><paramref name="range"/>の先頭のもののインデックス，見つからない場合は-1</returns>
        public int IndexOf(ValueRange range) => ranges.IndexOf(range);

        /// <summary>
        /// 指定したインデックスへ範囲を挿入します。
        /// </summary>
        /// <param name="index">挿入先のインデックス</param>
        /// <param name="range">挿入する範囲</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/>が0未満または<see cref="Count"/>より大きい</exception>
        public void Insert(int index, ValueRange range) => ranges.Insert(index, range);

        /// <summary>
        /// 対象の範囲のうち先頭のものを削除します。
        /// </summary>
        /// <param name="range">削除する範囲</param>
        /// <returns><paramref name="range"/>の削除に成功したら<see langword="true"/>，それ以外で<see langword="false"/></returns>
        public bool Remove(ValueRange range) => ranges.Remove(range);

        /// <summary>
        /// 指定したインデックスの要素を削除します。
        /// </summary>
        /// <param name="index">削除する要素のインデックス</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/>が0未満または<see cref="Count"/>以上</exception>
        public void RemoveAt(int index) => ranges.RemoveAt(index);

        #region Explicit

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion IEnumerable

        #region ICollection

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;

        void ICollection.CopyTo(Array array, int index) => ((ICollection)ranges).CopyTo(array, index);

        #endregion ICollection

        #region ICollection<T>

        bool ICollection<ValueRange>.IsReadOnly => false;

        #endregion ICollection<T>

        #region IList

        bool IList.IsFixedSize => false;

        bool IList.IsReadOnly => false;

        object? IList.this[int index]
        {
            get => ((IList)ranges)[index];
            set => ((IList)ranges)[index] = value;
        }

        int IList.Add(object? value) => ((IList)ranges).Add(value);

        bool IList.Contains(object? value) => ((IList)ranges).Contains(value);

        int IList.IndexOf(object? value) => ((IList)ranges).IndexOf(value);

        void IList.Insert(int index, object? value) => ((IList)ranges).Insert(index, value);

        void IList.Remove(object? value) => ((IList)ranges).Remove(value);

        #endregion IList

        #endregion Explicit

        #endregion Collection Interface Implementation

        /// <summary>
        /// 範囲を表す文字列を取得します。
        /// </summary>
        /// <returns>範囲を表す文字列</returns>
        public override string ToString() => ToString(null, null);

        /// <inheritdoc/>
        public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
        {
            return string.Join(Separator, ranges.Select(x => x.ToString(format, provider)));
        }

#if NET6_0_OR_GREATER

        /// <summary>
        /// 文字列を対象の<see cref="Span{T}"/>へ出力します。
        /// </summary>
        /// <param name="destination">出力先の<see cref="Span{T}"/></param>
        /// <param name="charsWritten"><paramref name="destination"/>へ書き込まれた文字数</param>
        /// <param name="format">出力フォーマット</param>
        /// <param name="provider">カルチャ依存のフォーマット情報を表すオブジェクト</param>
        /// <returns>文字列の書き込みに成功したら<see langword="true"/>，それ以外で<see langword="false"/></returns>
        public bool TryFormat(Span<char> destination, out int charsWritten, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
        {
            if (Count == 0)
            {
                charsWritten = 0;
                return true;
            }

            Span<ValueRange> rangesSpan = CollectionsMarshal.AsSpan(ranges);

            bool currentResult = rangesSpan[0].TryFormat(destination, out int currentCharsWritten, format, provider);
            charsWritten = currentCharsWritten;
            if (!currentResult || currentCharsWritten == destination.Length) return false;

            destination = destination[currentCharsWritten..];

            for (int i = 1; i < ranges.Count; i++)
            {
                currentResult = destination.TryWrite(provider, $"{Separator}", out currentCharsWritten);
                charsWritten += currentCharsWritten;
                if (!currentResult || currentCharsWritten == destination.Length) return false;
                destination = destination[currentCharsWritten..];

                currentResult = ranges[i].TryFormat(destination, out currentCharsWritten, format, provider);
                charsWritten += currentCharsWritten;
                if (!currentResult) return false;
                if (currentCharsWritten == destination.Length)
                {
                    if (i < rangesSpan.Length - 1) return false;
                }
                else destination = destination[currentCharsWritten..];
            }

            return true;
        }

#endif
    }
}
