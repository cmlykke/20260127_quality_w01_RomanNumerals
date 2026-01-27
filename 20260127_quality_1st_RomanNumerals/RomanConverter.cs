namespace _20260127_quality_1st_RomanNumerals;  // ← adjust if your namespace is different

public static class RomanConverter
{
    private static HashSet<char> allowedCharacters = new HashSet<char> { 'I', 'V', 'X', 'L', 'C', 'D', 'M' };
    
    public static int ToInteger(string roman)
    {
        // Check for invalid characters
        foreach (char c in roman)
        {
            if (!allowedCharacters.Contains(c))
            {
                throw new ArgumentException("only I, V, X, L, C, D, M are allowed");
            }
        }
        
        return 1;
    }
}