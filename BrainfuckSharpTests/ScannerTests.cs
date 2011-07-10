using System.Collections.Generic;
using BrainfuckSharpLibrary;
using NUnit.Framework;
using System.Collections;
using System.IO;

namespace BrainfuckSharpTests
{
    [TestFixture]
    class ScannerTests
    {
        [Test]
        public void ConstructorTest()
        {
            string test =
@"++bla bla fff - , [ ] ] [ ,. < >> < --

k ,
";
            IList<char> expected = new List<char>()
            {
                '+','+','-',',','[',']',']','[',',','.','<','>','>','<','-','-',
                ','
            };

            StringReader input = new StringReader(test);
            Assert.AreEqual(expected, Scanner.Scan(input));
        }
    }
}

/*
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
*/