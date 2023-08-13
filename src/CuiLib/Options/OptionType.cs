using System;

namespace CuiLib.Options
{
    /// <summary>
    /// オプションの種類を表します。
    /// </summary>
    [Flags]
    [Serializable]
    internal enum OptionType
    {
        /// <summary>
        /// デフォルト値
        /// </summary>
        /// <remarks>この値であってはならない</remarks>
        None = 0,

        /// <summary>
        /// フラグを表します。
        /// </summary>
        Flag = 1 << 0,

        /// <summary>
        /// 値を取るオプションを表します。
        /// </summary>
        Valued = 1 << 1,

        /// <summary>
        /// 1つの値をとります。
        /// </summary>
        SingleValue = 1 << 2,

        /// <summary>
        /// 複数の値をとります。
        /// </summary>
        MultiValue = 1 << 3,

        /// <summary>
        /// グループを表します。
        /// </summary>
        Group = 1 << 4,
    }
}
