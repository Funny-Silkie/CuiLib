using CuiLib.Checkers;
using CuiLib.Commands;
using CuiLib.Options;
using CuiLib.Parameters;
using System.Text.RegularExpressions;

namespace TextHandler.Commands
{
    /// <summary>
    /// 検索を行うコマンドを表します。
    /// </summary>
    public class SearchCommand : Command
    {
        #region Options

        private readonly FlagOption optionHelp;
        private readonly SingleValueOption<string> optionQuery;
        private readonly FlagOption optionFixedString;
        private readonly FlagOption optionCaseInsensitive;
        private readonly FlagOption optionOnlyMatching;

        #endregion Options

        #region Parameters

        private readonly SingleValueParameter<TextReader> parameterFile;

        #endregion Parameters

        /// <summary>
        /// <see cref="SearchCommand"/>の新しいインスタンスを初期化します。
        /// </summary>
        public SearchCommand() : base("search")
        {
            Description = "Search specified expression in text files";

            optionHelp = new FlagOption('h', "help")
            {
                Description = "Displays help message",
            };
            optionQuery = new SingleValueOption<string>('q', "query")
            {
                Description = "Query string or regex pattern",
                Required = true,
                Checker = ValueChecker.NotEmpty(),
            };
            optionFixedString = new FlagOption('F', "fixed-strings")
            {
                Description = "Handle query string as fixed string",
            };
            optionCaseInsensitive = new FlagOption("case-insensitive")
            {
                Description = "Search text on case insensitive mode",
            };
            optionOnlyMatching = new FlagOption('o', "only-matching")
            {
                Description = "Show only matched area",
            };

            Options.Add(optionHelp);
            Options.Add(optionQuery);
            Options.Add(optionFixedString);
            Options.Add(optionCaseInsensitive);
            Options.Add(optionOnlyMatching);

            parameterFile = new SingleValueParameter<TextReader>("input", 0)
            {
                Description = "Text file to read",
            };

            Parameters.Add(parameterFile);
        }

        /// <inheritdoc/>
        protected override void OnExecution()
        {
            if (optionHelp.Value)
            {
                WriteHelp(Console.Out);
                return;
            }

            TextReader inputFile;
            if (!parameterFile.ValueAvailable)
            {
                if (!Console.IsInputRedirected)
                {
                    Console.Error.WriteError("Input files are not specified and nothing is redirected.");
                    return;
                }
                inputFile = Console.In;
            }
            else inputFile = parameterFile.Value;

            string query = optionQuery.Value;
            TextWriter writer = Console.Out;

            try
            {
                if (optionFixedString.Value)
                {
                    StringComparison comparison = optionCaseInsensitive.Value ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
                    string? line;
                    while ((line = inputFile.ReadLine()) is not null)
                    {
                        if (line.Length == 0) continue;

                        if (line.Contains(query, comparison)) writer.WriteLine(line);
                    }
                }
                else
                {
                    var options = RegexOptions.Compiled;
                    if (optionCaseInsensitive.Value) options |= RegexOptions.IgnoreCase;
                    var regex = new Regex(query, options);

                    string? line;
                    while ((line = inputFile.ReadLine()) is not null)
                    {
                        if (line.Length == 0) continue;

                        MatchCollection matches = regex.Matches(line);
                        if (matches.Count == 0) continue;

                        if (optionOnlyMatching.Value)
                        {
                            foreach (Match currentMatch in matches) writer.WriteLine(currentMatch.ValueSpan);
                        }
                        else writer.WriteLine(line);
                    }
                }
            }
            finally
            {
                inputFile.Dispose();
            }
        }
    }
}
