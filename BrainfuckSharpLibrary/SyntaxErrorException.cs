using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrainfuckSharpLibrary
{
    /// <summary>
    /// Occurs when a syntax error is found.
    /// </summary>
    public class SyntaxErrorException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="SyntaxErrorException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public SyntaxErrorException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="SyntaxErrorException"/> class.
        /// </summary>
        public SyntaxErrorException(){}
    }
}
