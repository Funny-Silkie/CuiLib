using System;

namespace CuiLib.Internal.Versions
{
#if NETSTANDARD2_0

    internal delegate void SpanAction<T, in TArg>(Span<T> span, TArg arg);

    internal delegate void ReadOnlySpanAction<T, in TArg>(ReadOnlySpan<T> span, TArg arg);

#endif
}
