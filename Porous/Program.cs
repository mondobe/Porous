using System;
using System.Collections.Generic;
using TLexBase;
using TLexBase.Testing;
using TLexCLI;
using UtahBase;
using UtahBase.Testing;
using UtahCLI;

namespace Porous
{
    public class Program
    {
        static void Main(string[] args)
        {
            InterpretDictionary(ProcessProgram(@"
main = (: int)
{
    five
    two
    plus
}

five = 5
two = 2
plus = +
"), verbose: true);
        }

        /// <summary>
        /// Converts a given program string into a fully workable (although not necessarily immediately runnable) Porous dictionary.
        /// After lexing and parsing the program, it converts each function into a list of generic instructions, then type-checks those
        /// and converts them into specific instructions.
        /// </summary>
        /// <param name="program">The given program. Includes all necessary functions and declarations. Newlines can be included.</param>
        /// <param name="verbose">Whether or not to print debug information about the program.</param>
        /// <returns>A dictionary of Porous functions and declarations that may or may not contain a main function.</returns>
        /// <exception cref="NotImplementedException">Not implemented yet.</exception>
        public static Dictionary<string, Direction> ProcessProgram(string program, bool verbose = false)
        {
            // Lex and parse the program given
            List<Token> tokens = LexerParser.Lex(program, false);
            List<ParseToken> pts = LexerParser.Parse(tokens, false);

            // Write the list of parse tokens (for debug purposes)
            if(verbose)
                Console.WriteLine(pts.ParseTokensToString());

            foreach (ParseToken global in pts)
                DirectionProcessor.headers.Add(global.children[0].content);

            foreach (ParseToken global in pts)
                DirectionProcessor.workingDict.Add(global.children[0].content,
                    DirectionProcessor.ProcessDirection(global.children[2]));

            return DirectionProcessor.workingDict;
        }

        public static void InterpretDictionary(Dictionary<string, Direction> program, string main = "main", bool verbose = false)
        {
            Interpreter interpreter = new();

            Instruction mainInstr = program[main].Resolve(new Stack<PType>());
            mainInstr.Execute(ref interpreter.dataStack);
            new DoInstruction(mainInstr.signature, new ParseToken(new List<ParseToken>(), new List<string>()))
                .Execute(ref interpreter.dataStack);
            interpreter.dataStack.Pop();

            if(verbose)
                Console.WriteLine(interpreter);
        }
    }
}