using System;
using CuiLib.Internal.Versions;

namespace CuiLib.Converters.Implementations
{
    /// <summary>
    /// 配列を生成する<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
    /// </summary>
    [Serializable]
    internal sealed class ArrayValueConverter : IValueConverter<string, Array>
    {
        /// <summary>
        /// 要素の変換を行う<see cref="IValueConverter{TIn, TOut}"/>のインスタンスを取得します。
        /// </summary>
        public IValueConverter<string, object?> ElementConverter { get; }

        /// <summary>
        /// 要素の型を取得します。
        /// </summary>
        public Type ElementType { get; }

        /// <summary>
        /// 要素の区切り文字を取得します。
        /// </summary>
        public string Separator { get; }

        /// <summary>
        /// 文字列の分割時のオプションを取得します。
        /// </summary>
        public StringSplitOptions SplitOptions { get; }

        /// <summary>
        /// <see cref="ArrayValueConverter"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="elementType">要素の型</param>
        /// <param name="separator">区切り文字</param>
        /// <param name="converter">要素の変換を行う<see cref="IValueConverter{TIn, TOut}"/>のインスタンス</param>
        /// <param name="splitOptions">文字列分割時のオプション</param>
        internal ArrayValueConverter(string separator, Type elementType, IValueConverter<string, object?> converter, StringSplitOptions splitOptions)
        {
            ThrowHelpers.ThrowIfNull(elementType);
            ThrowHelpers.ThrowIfNull(converter);
            ThrowHelpers.ThrowIfNullOrEmpty(separator);

            Separator = separator;
            ElementType = elementType;
            ElementConverter = converter;
            SplitOptions = splitOptions;
        }

        /// <inheritdoc/>
        public Array Convert(string value)
        {
            ThrowHelpers.ThrowIfNull(value);

            if (value.Length == 0) return Array.CreateInstance(ElementType, 0);

            string[] elements = value.Split(Separator, SplitOptions);
            Array result = Array.CreateInstance(ElementType, elements.Length);
            for (int i = 0; i < elements.Length; i++) result.SetValue(ElementConverter.Convert(elements[i]), i);
            return result;
        }
    }

    /// <summary>
    /// 配列を生成する<see cref="IValueConverter{TIn, TOut}"/>のクラスです。
    /// </summary>
    /// <typeparam name="T">配列の要素の型</typeparam>
    [Serializable]
    internal sealed class ArrayValueConverter<T> : IValueConverter<string, T[]>
    {
        /// <summary>
        /// 要素の変換を行う<see cref="IValueConverter{TIn, TOut}"/>のインスタンスを取得します。
        /// </summary>
        public IValueConverter<string, T> ElementConverter { get; }

        /// <summary>
        /// 要素の区切り文字を取得します。
        /// </summary>
        public string Separator { get; }

        /// <summary>
        /// 文字列の分割時のオプションを取得します。
        /// </summary>
        public StringSplitOptions SplitOptions { get; }

        /// <summary>
        /// <see cref="ArrayValueConverter{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="separator">区切り文字</param>
        /// <param name="converter">要素の変換を行う<see cref="IValueConverter{TIn, TOut}"/>のインスタンス</param>
        /// <param name="splitOptions">文字列分割時のオプション</param>
        internal ArrayValueConverter(string separator, IValueConverter<string, T> converter, StringSplitOptions splitOptions)
        {
            ThrowHelpers.ThrowIfNull(converter);
            ThrowHelpers.ThrowIfNullOrEmpty(separator);

            Separator = separator;
            ElementConverter = converter;
            SplitOptions = splitOptions;
        }

        /// <inheritdoc/>
        public T[] Convert(string value)
        {
            ThrowHelpers.ThrowIfNull(value);

            if (value.Length == 0) return [];

            string[] elements = value.Split(Separator, SplitOptions);
            return Array.ConvertAll(elements, ElementConverter.Convert);
        }
    }
}
