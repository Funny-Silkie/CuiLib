namespace TextHandler
{
    /// <summary>
    /// 共通処理を記述します。
    /// </summary>
    internal static class Utils
    {
        /// <summary>
        /// エラーを出力します。
        /// </summary>
        /// <param name="writer">出力先</param>
        /// <param name="message">エラーメッセージ</param>
        public static void WriteError(this TextWriter writer, string? message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            writer.WriteLine(message);
            Console.ResetColor();
        }
    }
}
