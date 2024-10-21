namespace System.Runtime.CompilerServices
{
#if NETSTANDARD

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    internal sealed class CallerArgumentExpressionAttribute : Attribute
    {
        public string ParameterName { get; }

        public CallerArgumentExpressionAttribute(string parameterName)
        {
            ParameterName = parameterName;
        }
    }

#endif
}

namespace System.Diagnostics.CodeAnalysis
{
#if !NET7_0_OR_GREATER

    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class StringSyntaxAttribute : Attribute
    {
        public const string CompositeFormat = "CompositeFormat";
        public const string DateOnlyFormat = "DateOnlyFormat";
        public const string DateTimeFormat = "DateTimeFormat";
        public const string NumericFormat = "NumericFormat";
        public const string Regex = "Regex";
        public const string TimeOnlyFormat = "TimeOnlyFormat";
        public const string TimeSpanFormat = "TimeSpanFormat";

        public string Syntax { get; }

        public StringSyntaxAttribute(string syntax)
        {
            Syntax = syntax;
        }
    }

#endif
}
