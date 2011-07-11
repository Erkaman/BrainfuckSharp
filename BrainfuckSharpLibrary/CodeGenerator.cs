using System;
using System.Collections.Generic;
using BrainfuckSharpLibrary.AbstractSyntaxTree;
using System.Reflection.Emit;
using System.Reflection;
using System.IO;
using System.Linq;

namespace BrainfuckSharpLibrary 
{
    /// <summary>
    /// Generates the code of the compiler.
    /// </summary>
    public static class CodeGenerator
    {
        private static ILGenerator il;
        private static LocalBuilder p;
        private static LocalBuilder cells;
        private static LocalBuilder temp;

        /// <summary>
        /// Compiles a block code to a file.
        /// </summary>
        /// <param name="block">The block of code to compile.</param>
        /// <param name="compiledFile">
        /// The name of the compiled file.
        /// </param>
        /// <exception cref="System.NotSupportedException">
        /// The code contained a statement unsupported by this parser.
        /// </exception>
        public static void CompileBlock(Block block, string compiledFile)
        {
            AssemblyName assemblyName = new AssemblyName(
                Path.GetFileNameWithoutExtension(compiledFile));

            AssemblyBuilder assembly =
                AppDomain.CurrentDomain.DefineDynamicAssembly(
                assemblyName,
                AssemblyBuilderAccess.Save);

            ModuleBuilder moduleBuilder =
                assembly.DefineDynamicModule(compiledFile);
            TypeBuilder programType = moduleBuilder.DefineType("Program");

            MethodBuilder mainMethod =
                programType.DefineMethod(
                "Main",
                MethodAttributes.Static,
                typeof(void),
                Type.EmptyTypes);

            il = mainMethod.GetILGenerator();

            EmitMainMethod(block);

            // Create the program type.
            programType.CreateType();

            // Create the main method.
            moduleBuilder.CreateGlobalFunctions();

            assembly.SetEntryPoint(mainMethod);
            assembly.Save(compiledFile);
        }

        private static void EmitMainMethod(Block block)
        {
            il.BeginExceptionBlock();

            DeclareVariables();
            CompileBlock(block);

            il.BeginCatchBlock(typeof(ArgumentOutOfRangeException));

            il.Emit(OpCodes.Ldstr, "Attempted to access negative data pointer");
            il.Emit( 
                OpCodes.Call,  
                typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }));

            il.EndExceptionBlock();

