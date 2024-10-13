using CuiLib.Internal.Versions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace CuiLib.Data
{
    /// <summary>
    /// 値の範囲を表します。
    /// </summary>
    [Serializable]
    public readonly struct ValueRange : IEquatable<ValueRange>, IEnumerable<int>, IFormattable
#if NET6_0_OR_GREATER
        , ISpanFormattable
#endif
#if NET7_0_OR_GREATER
        , ISpanParsable<ValueRange>
#endif
    {
        /// <summary>
        /// 未確定の範囲を表す値
        /// </summary>
        internal const int NotExactValue = -1;

        /// <summary>
        /// 開始値を取得します。
        /// </summary>
        /// <remarks>値が明示されていない場合は-1</remarks>
        public int Start { get; }

        /// <summary>
        /// 終了値を取得します。
        /// </summary>
        /// <remarks>値が明示されていない場合は-1</remarks>
        public int End { get; }

        /// <summary>
        /// <see cref="ValueRange"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="value">整数値</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/>が0未満</exception>
        public ValueRange(int value)
        {
            ThrowHelpers.ThrowIfNegative(value);

            Start = End = value;
        }

        /// <summary>
        /// <see cref="ValueRange"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="start">開始値</param>
        /// <param name="end">終了値</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/>または<paramref name="end"/>が0未満</exception>
        /// <exception cref="ArgumentException"><paramref name="start"/>が<paramref name="end"/>より大きい</exception>
        public ValueRange(int start, int end)
        {
            ThrowHelpers.ThrowIfNegative(start);
            ThrowHelpers.ThrowIfNegative(end);
            if (start > end) throw new ArgumentException("開始値は終了値以下である必要があります", nameof(start));

            Start = start;
            End = end;
        }

        /// <summary>
        /// <see cref="ValueRange"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="start">開始値</param>
        /// <param name="end">終了値</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/>または<paramref name="end"/>が0未満</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="start"/>と<paramref name="end"/>が両方とも<see langword="null"/>
        /// -または-
        /// <paramref name="start"/>が<paramref name="end"/>より大きい
        /// </exception>
        public ValueRange(int? start = null, int? end = null)
        {
            if (start.HasValue)
            {
                if (end.HasValue)
                {
                    this = new ValueRange(start.Value, end.Value);
                    return;
                }

                int startValue = start.Value;
                ThrowHelpers.ThrowIfNegative(startValue);

                Start = startValue;
                End = NotExactValue;
                return;
            }
            if (end.HasValue)
            {
                int endValue = end.Value;
                ThrowHelpers.ThrowIfNegative(endValue);

                Start = NotExactValue;
                End = endValue;
                return;
            }

            throw new ArgumentException("開始値と終了値が両方ともnullです");
        }

        /// <summary>
        /// 二つの値の間を表す<see cref="ValueRange"/>の新しいインスタンスを生成します。
        /// </summary>
        /// <param name="value1">値1</param>
        /// <param name="value2">値2</param>
        /// <returns>二つの値の間を表す<see cref="ValueRange"/>の新しいインスタンス</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value1"/>または<paramref name="value2"/>が0未満</exception>
        public static ValueRange Between(int value1, int value2)
        {
            if (value1 < value2) return new ValueRange(value1, value2);
            else return new ValueRange(value2, value1);
        }

        /// <summary>
        /// 文字列から変換します。
        /// </summary>
        /// <param name="s">変換する文字列</param>
        /// <returns><paramref name="s"/>の変換結果</returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/>が<see langword="null"/></exception>
        /// <exception cref="FormatException">フォーマットが無効</exception>
        /// <exception cref="OverflowException">値がオーバーフローしている</exception>
        public static ValueRange Parse(string s) => Parse(s, null);

        /// <summary>
        /// 文字列から変換します。
        /// </summary>
        /// <param name="s">変換する文字列</param>
        /// <param name="provider">カルチャ依存のフォーマット情報を表すオブジェクト</param>
        /// <returns><paramref name="s"/>の変換結果</returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/>が<see langword="null"/></exception>
        /// <exception cref="FormatException">フォーマットが無効</exception>
        /// <exception cref="OverflowException">値がオーバーフローしている</exception>
        public static ValueRange Parse(string s, IFormatProvider? provider)
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
        public static ValueRange Parse(ReadOnlySpan<char> s) => Parse(s, null);

        /// <summary>
        /// 文字列から変換します。
        /// </summary>
        /// <param name="s">変換する文字列</param>
        /// <param name="provider">カルチャ依存のフォーマット情報を表すオブジェクト</param>
        /// <returns><paramref name="s"/>の変換結果</returns>
        /// <exception cref="FormatException">フォーマットが無効</exception>
        /// <exception cref="OverflowException">値がオーバーフローしている</exception>
        public static ValueRange Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
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
        private static ValueRange ParseCore(ReadOnlySpan<char> s, IFormatProvider? provider)
        {
            if (s.Length == 0) throw new FormatException("空文字です");

            s = s.Trim();

            if (s[0] == '-') return new ValueRange(end: VersionBufferExtensions.ParseToInt32(s[1..], provider));
            if (s[^1] == '-') return new ValueRange(start: VersionBufferExtensions.ParseToInt32(s[..^1], provider));

            int separatorIndex = s.IndexOf('-');
            if (separatorIndex == -1) return new ValueRange(VersionBufferExtensions.ParseToInt32(s, provider));

            if (s.LastIndexOf('-') != separatorIndex) throw new FormatException("区切り文字が複数あります");

            int start = VersionBufferExtensions.ParseToInt32(s[..separatorIndex], provider);
            int end = VersionBufferExtensions.ParseToInt32(s[(separatorIndex + 1)..], provider);
            if (start > end) throw new FormatException("開始値と終了値の大小関係が無効です");

            return new ValueRange(start, end);
        }

        /// <summary>
        /// 文字列から変換します。
        /// </summary>
        /// <param name="s">変換する文字列</param>
        /// <param name="result"><paramref name="s"/>の変換結果，変換に失敗した場合は既定値</param>
        /// <returns><paramref name="result"/>の変換に成功した場合は<see langword="true"/>，それ以外で<see langword="false"/></returns>
        public static bool TryParse([NotNullWhen(true)] string? s, out ValueRange result)
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
        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out ValueRange result)
        {
            return TryParseCore(s.AsSpan(), provider, out result);
        }

        /// <summary>
        /// 文字列から変換します。
        /// </summary>
        /// <param name="s">変換する文字列</param>
        /// <param name="result"><paramref name="s"/>の変換結果，変換に失敗した場合は既定値</param>
        /// <returns><paramref name="result"/>の変換に成功した場合は<see langword="true"/>，それ以外で<see langword="false"/></returns>
        public static bool TryParse(ReadOnlySpan<char> s, out ValueRange result)
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
        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out ValueRange result)
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
        private static bool TryParseCore(ReadOnlySpan<char> s, IFormatProvider? provider, out ValueRange result)
        {
            if (s.Length == 0)
            {
                result = default;
                return false;
            }

            s = s.Trim();

            if (s[0] == '-')
            {
                if (!VersionBufferExtensions.TryParseToInt32(s[1..], provider, out int value))
                {
                    result = default;
                    return false;
                }

                result = new ValueRange(end: value);
                return true;
            }
            if (s[^1] == '-')
            {
                if (!VersionBufferExtensions.TryParseToInt32(s[..^1], provider, out int value))
                {
                    result = default;
                    return false;
                }

                result = new ValueRange(start: value);
                return true;
            }

            int separatorIndex = s.IndexOf('-');
            if (separatorIndex == -1)
            {
                if (!VersionBufferExtensions.TryParseToInt32(s, provider, out int value))
                {
                    result = default;
                    return false;
                }
                result = new ValueRange(value);
                return true;
            }

            if (s.LastIndexOf('-') != separatorIndex)
            {
                result = default;
                return false;
            }

            if (!VersionBufferExtensions.TryParseToInt32(s[..separatorIndex], provider, out int start) ||
                !VersionBufferExtensions.TryParseToInt32(s[(separatorIndex + 1)..], provider, out int end) ||
                start > end)
            {
                result = default;
                return false;
            }

            result = new ValueRange(start, end);
            return true;
        }

        /// <inheritdoc/>
        public bool Equals(ValueRange other) => Start == other.Start && End == other.End;

        /// <inheritdoc/>
        public override bool Equals([NotNullWhen(true)] object? obj) => obj is ValueRange other && Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(Start, End);

        /// <summary>
        /// 指定した値が含まれているかどうかを検証します。
        /// </summary>
        /// <param name="value">検証する値</param>
        /// <returns><paramref name="value"/>が含まれていたら<see langword="true"/>，それ以外で<see langword="false"/></returns>
        public bool Contains(int value)
        {
            if (Start == NotExactValue) return 0 <= value && value <= End;
            if (End == NotExactValue) return Start <= value;
            return Start <= value && value <= End;
        }

        /// <inheritdoc/>
        /// <exception cref="InvalidOperationException"></exception>
        public IEnumerator<int> GetEnumerator()
        {
            if (Start == NotExactValue || End == NotExactValue) throw new InvalidOperationException();

            return GetEnumeratorCore();
        }

        private IEnumerator<int> GetEnumeratorCore()
        {
            for (int i = Start; i <= End; i++) yield return i;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// 範囲を表す文字列を取得します。
        /// </summary>
        /// <returns>範囲を表す文字列</returns>
        public override string ToString() => ToString(null, null);

        /// <inheritdoc/>
        public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? formatProvider = null)
        {
            if (Start == NotExactValue) return $"-{End.ToString(format, formatProvider)}";
            if (End == NotExactValue) return $"{Start.ToString(format, formatProvider)}-";
            if (Start == End) return Start.ToString(format, formatProvider);
            return $"{Start.ToString(format, formatProvider)}-{End.ToString(format, formatProvider)}";
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
            if (Start == NotExactValue) return destination.TryWrite(provider, $"-{End}", out charsWritten);
            if (End == NotExactValue) return destination.TryWrite(provider, $"{Start}-", out charsWritten);
            if (Start == End) return Start.TryFormat(destination, out charsWritten, format, provider);
            return destination.TryWrite(provider, $"{Start}-{End}", out charsWritten);
        }

