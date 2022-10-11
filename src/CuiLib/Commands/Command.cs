using System;
using CuiLib.Options;

namespace CuiLib.Commands
{
    /// <summary>
    /// コマンドの基底クラスです。
    /// </summary>
    [Serializable]
    public class Command
    {
        /// <summary>
        /// コマンド名を取得します。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 親コマンドを取得します。
        /// </summary>
        public Command? Parent { get; internal set; }

        /// <summary>
        /// オプションを取得します。
        /// </summary>
        public OptionCollection Options { get; }

        /// <summary>
        /// サブコマンドを取得します。
        /// </summary>
        public CommandCollection Children { get; }

        /// <summary>
        /// パラメータ引数を取得します。
        /// </summary>
        public ParameterCollection Parameters { get; }

        /// <summary>
        /// <see cref="Command"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="name">コマンド名</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="name"/>が空文字</exception>
        public Command(string name)
        {
            ThrowHelper.ThrowIfNullOrEmpty(name);

            Name = name;
            Children = new CommandCollection(this);
            Options = new OptionCollection();
            Parameters = new ParameterCollection();
        }

        /// <summary>
        /// コマンドを実行します。
        /// </summary>
        /// <param name="args">コマンド引数</param>
        /// <param name="commands">実行するコマンドのコレクション</param>
        /// <exception cref="ArgumentNullException"><paramref name="args"/>または<paramref name="commands"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="args"/>内の値がnull</exception>
        /// <exception cref="ArgumentAnalysisException">引数解析時のエラー</exception>
        public static void InvokeFromCollection(string[] args, CommandCollection commands)
        {
            ArgumentNullException.ThrowIfNull(args);
            ArgumentNullException.ThrowIfNull(commands);

            if (args.Length == 0) return;

            if (!commands.TryGetCommand(args[0], out Command? command)) throw new ArgumentAnalysisException($"コマンド'{args[0]}'は存在しません");
            command.Invoke(args.AsSpan(1));
        }

        /// <summary>
        /// コマンドを実行します。
        /// </summary>
        /// <param name="args">コマンド引数</param>
        /// <exception cref="ArgumentNullException"><paramref name="args"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="args"/>内の値がnull</exception>
        /// <exception cref="ArgumentAnalysisException">引数解析時のエラー</exception>
        public void Invoke(string[] args)
        {
            ArgumentNullException.ThrowIfNull(args);

            Invoke(args.AsSpan());
        }

        /// <summary>
        /// コマンドを実行します。
        /// </summary>
        /// <param name="args">引数</param>
        /// <exception cref="ArgumentException"><paramref name="args"/>内の値がnull</exception>
        /// <exception cref="ArgumentAnalysisException">引数解析時のエラー</exception>
        private void Invoke(ReadOnlySpan<string> args)
        {
            Option? currentOption = null;
            string? currentOptionName = null;
            int lastIndex = -1;
            for (int i = 0; i < args.Length; i++)
            {
                string argument = args[i];
                if (argument is null) throw new ArgumentException("コマンド引数にnullが含まれています", nameof(args));

                // オプション
                if (argument.StartsWith('-') && argument.Length >= 2)
                {
                    // -X
                    if (argument.Length == 2)
                    {
                        if (argument[1] == '-') throw new ArgumentAnalysisException("オプション'--'は無効です");
                        if (!Options.TryGetValue(argument[1], out Option? option)) throw new ArgumentAnalysisException($"オプション'{argument}'は無効です");
                        if (currentOption is not null) throw new ArgumentAnalysisException($"オプション'{currentOptionName}'に値が設定されていません");
                        if (option.IsValued)
                        {
                            currentOption = option;
                            currentOptionName = argument;
                        }
                        else
                        {
                            if (option.ValueAvailable) throw new ArgumentAnalysisException($"オプション'{argument}'が複数指定されています");
                            option.SetValue(null);
                        }
                        continue;
                    }

                    // --XXX
                    if (argument[1] == '-')
                    {
                        if (argument.Length < 3 || argument[1] != '-') throw new ArgumentAnalysisException($"オプション'{argument}'は無効です");
                        if (!Options.TryGetValue(argument[2..], out Option? option)) throw new ArgumentAnalysisException($"オプション'{argument}'は無効です");
                        if (currentOption is not null) throw new ArgumentAnalysisException($"オプション'{currentOptionName}'に値が設定されていません");
                        if (option.IsValued)
                        {
                            currentOption = option;
                            currentOptionName = argument;
                        }
                        else
                        {
                            if (option.ValueAvailable) throw new ArgumentAnalysisException($"オプション'{argument}'が複数指定されています");
                            option.SetValue(null);
                        }
                        continue;
                    }

                    throw new ArgumentAnalysisException($"オプション'{argument}'は無効です");
                }
                if (currentOption is not null)
                {
                    if (currentOption.ValueAvailable) throw new ArgumentAnalysisException($"オプション'{currentOptionName}'が複数指定されています");
                    currentOption.SetValue(argument);
                    currentOption = null;
                    currentOptionName = null;
                    continue;
                }

                lastIndex = i;
                break;
            }

            if (currentOption is not null) throw new ArgumentAnalysisException($"オプション'{currentOptionName}'に値が設定されていません");

            if (Children.Count > 0)
            {
                if (lastIndex < 0 || lastIndex >= Children.Count) throw new ArgumentAnalysisException("子コマンド名が指定されていません");

                if (!Children.TryGetCommand(args[lastIndex], out Command? next)) throw new ArgumentAnalysisException($"コマンド'{args[lastIndex]}'は無効です");
                lastIndex++;
                ReadOnlySpan<string> values = lastIndex >= 0 && lastIndex < args.Length ? args[lastIndex..] : ReadOnlySpan<string>.Empty;
                next.Invoke(values);
            }
            else
            {
                ReadOnlySpan<string> values = lastIndex >= 0 && lastIndex < args.Length ? args[lastIndex..] : ReadOnlySpan<string>.Empty;
                Parameters.SetValues(values);
                Execute();
            }
        }

        /// <summary>
        /// オーバーライドしてコマンドの処理を記述します。
        /// </summary>
        /// <remarks>子コマンドが存在する場合は実行されません</remarks>
        protected virtual void Execute()
        {
        }
    }
}
