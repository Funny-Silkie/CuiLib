using System;

namespace CuiLib.Checkers.Implementations
{
    /// <summary>
    /// 列挙型が定義された値かどうかを検証します。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    internal sealed class DefinedEnumValueChecker<T> : IValueChecker<T>
        where T : struct, Enum
    {
        /// <summary>
        /// <see cref="DefinedEnumValueChecker{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        internal DefinedEnumValueChecker()
        {
        }

        /// <inheritdoc/>

#if NET6_0_OR_GREATER

        public ValueCheckState CheckValue(T value)
        {
            if (Enum.IsDefined(value)) return ValueCheckState.Success;
            return ValueCheckState.AsError($"定義されていない値です。[{string.Join(", ", Enum.GetNames<T>())}]の中から選択してください");
        }

#else

        public ValueCheckState CheckValue(T value)
        {
            if (Enum.IsDefined(typeof(T), value)) return ValueCheckState.Success;
            return ValueCheckState.AsError($"定義されていない値です。[{string.Join(", ", Enum.GetNames(typeof(T)))}]の中から選択してください");
        }

#endif

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is DefinedEnumValueChecker<T>;

        /// <inheritdoc/>
        public override int GetHashCode() => GetType().Name.GetHashCode();
    }
}
