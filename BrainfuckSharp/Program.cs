using System;
using System.Collections.Generic;
using System.IO;

namespace BrainfuckSharp
{
    class Program
    {
        static List<byte> cells;
        static int p;

        public static void InterpretBrainfuck(
            string code,TextReader input,TextWriter output)
        {
            p = 0;

            cells = new List<byte>();
            cells.Add(0);

            InterpretBrainfuckHelper(code, input, output);
        }

        private static void InterpretBrainfuckHelper(
            string code,TextReader input,TextWriter output)
        {
            for (int i = 0; i < code.Length; ++i)
            {
                char ch = code[i];

                switch (ch)
                {
                    case '>':
                        if ((p + 1) == cells.Count)
                            cells.Add(0);
                        ++p;
                        break;
                    case '<':
                        --p;
                        break;
                    case '+':
                        ++cells[p];
                        break;
                    case '-':
                        --cells[p];
                        break;
                    case '.':
                        output.Write((char)cells[p]);
                        break;
                    case ',':
                        cells[p] = (byte) input.Read();
                        break;
                    case '[':
                        {
                            int unmatchedBrackets = 0;
                            for (int j = i; j < code.Length; ++j)
                            {
                                if (code[j] == '[')
                                    ++unmatchedBrackets;
                                else if(code[j] == ']')
                                    --unmatchedBrackets;

                                if (unmatchedBrackets == 0)
                                {

                                    while(cells[p] != 0)
                                        InterpretBrainfuckHelper(
                                            code.Substring(i+1, j-i-1),
                                            input,
                                            output);
                                    i = j;
                                    break;
                                }
                            }

                                break;
                        }
                    case ']':
                        throw new Exception("Unbalanced bracket");
                }
            }
        }

        /*static void Main(string[] args)
        {
            string test = @"++++++++++[>+++++++>++++++++++>+++>+<<<<-]>++.>+.+++++++..+++.>++.<<+++++++++++++++.>.+++.------.--------.>+.>.";

            InterpretBrainfuck(test,Console.In,Console.Out);

            Console.WriteLine("Done");
            Console.ReadLine();
        }*/
    }
}
