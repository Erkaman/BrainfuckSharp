using System.Collections.Generic;
using System.IO;

namespace BrainfuckSharp
{
    /// <summary>
    /// Scans the code the tokens.
    /// </summary>
    public class Scanner
    {
        /// <summary>
        /// Gets or sets the tokens of the code.
        /// </summary>
        /// <value>
        /// The tokens of the code.
        /// </value>
        public IList<char> Tokens { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Scanner"/> class.
        /// </summary>
        /// <param name="input">The input to scan the tokens from.</param>
        public Scanner(TextReader input)
        {
            Tokens = new List<char>();
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
            Scan(input);
        }

        private List<char> symbols;

        private void Scan(TextReader input)
        {
            while (input.Peek() != -1)
            {
                char ch = (char)input.Read();

                // The item is a token if and only if it is a character in the 
                // syntax of Brainfuck.
                if (symbols.Contains(ch))
                    Tokens.Add(ch);
            }
        }

    }
}