using BrainfuckSharpLibrary;
using System;
using System.IO;

namespace BrainfuckSharp
{
    static class Program
    {
        static void ShowOption(string option,string description)
        {
            Console.WriteLine("   {0}\t\t{1}",option,description);
        }

        static void ShowHelp()
        {
            Console.WriteLine("Usage: [options] file [outputfile]");
            Console.WriteLine("Options:");
            ShowOption("--help,-h", "Show this");
            ShowOption("--inl,-i", "Compile an inline snippet to a file");
            //ShowOption("--debug,-d", "Show debug information");

        }

        static void Main(string[] args)
        {
            if (args.Length > 0)
                if (args[0] == "--help" || args[0] == "-h")
                    ShowHelp();
                else if (args[0] == "--inl" || args[0] == "i")
                {
                    bool passed = true;

                    if (args[1] == null)
                    {
                        Console.WriteLine("No inline expression specified");
                        passed = false;
                    }


                    if (args[2] == null)
                    { 
                        Console.WriteLine("No output file specified");
                        passed = false;
                    }

                    if (passed)
                    {
                        CodeGenerator.CompileTextReader(
                            new StringReader(args[1]),
                            args[2]);
                    }
                }
                else
                {
                    bool passed = true;
                    string output = args[1];
                    if (args[0] == null)
                    {
                        Console.WriteLine("No input file specified");
                        passed = false;
                    }

                    if (args[1] == null)
                    {
                        Console.WriteLine("No output file specified");
                        output = args[0] + ".exe";
                    }

                    if(passed)                  
                        CodeGenerator.CompileFile(args[0],output );
                }
            else
                Console.WriteLine("No files specified.");
        }
    }
}