using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Porous
{
    /// <summary>
    /// Represents an interpreter that executes instructions in real time.
    /// </summary>
    public class Interpreter
    {
        public Stack<object> dataStack;
        public Stack<PType> typeStack;

        public Interpreter()
        {
            dataStack = new Stack<object>();
            typeStack = new Stack<PType>();
        }

        /// <summary>
        /// Interprets a direction automatically without having to execute an entire function or 
        /// resolve it beforehand.
        /// </summary>
        /// <param name="direction">The direction to execute.</param>
        public void DebugInterpret(Direction direction)
        {
            if (Program.verbose)
                Console.WriteLine("Doing " + direction);
            Instruction instruction = direction.Resolve(typeStack);
            instruction.signature.Execute(ref typeStack);
            instruction.Execute(ref dataStack);
        }

        /// <summary>
        /// If the context of the interpreter includes a dynamic type stack, this function
        /// prints out the data stack along with the type of each element.
        /// </summary>
        /// <returns>A string representing the currect data and type stacks.</returns>
        public string DebugToString()
        {
            string toRet = "";

            int count = dataStack.Count;

            for (int i = 0; i < count; i++)
                toRet += dataStack.Pop() + "\t- " + typeStack.Pop() + "\n";

            return toRet;
        }
        
        /// <summary>
        /// Prints the current data stack.
        /// </summary>
        /// <returns>The data stack in a human-readable format.</returns>
        public override string ToString()
        {
            string toRet = "";

            int count = dataStack.Count;

            for (int i = 0; i < count; i++)
                toRet += dataStack.Pop() + "\t- " + i + "\n";

            return toRet;
        }
    }
}
