using CuiLib.Commands;
using CuiLib.Options;
using CuiLib.Parameters;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CuiLib.Output
{
    /// <summary>
    /// <see cref="IHelpMessageProvider"/>の実装を表します。
    /// </summary>
    public partial class HelpMessageProvider : IHelpMessageProvider
    {
        // lang=regex
        private const string NewLineRegex = @"\r?\n";

        /// <summary>
        /// <see cref="HelpMessageProvider"/>の新しいインスタンスを初期化します。
        /// </summary>
        public HelpMessageProvider()
        {
        }

        /// <summary>
        /// ヘルプメッセージのヘッダー部分を出力します。
        /// </summary>
        /// <param name="writer">出力先の<see cref="TextWriter"/>のインスタンス</param>
        /// <param name="command">ヘルプメッセージを出力するコマンド</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/>または<paramref name="command"/>が<see langword="null"/></exception>
        /// <exception cref="ObjectDisposedException"><paramref name="writer"/>が既に破棄されている</exception>
        public virtual void WriteHeader(TextWriter writer, Command command)
        {
            ThrowHelpers.ThrowIfNull(writer);
            ThrowHelpers.ThrowIfNull(command);

            // Title
            if (!string.IsNullOrEmpty(command.Name)) writer.WriteLine(command.Name);

            // Description
            if (!string.IsNullOrEmpty(command.Description))
            {
                if (!string.IsNullOrEmpty(command.Name)) writer.WriteLine();

                writer.WriteLine("Description:");
                writer.WriteLine(command.Description);
            }
        }

        /// <summary>
        /// ヘルプメッセージのUsage部分を出力します。
        /// </summary>
        /// <param name="writer">出力先の<see cref="TextWriter"/>のインスタンス</param>
        /// <param name="command">ヘルプメッセージを出力するコマンド</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/>または<paramref name="command"/>が<see langword="null"/></exception>
        /// <exception cref="ObjectDisposedException"><paramref name="writer"/>が既に破棄されている</exception>
        public virtual void WriteUsage(TextWriter writer, Command command)
        {
            ThrowHelpers.ThrowIfNull(writer);
            ThrowHelpers.ThrowIfNull(command);

            writer.WriteLine("Usage:");
            writer.Write(command.Name);

            foreach (Option option in command.Options)
            {
                writer.Write(' ');
                if (!option.Required) writer.Write('[');
                WriteOption(writer, option);
                if (option.IsValued) writer.Write($" {option.ValueTypeName}");
                if (!option.Required) writer.Write(']');

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

            if (command.Children.Count > 0) writer.Write(" [Subcommand]");
            else if (command.Parameters.Count > 0)
                foreach (Parameter current in command.Parameters)
                {
                    writer.Write(" <");
                    writer.Write(current.Name);
                    if (current.IsArray) writer.Write(" ..");
                    writer.Write('>');
                }
            writer.WriteLine();
        }

        /// <summary>
        /// ヘルプメッセージのオプション部分を出力します。
        /// </summary>
        /// <param name="writer">出力先の<see cref="TextWriter"/>のインスタンス</param>
        /// <param name="command">ヘルプメッセージを出力するコマンド</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/>または<paramref name="command"/>が<see langword="null"/></exception>
        /// <exception cref="ObjectDisposedException"><paramref name="writer"/>が既に破棄されている</exception>
        public virtual void WriteOptions(TextWriter writer, Command command)
        {
            ThrowHelpers.ThrowIfNull(writer);
            ThrowHelpers.ThrowIfNull(command);

            if (command.Options.Count == 0) return;

            writer.WriteLine("Options:");
            int maxNameLength = command.Options.SelectMany(x => x.GetAllNames(false), (_, x) => x.Length).Max();
            foreach (Option option in command.Options)
            {
                WriteOption(writer, option, maxNameLength);

                static void WriteOption(TextWriter writer, Option option, int maxNameLength)
                {
                    switch (option)
                    {
                        case NamedOption named:
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
                                writer.Write(named.FullName.PadRight(maxNameLength));
                            }
                            else writer.Write(new string(' ', maxNameLength + 2));

                            writer.Write("  ");

                            string[] descriptions = string.IsNullOrEmpty(named.Description) ? [] : GetNewLineRegex().Split(named.Description);
                            string blanks = new string(' ', maxNameLength + 10);

                            if (descriptions.Length > 0)
                            {
                                writer.WriteLine(descriptions[0]);

                                for (int i = 1; i < descriptions.Length; i++)
                                {
                                    writer.Write(blanks);
                                    writer.WriteLine(descriptions[i]);
                                }
                            }
                            else writer.WriteLine();
                            break;

                        case GroupOption group:
                            foreach (Option child in group)
                                WriteOption(writer, child, maxNameLength);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// ヘルプメッセージのサブコマンド部分を出力します。
        /// </summary>
        /// <param name="writer">出力先の<see cref="TextWriter"/>のインスタンス</param>
        /// <param name="command">ヘルプメッセージを出力するコマンド</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/>または<paramref name="command"/>が<see langword="null"/></exception>
        /// <exception cref="ObjectDisposedException"><paramref name="writer"/>が既に破棄されている</exception>
        public virtual void WriteSubcommands(TextWriter writer, Command command)
        {
            ThrowHelpers.ThrowIfNull(writer);
            ThrowHelpers.ThrowIfNull(command);

            if (command.Children.Count == 0) return;

            writer.WriteLine("Subcommands:");
            int maxLength = command.Children.Max(x => x.Name.Length);
            foreach (Command child in command.Children)
            {
                writer.Write("  ");
                writer.Write(child.Name.PadLeft(maxLength));
                writer.Write("  ");

                string[] descriptions = string.IsNullOrEmpty(child.Description) ? [] : GetNewLineRegex().Split(child.Description);
                string blanks = new string(' ', maxLength + 4);

                if (descriptions.Length > 0)
                {
                    writer.WriteLine(descriptions[0]);
                    for (int i = 1; i < descriptions.Length; i++)
                    {
                        writer.Write(blanks);
                        writer.WriteLine(descriptions[i]);
                    }
                }
                else writer.WriteLine();
            }
        }

        /// <summary>
        /// ヘルプメッセージのパラメータ部分を出力します。
        /// </summary>
        /// <param name="writer">出力先の<see cref="TextWriter"/>のインスタンス</param>
        /// <param name="command">ヘルプメッセージを出力するコマンド</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/>または<paramref name="command"/>が<see langword="null"/></exception>
        /// <exception cref="ObjectDisposedException"><paramref name="writer"/>が既に破棄されている</exception>
        public virtual void WriteParameters(TextWriter writer, Command command)
        {
            ThrowHelpers.ThrowIfNull(writer);
            ThrowHelpers.ThrowIfNull(command);

            if (command.Parameters.Count == 0) return;

            writer.WriteLine("Parameters:");
            int maxLength = command.Parameters.Max(x => x.Name.Length);
            foreach (Parameter parameter in command.Parameters)
            {
                writer.Write("  ");
                writer.Write(parameter.Name.PadLeft(maxLength));
                writer.Write("  ");

                string[] descriptions = string.IsNullOrEmpty(parameter.Description) ? [] : GetNewLineRegex().Split(parameter.Description);
                string blanks = new string(' ', maxLength + 4);

                if (descriptions.Length > 0)
                {
                    writer.WriteLine(descriptions[0]);
                    for (int i = 1; i < descriptions.Length; i++)
                    {
                        writer.Write(blanks);
                        writer.WriteLine(descriptions[i]);
                    }
                }
                else writer.WriteLine();
            }
        }

        /// <inheritdoc/>
        public virtual void WriteHelp(TextWriter writer, Command command)
        {
            WriteHeader(writer, command);
            writer.WriteLine();

            WriteUsage(writer, command);
            writer.WriteLine();

            WriteOptions(writer, command);
            writer.WriteLine();

            if (command.Children.Count > 0) WriteSubcommands(writer, command);
            else WriteParameters(writer, command);
        }

#if NET7_0_OR_GREATER

        [GeneratedRegex(NewLineRegex)]
        private static partial Regex GetNewLineRegex();

#else

        private static Regex GetNewLineRegex() => _newLineRegex;

        private static readonly Regex _newLineRegex = new Regex(NewLineRegex);

#endif
    }
}
