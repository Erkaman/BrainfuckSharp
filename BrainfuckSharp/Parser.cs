using System.Collections.Generic;
using System;
using System.Linq;
using BrainfuckSharp.AbstractSyntaxTree;

namespace BrainfuckSharp
{
    /// <summary>
    /// Parses the tokens of Brainfuck source code.
    /// </summary>
    public static class Parser
    {
        /// <summary>
        /// Parses the tokens into an abstract syntax tree.
        /// </summary>
        /// <param name="tokens">The tokens to parsw.</param>
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
                                throw new Exception("Unbalanced bracket");

                            break;
                        }
                    case ']':
                        throw new Exception("Unbalanced bracket");
                }
            }
            return result;
        }

    }
}