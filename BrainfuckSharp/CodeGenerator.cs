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
                throw new Exception("can only output into current directory!");

            AssemblyName assemblyName = new AssemblyName(
                Path.GetFileNameWithoutExtension(moduleName));

            AssemblyBuilder assembly =
                AppDomain.CurrentDomain.DefineDynamicAssembly(
                assemblyName,
                AssemblyBuilderAccess.Save);

            ModuleBuilder moduleBuilder = 
                assembly.DefineDynamicModule(moduleName);
            TypeBuilder programType = moduleBuilder.DefineType("Program");

            MethodBuilder mainMethod =
                programType.DefineMethod(
                "Main",
                MethodAttributes.Static,
                typeof(void),
                Type.EmptyTypes);

            il = mainMethod.GetILGenerator();

            DeclareVariables();
            CompileBlock(block);

            // Return from the main method
            il.Emit(OpCodes.Ret);

            // Create the program type.
            programType.CreateType();

            // Create the main method.
            moduleBuilder.CreateGlobalFunctions();

            assembly.SetEntryPoint(mainMethod);
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
                        case CommandType.InputCharacter:
                            InputCharacter();
                            break;
                        case CommandType.IncrementAtPointer:
                            IncrementAtPointer(1);
                            break;
                        case CommandType.DecrementAtPointer:
                            DecrementAtPointer(1);
                            break;
                    }
                }
                else if (statement is Loop)
                {
                    CompileLoop((Loop)statement);
                }
                else
                    throw new Exception(
                        "Don't know how to gen a " + statement.GetType().Name);
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

            /*EmitUtility.LoadLocal(il, cells);
            EmitUtility.LoadLocal(il, p);
            EmitUtility.LoadLocal(il, temp);
            il.Emit(
                OpCodes.Call,
                typeof(Console).GetMethod("set_Item", new Type[] { typeof(byte) }));*/
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

            AddZeroToCells();

            il.MarkLabel(incrementPointerLabel);
        }
    }
}
