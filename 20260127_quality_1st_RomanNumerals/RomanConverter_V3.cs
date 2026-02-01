namespace _20260127_quality_1st_RomanNumerals;

using System.Linq;

public class RomanConverter_V3
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
        
        return ToIntegerHelper(new List<int>(),romanpairs, 0, roman);
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
        int secondGreaterThanFirst = (romanpairs.First().Item2 ?? 0) - (romanpairs.First().Item1 ?? 0);
        int cumulativeSum = result.Sum();
        
        return (cumulativeSum, repetition, firstHeadDigit, secondHeadDigit, secondGreaterThanFirst) switch
        {
            // Largest_value_is_3999
            (> 3999, _,int, null, _)
                => throw new ArgumentException(originalRoman,
                    "Numeric value is above maximum allowed: 3999"),
            
            // last item is null - recursion ends
            (<= 3999, _, int, null, _)
                => cumulativeSum,
            
            // first item is null - first iteration
            (_, _, null, int, _) 
                => ToIntegerHelper([..result, romanpairs.First().Item2.Value], 
                    romanpairs.Skip(1).ToList(), repetition + 1, originalRoman),
            
            // if both tupple values are the same, increment the repetition counter
            (_, < 3, 1, 1, 0) => 
                ToIntegerHelper(
                    [.. result[..^1], romanpairs.First().Item2.Value + result[^1]], 
                    romanpairs.Skip(1).ToList(), repetition + 1, originalRoman),
            
            // a smaller roman numeral found, = reset the repetition counter
            (_, _, int, int, < 0) => 
                ToIntegerHelper(
                    [.. result, romanpairs.First().Item2.Value],
                    romanpairs.Skip(1).ToList(), 1, originalRoman),
            
            // IXCM_can_be_repeated_3_times_NegativeTests
            (_, 3,1,1, 0)
                => throw new ArgumentException(originalRoman,
                    "Roman numerals cannot repeat more than three times"),
            
            // VLD_can_not_be_repeated_NetagiveTest
            (_, _,5,5, 0)
                => throw new ArgumentException(originalRoman,
                    "Roman numerals V, L, and D can not be repeated"),
            
            // smaller_value_precedes_larger (perform substraction)
            (_, _, 1, int, > 0) => 
                ToIntegerHelper(
                    [.. result[..^1], romanpairs.First().Item2.Value - result[^1]],
                    romanpairs.Skip(1).ToList(), 1, originalRoman),
            
            // Smaller_value_precedes_larger_NegativeTests (illegal substraction)
            (_, _, 5, 1, > 0)
                => throw new ArgumentException(originalRoman,
                    "Invalid Roman numeral substraction"),
            
            _ => throw new ArgumentException(originalRoman,
            "case should never be reached, please report this bug"),
        };
    }
}



/*
 
 
            // old ...........................................
            
            // Smaller_value_precedes_larger (perform substraction)
            (_, _, char, _, > 0)
                => ToIntegerHelper(
                    [..result, 
                        romanpairs.First().Item2.Value - (romanpairs.First().Item1.Value * repetition)],
                    romanpairs.Skip(1).ToList(),
                    1,
                    originalRoman),
            
            // a smaller roman numeral found, = reset the repetition counter
            (_, _, _, _,< 0)
                => ToIntegerHelper(
                    [..result, + (romanpairs.First().Item1.Value * repetition)],
                    romanpairs.Skip(1).ToList(),
                    1,
                    originalRoman),
            
            // if both tupple values are the same, increment the repetition counter
            (_, _, char,_, 0) 
                => ToIntegerHelper(
                    result,
                    romanpairs.Skip(1).ToList(),
                    repetition + 1,
                    originalRoman),
            
            // if first recursion, increment the repetition counter
            (_, _, null,_, _)
                => ToIntegerHelper(
                    result,
                    romanpairs.Skip(1).ToList(),
                    repetition + 1,
                    originalRoman),

*/

/*
// 
            
            // Largest_value_is_3999
            (> 3999, _, _, _, null, _)
                => throw new ArgumentException(originalRoman,
                    "Numeric value is above maximum allowed: 3999"),
            
            // substraction
            (_, _, > 0, _, _, > 0) => 
                ToIntegerHelper(result - (romanpairs.First().Item1.Value * repetition * 2),
                    romanpairs.Skip(1).ToList(), 1, originalRoman),
            
            (_, _, _, _, _, < 0)
                => ToIntegerHelper(
                result + romanpairs.First().Item2.Value,
                romanpairs.Skip(1).ToList(),
                1,
                originalRoman),
*/