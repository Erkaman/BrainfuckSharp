using BrainfuckSharp;
using System.IO;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            string code = @"+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++.---.++.-------.>
            
+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++.<----.";
            Scanner scanner = new Scanner(new StringReader(code));
            Parser parser = new Parser(scanner.Tokens);
            CodeGen codeGen = new CodeGen(parser.Result, "HelloWorld.exe");
        }
    }
}
