namespace _20260127_quality_1st_RomanNumerals;

// Thrown when an internal invariant (assumed unreachable code path) is violated.
// Exposed as internal and visible to tests via InternalsVisibleTo.
internal sealed class InternalInvariantViolationException : Exception
{
    public InternalInvariantViolationException() { }
    public InternalInvariantViolationException(string? message) : base(message) { }
    public InternalInvariantViolationException(string? message, Exception? innerException) : base(message, innerException) { }
}