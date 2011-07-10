using System;

namespace BrainfuckSharpLibrary.AbstractSyntaxTree
{
    /// <summary>
    /// A single command in the Brainfuck language.
    /// </summary>
    public class Command : Statement, IEquatable<Command>
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

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> 
        /// is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with 
        /// this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is 
        ///   equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as Command);
        }

        /// <summary>
        /// Determines whether the specified 
        /// <see cref="BrainfuckSharpLibrary.AbstractSyntaxTree.Command"/> 
        /// is equal to this instance.
        /// </summary>
        /// <param name="command">The 
        /// <see cref="BrainfuckSharpLibrary.AbstractSyntaxTree.Command"/> to compare with 
        /// this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified 
        ///   <see cref="BrainfuckSharpLibrary.AbstractSyntaxTree.Command"/> is 
        ///   equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(Command command)
        {
            if (Object.ReferenceEquals(command, null))
                return false;

            if (Object.ReferenceEquals(this, command))
                return true;

            if (this.GetType() != command.GetType())
                return false;

            return this.CommandType == command.CommandType;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing
        /// algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return CommandType.GetHashCode();
        }
    }
}
