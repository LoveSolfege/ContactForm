namespace ContactForm.Utilities {
    internal static class Utils
    {
        /// <summary>
        /// Read user input after displaying custom text with specified color
        /// </summary>
        /// <param name="prompt">text to display</param>
        /// <param name="color">color from ConsoleColor enum</param>
        /// <returns></returns>
        public static string GetInput(string prompt, ConsoleColor color = ConsoleColor.Gray) {
            Console.ForegroundColor = color;
            Console.Write(prompt);
            Console.ResetColor();
            return Console.ReadLine() ?? string.Empty;
        }


        /// <summary>
        /// Prints colorized text
        /// </summary>
        /// <param name="text">text to print</param>
        /// <param name="color">color from ConsoleColor enum</param>
        public static void PrintColoredText(string prompt, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(prompt);
            Console.ResetColor();
        }

        /// <summary>
        /// Clears console and puts colorized given string on top of it with possible line break
        /// </summary>
        /// <param name="header">string text to print</param>
        /// <param name="color">color from ConsoleColor enum<</param>
        /// <param name="addLineBreak">line break bool</param>
        public static void ClearConsolePlaceHeader(string header, ConsoleColor color = ConsoleColor.Gray, bool addLineBreak = false) {
            Console.Clear();
            Console.Write("\x1b[3J");
            if (addLineBreak) {
                PrintColoredText(header + "\n", color);
            }
            else {
                PrintColoredText(header, color);
            }
        }
    }
}
