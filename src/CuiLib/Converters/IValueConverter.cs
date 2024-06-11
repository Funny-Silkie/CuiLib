namespace CuiLib.Converters
{
    /// <summary>
    /// 値の変換を行う基底クラスです。
    /// </summary>
    /// <typeparam name="TIn">変換前の型</typeparam>
    /// <typeparam name="TOut">変換後の型</typeparam>
    public interface IValueConverter<in TIn, out TOut>
    {
        /// <summary>
        /// 値の変換を行います。
        /// </summary>
        /// <param name="value">変換する値</param>
        /// <returns>変換後の値</returns>
        TOut Convert(TIn value);
    }
}
