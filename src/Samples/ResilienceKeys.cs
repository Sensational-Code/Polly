using Polly;

namespace Samples;

internal static class ResilienceKeys
{
    public static readonly ResiliencePropertyKey<bool> LongOperation = new("long-operation");
}
