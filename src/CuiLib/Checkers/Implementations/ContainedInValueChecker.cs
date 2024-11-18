using System;
using System.Collections.Generic;
using System.Linq;

namespace CuiLib.Checkers.Implementations
{
    /// <summary>
    /// 要素がコレクションに含まれているかどうかを検証します。
    /// </summary>
    /// <typeparam name="TCollection">コレクションの型</typeparam>
    /// <typeparam name="TElement">要素の型</typeparam>
    [Serializable]
    internal sealed class ContainedInValueChecker<TCollection, TElement> : IValueChecker<TElement>
        where TCollection : IEnumerable<TElement>
    {
        /// <summary>
        /// 対象のコレクションを取得します。
        /// </summary>
        public TCollection Source { get; }

        public IEqualityComparer<TElement> Comparer { get; }

        /// <summary>
        /// <see cref="ContainedInValueChecker{TCollection, TElement}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="source">候補のコレクション</param>
        /// <param name="comparer">要素を比較するオブジェクト。nullで<see cref="EqualityComparer{T}.Default"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="source"/>が空</exception>
        internal ContainedInValueChecker(TCollection source, IEqualityComparer<TElement>? comparer)
        {
            ThrowHelpers.ThrowIfNull(source);
            if (!source.Any()) throw new ArgumentException("空のコレクションです", nameof(source));

            Source = source;
            Comparer = comparer ?? EqualityComparer<TElement>.Default;
        }

        /// <inheritdoc/>
        public ValueCheckState CheckValue(TElement value)
        {
            if (Source.Contains(value, Comparer)) return ValueCheckState.Success;
            return ValueCheckState.AsError($"値が含まれていません。[{string.Join(", ", Source)}]の何れかを選択してください");
        }
    }
}
