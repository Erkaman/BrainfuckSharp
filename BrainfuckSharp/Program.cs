using BrainfuckSharpLibrary;
using System;
using NDesk.Options;
using System.Collections.Generic;
using System.IO;

namespace BrainfuckSharp
{
    static class Program
    {
        static void ShowHelp(OptionSet options)
        {
            string helpMessage =
@"Usage: BrainfuckSharp [options] file [outputfile]
Allows compiling brainfuck source files to executables
Options:

";
            Console.WriteLine(helpMessage);
            options.WriteOptionDescriptions(Console.Out);
        }

        static void Main(string[] args)
        {
            bool showHelp = false;

            var options = new OptionSet()         
            {                                                             
            { 
                "h|help",  "show this message and exit.",              
                v => showHelp = v != null }  
            };

            List<string> extra;
            try
            {
                extra = options.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("BrainfuckSharp: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `BrainfuckSharp --help' for more information.");
                return;
            }

            if (showHelp)
                ShowHelp(options);
            else
            {
                try
                {
                    if (extra.Count > 0)
                    {
                        if (extra.Count < 2)
                            CodeGenerator.CompileFile(
                                args[0],                          
                                Path.GetFileNameWithoutExtension(args[0]) + ".exe");
                        else
                            CodeGenerator.CompileFile(args[0], args[1]);
                    }
                    else
                        Console.WriteLine("No files specified.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}