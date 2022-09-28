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
            ProcessProgram(@"
");
        }

        /// <summary>
        /// Converts a given program string into a fully workable (although not necessarily immediately runnable) Porous dictionary.
        /// After lexing and parsing the program, it converts each function into a list of generic instructions, then type-checks those
        /// and converts them into specific instructions.
        /// </summary>
        /// <param name="program">The given program. Includes all necessary functions and declarations. Newlines can be included.</param>
        /// <returns>A dictionary of Porous functions and declarations that may or may not contain a main function.</returns>
        /// <exception cref="NotImplementedException">Not implemented yet.</exception>
        public static Dictionary<string, object> ProcessProgram(string program)
        {
            // Lex and parse the program given
            List<Token> tokens = LexerParser.Lex(program, false);
            List<ParseToken> pts = LexerParser.Parse(tokens, false);

            // Write the list of parse tokens (for debug purposes)
            Console.WriteLine(pts.ParseTokensToString());

            foreach(ParseToken token in pts)
                Console.WriteLine(PType.ParseType(token));

            throw new NotImplementedException();
        }
    }
}