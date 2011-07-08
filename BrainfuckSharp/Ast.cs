using System.Collections.Generic;
namespace BrainfuckSharp
{
    /// <summary>
    /// A statement in the Brainfuck language.
    /// </summary>
    public abstract class Statement
    {
    }

    /// <summary>
    /// A single command in the Brainfuck language.
    /// </summary>
    public class Command : Statement
    {
        /// <summary>
        /// The type of the command.
        /// </summary>
        public CommandType CommandType;

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="commandType">The type of command.</param>
        public Command(CommandType commandType)
        {
            this.CommandType = commandType;
        }
    }

    /// <summary>
    /// The type of a command.
    /// </summary>
    public enum CommandType
    {
        /// <summary>
        /// Increment the data pointer.
        /// </summary>
        IncrementPointer,

        /// <summary>
        /// Decrement the data pointer,
        /// </summary>
        DecrementPointer,


        /// <summary>
        /// Increment the value at the data pointer.
        /// </summary>
        IncrementAtPointer,


        /// <summary>
        /// Decrement the value at the data pointer.
        /// </summary>
        DecrementAtPointer,


        /// <summary>
        /// Output the value at the pointer as a character.
        /// </summary>
        OutputCharacter,


        /// <summary>
        /// Input a value to the value at the pointer .
        /// </summary>
        InputCharacter,
    }

    /// <summary>
    /// A block of statements in the brainfuck language.
    /// </summary>
    public class Block : Statement
    {
        /// <summary>
        /// A block of statements.
        /// </summary>
        public IList<Statement> Statements;
    }

    /// <summary>
    /// A loop in the Brainfuck language. It's of the form "[...]".
    /// </summary>
    public class Loop : Statement
    {
        /// <summary>
        /// The body of the loop.
        /// </summary>
        public Block Body;

        /// <summary>
        /// Initializes a new instance of the <see cref="Loop"/> class.
        /// </summary>
        /// <param name="body">The body of the loop.</param>
        public Loop(Block body)
        {
            Body = body;
        }
    }
}
