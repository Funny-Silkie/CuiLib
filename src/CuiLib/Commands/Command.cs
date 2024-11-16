using System;
using System.IO;
using System.Threading.Tasks;
using CuiLib.Options;
using CuiLib.Parameters;
using CuiLib.Parsing;

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
        /// コマンドの説明を取得または設定します。
        /// </summary>
        public string? Description { get; set; }

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
        public Command(string name)
        {
            ThrowHelpers.ThrowIfNull(name);

            Name = name;
            Children = new CommandCollection(this);
            Options = [];
            Parameters = [];
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
            ThrowHelpers.ThrowIfNull(commands);
            var parser = new ArgumentParser(args);

            if (args.Length == 0) return;

            Command command = parser.GetTargetCommand(commands) ?? throw new ArgumentAnalysisException($"コマンド'{args[0]}'は存在しません");
            command.Invoke(args, parser);
        }

        /// <summary>
        /// コマンドを実行します。
        /// </summary>
        /// <param name="args">コマンド引数</param>
        /// <param name="commands">実行するコマンドのコレクション</param>
        /// <exception cref="ArgumentNullException"><paramref name="args"/>または<paramref name="commands"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="args"/>内の値がnull</exception>
        /// <exception cref="ArgumentAnalysisException">引数解析時のエラー</exception>
        public static async Task InvokeFromCollectionAsync(string[] args, CommandCollection commands)
        {
            ThrowHelpers.ThrowIfNull(commands);
            var parser = new ArgumentParser(args);

            if (args.Length == 0) return;

            Command command = parser.GetTargetCommand(commands) ?? throw new ArgumentAnalysisException($"コマンド'{args[0]}'は存在しません");
            await command.InvokeAsync(args, parser);
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
            Invoke(args, new ArgumentParser(args));
        }

        /// <summary>
        /// コマンドを実行します。
        /// </summary>
        /// <param name="args">コマンド引数</param>
        /// <exception cref="ArgumentNullException"><paramref name="args"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="args"/>内の値がnull</exception>
        /// <exception cref="ArgumentAnalysisException">引数解析時のエラー</exception>
        public async Task InvokeAsync(string[] args)
        {
            await InvokeAsync(args, new ArgumentParser(args));
        }

        /// <summary>
        /// コマンドを実行します。
        /// </summary>
        /// <param name="args">引数</param>
        /// <param name="parser">引数解析用のパーサー</param>
        /// <exception cref="ArgumentException"><paramref name="args"/>内の値がnull</exception>
        /// <exception cref="ArgumentAnalysisException">引数解析時のエラー</exception>
        private void Invoke(string[] args, ArgumentParser parser)
        {
            while (parser.ParseOption(Options) is not null) ;

            if (Children.Count > 0)
            {
                Command? child = parser.GetTargetCommand(Children);
                if (child is not null)
                {
                    child.Invoke(args, parser);
                    return;
                }
            }

            parser.ParseParameters(Parameters);

            OnExecution();
            OnExecutionAsync().Wait();
        }

        /// <summary>
        /// コマンドを実行します。
        /// </summary>
        /// <param name="args">引数</param>
        /// <param name="parser">引数解析用のパーサー</param>
        /// <exception cref="ArgumentException"><paramref name="args"/>内の値がnull</exception>
        /// <exception cref="ArgumentAnalysisException">引数解析時のエラー</exception>
        private async Task InvokeAsync(string[] args, ArgumentParser parser)
        {
            while (parser.ParseOption(Options) is not null) ;

            if (Children.Count > 0)
            {
                Command? child = parser.GetTargetCommand(Children);
                if (child is not null)
                {
                    await child.InvokeAsync(args, parser);
                    return;
                }
            }

            parser.ParseParameters(Parameters);

            OnExecution();
            await OnExecutionAsync();
        }

        /// <summary>
        /// オーバーライドしてコマンドの処理を記述します。
        /// </summary>
        /// <remarks>子コマンドが実行された場合は実行されません</remarks>
        protected virtual void OnExecution()
        {
        }

        /// <summary>
        /// オーバーライドしてコマンドの処理を記述します。
        /// </summary>
        /// <remarks>子コマンドが実行された場合は実行されません</remarks>
        protected virtual Task OnExecutionAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// ヘルプを表示します。
        /// </summary>
        /// <param name="writer">出力先</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/>がnull</exception>
        [Obsolete($"Use '{nameof(WriteHelp)}({nameof(TextWriter)}, {nameof(HelpMessageProvider)}) method instead.'")]
        public virtual void WriteHelp(TextWriter writer)
        {
            var provider = new HelpMessageProvider();
            provider.WriteHelp(writer, this);
        }

        /// <summary>
        /// ヘルプを表示します。
        /// </summary>
        /// <param name="writer">出力先</param>
        /// <param name="messageProvider">ヘルプメッセージを提供するオブジェクト</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/>がnull</exception>
        public void WriteHelp(TextWriter writer, IHelpMessageProvider? messageProvider = null)
        {
            messageProvider ??= new HelpMessageProvider();
            messageProvider.WriteHelp(writer, this);
        }
    }
}
