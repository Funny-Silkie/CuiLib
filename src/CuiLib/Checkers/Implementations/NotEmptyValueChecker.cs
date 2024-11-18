using CuiLib.Internal.Versions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CuiLib.Checkers.Implementations
{
    /// <summary>
    /// 文字列が空でないかを検証します。
    /// </summary>
    [Serializable]
    internal sealed class NotEmptyValueChecker : IValueChecker<string?>
    {
        /// <summary>
        /// <see cref="NotEmptyValueChecker"/>の新しいインスタンスを初期化します。
        /// </summary>
        internal NotEmptyValueChecker()
        {
        }

        /// <inheritdoc/>
        public ValueCheckState CheckValue(string? value)
        {
            if (string.IsNullOrEmpty(value)) return ValueCheckState.AsError("空文字です");
            return ValueCheckState.Success;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is NotEmptyValueChecker;

        /// <inheritdoc/>
        public override int GetHashCode() => GetType().Name.GetHashCode();
    }

    /// <summary>
    /// コレクションが空でないかを検証します。
    /// </summary>
    /// <typeparam name="TElement">値の型</typeparam>
    [Serializable]
    internal sealed class NotEmptyValueChecker<TElement> : IValueChecker<IEnumerable<TElement>?>
    {
        /// <summary>
        /// <see cref="NotEmptyValueChecker{TElement}"/>の新しいインスタンスを初期化します。
        /// </summary>
        public NotEmptyValueChecker()
        {
        }

        /// <inheritdoc/>
        public ValueCheckState CheckValue(IEnumerable<TElement>? value)
        {
            bool isEmpty = value is null || value.TryGetNonEnumeratedCount(out int count) && count == 0 || !value.Any();
            if (!isEmpty) return ValueCheckState.Success;

            return ValueCheckState.AsError("コレクションが空です");
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is NotEmptyValueChecker<TElement>;

        /// <inheritdoc/>
        public override int GetHashCode() => GetType().Name.GetHashCode();
    }
}
