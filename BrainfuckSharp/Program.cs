using BrainfuckSharpLibrary;
using System;

namespace BrainfuckSharp
{
    static class Program
    {
        static void ShowHelp()
        {
            Console.WriteLine("Usage: [options] file [outputfile]");
            Console.WriteLine("Options:");
        }

        static void Main(string[] args)
        {
            if (args.Length > 0)
                if (args[0] == "--help" && args[0] == "-h")
                    ShowHelp();
                else
                {
                    BrainfuckSharpLibrary.CodeGenerator.CompileFile(args[0], args[1]);
                    // we'll ignore options and error handling for now...
                    //CodeGenerator.CompileFile(args[0], args[1]);
                }
            else
                Console.WriteLine("No files specified.");
        }
    }
}