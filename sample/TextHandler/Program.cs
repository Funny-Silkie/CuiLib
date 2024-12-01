using CuiLib;
using TextHandler.Commands;

namespace TextHandler
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var command = new MainCommand();
            if (args.Length == 0)
            {
                command.WriteHelp(Console.Out);
                return;
            }

            try
            {
                command.Invoke(args);
            }
            catch (ArgumentAnalysisException e)
            {
                Console.Error.WriteError(e.Message);
            }
        }
    }
}