            // Return from the main method
            il.Emit(OpCodes.Ret);

        }

        /// <summary>
        /// Compiles a source file an output to file.
        /// </summary>
        /// <param name="file">The source file to compile.</param>
        /// <param name="compiledFile">
        /// The name of the compiled file.
        /// </param>
        public static void CompileFile(string file, string compiledFile)
        {
            CompileBlock(Parser.ParseTokens(file), compiledFile);
        }

        /// <summary>
        /// Compiles a TextReader to a file.
        /// </summary>
        /// <param name="input">The input to compile.</param>
        /// <param name="compiledFile">
        /// The name of the compiled file.
        /// </param>
        public static void CompileTextReader(TextReader input, string compiledFile)
        {
            CompileBlock(Parser.ParseTokens(input), compiledFile);
        }

        /// <summary>
        /// Compiles a stream to a file.
        /// </summary>
        /// <param name="stream">The stream to compile.</param>
        /// <param name="compiledFile">
        /// The name of the compiled file.
        /// </param>
        public static void CompileStream(Stream stream, string compiledFile)
        {
            CompileBlock(Parser.ParseTokens(stream), compiledFile);
        }

        static int repetitions = 1;

        private static void CompileBlock(Block block)
        {
            // generate the code for every statement.
            //foreach (Statement statement in block.Statements)
            for (int i = 0; i < block.Statements.Count; ++i )
            {
                Statement statement = block.Statements[i];
                if (statement is Command)
                {
                    Command command = (Command)statement;


                    if ((i != block.Statements.Count - 1) &&
                        block.Statements[i + 1] is Command &&
                        command.CommandType == ((Command)block.Statements[i + 1]).CommandType &&
                        command.CommandType != CommandType.OutputCharacter &&
                        command.CommandType != CommandType.InputCharacter)
                    {
                        ++repetitions;
                    }
                    else
                    {
                        switch (command.CommandType)
                        {
                            case CommandType.IncrementPointer:
                                AccommodateSize();
                                IncrementPointer(repetitions);
                                break;
                            case CommandType.DecrementPointer:
                                DecrementPointer(repetitions);
                                break;
                            case CommandType.IncrementAtPointer:
                                IncrementAtPointer(repetitions);
                                break;
                            case CommandType.DecrementAtPointer:
                                DecrementAtPointer(repetitions);
                                break;
                            case CommandType.OutputCharacter:
                                OutputCharacter();
                                break;
                            case CommandType.InputCharacter:
                                InputCharacter();
                                break;
                        }
                        repetitions = 1;
                    }
                }
                else if (statement is Loop)
                {
                    CompileLoop((Loop)statement);
                }
                else
                    throw new NotSupportedException(
                        "Don't know how to generate code for a " + 
                        statement.GetType().Name);
            }
        }

        private static void CompileLoop(Loop loop)
        {
            Label testLabel = il.DefineLabel();
            Label bodyLabel = il.DefineLabel();

            //  Go and make the test first.
            il.Emit(OpCodes.Br, testLabel);

            // The body of the loop.
            il.MarkLabel(bodyLabel);
            CompileBlock(loop.Body);

            // Make the test of the loop.
            il.MarkLabel(testLabel);

            // cells[p]
            EmitUtility.LoadLocal(il, cells);
            EmitUtility.LoadLocal(il, p);
            il.Emit(OpCodes.Call,
                typeof(List<byte>).GetMethod("get_Item", new Type[] { typeof(int) }));

            EmitUtility.LoadInt32(il, 0);

            // while(cells[p] != 0)
            il.Emit(OpCodes.Ceq);
            il.Emit(OpCodes.Brfalse, bodyLabel);

            // if the test is true go to label bodyLabel; else do nothing.
        }

        private static void OutputCharacter()
        {
            EmitUtility.LoadLocal(il, cells);
            EmitUtility.LoadLocal(il, p);

            il.Emit(OpCodes.Call,
                typeof(List<byte>).GetMethod("get_Item", new Type[] { typeof(int) })
                );

            il.Emit(
                OpCodes.Call,
                typeof(Console).GetMethod("Write", new Type[] { typeof(byte) }));
        }

        // cells[p] = (byte) input.Read();
        private static void InputCharacter()
        {
            il.Emit(OpCodes.Call,
                typeof(Console).GetMethod("Read", Type.EmptyTypes));
            EmitUtility.StoreLocal(il, temp);

            AssignAtPointer(temp);
        }

        private static void AddZeroToCells()
        {
            // cells.Add(0);
            EmitUtility.LoadLocal(il, cells);
            EmitUtility.LoadInt32(il, 0);
            il.Emit(OpCodes.Call,
                typeof(List<byte>).GetMethod("Add", new Type[] { typeof(byte) })
                );
        }

        private static void DeclareVariables()
        {
            // int p
            p = il.DeclareLocal(typeof(int));

            // List<byte> cells = new List<byte>();
            cells = il.DeclareLocal(typeof(List<byte>));
            il.Emit(
                OpCodes.Newobj,
                typeof(List<byte>).GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Stloc, cells);

            // temporary variable used in additions and subtractions.
            temp = il.DeclareLocal(typeof(byte));


            AddZeroToCells();
        }

        // p += increase
        private static void IncrementPointer(int increase)
        {
            ChangePointer(OpCodes.Add,increase);
        }

        // p += increase
        private static void DecrementPointer(int decrease)
        {
            ChangePointer(OpCodes.Sub, decrease);
        }

        private static void ChangePointer(OpCode operation,int change)
        {
            EmitUtility.LoadLocal(il, p);
            EmitUtility.LoadInt32(il, change);
            il.Emit(operation);
            EmitUtility.StoreLocal(il, p);
        }

        // cells[p] += increase;
        private static void IncrementAtPointer(int increase)
        {
            ChangeAtPointer(OpCodes.Add, increase);
        }

        // cells[p] -= decrease;
        private static void DecrementAtPointer(int decrease)
        {
            ChangeAtPointer(OpCodes.Sub, decrease);
        }

        private static void ChangeAtPointer(OpCode operation, int change)
        {
            // get cells[p]
            EmitUtility.LoadLocal(il, cells);
            EmitUtility.LoadLocal(il, p);
            il.Emit(OpCodes.Call,
                typeof(List<byte>).GetMethod("get_Item", new Type[] { typeof(int) })
                );

            // increase op cells[p]
            EmitUtility.LoadInt32(il, change);
            il.Emit(operation);

            // temp = increase op cells[p]
            il.Emit(OpCodes.Stloc, temp);

            // cells[p] = temp
            AssignAtPointer(temp);
        }

        private static void AssignAtPointer(LocalBuilder assignment)
        {
            // cells[p] = assignment
            il.Emit(OpCodes.Ldloc, cells);
            il.Emit(OpCodes.Ldloc, p);
            il.Emit(OpCodes.Ldloc, assignment);
            il.Emit(OpCodes.Call,
                typeof(List<byte>).GetMethod("set_Item", new Type[] { typeof(int), typeof(byte) })
                );
        }

        private static void AccommodateSize()
        {
            Label incrementPointerLabel = il.DefineLabel();

            Action<int> PushValues = (i) =>
            {
                // (p +  repetitions);
                EmitUtility.LoadLocal(il, p);
                EmitUtility.LoadInt32(il, repetitions + i);
                il.Emit(OpCodes.Add);

                // cells.Count;
                EmitUtility.LoadLocal(il, cells);
                il.Emit(OpCodes.Call,
                    typeof(List<byte>).GetMethod("get_Count", Type.EmptyTypes)
                    );
            };

            PushValues(0);
            // if (p + repetitions >= list.Count)
            il.Emit(OpCodes.Blt_S, incrementPointerLabel);

            if (repetitions > 1)
            {
                // (p +v 1 + repetitions - list.Count)
                PushValues(1);
                il.Emit(OpCodes.Sub);
                // store the number of cells to add.
                EmitUtility.StoreLocal(il, temp);

                RepeatAction(temp, AddZeroToCells);
            }
            else
                AddZeroToCells();

            il.MarkLabel(incrementPointerLabel);
        }

        private static void RepeatAction(
            LocalBuilder repetitions, 
            Action action)
        {
            // for loop.
            Label testLabel = il.DefineLabel();
            Label bodyLabel = il.DefineLabel();

            // initialization:
            LocalBuilder i = il.DeclareLocal(typeof(int));
            EmitUtility.LoadInt32(il, 0);
            EmitUtility.StoreLocal(il, i);

            il.Emit(OpCodes.Br, testLabel);

            // body
            il.MarkLabel(bodyLabel);
            action();

            // increment:
            EmitUtility.LoadLocal(il, i);
            EmitUtility.LoadInt32(il, 1);
            il.Emit(OpCodes.Add);
            EmitUtility.StoreLocal(il, i);

            //test.
            il.MarkLabel(testLabel);

            EmitUtility.LoadLocal(il, i);
            EmitUtility.LoadLocal(il, temp);
            il.Emit(OpCodes.Blt, bodyLabel);
        }
    }
}
