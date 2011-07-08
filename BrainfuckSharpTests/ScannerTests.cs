﻿using System.Collections.Generic;
using BrainfuckSharp;
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
@"<>+-.,";

            StringReader input = new StringReader(test);

            Scanner scanner = new Scanner(input);
            Parser parser = new Parser(scanner.Tokens);
        }
    }
}