#endif

        /// <summary>
        /// 開始値を与えられた<see cref="ValueRange"/>の新しいインスタンスを取得します。
        /// </summary>
        /// <param name="start">開始値</param>
        /// <returns>開始値が<paramref name="start"/>となるインスタンス。<see cref="Start"/>が既に明確な値を持つ場合は変更なし</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/>が0未満</exception>
        /// <exception cref="ArgumentException"><paramref name="start"/>が<see cref="End"/>より大きい</exception>
        public ValueRange WithExactStart(int start)
        {
            return Start == NotExactValue ? new ValueRange(start, End) : this;
        }

        /// <summary>
        /// 終了値を与えられた<see cref="ValueRange"/>の新しいインスタンスを取得します。
        /// </summary>
        /// <param name="end">開始値</param>
        /// <returns>開始値が<paramref name="end"/>となるインスタンス。<see cref="End"/>が既に明確な値を持つ場合は変更なし</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="end"/>が0未満</exception>
        /// <exception cref="ArgumentException"><paramref name="end"/>が<see cref="Start"/>より大きい</exception>
        public ValueRange WithExactEnd(int end)
        {
            return End == NotExactValue ? new ValueRange(Start, end) : this;
        }

        /// <summary>
        /// 二つの値の等価性を検証します。
        /// </summary>
        /// <param name="left">値1</param>
        /// <param name="right">値2</param>
        /// <returns><paramref name="left"/>と<paramref name="right"/>が等価で<see langword="true"/>，それ以外で<see langword="false"/></returns>
        public static bool operator ==(ValueRange left, ValueRange right) => left.Equals(right);

        /// <summary>
        /// 二つの値の非等価性を検証します。
        /// </summary>
        /// <param name="left">値1</param>
        /// <param name="right">値2</param>
        /// <returns><paramref name="left"/>と<paramref name="right"/>が非等価で<see langword="true"/>，それ以外で<see langword="false"/></returns>
        public static bool operator !=(ValueRange left, ValueRange right) => !(left == right);

        /// <summary>
        /// <see cref="int"/>から暗黙的に変換します。
        /// </summary>
        /// <param name="value">変換する値</param>
        public static implicit operator ValueRange(int value) => new ValueRange(value);
    }
}
