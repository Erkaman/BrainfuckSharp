using System.Collections.Generic;
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
