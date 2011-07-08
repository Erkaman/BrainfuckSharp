using System;

namespace BrainfuckSharp.AbstractSyntaxTree
{
    /// <summary>
    /// A loop in the Brainfuck language. It's of the form "[...]".
    /// </summary>
    public class Loop : Statement, IEquatable<Loop>
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


        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is 
        /// equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with 
        /// this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is 
        ///   equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as Loop);
        }

        /// <summary>
        /// Determines whether the specified 
        /// <see cref="BrainfuckSharp.AbstractSyntaxTree.Loop"/> is 
        /// equal to this instance.
        /// </summary>
        /// <param name="loop">The 
        /// <see cref="BrainfuckSharp.AbstractSyntaxTree.Loop"/> to compare 
        /// with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified
        ///   <see cref="BrainfuckSharp.AbstractSyntaxTree.Loop"/> is equal 
        ///   to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(Loop loop)
        {
            if (Object.ReferenceEquals(loop, null))
                return false;

            if (Object.ReferenceEquals(this, loop))
                return true;

            if (this.GetType() != loop.GetType())
                return false;

            return loop.Body.Equals(loop.Body);
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
            return this.Body.GetHashCode();
        }
    }
}
