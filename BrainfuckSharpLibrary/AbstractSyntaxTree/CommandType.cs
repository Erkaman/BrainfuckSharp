namespace BrainfuckSharpLibrary.AbstractSyntaxTree
{
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

}
