using System;
using System.Collections.Generic;
using System.Linq;

namespace BrainfuckSharpLibrary.AbstractSyntaxTree
{
    /// <summary>
    /// A block of statements in the brainfuck language.
    /// </summary>
    public class Block : Statement, IEquatable<Block>
    {
        /// <summary>
        /// A block of statements.
        /// </summary>
        public IList<Statement> Statements;

        /// <summary>
        /// Initializes a new instance of the <see cref="Block"/> class.
        /// </summary>
        public Block()
        {
            Statements = new List<Statement>();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> 
        /// is equal to this instance. 
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with 
        /// this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as Block);
        }

        /// <summary>
        /// Compares this instance with another for equality.
        /// </summary>
        /// <param name="block">The other instance to compare with..</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="Block"/> is equal to this 
        /// instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(Block block)
        {
            if (Object.ReferenceEquals(block, null))
                return false;

            if (Object.ReferenceEquals(this, block))
                return true;

            if (this.GetType() != block.GetType())
                return false;

            return this.Statements.SequenceEqual(block.Statements); 
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
            return Statements.GetHashCode();
        }
    }

}
