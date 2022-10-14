using System;
using System.Diagnostics.CodeAnalysis;

namespace CuiLib.Options
{
    /// <summary>
    /// 値のチェック結果を表します。
    /// </summary>
    [Serializable]
    public readonly struct ValueCheckState : IEquatable<ValueCheckState>
    {
        /// <summary>
        /// 値が正常であることを表すインスタンスを取得します。
        /// </summary>
        public static ValueCheckState Success { get; } = new ValueCheckState(true, null);

        /// <summary>
        /// 値が有効かどうかを表す値を取得します。
        /// </summary>
        [MemberNotNullWhen(false, "Error")]
        public bool IsValid { get; }

        /// <summary>
        /// エラーメッセージを取得します。
        /// </summary>
        public string? Error { get; }

        /// <summary>
        /// <see cref="ValueCheckState"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="isValid">値が有効かどうか</param>
        /// <param name="error">エラーメッセージ</param>
        private ValueCheckState(bool isValid, string? error)
        {
            IsValid = isValid;
            Error = error;
        }

        /// <summary>
        /// エラーとして<see cref="ValueCheckState"/>のインスタンスを生成します。
        /// </summary>
        /// <param name="error">エラーメッセージ</param>
        /// <returns>無効な結果を表すインスタンス</returns>
        public static ValueCheckState AsError(string? error)
        {
            return new ValueCheckState(false, error);
        }

        /// <inheritdoc/>
        public bool Equals(ValueCheckState other)
        {
            return IsValid == other.IsValid
                   && Error == other.Error;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is ValueCheckState state && Equals(state);

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(IsValid, Error);

#pragma warning disable CS1591 // 公開されている型またはメンバーの XML コメントがありません

        public static bool operator ==(ValueCheckState left, ValueCheckState right) => left.Equals(right);

        public static bool operator !=(ValueCheckState left, ValueCheckState right) => !(left == right);

#pragma warning restore CS1591 // 公開されている型またはメンバーの XML コメントがありません
    }
}
