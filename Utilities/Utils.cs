﻿namespace ContactForm.Utilities {
    internal static class Utils
    {
        public static string GetInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine() ?? string.Empty;
        }


        /// <summary>
        /// Prints colorized text
        /// </summary>
        /// <param name="text">text to print</param>
        /// <param name="color">text color from ConsoleColor enum</param>
        public static void PrintColoredText(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}
