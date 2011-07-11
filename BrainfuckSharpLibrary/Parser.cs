using System.Collections.Generic;
using System;
using System.Linq;
using BrainfuckSharpLibrary.AbstractSyntaxTree;
using System.IO;

namespace BrainfuckSharpLibrary
{
    /// <summary>
    /// Parses the tokens of Brainfuck source code.
    /// </summary>
    public static class Parser
    {                
        /// <summary>
        /// Parses the input into an abstract syntax tree.
        /// </summary>
        /// <param name="input">The input to parse.</param>
        /// <returns>
        /// The abstract syntax tree parsed from the input.
        /// </returns>
        public static Block ParseTokens(TextReader input)
        {
            return ParseTokens(Scanner.Scan(input));
        }

        /// <summary>
        /// Parses the stream into an abstract syntax tree.
        /// </summary>
        /// <param name="stream">The stream to parse.</param>
        /// <returns>
        /// The abstract syntax tree parsed from the stream.
        /// </returns>
        public static Block ParseTokens(Stream stream)
        {
            return ParseTokens(Scanner.Scan(stream));
        }

        /// <summary>
        /// Parses the contents of a file into an abstract syntax tree.
        /// </summary>
        /// <param name="file">The file to parse.</param>
        /// <returns>
        /// The abstract syntax tree parsed from the contents of the file.
        /// </returns>
        public static Block ParseTokens(string file)
        {
            return ParseTokens(Scanner.Scan(file));
        }

        /// <summary>
        /// Parses the tokens into an abstract syntax tree.
        /// </summary>
        /// <param name="tokens">The tokens to parse.</param>
        /// <returns>
        /// The abstract syntax tree parsed from the tokens.
        /// </returns>
        public static Block ParseTokens(IList<char> tokens)
        {
            Block result = new Block();
            result.Statements = new List<Statement>();

            // A shortcut that makes it easier to add commands to the result.
            Action<CommandType> AddCommand = type => 
                result.Statements.Add(new Command(type));

            for (int i = 0; i < tokens.Count; ++i)
            {
                char ch = tokens[i];

                switch (ch)
                {
                    case '>':
                        AddCommand(CommandType.IncrementPointer);
                        break;
                    case '<':
                        AddCommand(CommandType.DecrementPointer);
                        break;
                    case '+':
                        AddCommand(CommandType.IncrementAtPointer);
                        break;
                    case '-':
                        AddCommand(CommandType.DecrementAtPointer);
                        break;
                    case '.':
                        AddCommand(CommandType.OutputCharacter);
                        break;
                    case ',':
                        AddCommand(CommandType.InputCharacter);
                        break;
                    case '[':
                        {
                            int unmatchedBrackets = 0;

                            // find the matching bracket.
                            for (int j = i; j < tokens.Count ; ++j)
                            {
                                // handle inner brackets.

                                if (tokens[j] == '[')
                                    ++unmatchedBrackets;
                                else if (tokens[j] == ']')
                                    --unmatchedBrackets;

                                if (unmatchedBrackets == 0)
                                {
                                    // get the tokens of the loop.
                                    IList<char> loopTokens = new List<char>( 
                                        tokens.Skip(i + 1).Take(j - i - 1));

                                    // Parse the body of the loop.
                                    Loop loop = 
                                        new Loop(ParseTokens(loopTokens));

                                    // add the loop to the result.
                                    result.Statements.Add(loop);

                                    // continue the parsing after the loop.
                                    i = j;

                                    break;
                                }
                            }
                            if (unmatchedBrackets != 0)
                                throw new SyntaxErrorException(
                                    "Unbalanced brackets.");
                            break;
                        }
                    case ']':
                        throw new SyntaxErrorException(
                            "Unbalanced brackets.");
                }
            }
            return result;
        }

    }
}