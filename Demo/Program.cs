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
            //IList<char> tokens = Scanner.Scan(new StringReader(code));
            CodeGenerator.CompileBlock(Parser.ParseTokens(new StringReader(code)), "HelloWorld.exe");
        }
    }
}
