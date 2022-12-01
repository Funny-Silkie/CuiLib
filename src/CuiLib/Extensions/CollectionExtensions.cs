using System;
using System.Collections.Generic;

namespace CuiLib.Extensions
{
    /// <summary>
    /// コレクションの拡張を表します。
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// インデックスに対応する要素を取得します。
        /// </summary>
        /// <typeparam name="T">要素の型</typeparam>
        /// <param name="list">リスト</param>
        /// <param name="index">インデックス</param>
        /// <returns><paramref name="index"/>が<paramref name="list"/>の範囲内の場合，そのインデックスの要素。それ以外で<typeparamref name="T"/>の既定値</returns>
        /// <exception cref="ArgumentNullException"><paramref name="list"/>がnull</exception>
        public static T GetOrDefault<T>(this IList<T> list, int index)
        {
            return GetOrDefault(list, index, default!);
        }

        /// <summary>
        /// インデックスに対応する要素を取得します。
        /// </summary>
        /// <typeparam name="T">要素の型</typeparam>
        /// <param name="list">リスト</param>
        /// <param name="index">インデックス</param>
        /// <param name="defaultValue">既定値</param>
        /// <returns><paramref name="index"/>が<paramref name="list"/>の範囲内の場合，そのインデックスの要素。それ以外で<paramref name="defaultValue"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="list"/>がnull</exception>
        public static T GetOrDefault<T>(this IList<T> list, int index, T defaultValue)
        {
            ArgumentNullException.ThrowIfNull(list);

            if ((uint)index >= (uint)list.Count) return defaultValue;

            return list[index];
        }
    }
}
