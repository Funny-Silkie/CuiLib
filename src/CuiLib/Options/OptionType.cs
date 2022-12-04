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
        None = 0b0000,

        /// <summary>
        /// フラグを表します。
        /// </summary>
        Flag = 0b0001,

        /// <summary>
        /// 値を取るオプションを表します。
        /// </summary>
        Valued = 0b0010,

        /// <summary>
        /// 1つの値をとります。
        /// </summary>
        SingleValue = 0b0100,

        /// <summary>
        /// 複数の値をとります。
        /// </summary>
        MultiValue = 0b1000,
    }
}
