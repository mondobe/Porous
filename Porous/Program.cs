using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public static bool verbose;

        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            InterpretDictionary(ProcessProgram(@"
main = ( : )
{	
	 1 1 fibo : drop
}

fibo = (int int : )
{
	over printInt : drop
	',' . ' ' .
	dup printInt : drop
	',' . ' ' .

	fiboInternal : drop
	drop drop
}

fiboInternal = (int int : int int)
{
    swap over +
    dup printInt : drop
	',' . ' ' .
    fiboInternal : drop
}

printInt = (int : )
{
	dup 0 <
	(int : int) { } swap
	(int : int) { -1 * '-' . } swap
	? : drop
	printPart : drop
}

printPart = (int : )
{
	dup
	(int : ) { digitToChar : drop . } swap
	(int : )
	{
		dup 10 / printPart : drop
		10 % digitToChar : drop .
	} swap
	9 > ? : drop
}

digitToChar = (int : char)
{
    %!dtc
}

", false));
            stopwatch.Stop();
            Console.WriteLine("\nDone in " + stopwatch.ElapsedMilliseconds + " ms");
        }

        public static void DigitToChar(ref Stack<object> stack)
        {
            int digit = (int)stack.Pop();
            stack.Push("0123456789"[digit]);
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
            List<Token> tokens = LexerParser.Lex(program);
            List<ParseToken> pts = LexerParser.Parse(tokens);

            Program.verbose = verbose;

            // Write the list of parse tokens (for debug purposes)
            if(verbose)
                Console.WriteLine(pts.ParseTokensToString());

            // Add all of the headers to a global list
            foreach (ParseToken global in pts)
                DirectionProcessor.headers.Add(global.children[0].content);

            // Using the headers as references, process each of the directions and add them to a dictionary
            foreach (ParseToken global in pts)
                DirectionProcessor.workingDict.Add(global.children[0].content,
                    DirectionProcessor.ProcessDirection(global.children[2]));

            return DirectionProcessor.workingDict;
        }

        /// <summary>
        /// Interprets a given dictionary, starting at the given main function and traversing other elements
        /// of the dictionary as needed.
        /// </summary>
        /// <param name="program">The dictionary of the program, provided by ProcessProgram.</param>
        /// <param name="main">The main function to use, defaulting to "main".</param>
        public static void InterpretDictionary(Dictionary<string, Direction> program, string main = "main")
        {
            // Initialize the interpreter
            Interpreter interpreter = new();

            // Resolve the main instruction (this also resolves and type-checks other functions; maybe separate this
            // into another method?
            Instruction mainInstr = program[main].Resolve(new Stack<PType>());

            // Execute the main instruction
            mainInstr.Execute(ref interpreter.dataStack);

            // Since the main instruction should be a function, also execute :
            new DoInstruction(mainInstr.signature, new Stack<PType>(new List<PType> { mainInstr.signature }), 
                new ParseToken(new List<ParseToken>(), new List<string>()))
                .Execute(ref interpreter.dataStack);

            // Drop the main function from the stack and print everything else
            interpreter.dataStack.Pop();
            if(verbose)
                Console.WriteLine(interpreter);
        }
    }
}

