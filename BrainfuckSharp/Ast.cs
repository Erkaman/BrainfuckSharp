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
    /// A sequence of statements in the brainfuck language.
    /// </summary>
    public class Sequence : Statement
    {
        /// <summary>
        /// The first statment,
        /// </summary>
        public Statement First;

        /// <summary>
        /// The second statement.
        /// </summary>
        public Statement Second;
    }

    /// <summary>
    /// A loop in the Brainfuck language. It's of the form "[...]".
    /// </summary>
    public class Loop : Statement
    {
        /// <summary>
        /// The body of the loop.
        /// </summary>
        public Statement Body;
    }
}
