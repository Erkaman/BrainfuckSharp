﻿using System.Collections.Generic;
using BrainfuckSharpLibrary;
using NUnit.Framework;
using System.IO;
using System;
using BrainfuckSharpLibrary.AbstractSyntaxTree;

namespace BrainfuckSharpTests
{
    [TestFixture]
    class ParserTests
    {
        [Test]
        public void ParseTokensTest()
        {
            string test = @"<>+-.[,+ [++] - ]";

            StringReader input = new StringReader(test);

            Block result = new Block();
            result.Statements = new List<Statement>();
            Action<CommandType> AddCommand = type =>
                result.Statements.Add(new Command(type));

            // add the first block.
            AddCommand(CommandType.DecrementPointer);
            AddCommand(CommandType.IncrementPointer);
            AddCommand(CommandType.IncrementAtPointer);
            AddCommand(CommandType.DecrementAtPointer);
            AddCommand(CommandType.OutputCharacter);

            Block loopBody = new Block();
                      
            AddCommand = type =>
                loopBody.Statements.Add(new Command(type));

            // the first two characters in the loop.
            AddCommand(CommandType.InputCharacter);
            AddCommand(CommandType.IncrementAtPointer);

            // the nested loop.
            Block nestedLoopBody = new Block();

            // the two statements in the nested loop.
            nestedLoopBody.Statements.Add(new Command(CommandType.IncrementAtPointer));
            nestedLoopBody.Statements.Add(new Command(CommandType.IncrementAtPointer));

            Loop nestedLoop = new Loop(nestedLoopBody);

            // add the nested loop.
            loopBody.Statements.Add(nestedLoopBody);

            // add the last command after the nested loop.
            AddCommand(CommandType.DecrementAtPointer);

            Loop loop = new Loop(loopBody);

            result.Statements.Add(loop);

            Assert.AreEqual(result,Parser.ParseTokens(input));
        }

        [Test]
        public void ParseTokensExceptionTest()
        {
            const string errorMessage = "Unbalanced brackets.";

            Action<string> AssertThrowsException = (input) =>
            {
                Assert.That(() => Parser.ParseTokens(new StringReader(input)),
                Throws.Exception.TypeOf<SyntaxErrorException>()
                .With.Message.Contains(errorMessage));
            };

            AssertThrowsException("[");
            AssertThrowsException("[]]");
            AssertThrowsException("[[]][][]]");
            AssertThrowsException("[[]");
        }
    }
}
