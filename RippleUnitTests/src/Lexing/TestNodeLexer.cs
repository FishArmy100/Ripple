using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raucse;
using Raucse.Extensions;
using Raucse.Diagnostics;

namespace RippleUnitTests.Lexing
{
    static class TestNodeLexer
    {
        public const string TEST_SEPERATOR = "###";

        public static Result<List<TestNode>, List<NodeLexingError>> Lex(string source)
        {
            string[] tests = source.Split(TEST_SEPERATOR, StringSplitOptions.RemoveEmptyEntries);
            List<TestNode> nodes = new List<TestNode>();
            List<NodeLexingError> errors = new List<NodeLexingError>();
            
            foreach(string test in tests)
            {
                int index = test.IndexOf('\n');
                if (index == -1)
                {
                    errors.Add(new NodeLexingError("Malformed test"));
                    continue;
                }

                var (settings, code) = SplitAt(test, index);
                code = code.Trim();
                if(code.IsNullOrEmpty())
                {
                    errors.Add(new NodeLexingError("No code given for unit test"));
                    continue;
                }

                var lexedResult = LexArguments(settings);
                lexedResult.Match(
                    ok => nodes.Add(new TestNode(code, ok)), 
                    fail => errors.AddRange(fail));
            }

            if (errors.Any())
                return errors;

            return nodes;
        }

        private static Result<List<ArgumentToken>, List<NodeLexingError>> LexArguments(string arguments)
        {
            List<ArgumentToken> tokens = new List<ArgumentToken>();
            List<NodeLexingError> errors = new List<NodeLexingError>();
            for(int i = 0; i < arguments.Length; i++)
            {
                char current = arguments[i];
                if (arguments[i] == '-')
                {
                    LexCommand(arguments, ref i).Match(
                        ok => tokens.Add(new ArgumentToken(ok, null)),
                        error => errors.Add(error));
                }
                else if (arguments[i] == '"')
                {
                    LexString(arguments, ref i).Match(
                        ok => tokens.Add(ok),
                        error => errors.Add(error));
                }
                else if(arguments[i].IsWhiteSpace())
                {
                    continue;
                }
                else
                {
                    errors.Add(new NodeLexingError($"unknown token '{arguments[i]}'")); 
                }
            }

            if (errors.Any())
                return errors;

            return tokens;
        }

        private static Result<ArgumentTokenType, NodeLexingError> LexCommand(string text, ref int index)
        {
            Debug.Assert(text[index] == '-', "Argument must start with a -");
            int length;
            for (length = 0; length + index < text.Length && !text[index + length].IsWhiteSpace(); length++)
                continue;

            if (length <= 1)
                return new NodeLexingError("Command must have a name");

            string sub = text.Substring(index, length);
            index += length;
            if (sub == ArgumentCommands.DASH_NAME)
            {
                return ArgumentTokenType.NameArg;
            }
            else if (sub == ArgumentCommands.DASH_INSERT_MAIN)
            {
                return ArgumentTokenType.InsertMainArg;
            }
            else if (sub == ArgumentCommands.DASH_NO_COMPILE)
            {
                return ArgumentTokenType.ShouldNotCompileArg;
            }

            return new NodeLexingError($"Unknown command: {sub}");
        }

        private static Result<ArgumentToken, NodeLexingError> LexString(string text, ref int index)
        {
            Debug.Assert(text[index] == '"', "String must start with a \"");
            if (text.Length <= index + 1)
                return new NodeLexingError("Unterminated string");

            int length;
            for (length = 1; length + index < text.Length && text[index + length] != '"'; length++)
                continue;

            if (length + index >= text.Length)
                return new NodeLexingError("Unterminated string");
            length += 1;
            string sub = text.Substring(index, length);
            index += length;

            return new ArgumentToken(ArgumentTokenType.String, sub);
        }

        private static Pair<string, string> SplitAt(string src, int index)
        {
            string first = src.Substring(0, index);
            string second = src.Substring(index, src.Length - index);
            return new Pair<string, string>(first, second);
        }
    }
}
