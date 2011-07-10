using System;
using System.Reflection;
using System.Reflection.Emit;

namespace BrainfuckSharpLibrary
{
    /// <summary>
    /// Utilty method for System.Reflection.Emit.
    /// Source:
    /// http://www.java2s.com/Open-Source/CSharp/Database/NBearLite/NBearLite/Emit/EmitUtils.cs.htm
    /// </summary>
    internal static class EmitUtility
    {
        /// <summary>
        /// Pushes an integer onto the stack.
        /// </summary>
        /// <param name="gen">The IL generator to use.</param>
        /// <param name="value">The integer to push onto the stack.</param>
        internal static void LoadInt32(ILGenerator gen, int value)
        {
            if (gen == null)
                throw new ArgumentNullException("gen");

            switch (value)
            {
                case -1:
                    gen.Emit(OpCodes.Ldc_I4_M1);
                    break;
                case 0:
                    gen.Emit(OpCodes.Ldc_I4_0);
                    break;
                case 1:
                    gen.Emit(OpCodes.Ldc_I4_1);
                    break;
                case 2:
                    gen.Emit(OpCodes.Ldc_I4_2);
                    break;
                case 3:
                    gen.Emit(OpCodes.Ldc_I4_3);
                    break;
                case 4:
                    gen.Emit(OpCodes.Ldc_I4_4);
                    break;
                case 5:
                    gen.Emit(OpCodes.Ldc_I4_5);
                    break;
                case 6:
                    gen.Emit(OpCodes.Ldc_I4_6);
                    break;
                case 7:
                    gen.Emit(OpCodes.Ldc_I4_7);
                    break;
                case 8:
                    gen.Emit(OpCodes.Ldc_I4_8);
                    break;
                default:
                    if (value >= SByte.MinValue && value <= SByte.MaxValue)
                        gen.Emit(OpCodes.Ldc_I4_S, (SByte)value);
                    else
                        gen.Emit(OpCodes.Ldc_I4, value);
                    break;
            }
        }

        /// <summary>
        /// Pops the current value from the stack and stores in the local 
        /// variable at the index
        /// </summary>
        /// <param name="gen">The IL generator to use.</param>
        /// <param name="index">
        /// The index to the store the popped value at.
        /// </param>
        internal static void StoreLocal(ILGenerator gen, int index)
        {
            if (gen == null)
                throw new ArgumentNullException("gen");

            switch (index)
            {
                case 0:
                    gen.Emit(OpCodes.Stloc_0);
                    break;
                case 1:
                    gen.Emit(OpCodes.Stloc_1);
                    break;
                case 2:
                    gen.Emit(OpCodes.Stloc_2);
                    break;
                case 3:
                    gen.Emit(OpCodes.Stloc_3);
                    break;
                default:
                    if (index <= 255)
                        gen.Emit(OpCodes.Stloc_S, index);
                    else
                        gen.Emit(OpCodes.Stloc, index);
                    break;
            }
        }

        /// <summary>
        /// Pops the current value from the stack and stores in the local 
        /// variable.
        /// </summary>
        /// <param name="gen">The IL generator to use.</param>
        /// <param name="local">
        /// The variable to store the popped value in.
        /// </param>
        internal static void StoreLocal(ILGenerator gen, LocalBuilder local)
        {
            StoreLocal(gen, local.LocalIndex);
        }

        /// <summary>
        /// Load a variable onto the stack.
        /// </summary>
        /// <param name="gen">The IL generator to use.</param>
        /// <param name="local">
        /// The variable to load.
        /// </param>
        internal static void LoadLocal(ILGenerator gen, LocalBuilder local)
        {
            LoadLocal(gen, local.LocalIndex);
        }

        /// <summary>
        /// Load a variable onto the stack from the specfied index.
        /// </summary>
        /// <param name="gen">The IL generator to use.</param>
        /// <param name="index">
        /// The index to load the variable from.
        /// </param>
        internal static void LoadLocal(ILGenerator gen, int index)
        {
            if (gen == null)
                throw new ArgumentNullException("gen");

            switch (index)
            {
                case 0:
                    gen.Emit(OpCodes.Ldloc_0);
                    break;
                case 1:
                    gen.Emit(OpCodes.Ldloc_1);
                    break;
                case 2:
                    gen.Emit(OpCodes.Ldloc_2);
                    break;
                case 3:
                    gen.Emit(OpCodes.Ldloc_3);
                    break;
                default:
                    if (index <= 255)
                        gen.Emit(OpCodes.Ldloc_S, index);
                    else
                        gen.Emit(OpCodes.Ldloc, index);
                    break;
            }
        }
    }
}
