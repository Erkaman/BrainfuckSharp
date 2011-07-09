using System;
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
            if (Path.GetFileName(moduleName) != moduleName)
                throw new System.Exception(
                    "can only output into current directory!");

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

            // CodeGenerator
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

        private void AddZeroToCells()
        {
            il.Emit(OpCodes.Ldloc, cells);
            il.Emit(OpCodes.Ldc_I4_0);

            il.Emit(OpCodes.Call,
                typeof(List<byte>).GetMethod("Add", new System.Type[] { typeof(byte) })
                );
        }

        private void DeclareVariables()
        {
            p = il.DeclareLocal(typeof(int));


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
                            il.Emit(OpCodes.Stloc,temp);
                            il.Emit(OpCodes.Ldloc,temp);

                            il.Emit(OpCodes.Brfalse_S,incrementPointerLabel);

                            // increase the size by one.
                            AddZeroToCells();

                            il.MarkLabel(incrementPointerLabel);
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

                    il.Emit(OpCodes.Ldloc,cells);
                    il.Emit(OpCodes.Ldloc,p);

                                il.Emit(OpCodes.Call,
                typeof(List<byte>).GetMethod("get_Item", new System.Type[] { typeof(int) })
                );
  //IL_001b:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<uint8>::get_Item(int32)

                    il.Emit(OpCodes.Ldc_I4_0);
  //IL_0020:  ldc.i4.0
                    il.Emit(OpCodes.Ceq);
 // IL_0021:  ceq
                    il.Emit(OpCodes.Ldc_I4_0);
  //IL_0023:  ldc.i4.0
  //IL_0024:  ceq
      il.Emit(OpCodes.Ceq);
                    LocalBuilder temp = il.DeclareLocal(typeof(int));
                    il.Emit(OpCodes.Stloc,temp);
                    il.Emit(OpCodes.Ldloc,temp);
  /*IL_0026:  stloc.2
  IL_0027:  ldloc.2*/
                    il.Emit(OpCodes.Brtrue,bodyLabel);
  //IL_0028:  brtrue.s   IL_0013



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
