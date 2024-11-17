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
        private readonly IValueConverter<string, object?> elementConverter;
        private readonly Type elementType;
        private readonly string separator;
        private readonly StringSplitOptions splitOptions;

        /// <summary>
        /// <see cref="ArrayValueConverter{T}"/>の新しいインスタンスを初期化します。
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

            this.separator = separator;
            this.elementType = elementType;
            elementConverter = converter;
            this.splitOptions = splitOptions;
        }

        /// <inheritdoc/>
        public Array Convert(string value)
        {
            ThrowHelpers.ThrowIfNull(value);

            if (value.Length == 0) return Array.CreateInstance(elementType, 0);

            string[] elements = value.Split(separator, splitOptions);
            Array result = Array.CreateInstance(elementType, elements.Length);
            for (int i = 0; i < elements.Length; i++) result.SetValue(elementConverter.Convert(elements[i]), i);
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
        private readonly IValueConverter<string, T> elementConverter;
        private readonly string separator;
        private readonly StringSplitOptions splitOptions;

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

            this.separator = separator;
            elementConverter = converter;
            this.splitOptions = splitOptions;
        }

        /// <inheritdoc/>
        public T[] Convert(string value)
        {
            ThrowHelpers.ThrowIfNull(value);

            if (value.Length == 0) return [];

            string[] elements = value.Split(separator, splitOptions);
            return Array.ConvertAll(elements, elementConverter.Convert);
        }
    }
}
