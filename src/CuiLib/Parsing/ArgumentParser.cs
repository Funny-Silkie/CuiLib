using CuiLib.Commands;
using CuiLib.Options;
using CuiLib.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CuiLib.Parsing
{
    /// <summary>
    /// コマンドライン引数のパーサーを表します。
    /// </summary>
    [Serializable]
    public class ArgumentParser
    {
        private const string ForcingParameterToken = "--";

        private readonly string[] arguments;

        /// <summary>
        /// 現在パースの進んだ引数のインデックスを取得します。
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// 以降の値をパラメータとして強制するかどうかを表す値を取得します。
        /// </summary>
        public bool ForcingParameter { get; private set; }

        /// <summary>
        /// 引数解析が終了したか否かを表す値を取得します。
        /// </summary>
        public bool EndOfArguments => Index >= arguments.Length;

        /// <summary>
        /// <see cref="ArgumentParser"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="arguments">コマンドライン引数</param>
        /// <exception cref="ArgumentNullException"><paramref name="arguments"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="arguments"/>の要素が<see langword="null"/></exception>
        public ArgumentParser(string[] arguments)
        {
            ThrowHelpers.ThrowIfNull(arguments);
            if (arguments.Any(x => x is null)) throw new ArgumentException("コマンド引数にnullが含まれています", nameof(arguments));

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
        /// 実行対象コマンドを取得し，その分<see cref="Index"/>を進めます。
        /// </summary>
        /// <param name="commands">コマンド一覧</param>
        /// <returns>実行されるコマンドのインスタンス，対象がない場合は<see langword="null"/></returns>
        public Command? GetTargetCommand(CommandCollection commands)
        {
            ThrowHelpers.ThrowIfNull(commands);

            if (EndOfArguments) return null;

            ref string argumentRef = ref arguments[Index];
            Command? result = null;
            int startIndex = Index;

            while (!EndOfArguments && commands.Count > 0 && commands.TryGetCommand(argumentRef, out Command? currentCommand))
            {
                result = currentCommand;
                commands = currentCommand.Children;
                Index++;
                argumentRef = ref Unsafe.Add(ref argumentRef, 1);
            }
            if (result is null) Index = startIndex;
            return result;
        }

        /// <summary>
        /// オプションを検索して値を設定，その分<see cref="Index"/>を進めます。
        /// </summary>
        /// <param name="options">検索対象オプション一覧</param>
        /// <returns>値が設定された<paramref name="options"/>の要素一覧，オプションが指定されていない場合は<see langword="null"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="options"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentAnalysisException">引数解析エラー</exception>
        public Option[]? ParseOption(OptionCollection options)
        {
            ThrowHelpers.ThrowIfNull(options);

            if (EndOfArguments || ForcingParameter) return null;

            ref string argumentRef = ref arguments[Index];
            if (argumentRef == ForcingParameterToken) return null;

            (string[]? optionNames, bool isSingle) = GetOptionName(argumentRef);

            // Current value does not represent an option name
            if (optionNames is null) return null;

            SkipArguments(1);

            var list = new List<Option>(optionNames.Length);
            int optionIndex = 0;

            foreach (string optionName in optionNames)
            {
                string actualName = isSingle ? $"-{optionName}" : $"--{optionName}";

                // Current value represents missing option
                if (!options.TryGetValue(optionName, out Option? target)) throw new ArgumentAnalysisException($"オプション'{actualName}'は無効です");
                list.Add(target);

                Option actualTarget = target.GetActualOption(optionName, isSingle);
                if (actualTarget.IsValued)
                {
                    if (EndOfArguments || optionIndex < optionNames.Length - 1) throw new ArgumentAnalysisException($"オプション'{actualName}'に値が設定されていません");

                    if (!actualTarget.CanMultiValue && actualTarget.ValueAvailable) throw new ArgumentAnalysisException($"オプション'{actualName}'が複数指定されています");

                    int valueCount = ((IValuedOption)actualTarget).ValueCount;

                    if (valueCount == 0)
                    {
                        while (true)
                        {
                            if (EndOfArguments) break;

                            argumentRef = ref Unsafe.Add(ref argumentRef, 1);

                            if (GetOptionName(argumentRef).optionNames is not null) break;

                            if (!ForcingParameter && argumentRef == ForcingParameterToken)
                            {
                                ForcingParameter = true;
                                argumentRef = ref Unsafe.Add(ref argumentRef, 1);
                                SkipArguments(1);
                            }
                            target.ApplyValue(optionName, argumentRef);
                            SkipArguments(1);
                        }
                    }
                    else
                    {
                        for (int valueIndex = 0; valueIndex < valueCount; valueIndex++)
                        {
                            if (EndOfArguments) throw new ArgumentAnalysisException($"オプション'{actualName}'の値の数が足りません");

                            argumentRef = ref Unsafe.Add(ref argumentRef, 1);

                            if (!ForcingParameter && argumentRef == ForcingParameterToken)
                            {
                                ForcingParameter = true;
                                argumentRef = ref Unsafe.Add(ref argumentRef, 1);
                                SkipArguments(1);
                            }
                            target.ApplyValue(optionName, argumentRef);
                            SkipArguments(1);
                        }
                    }

                    break;
                }

                if (!actualTarget.CanMultiValue && actualTarget.ValueAvailable) throw new ArgumentAnalysisException($"オプション'{actualName}'が複数指定されています");
                target.ApplyValue(optionName, string.Empty);

                optionIndex++;
            }

            return list.ToArray();
        }

        /// <summary>
        /// ハイフン抜きのオプション名を取得します。
        /// </summary>
        /// <param name="value">コマンドライン引数の文字列</param>
        /// <returns>ハイフン抜きのオプション名。短縮形の場合は全てのオプション名</returns>
        private static (string[]? optionNames, bool isSingle) GetOptionName(string value)
        {
            if (value.Length <= 1 || value == ForcingParameterToken) return (null, false);

            if (value[0] == '-')
            {
                // As full name
                if (value.Length > 2 && value[1] == '-') return ([value[2..]], false);
                // As short names
                return (value.Skip(1).Select(x => x.ToString()).ToArray(), true);
            }

            return (null, false);
        }

        /// <summary>
        /// パラメータの値を設定，その分<see cref="Index"/>を進めます。
        /// </summary>
        /// <param name="parameters">対象パラメータ一覧</param>
        /// <exception cref="ArgumentNullException"><paramref name="parameters"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentAnalysisException">存在しないパラメータが指定された</exception>
        public void ParseParameters(ParameterCollection parameters)
        {
            ThrowHelpers.ThrowIfNull(parameters);

            if (EndOfArguments || parameters.Count == 0) return;

            Span<string> args = arguments.AsSpan()[Index..];
            if (!ForcingParameter)
            {
                int forcingTokenIndex = args.IndexOf(ForcingParameterToken);
                if (forcingTokenIndex != -1)
                {
                    if (forcingTokenIndex == args.Length - 1) args = args[..^1];
                    else if (forcingTokenIndex == 0) args = args[1..];
                    else
                    {
                        var newArgs = new Span<string>(new string[args.Length - 1]);
                        args[..forcingTokenIndex].CopyTo(newArgs);
                        args[(forcingTokenIndex + 1)..].CopyTo(newArgs[forcingTokenIndex..]);
                        args = newArgs;
                    }
                    ForcingParameter = true;
                }
            }

            try
            {
                parameters.SetValues(args);
            }
            catch (InvalidOperationException e)
            {
                throw new ArgumentAnalysisException(e.Message);
            }
            SkipArguments(arguments.Length - Index);
        }
    }
}
