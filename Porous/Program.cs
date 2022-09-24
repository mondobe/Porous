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
printInt = (int : )
{
	dup 0 <
	(int : int) { 0 drop } swap
	(int : int) { -1 * '-' . } swap
	? do
	printPart
}

printPart = (int : )
{
	dup
	(int : ) { digitToChar . } swap
	(int : )
	{
		dup 10 / printPart
		10 % digitToChar .
	} swap
	9 > ? do
}

digitToChar = (int : char)
{
	dup
	'0' swap
	'1' swap 1 == ?
	over
	'2' swap 2 == ?
	over
	'3' swap 3 == ?
	over
	'4' swap 4 == ?
	over
	'5' swap 5 == ?
	over
	'6' swap 6 == ?
	over
	'7' swap 7 == ?
	over
	'8' swap 8 == ?
	over
	'9' swap 9 == ?
	swap drop
}
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
        public static Dictionary<string, TPValue> ProcessProgram(string program)
        {
            // Lex and parse the program given
            List<Token> tokens = LexerParser.Lex(program);
            List<ParseToken> pts = LexerParser.Parse(tokens, false);

            // Write the list of parse tokens (for debug purposes)
            Console.WriteLine(pts.ParseTokensToString());

            throw new NotImplementedException();
        }
    }
}