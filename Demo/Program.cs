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
            IList<char> tokens = Scanner.Scan(new StringReader(code));
            Parser parser = new Parser(tokens);
            CodeGenerator.CompileBlock(parser.Result, "HelloWorld.exe");
        }
    }
}
