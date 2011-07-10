using System;
using System.Collections.Generic;
using BrainfuckSharp.AbstractSyntaxTree;
using System.Reflection.Emit;
using System.Reflection;
using System.IO;

namespace BrainfuckSharp
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
        /// Initializes a new instance of the <see cref="CodeGenerator"/> class.
        /// </summary>
        /// <param name="block">The block of code to compile.</param>
        /// <param name="moduleName">
        /// The name of the compiled assembly.
        /// </param>
        public static void CompileBlock(Block block, string moduleName)
        {
            if (Path.GetFileName(moduleName) != moduleName)
            {
                throw new Exception("can only output into current directory!");
            }

            AssemblyName assemblyName = new AssemblyName(
                Path.GetFileNameWithoutExtension(moduleName));

            AssemblyBuilder assembly =
                AppDomain.CurrentDomain.DefineDynamicAssembly(
                assemblyName,
                AssemblyBuilderAccess.Save);

            ModuleBuilder moduleBuilder = assembly.DefineDynamicModule(moduleName);
            TypeBuilder programType = moduleBuilder.DefineType("Program");

            MethodBuilder mainMethod =
                programType.DefineMethod(
                "Main",
                MethodAttributes.Static,
                typeof(void),
                Type.EmptyTypes);

            il = mainMethod.GetILGenerator();

            DeclareVariables();

            // Actually compile the code.
            CompileBlock(block);

            // Return from the main function.
            il.Emit(OpCodes.Ret);

            // Create the program type.
            programType.CreateType();

            // Create the main method.
            moduleBuilder.CreateGlobalFunctions();

            // Set the entrypoint to the main method.
            assembly.SetEntryPoint(mainMethod);

            // Save the module to the file.
            assembly.Save(moduleName);
        }

        private static void CompileBlock(Block block)
        {
            // generate the code for every statement.
            foreach (Statement statement in block.Statements)
            {
                if (statement is Command)
                {
                    Command command = (Command)statement;
                    switch (command.CommandType)
                    {
                        case CommandType.IncrementPointer:
                            IncreaseSizeIfNecessary();
                            IncrementPointer(1);
                            break;
                        case CommandType.DecrementPointer:
                            DecrementPointer(1);
                            break;
                        case CommandType.OutputCharacter:
                            OutputCharacter();
                            break;
                        case CommandType.IncrementAtPointer:
                            IncrementAtPointer(1);
                            break;
                        case CommandType.DecrementAtPointer:
                            IncrementAtPointer(-1);
                            break;
                    }
                }
                else if (statement is Loop)
                {
                    Loop loop = (Loop)statement;
                    Label testLabel = il.DefineLabel();
                    Label bodyLabel = il.DefineLabel();

                    // go to testLabel and make the while loop test
                    il.Emit(OpCodes.Br, testLabel);

                    // bodyLabel:
                    il.MarkLabel(bodyLabel);
                    CompileBlock(loop.Body);

                    // testLabel: make the test.
                    il.MarkLabel(testLabel);

                    il.Emit(OpCodes.Ldloc, cells);
                    il.Emit(OpCodes.Ldloc, p);

                    il.Emit(OpCodes.Call,
                        typeof(List<byte>).GetMethod("get_Item", new System.Type[] { typeof(int) }));

                    il.Emit(OpCodes.Ldc_I4_0);
                    il.Emit(OpCodes.Ceq);
                    il.Emit(OpCodes.Ldc_I4_0);
                    il.Emit(OpCodes.Ceq);
                    LocalBuilder temp = il.DeclareLocal(typeof(int));
                    il.Emit(OpCodes.Stloc, temp);
                    il.Emit(OpCodes.Ldloc, temp);
                    il.Emit(OpCodes.Brtrue, bodyLabel);

                    // if the test is true go to label bodyLabel. else do nothing.
                }
                else
                {
                    throw new Exception(
                        "Don't know how to gen a " + statement.GetType().Name);
                }
            }
        }

        #region Private helpers

        private static void OutputCharacter()
        {
            EmitUtility.LoadLocal(il, cells);
            EmitUtility.LoadLocal(il, p);

            il.Emit(OpCodes.Call,
                typeof(List<byte>).GetMethod("get_Item", new System.Type[] { typeof(int) })
                );

            il.Emit(
                OpCodes.Call,
                typeof(System.Console).GetMethod("Write", new System.Type[] { typeof(byte) }));
        }

        /// <summary>
        /// Add a new cell containing zero to the list of the cells.
        /// </summary>
        private static void AddZeroToCells()
        {
            // cells.Add(0);
            EmitUtility.LoadLocal(il, cells);
            EmitUtility.LoadInt32(il, 0);
            il.Emit(OpCodes.Call,
                typeof(List<byte>).GetMethod("Add", new System.Type[] { typeof(byte) })
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

        // ++cells[p];
        private static void IncrementAtPointer(int increase)
        {
            // get cells[p]
            EmitUtility.LoadLocal(il, cells);
            EmitUtility.LoadLocal(il, p);
            il.Emit(OpCodes.Call,
                typeof(List<byte>).GetMethod("get_Item", new System.Type[] { typeof(int) })
                );

            // increase + cells[p]
            EmitUtility.LoadInt32(il, increase);
            il.Emit(OpCodes.Add);

            // temp = increase + cells[p]
            il.Emit(OpCodes.Stloc, temp);
            
            // cells[p] = temp
            il.Emit(OpCodes.Ldloc, cells);
            il.Emit(OpCodes.Ldloc, p);
            il.Emit(OpCodes.Ldloc, temp);
            il.Emit(OpCodes.Call,
                typeof(List<byte>).GetMethod("set_Item", new System.Type[] { typeof(int), typeof(byte) })
                );
        }

        private static void IncreaseSizeIfNecessary()
        {
            Label incrementPointerLabel = il.DefineLabel();

            // (p + 1 );
            EmitUtility.LoadLocal(il, p);
            EmitUtility.LoadInt32(il, 1);
            il.Emit(OpCodes.Add);

            // cells.Count;
            EmitUtility.LoadLocal(il, cells);
            il.Emit(OpCodes.Call,
                typeof(List<byte>).GetMethod("get_Count", Type.EmptyTypes)
                );

            // if (num + 1 == list.Count)
            il.Emit(OpCodes.Ceq);
            il.Emit(OpCodes.Brfalse_S, incrementPointerLabel);

            // increase the size by one.
            AddZeroToCells();

            il.MarkLabel(incrementPointerLabel);
        }

        #endregion
    }
}
