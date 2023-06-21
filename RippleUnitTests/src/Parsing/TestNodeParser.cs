using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raucse;
using Raucse.Extensions;
using Raucse.Diagnostics;

namespace RippleUnitTests.Parsing
{
    static class TestNodeParser
    {
        public const string TEST_SEPERATOR = "###";

        public static Result<List<TestNode>, List<NodeParsingError>> Lex(string source)
        {
            string[] tests = GetTests(source);
            var result = ParseTests(tests);
            return result;
        }

        private static string[] GetTests(string source)
        {
            return source.Split(TEST_SEPERATOR, StringSplitOptions.RemoveEmptyEntries);
        }

        private static Result<List<TestNode>, List<NodeParsingError>> ParseTests(string[] tests)
        {
            return tests.Select(test =>
            {
                int index = test.IndexOf('\n');
                if (index == -1)
                {
                    return GenError("Malformed test");
                }

                var (settings, code) = SplitAt(test, index);
                code = code.Trim();
                if (code.IsNullOrEmpty())
                {
                    return GenError("No code given for unit test");
                }

                return GetNodeFromArgs(settings, code);
            }).AggregateResults();
        }

        private static Result<TestNode, List<NodeParsingError>> GenError(string message)
        {
            return new Result<TestNode, List<NodeParsingError>>(new List<NodeParsingError> { new NodeParsingError(message) });
        }

        private static Result<TestNode, List<NodeParsingError>> GetNodeFromArgs(string arguments, string code)
        {
            Option<string> name = new Option<string>();
            bool shouldCompile = true;
            bool insertMain = false;

            List<NodeParsingError> errors = new List<NodeParsingError>();
            for(int i = 0; i < arguments.Length; i++)
            {
                char current = arguments[i];
                if (arguments[i] == '-')
                {
                    LexCommand(arguments, ref i).Match(
                        ok =>
                        {
                            if(ok == TestArgumentType.Name)
                            {
                                while (i < arguments.Length && arguments[i].IsChar(' '))
                                    i++;

                                if(i >= arguments.Length)
                                {
                                    errors.Add(new NodeParsingError("Expected a name"));
                                    return;
                                }    

                                if(arguments[i] == '"')
                                {
                                    LexString(arguments, ref i).Match(
                                        ok => name = ok,
                                        error => errors.Add(error));
                                }
                                else
                                {
                                    errors.Add(new NodeParsingError("Expected a string"));
                                }
                            }
                            else
                            {
                                if (ok == TestArgumentType.InsertMain)
                                    insertMain = true;

                                if (ok == TestArgumentType.ShouldNotCompile)
                                    shouldCompile = false;
                            }
                        },
                        error => errors.Add(error));
                }
                else if(arguments[i].IsWhiteSpace())
                {
                    continue;
                }
                else
                {
                    errors.Add(new NodeParsingError($"unknown token '{arguments[i]}'"));
                    break;
                }
            }

            if (errors.Any())
                return errors;

            return new TestNode(code, name.Value, shouldCompile, insertMain);
        }

        private static Result<TestArgumentType, NodeParsingError> LexCommand(string text, ref int index)
        {
            Debug.Assert(text[index] == '-', "Argument must start with a -");
            int length;
            for (length = 0; length + index < text.Length && !text[index + length].IsWhiteSpace(); length++)
                continue;

            if (length <= 1)
                return new NodeParsingError("Command must have a name");

            string sub = text.Substring(index, length);
            index += length;
            if (sub == ArgumentCommands.DASH_NAME)
            {
                return TestArgumentType.Name;
            }
            else if (sub == ArgumentCommands.DASH_INSERT_MAIN)
            {
                return TestArgumentType.InsertMain;
            }
            else if (sub == ArgumentCommands.DASH_NO_COMPILE)
            {
                return TestArgumentType.ShouldNotCompile;
            }

            return new NodeParsingError($"Unknown command: {sub}");
        }

        private static Result<string, NodeParsingError> LexString(string text, ref int index)
        {
            Debug.Assert(text[index] == '"', "String must start with a \"");
            if (text.Length <= index + 1)
                return new NodeParsingError("Unterminated string");

            int length;
            for (length = 1; length + index < text.Length && text[index + length] != '"'; length++)
                continue;

            if (length + index >= text.Length)
                return new NodeParsingError("Unterminated string");
            length += 1;
            string sub = text.Substring(index, length);
            index += length;

            return sub[1..(sub.Length - 1)]; // should return the substring without the ""
        }

        private static Pair<string, string> SplitAt(string src, int index)
        {
            string first = src.Substring(0, index);
            string second = src.Substring(index, src.Length - index);
            return new Pair<string, string>(first, second);
        }
    }
}
