﻿using System;
using System.Collections.Generic;
using BrainfuckSharp.AbstractSyntaxTree;
using System.Reflection.Emit;
using System.Reflection;
using System.IO;

namespace BrainfuckSharp
{
    /// <summary>
    /// The code generator
    /// </summary>
    public sealed class CodeGen
    {
        ILGenerator il = null;
        LocalBuilder p;
        LocalBuilder cells;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeGen"/> class.
        /// </summary>
        /// <param name="block">The block to compile.</param>
        /// <param name="moduleName">The name of the compiled assembly..</param>
        public CodeGen(Block block, string moduleName)
        {
            AssemblyName name = new AssemblyName(
                Path.GetFileNameWithoutExtension(moduleName));

            AssemblyBuilder asmb =
                AppDomain.CurrentDomain.DefineDynamicAssembly(
                name,
                AssemblyBuilderAccess.Save);

            ModuleBuilder modb = asmb.DefineDynamicModule(moduleName);
            TypeBuilder typeBuilder = modb.DefineType("Program");

            MethodBuilder methb =
                typeBuilder.DefineMethod(
                "Main",
                MethodAttributes.Static,
                typeof(void),
                System.Type.EmptyTypes);

            this.il = methb.GetILGenerator();

            DeclareVariables();

            // Compile the code.
            this.GenBlock(block);

            // return from the main function.
            il.Emit(OpCodes.Ret);
            typeBuilder.CreateType();
            modb.CreateGlobalFunctions();
            asmb.SetEntryPoint(methb);
            asmb.Save(moduleName);
            this.il = null;
        }

        /// <summary>
        /// Add a new cell containing zero to the list of the cells.
        /// </summary>
        private void AddZeroToCells()
        {
            // cells.Add(0);
            il.Emit(OpCodes.Ldloc, cells);
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Call,
                typeof(List<byte>).GetMethod("Add", new System.Type[] { typeof(byte) })
                );
        }

        private void DeclareVariables()
        {
            // int p
            p = il.DeclareLocal(typeof(int));

            // List<byte> cells = new List<byte>();
            cells = il.DeclareLocal(typeof(List<byte>));
            il.Emit(
                OpCodes.Newobj,
                typeof(List<byte>).GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Stloc, cells);

            AddZeroToCells();
        }

        private void IncrementPointer(int increase)
        {
            // load the variable from the stack.
            this.il.Emit(OpCodes.Ldloc, p);

            //push increase onto the stack as a in32
            this.il.Emit(OpCodes.Ldc_I4, increase);

            // the two values.
            this.il.Emit(OpCodes.Add);

            // pop the current value on the stack as local variable to p.
            this.il.Emit(OpCodes.Stloc, p);
        }

        private void IncrementAtPointer(int increase)
        {
            il.Emit(OpCodes.Ldloc, cells);
            il.Emit(OpCodes.Ldloc, p);

            il.Emit(OpCodes.Call,
                typeof(List<byte>).GetMethod("get_Item", new System.Type[] { typeof(int) })
                );

            il.Emit(OpCodes.Ldc_I4, increase);
            il.Emit(OpCodes.Add);
            il.Emit(OpCodes.Conv_I1);

            LocalBuilder temp = il.DeclareLocal(typeof(byte));
            il.Emit(OpCodes.Stloc, temp);

            il.Emit(OpCodes.Ldloc, cells);
            il.Emit(OpCodes.Ldloc, p);
            il.Emit(OpCodes.Ldloc, temp);

            il.Emit(OpCodes.Call,
                typeof(List<byte>).GetMethod("set_Item", new System.Type[] { typeof(int), typeof(byte) })
                );

        }

        private void IncreaseSizeIfNecessary()
        {
            Label incrementPointerLabel = il.DefineLabel();

            // (p + 1 );
            il.Emit(OpCodes.Ldloc, p);
            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Add);

            // cells.Count;
            il.Emit(OpCodes.Ldloc, cells);
            il.Emit(OpCodes.Call,
                typeof(List<byte>).GetMethod("get_Count", Type.EmptyTypes)
                );

            // bool b = (p + 1) == cells.Count;
            il.Emit(OpCodes.Ceq);

            LocalBuilder temp = il.DeclareLocal(typeof(bool));
            il.Emit(OpCodes.Stloc, temp);
            il.Emit(OpCodes.Ldloc, temp);

            il.Emit(OpCodes.Brfalse_S, incrementPointerLabel);

            // increase the size by one.
            AddZeroToCells();

            il.MarkLabel(incrementPointerLabel);
        }

        private void GenBlock(Block block)
        {
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
                            // increase size.
                            break;
                        case CommandType.DecrementPointer:
                            IncrementPointer(-1);
                            break;
                        case CommandType.OutputCharacter:

                            il.Emit(OpCodes.Ldloc, cells);
                            il.Emit(OpCodes.Ldloc, p);

                            il.Emit(OpCodes.Call,
                                typeof(List<byte>).GetMethod("get_Item", new System.Type[] { typeof(int) })
                                );

                            il.Emit(
                                OpCodes.Call,
                                typeof(System.Console).GetMethod("Write", new System.Type[] { typeof(byte) }));

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
                    GenBlock(loop.Body);

                    // testLabel: make the test.
                    il.MarkLabel(testLabel);

                    il.Emit(OpCodes.Ldloc, cells);
                    il.Emit(OpCodes.Ldloc, p);

                    il.Emit(OpCodes.Call,
                        typeof(List<byte>).GetMethod("get_Item", new System.Type[] { typeof(int) }) );

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
                    throw new System.Exception(
                        "don't know how to gen a " + statement.GetType().Name);
                }
            }
        }
    }
}