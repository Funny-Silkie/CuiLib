using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CuiLib.Internal;
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
            int lastIndex = parser.Index;

            if (!parser.EndOfArguments) Parameters.SetValues(args.AsSpan()[lastIndex..]);

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
            int lastIndex = parser.Index;

            if (!parser.EndOfArguments) Parameters.SetValues(args.AsSpan()[lastIndex..]);

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
        public virtual void WriteHelp(TextWriter writer)
        {
            ThrowHelpers.ThrowIfNull(writer);

            // Title
            if (!string.IsNullOrEmpty(Name))
            {
                writer.WriteLine(Name);
                writer.WriteLine();
            }

            // Description
            if (!string.IsNullOrEmpty(Description))
            {
                writer.WriteLine("Description:");
                writer.WriteLine(Description);
                writer.WriteLine();
            }

            // Usage
            writer.WriteLine("Usage:");
            if (!string.IsNullOrEmpty(Name))
            {
                writer.Write(Name);
                writer.Write(' ');
            }
            if (Options.Count > 0)
                foreach (Option option in Options)
                {
                    if (!option.Required) writer.Write('[');
                    WriteOption(writer, option);
                    if (option.IsValued) writer.Write($" {option.ValueTypeName}");
                    if (!option.Required) writer.Write(']');
                    writer.Write(' ');

                    static void WriteOption(TextWriter writer, Option option)
                    {
                        if (option is NamedOption named)
                        {
                            if (named.ShortName is not null) writer.Write($"-{named.ShortName}");
                            else writer.Write($"--{named.FullName}");
                            return;
                        }
                        if (option is GroupOption group)
                        {
                            char separator = group switch
                            {
                                OrGroupOption => '|',
                                AndGroupOption => '&',
                                XorGroupOption => '^',
                                _ => throw new InvalidOperationException("無効なグループです"),
                            };
                            writer.Write('(');
                            int index = 0;
                            foreach (Option child in group)
                            {
                                if (index > 0) writer.Write(separator);
                                WriteOption(writer, child);
                                index++;
                            }
                            writer.Write(')');
                        }
                    }
                }

            if (Children.Count > 0) writer.Write("[Subcommand]");
            else if (Parameters.Count > 0)
                foreach (Parameter current in Parameters)
                {
                    writer.Write('<');
                    writer.Write(current.Name);
                    if (current.IsArray) writer.Write(" ..");
                    writer.Write('>');
                    writer.Write(' ');
                }
            writer.WriteLine();
            writer.WriteLine();

            // Options
            if (Options.Count > 0)
            {
                writer.WriteLine("Options:");
                int maxNameLength = Options.SelectMany(x => x.GetAllNames(false), (_, x) => x.Length).Max();
                foreach (Option option in Options)
                {
                    WriteOption(writer, option, maxNameLength);

                    static void WriteOption(TextWriter writer, Option option, int maxNameLength)
                    {
                        if (option is NamedOption named)
                        {
                            writer.Write("  ");
                            if (named.ShortName != null)
                            {
                                writer.Write('-');
                                writer.Write(named.ShortName);
                                if (named.FullName != null) writer.Write(", ");
                                else writer.Write("  ");
                            }
                            else writer.Write("    ");
                            if (named.FullName != null)
                            {
                                writer.Write("--");
                                writer.Write(named.FullName);
                                int surplus = maxNameLength - named.FullName.Length;
                                if (surplus > 0) writer.Write(new string(' ', surplus));
                            }
                            else writer.Write(new string(' ', maxNameLength + 2));

                            writer.Write("  ");

                            string? desc = named.Description;
                            //var headerValues = new List<string>();
                            //if (option.ValueTypeName is not null) headerValues.Add($"type={option.ValueTypeName}");
                            //string? defaultValue = option.DefaultValueString;
                            //if (defaultValue is not null && option is not FlagOption) headerValues.Add($"default={defaultValue.ReplaceSpecialCharacters()}");
                            //if (option.Required) headerValues.Add("required");
                            //if (option.CanMultiValue) headerValues.Add("multi valued");
                            //if (headerValues.Count > 0) desc += "\n* " + string.Join(", ", headerValues);
                            string[] descriptions = desc?.Split('\n') ?? [];
                            if (descriptions.Length > 0)
                            {
                                writer.WriteLine(descriptions[0]);
                                for (int i = 1; i < descriptions.Length; i++)
                                {
                                    writer.Write(new string(' ', maxNameLength + 10));
                                    writer.WriteLine(descriptions[i]);
                                }
                            }
                            else writer.WriteLine();
                        }
                        if (option is GroupOption group)
                            foreach (Option child in group)
                                WriteOption(writer, child, maxNameLength);
                    }
                }

                writer.WriteLine();
            }

            // Subcommands
            if (Children.Count > 0)
            {
                writer.WriteLine("Subcommands:");
                int maxLength = Children.Max(x => x.Name.Length);
                foreach (Command child in Children)
                {
                    writer.Write("  ");
                    int surplus = maxLength - child.Name.Length;
                    if (surplus > 0) writer.Write(new string(' ', surplus));
                    writer.Write(child.Name);
                    writer.Write("  ");

                    string[] descriptions = child.Description?.Split('\n') ?? [];
                    if (descriptions.Length > 0)
                    {
                        writer.WriteLine(descriptions[0]);
                        for (int i = 1; i < descriptions.Length; i++)
                        {
                            writer.Write(new string(' ', maxLength + 4));
                            writer.WriteLine(descriptions[i]);
                        }
                    }
                    else writer.WriteLine();
                }
            }
            // Parameters
            else if (Parameters.Count > 0)
            {
                writer.WriteLine("Parameters:");
                int maxLength = Parameters.Max(x => x.Name.Length);
                foreach (Parameter parameter in Parameters)
                {
                    writer.Write("  ");
                    int surplus = maxLength - parameter.Name.Length;
                    if (surplus > 0) writer.Write(new string(' ', surplus));
                    writer.Write(parameter.Name);
                    writer.Write("  ");

                    string[] descriptions = parameter.Description?.Split('\n') ?? [];
                    if (descriptions.Length > 0)
                    {
                        writer.WriteLine(descriptions[0]);
                        for (int i = 1; i < descriptions.Length; i++)
                        {
                            writer.Write(new string(' ', maxLength + 4));
                            writer.WriteLine(descriptions[i]);
                        }
                    }
                    else writer.WriteLine();
                }
            }
        }
    }
}
