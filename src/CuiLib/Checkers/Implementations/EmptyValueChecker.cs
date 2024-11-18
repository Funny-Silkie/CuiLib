using CuiLib.Internal.Versions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CuiLib.Checkers.Implementations
{
    /// <summary>
    /// 文字列が空であるかを検証します。
    /// </summary>
    [Serializable]
    internal sealed class EmptyValueChecker : IValueChecker<string?>
    {
        /// <summary>
        /// <see cref="EmptyValueChecker"/>の新しいインスタンスを初期化します。
        /// </summary>
        internal EmptyValueChecker()
        {
        }

        /// <inheritdoc/>
        public ValueCheckState CheckValue(string? value)
        {
            if (!string.IsNullOrEmpty(value)) return ValueCheckState.AsError("文字が入力されています");
            return ValueCheckState.Success;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is EmptyValueChecker;

        /// <inheritdoc/>
        public override int GetHashCode() => GetType().Name.GetHashCode();
    }

    /// <summary>
    /// コレクションが空であるかを検証します。
    /// </summary>
    /// <typeparam name="TElement">値の型</typeparam>
    [Serializable]
    internal sealed class EmptyValueChecker<TElement> : IValueChecker<IEnumerable<TElement>?>
    {
        /// <summary>
        /// <see cref="EmptyValueChecker{TElement}"/>の新しいインスタンスを初期化します。
        /// </summary>
        public EmptyValueChecker()
        {
        }

        /// <inheritdoc/>
        public ValueCheckState CheckValue(IEnumerable<TElement>? value)
        {
            bool isEmpty = value is null || value.TryGetNonEnumeratedCount(out int count) && count == 0 || !value.Any();
            if (isEmpty) return ValueCheckState.Success;

            return ValueCheckState.AsError("要素が存在します");
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is EmptyValueChecker<TElement>;

        /// <inheritdoc/>
        public override int GetHashCode() => GetType().Name.GetHashCode();
    }
}
