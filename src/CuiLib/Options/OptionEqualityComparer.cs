using System;
using System.Collections.Generic;

namespace CuiLib.Options
{
    /// <summary>
    /// <see cref="Option"/>を比較する<see cref="IEqualityComparer{T}"/>の実装です。
    /// </summary>
    [Serializable]
    internal sealed class OptionEqualityComparer : IEqualityComparer<Option>
    {
        /// <summary>
        /// <see cref="OptionEqualityComparer"/>の新しいインスタンスを初期化します。
        /// </summary>
        internal OptionEqualityComparer()
        {
        }

        /// <inheritdoc/>
        public bool Equals(Option? x, Option? y)
        {
            if (x is null) return y is null;
            if (y is null) return false;
            return x.ShortName == y.ShortName && x.FullName == y.FullName;
        }

        /// <inheritdoc/>
        public int GetHashCode(Option obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            return HashCode.Combine(obj.ShortName, obj.FullName);
        }
    }
}
