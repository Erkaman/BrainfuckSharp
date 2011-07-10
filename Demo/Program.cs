using BrainfuckSharp;
using System.IO;
using System.Collections.Generic;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            string code = @"
++++++++++[>+++++++>++++++++++>+++>+<<<<-]>++.>+.+++++++..+
++.>++.<<+++++++++++++++.>.+++.------.--------.>+.>.";
            CodeGenerator.CompileTextReader(new StringReader(code), "HelloWorld.exe");
        }
    }
}
