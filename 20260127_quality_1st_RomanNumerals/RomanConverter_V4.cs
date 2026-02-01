
namespace _20260127_quality_1st_RomanNumerals;

using System;
using System.Linq;


public class RomanConverter_V4
{
    private static readonly Dictionary<char, int> romanToInt = new()
    {
        ['I'] = 1, ['V'] = 5, ['X'] = 10, ['L'] = 50, ['C'] = 100, ['D'] = 500, ['M'] = 1000
    };

    private static int getRomanOrThrow(char c)
    {
        if (!romanToInt.TryGetValue(c, out var value))
            throw new ArgumentException("only I, V, X, L, C, D, M are allowed");
        return value;
    }
    
    public static int ToInteger(string roman)
    {
        if (roman is null) throw new ArgumentNullException(nameof(roman),
            "null is not allowed as an input, only I, V, X, L, C, D, M are allowed");
        if (roman is "") throw new ArgumentException(nameof(roman),
            "an empty string is not allowed");
        List<int?> romanints = roman.Select(x => (int?)getRomanOrThrow(x)).ToList();
        List<(int?,int?)> romanpairs = romanints.Prepend(null).Zip(romanints.Append(null), (a, b) => (a, b)).ToList();
        
        int result = ToIntegerHelper(new List<int>(),romanpairs, 0, roman);
        if (result > 3999) throw new ArgumentException(roman,
                "Numeric value is above maximum allowed: 3999");
        return result;
    }

    private static int ToIntegerHelper(
        List<int> result,
        List<(int?,int?)> romanpairs,
        int repetition,
        string originalRoman)
    {
        int? firstHeadDigit = romanpairs.First().Item1.HasValue ? 
            int.Parse(romanpairs.First().Item1.Value.ToString().First().ToString()) : null;
        int? secondHeadDigit = romanpairs.First().Item2.HasValue ? 
            int.Parse(romanpairs.First().Item2.Value.ToString().First().ToString()) : null;
        int firstGreaterThanSecond = (romanpairs.First().Item1 ?? 0) - (romanpairs.First().Item2 ?? 0);
        // Single source of truth: classify using Decide(...), then act on the decision
        var decision = Decide(repetition, firstHeadDigit, secondHeadDigit, firstGreaterThanSecond);

        switch (decision)
        {
            case Decision.End:
                // Last item is null - recursion ends
                return result.Sum();

            case Decision.FirstIter:
                // First item is null - first iteration
                return ToIntegerHelper(
                    [.. result, romanpairs.First().Item2!.Value],
                    romanpairs.Skip(1).ToList(), repetition + 1, originalRoman);

            case Decision.LargerPrecedesSmaller:
                // Larger value precedes smaller: push current and reset repetition
                return ToIntegerHelper(
                    [.. result, romanpairs.First().Item2!.Value],
                    romanpairs.Skip(1).ToList(), 1, originalRoman);

            case Decision.AddRepeat:
                // Equal 1s under repeat limit: fold into last and increment repetition
                return ToIntegerHelper(
                    [.. result[..^1], romanpairs.First().Item2!.Value + result[^1]],
                    romanpairs.Skip(1).ToList(), repetition + 1, originalRoman);

            case Decision.TooManyRepeats:
                // More than three repeats of I/X/C/M are not allowed
                throw new ArgumentException(originalRoman,
                    "Roman numerals cannot repeat more than three times");

            case Decision.RepeatVLD:
                // V, L, D cannot repeat
                throw new ArgumentException(originalRoman,
                    "Roman numerals V, L, and D can not be repeated");

            case Decision.Subtract:
                // Subtractive pair with head 1: compute difference and reset repetition
                return ToIntegerHelper(
                    [.. result[..^1], romanpairs.First().Item2!.Value - result[^1]],
                    romanpairs.Skip(1).ToList(), 1, originalRoman);

            case Decision.IllegalSubtract:
                // Illegal subtract (e.g., V before X)
                throw new ArgumentException(originalRoman,
                    "Invalid Roman numeral substraction");

            default:
                // DefaultUnexpected → our internal invariant
                throw new InternalInvariantViolationException(
                    $"Default arm reached in ToIntegerHelper for tuple " +
                    $"(rep={repetition}, " +
                    $"a={firstHeadDigit?.ToString() ?? "null"}, " +
                    $"b={secondHeadDigit?.ToString() ?? "null"}, " +
                    $"cmp={firstGreaterThanSecond}) while processing '{originalRoman}'");
        }
    }
    
    // Internal decision surface for tests only
    internal enum Decision
    {
        End,
        FirstIter,
        LargerPrecedesSmaller,
        AddRepeat,
        TooManyRepeats,
        RepeatVLD,
        Subtract,
        IllegalSubtract,
        DefaultUnexpected
    }

    // Mirrors the predicates used by the ToIntegerHelper switch.
    // It does not perform any state mutation; it only classifies the current tuple.
    internal static Decision Decide(
        int repetition,
        int? firstHeadDigit,
        int? secondHeadDigit,
        int firstGreaterThanSecond)
    {
        return (repetition, firstHeadDigit, secondHeadDigit, firstGreaterThanSecond) switch
        {
            // End
            (_, int, null, _) => Decision.End,

            // First iteration
            (_, null, int, _) => Decision.FirstIter,

            // Larger value precedes smaller
            (_, int, int, > 0) => Decision.LargerPrecedesSmaller,

            // Equal 1s under repeat limit
            (< 3, 1, 1, 0) => Decision.AddRepeat,

            // Too many repeats of 1
            (>= 3, 1, 1, 0) => Decision.TooManyRepeats,

            // V/L/D cannot repeat
            (_, 5, 5, 0) => Decision.RepeatVLD,

            // Valid subtract (I before V/X/C/M → head 1, sign < 0)
            (_, 1, int, < 0) => Decision.Subtract,

            // Illegal subtract (V/L/D before larger)
            (_, 5, int, < 0) => Decision.IllegalSubtract,

            _ => Decision.DefaultUnexpected
        };
    }
}
