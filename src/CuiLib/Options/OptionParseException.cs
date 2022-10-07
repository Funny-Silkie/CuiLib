using System;
using System.Runtime.Serialization;

namespace CuiLib.Options
{
    /// <summary>
    /// オプションの変換過程の例外を表します。
    /// </summary>
    [Serializable]
    public class OptionParseException : Exception
    {
        public OptionParseException()
        {
        }

        public OptionParseException(string? message) : base(message)
        {
        }

        public OptionParseException(string? message, Exception? inner) : base(message, inner)
        {
        }

        protected OptionParseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
