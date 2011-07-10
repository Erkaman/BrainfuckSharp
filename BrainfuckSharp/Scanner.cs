using System.Collections.Generic;
using System.IO;

namespace BrainfuckSharp
{
    /// <summary>
    /// Scans the code the tokens.
    /// </summary>
    public static class Scanner
    {
        static Scanner()
        {
            symbols = new List<char>()
            {
                '<',
                '>',
                '+',
                '-',
                '.',
                ',',
                '[',
                ']'
            };
        }

        private static List<char> symbols;

        /// <summary>
        /// Scans the file for the tokens of the Brainfuck language.
        /// </summary>
        /// <param name="file">The file to scan.</param>
        /// <returns>The tokens of the string.</returns>
        public static IList<char> Scan(string file)
        {
            using (Stream stream = new FileStream(file,FileMode.Open))
            {
                return Scan(stream);
            }
        }

        /// <summary>
        /// Scans the stream for the tokens of the Brainfuck language.
        /// </summary>
        /// <param name="stream">The stream to scan.</param>
        /// <returns>The tokens of the stream.</returns>
        public static IList<char> Scan(Stream stream)
        {
            return Scan(new StreamReader(stream));
        }

        /// <summary>
        /// Scans the input for the tokens of the Brainfuck language.
        /// </summary>
        /// <param name="input">The input to scan.</param>
        /// <returns>The tokens of the input.</returns>
        public static IList<char> Scan(TextReader input)
        {
            List<char> tokens = new List<char>();
            while (input.Peek() != -1)
            {
                char ch = (char)input.Read();

                if (symbols.Contains(ch))
                    tokens.Add(ch);
            }
            return tokens;
        }

    }
}