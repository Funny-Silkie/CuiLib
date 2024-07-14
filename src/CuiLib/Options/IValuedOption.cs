namespace CuiLib.Options
{
    /// <summary>
    /// 値を取るオプションを表します。
    /// </summary>
    public interface IValuedOption
    {
        /// <summary>
        /// 取る連続した値の個数を取得します。
        /// </summary>
        /// <remarks>0にすることで次のオプション迄の全ての値を取得</remarks>
        int ValueCount { get; }
    }
}
