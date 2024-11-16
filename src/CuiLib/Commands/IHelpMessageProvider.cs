using System;
using System.IO;

namespace CuiLib.Commands
{
    /// <summary>
    /// ヘルプメッセージを提供するインターフェイスを表します。
    /// </summary>
    public interface IHelpMessageProvider
    {
        /// <summary>
        /// コマンドのヘルプメッセージを出力します。
        /// </summary>
        /// <param name="writer">出力先の<see cref="TextWriter"/>のインスタンス</param>
        /// <param name="command">ヘルプメッセージを出力するコマンド</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/>または<paramref name="command"/>が<see langword="null"/></exception>
        /// <exception cref="ObjectDisposedException"><paramref name="writer"/>が既に破棄されている</exception>
        void WriteHelp(TextWriter writer, Command command);
    }
}
