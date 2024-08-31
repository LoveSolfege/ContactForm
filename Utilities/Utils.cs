using static System.Net.Mime.MediaTypeNames;

namespace ContactForm.Utilities {
    internal static class Utils
    {
        /// <summary>
        /// Read user input after displaying custom text
        /// </summary>
        /// <param name="prompt">text to display</param>
        /// <returns>read text as string</returns>
        public static string GetInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine() ?? string.Empty;
        }

        /// <summary>
        /// Read user input after displaying custom text with specified color
        /// </summary>
        /// <param name="prompt">text to display</param>
        /// <param name="color">color from ConsoleColor enum</param>
        /// <returns></returns>
        public static string GetInput(string prompt, ConsoleColor color) {
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
        /// makes firt letter of a string capital
        /// </summary>
        /// <param name="str">input string</param>
        /// <returns>string with capital first letter</returns>
        public static string FirstLetterToUpper(this string str) {
            if (str == null)
                return "";

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }
    }
}
