using CuiLib.Commands;
using System;
using System.Runtime.CompilerServices;

namespace CuiLib.Parsing
{
    /// <summary>
    /// コマンドライン引数のパーサーを表します。
    /// </summary>
    [Serializable]
    internal class ArgumentParser
    {
        private readonly string[] arguments;

        /// <summary>
        /// 現在パースの進んだ引数のインデックスを取得します。
        /// </summary>
        internal int Index { get; private set; }

        /// <summary>
        /// 引数解析が終了したか否かを表す値を取得します。
        /// </summary>
        public bool EndOfArguments => Index >= arguments.Length;

        /// <summary>
        /// <see cref="ArgumentParser"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="arguments">コマンドライン引数</param>
        /// <exception cref="ArgumentNullException"><paramref name="arguments"/>が<see langword="null"/></exception>
        public ArgumentParser(string[] arguments)
        {
            ThrowHelpers.ThrowIfNull(arguments);

            this.arguments = arguments;
        }

        /// <summary>
        /// 指定した個数の引数の解析をスキップします。
        /// </summary>
        /// <param name="count">スキップする個数</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/>が0未満</exception>
        public void SkipArguments(int count)
        {
            ThrowHelpers.ThrowIfNegative(count);

            if (count == 0 || EndOfArguments) return;
            Index += count;
            if (Index > arguments.Length) Index = arguments.Length;
        }

        /// <summary>
        /// 実行対象コマンドを取得します。
        /// </summary>
        /// <param name="commands">コマンド一覧</param>
        /// <returns>実行されるコマンドのインスタンス，対象がない場合は<see langword="null"/></returns>
        public Command? GetTargetCommand(CommandCollection commands)
        {
            ThrowHelpers.ThrowIfNull(commands);

            ref string argumentRef = ref arguments[Index];
            Command? result = null;
            int startIndex = Index;

            while (!EndOfArguments && commands.Count > 0 && commands.TryGetCommand(argumentRef, out result))
            {
                commands = result.Children;
                Index++;
                argumentRef = ref Unsafe.Add(ref argumentRef, 1);
            }
            if (result is null) Index = startIndex;
            return result;
        }
    }
}
