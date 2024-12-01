using CuiLib.Commands;
using CuiLib.Options;
using System.Reflection;

namespace TextHandler.Commands
{
    /// <summary>
    /// メインのコマンドを表します。
    /// </summary>
    internal class MainCommand : Command
    {
        #region Options

        private readonly FlagOption optionHelp;
        private readonly FlagOption optionVersion;

        #endregion Options

        /// <summary>
        /// <see cref="MainCommand"/>の新しいインスタンスを初期化します。
        /// </summary>
        public MainCommand() : base("TextHandler")
        {
            Description = "Handles text file";

            optionHelp = new FlagOption('h', "help")
            {
                Description = "Displays help message",
            };
            optionVersion = new FlagOption('v', "version")
            {
                Description = "Displays version",
            };

            Options.Add(optionHelp);
            Options.Add(optionVersion);

            Children.Add(new CatCommand());
            Children.Add(new SearchCommand());
            Children.Add(new HeadCommand());
        }

        /// <inheritdoc/>
        protected override void OnExecution()
        {
            // This operation does not run when subcommand is specified.

            if (optionHelp.Value)
            {
                WriteHelp(Console.Out);
                return;
            }
            if (optionVersion.Value)
            {
                Console.WriteLine(Assembly.GetExecutingAssembly().GetName().Version!.ToString());
                return;
            }
        }
    }
}
