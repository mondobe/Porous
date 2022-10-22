using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Porous
{
    /// <summary>
    /// Represents a generic function: a function that contains types in the signature that
    /// are not known compile-time types, but rather resolved at runtime. This works because
    /// there are instructions like : or the stack manipulation instructions that are
    /// themselves generic.
    /// </summary>
    public class GenericFunction : IEnumerable<Direction>
    {
        public PSignatureType signature;
        public List<Direction> body;
        public Dictionary<List<PType>, Function> resolutions;

        public GenericFunction(PSignatureType signature, List<Direction> body, List<string> generics)
        {
            this.signature = signature;
            this.body = body;
            resolutions = new Dictionary<List<PType>, Function>();
        }

        /// <summary>
        /// Transforms this GenericFunction into a specific function, resolving its types automatically
        /// based on the given typeStack.
        /// </summary>
        /// <param name="typeStack">The typeStack from which to resolve the function's types. If the 
        /// input types match the typeStack (except for the determining generics), the instructions use
        /// the resolved types.</param>
        /// <returns>A specific function with the types known ahead of time.</returns>
        /// <exception cref="PorousException">Thrown if the ending signature does not match what is given.</exception>
        public Function TypeCheck(Stack<PType> typeStack)
        {
            List<PType> typeList = typeStack.ToList();

            foreach(KeyValuePair<List<PType>, Function> resolution in resolutions)
            {
                bool works = true;
                if (resolution.Key.Count != typeList.Count)
                    continue;
                for (int i = 0; i < typeList.Count; i++)
                    if (!typeList[i].Equals(resolution.Key[i]))
                    {
                        works = false;
                        break;
                    }
                if (works)
                    return resolution.Value;
            }

            if (Program.verbose)
                Console.WriteLine("Type checking " + signature + "(" + GetHashCode() + " / " + typeStack.GetHashCode() + ")");

            // Match the input signature with the current type stack, throwing an error if anything
            // doesn't match.
            Dictionary<string, PType> replacements = new();

            List<PType> specifics = new(typeStack);
            for (int i = 0; i < signature.ins.Count; i++)
                signature.ins[i].MatchGenerics(ref replacements, specifics[signature.ins.Count - i - 1]);

            // Resolve all of the directions given the input signature with the generics replaced.
            Function nFunc = new(signature.ReplaceGenerics(replacements), new List<Instruction>());
            Stack<PType> checkStack = new(signature.ReplaceGenerics(replacements).ins);

            foreach(Direction d in this)
            {
                Instruction resolved = d.Resolve(checkStack);
                resolved.signature.Execute(ref checkStack);
                nFunc.body.Add(resolved);
            }

            // Make sure the ending signatures match with the generics replaced
            for(int i = signature.outs.Count - 1; i >= 0; i--)
            {
                if(Program.verbose)
                    Console.WriteLine("Out: " + signature.outs[i].ReplaceGenerics(replacements) + "\tGot: " + checkStack.Peek());
                if (!checkStack.Pop().Equals(signature.outs[i].ReplaceGenerics(replacements)))
                    throw new PorousException(body[0].token, "Ending signature of function does not match result of type checking!");
            }

            if (checkStack.Count != 0)
                throw new PorousException(body[0].token, "Too many remaining values in function! Did you forget to drop a value?");

            resolutions.Add(typeList, nFunc);
            return nFunc;
        }

        public IEnumerator<Direction> GetEnumerator() => body.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>
    /// Represents a specific function with concrete types, as determined by the TypeCheck method 
    /// in the GenericFunction class. <see cref="GenericFunction.TypeCheck(Stack{PType})"/>
    /// </summary>
    public class Function : IEnumerable<Instruction>
    {
        public PSignatureType signature;
        public List<Instruction> body;
        public Stack<object> curryStack = new();

        public Function(PSignatureType signature, List<Instruction> body)
        {
            this.signature = signature;
            this.body = body;
        }

        public IEnumerator<Instruction> GetEnumerator() => body.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString() => signature.ToString();

        public void Execute(ref Stack<object> stack)
        {
            Stack<object> tempStack = new();

            // Transfer the inputs of the function out temporarily
            for (int t = 0; t < signature.ins.Count; t++)
                tempStack.Push(stack.Pop());

            // Add the curried inputs
            int curryCount = curryStack.Count;
            for (int i = 0; i < curryCount; i++)
                stack.Push(curryStack.Pop());

            // Transfer the regular inputs back
            for (int t = 0; t < signature.ins.Count; t++)
                stack.Push(tempStack.Pop());

            // Execute the function
            foreach (Instruction i in body)
                i.Execute(ref stack);

            // Transfer the outputs of the function out temporarily
            for (int t = 0; t < signature.outs.Count; t++)
                tempStack.Push(stack.Pop());

            // Re-curry the outputs
            for (int i = 0; i < curryCount; i++)
                curryStack.Push(stack.Pop());

            // Transfer the regular outputs back
            for (int t = 0; t < signature.outs.Count; t++)
                stack.Push(tempStack.Pop());
        }

        public void Curry(List<PType> types, ref Stack<object> stack)
        {
            if (Program.verbose)
                Console.WriteLine("Currying " + signature);

            signature.Curry(types);
            for(int i = 0; i < types.Count; i++)
            {
                curryStack.Push(stack.Pop());
                if(Program.verbose)
                    Console.WriteLine("\tCurrying with " + types[i] + "(" + curryStack.Peek() + ")");
            }

            if (Program.verbose)
                Console.WriteLine("Curried " + signature);
        }
    }
}
