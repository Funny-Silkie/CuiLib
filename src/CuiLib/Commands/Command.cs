using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CuiLib.Log;
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
            ArgumentNullException.ThrowIfNull(name);

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
        /// <param name="commands">実行するコマンドのコレクション</param>
        /// <exception cref="ArgumentNullException"><paramref name="args"/>または<paramref name="commands"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="args"/>内の値がnull</exception>
        /// <exception cref="ArgumentAnalysisException">引数解析時のエラー</exception>
        public static async Task InvokeFromCollectionAsync(string[] args, CommandCollection commands)
        {
            ArgumentNullException.ThrowIfNull(args);
            ArgumentNullException.ThrowIfNull(commands);

            if (args.Length == 0) return;

            if (!commands.TryGetCommand(args[0], out Command? command)) throw new ArgumentAnalysisException($"コマンド'{args[0]}'は存在しません");
            await command.InvokeAsync(new ReadOnlyMemory<string>(args, 1, args.Length - 1));
        }

        /// <summary>
        /// 引数を解析します。
        /// </summary>
        /// <param name="args">コマンド引数</param>
        /// <returns><paramref name="args"/>における非オプションのコマンド引数の開始インデックス</returns>
        /// <exception cref="ArgumentException"><paramref name="args"/>内の値がnull</exception>
        /// <exception cref="ArgumentAnalysisException">引数解析時のエラー</exception>
        private int ParseArguments(ReadOnlySpan<string> args)
        {
            Option? currentOption = null;
            string? currentOptionName = null;
            int result = -1;
            for (int i = 0; i < args.Length; i++)
            {
                string argument = args[i];
                if (argument is null) throw new ArgumentException("コマンド引数にnullが含まれています", nameof(args));

                // オプション
                if (argument.StartsWith('-') && argument.Length >= 2)
                {
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
                            if (!option.CanMultiValue && option.ValueAvailable) throw new ArgumentAnalysisException($"オプション'{argument}'が複数指定されています");
                            option.ApplyValue(string.Empty);
                        }
                        continue;
                    }

                    // -X
                    {
                        if (currentOption is not null) throw new ArgumentAnalysisException($"オプション'{currentOptionName}'に値が設定されていません");

                        for (int j = 1; j < argument.Length; j++)
                        {
                            char c = argument[j];
                            if (!Options.TryGetValue(c, out Option? option)) throw new ArgumentAnalysisException($"オプション'-{c}'は無効です");
                            if (!option.CanMultiValue && option.ValueAvailable) throw new ArgumentAnalysisException($"オプション'-{c}'が複数指定されています");
                            if (option.IsValued)
                            {
                                if (j != argument.Length - 1) throw new ArgumentAnalysisException($"オプション'-{c}'に値が設定されていません");
                                currentOption = option;
                                currentOptionName = $"{c}";
                            }
                            else option.ApplyValue(string.Empty);
                        }
                        continue;
                    }
                }
                if (currentOption is not null)
                {
                    if (!currentOption.CanMultiValue && currentOption.ValueAvailable) throw new ArgumentAnalysisException($"オプション'{currentOptionName}'が複数指定されています");
                    currentOption.ApplyValue(argument);
                    currentOption = null;
                    currentOptionName = null;
                    continue;
                }

                result = i;
                break;
            }

            if (currentOption is not null) throw new ArgumentAnalysisException($"オプション'{currentOptionName}'に値が設定されていません");

            return result;
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
        /// <param name="args">コマンド引数</param>
        /// <exception cref="ArgumentNullException"><paramref name="args"/>がnull</exception>
        /// <exception cref="ArgumentException"><paramref name="args"/>内の値がnull</exception>
        /// <exception cref="ArgumentAnalysisException">引数解析時のエラー</exception>
        public async Task InvokeAsync(string[] args)
        {
            ArgumentNullException.ThrowIfNull(args);

            await InvokeAsync(new ReadOnlyMemory<string>(args));
        }

        /// <summary>
        /// コマンドを実行します。
        /// </summary>
        /// <param name="args">引数</param>
        /// <exception cref="ArgumentException"><paramref name="args"/>内の値がnull</exception>
        /// <exception cref="ArgumentAnalysisException">引数解析時のエラー</exception>
        private void Invoke(ReadOnlySpan<string> args)
        {
            int lastIndex = ParseArguments(args);

            if (Children.Count > 0 && 0 <= lastIndex && lastIndex <= args.Length - 1 && Children.TryGetCommand(args[lastIndex], out Command? next))
            {
                lastIndex++;
                ReadOnlySpan<string> values = lastIndex >= 0 && lastIndex < args.Length ? args[lastIndex..] : ReadOnlySpan<string>.Empty;
                next.Invoke(values);
                return;
            }
            else
            {
                ReadOnlySpan<string> values = lastIndex >= 0 && lastIndex < args.Length ? args[lastIndex..] : ReadOnlySpan<string>.Empty;
                Parameters.SetValues(values);
                OnExecution();
                OnExecutionAsync().Wait();
            }
        }

        /// <summary>
        /// コマンドを実行します。
        /// </summary>
        /// <param name="args">引数</param>
        /// <exception cref="ArgumentException"><paramref name="args"/>内の値がnull</exception>
        /// <exception cref="ArgumentAnalysisException">引数解析時のエラー</exception>
        private async Task InvokeAsync(ReadOnlyMemory<string> args)
        {
            int lastIndex = ParseArguments(args.Span);

            if (Children.Count > 0 && 0 <= lastIndex && lastIndex <= args.Length - 1 && Children.TryGetCommand(args.Span[lastIndex], out Command? next))
            {
                lastIndex++;
                ReadOnlyMemory<string> values = lastIndex >= 0 && lastIndex < args.Length ? args[lastIndex..] : ReadOnlyMemory<string>.Empty;
                await next.InvokeAsync(values);
                return;
            }
            else
            {
                ReadOnlyMemory<string> values = lastIndex >= 0 && lastIndex < args.Length ? args[lastIndex..] : ReadOnlyMemory<string>.Empty;
                Parameters.SetValues(values.Span);
                OnExecution();
                await OnExecutionAsync();
            }
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
        /// <param name="logger">出力先</param>
        /// <exception cref="ArgumentNullException"><paramref name="logger"/>がnull</exception>
        public virtual void WriteHelp(Logger logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            // Title
            if (!string.IsNullOrEmpty(Name))
            {
                logger.WriteLine(Name);
                logger.WriteLine();
            }

            // Description
            if (!string.IsNullOrEmpty(Description))
            {
                logger.WriteLine("Description:");
                logger.WriteLine(Description);
                logger.WriteLine();
            }

            // Usage
            logger.WriteLine("Usage:");
            if (!string.IsNullOrEmpty(Name))
            {
                logger.Write(Name);
                logger.Write(' ');
            }
            if (Options.Count > 0)
                foreach (var option in Options)
                {
                    if (!option.Required) logger.Write('[');
                    if (option.ShortName is not null) logger.Write($"-{option.ShortName}");
                    else logger.Write($"--{option.FullName}");
                    if (option.IsValued) logger.Write($" {option.ValueTypeName}");
                    if (!option.Required) logger.Write(']');
                    logger.Write(' ');
                }

            if (Children.Count > 0) logger.Write("[Subcommand]");
            else if (Parameters.Count > 0)
                foreach (Parameter current in Parameters)
                {
                    logger.Write('<');
                    logger.Write(current.Name);
                    if (current.IsArray) logger.Write(" ..");
                    logger.Write('>');
                    logger.Write(' ');
                }
            logger.WriteLine();
            logger.WriteLine();

            // Options
            if (Options.Count > 0)
            {
                logger.WriteLine("Options:");
                int maxNameLength = Options.Max(x => x.FullName?.Length ?? 0);
                foreach (Option option in Options)
                {
                    logger.Write("  ");
                    if (option.ShortName != null)
                    {
                        logger.Write('-');
                        logger.Write(option.ShortName);
                        if (option.FullName != null) logger.Write(", ");
                        else logger.Write("  ");
                    }
                    else logger.Write("    ");
                    if (option.FullName != null)
                    {
                        logger.Write("--");
                        logger.Write(option.FullName);
                        int surplus = maxNameLength - option.FullName.Length;
                        if (surplus > 0) logger.Write(new string(' ', surplus));
                    }
                    else logger.Write(new string(' ', maxNameLength + 2));

                    logger.Write("  ");

                    string? desc = option.Description;
                    //var headerValues = new List<string>();
                    //if (option.ValueTypeName is not null) headerValues.Add($"type={option.ValueTypeName}");
                    //string? defaultValue = option.DefaultValueString;
                    //if (defaultValue is not null && option is not FlagOption) headerValues.Add($"default={defaultValue.ReplaceSpecialCharacters()}");
                    //if (option.Required) headerValues.Add("required");
                    //if (option.CanMultiValue) headerValues.Add("multi valued");
                    //if (headerValues.Count > 0) desc += "\n* " + string.Join(", ", headerValues);
                    string[] descriptions = desc?.Split('\n') ?? Array.Empty<string>();
                    if (descriptions.Length > 0)
                    {
                        logger.WriteLine(descriptions[0]);
                        for (int i = 1; i < descriptions.Length; i++)
                        {
                            logger.Write(new string(' ', maxNameLength + 10));
                            logger.WriteLine(descriptions[i]);
                        }
                    }
                    else logger.WriteLine();
                }
            }

            // Subcommands
            if (Children.Count > 0)
            {
                logger.WriteLine("Subcommands:");
                int maxLength = Children.Max(x => x.Name.Length);
                foreach (Command child in Children)
                {
                    logger.Write("  ");
                    int surplus = maxLength - child.Name.Length;
                    if (surplus > 0) logger.Write(new string(' ', surplus));
                    logger.Write(child.Name);
                    logger.Write("  ");

                    string[] descriptions = child.Description?.Split('\n') ?? Array.Empty<string>();
                    if (descriptions.Length > 0)
                    {
                        logger.WriteLine(descriptions[0]);
                        for (int i = 1; i < descriptions.Length; i++)
                        {
                            logger.Write(new string(' ', maxLength + 4));
                            logger.WriteLine(descriptions[i]);
                        }
                    }
                    else logger.WriteLine();
                }
            }
            // Parameters
            else if (Parameters.Count > 0)
            {
                logger.WriteLine("Parameters:");
                int maxLength = Parameters.Max(x => x.Name.Length);
                foreach (Parameter parameter in Parameters)
                {
                    logger.Write("  ");
                    int surplus = maxLength - parameter.Name.Length;
                    if (surplus > 0) logger.Write(new string(' ', surplus));
                    logger.Write(parameter.Name);
                    logger.Write("  ");

                    string[] descriptions = parameter.Description?.Split('\n') ?? Array.Empty<string>();
                    if (descriptions.Length > 0)
                    {
                        logger.WriteLine(descriptions[0]);
                        for (int i = 1; i < descriptions.Length; i++)
                        {
                            logger.Write(new string(' ', maxLength + 4));
                            logger.WriteLine(descriptions[i]);
                        }
                    }
                    else logger.WriteLine();
                }
            }
        }
    }
}
