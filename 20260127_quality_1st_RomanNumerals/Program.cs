// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using System;

namespace _20260127_quality_1st_RomanNumerals; // ← make sure this matches your .csproj / namespace

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Roman Numeral → Integer Converter");
        Console.WriteLine("================================\n");

        Console.WriteLine("Enter a Roman numeral (e.g. IV, MCMXCIV, MMMCMXCIX)");
        Console.WriteLine("or 'q' to quit\n");

        while (true)
        {
            Console.Write("Roman numeral: ");
            string? input = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Please enter something.\n");
                continue;
            }

            if (input.ToLower() == "q" || input.ToLower() == "quit")
            {
                Console.WriteLine("\nGoodbye!");
                break;
            }

            try
            {
                int number = RomanConverter.ToInteger(input);
                Console.WriteLine($"{input.ToUpper()} → {number}\n");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}\n");
            }
            catch (Exception)
            {
                Console.WriteLine("Something went wrong. Please enter a valid Roman numeral.\n");
            }
        }
    }
}
