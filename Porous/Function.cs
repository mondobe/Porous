using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Porous
{
    public class GenericFunction : IEnumerable<Direction>
    {
        public PSignatureType signature;
        public List<string> generics;
        public List<Direction> body;

        public GenericFunction(PSignatureType signature, List<Direction> body, List<string> generics)
        {
            this.signature = signature;
            this.body = body;
            this.generics = generics;
        }

        public Function TypeCheck(List<PType> generics)
        {
            Dictionary<string, PType> replacements = new();

            if (this.generics.Count != generics.Count)
                throw new PorousException(body[0].token, "Number of generics does not match.");

            for (int i = 0; i < generics.Count; i++)
                replacements.Add(this.generics[i], generics[i]);

            Function nFunc = new(signature.ReplaceGenerics(replacements), new List<Instruction>());
            Stack<PType> checkStack = new(signature.ReplaceGenerics(replacements).ins);

            foreach(Direction d in this)
            {
                Instruction resolved = d.Resolve(checkStack);
                resolved.signature.Execute(ref checkStack);
                nFunc.body.Add(resolved);
            }

            for(int i = signature.outs.Count - 1; i >= 0; i--)
            {
                if (checkStack.Pop() != signature.outs[i])
                    throw new PorousException(body[0].token, "Ending signature of function does not match result of type checking!");
            }

            if (checkStack.Count != 0)
                throw new PorousException(body[0].token, "Too many remaining values in function! Did you forget to drop a value?");

            return nFunc;
        }

        public IEnumerator<Direction> GetEnumerator() => body.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class Function : IEnumerable<Instruction>
    {
        public PSignatureType signature;
        public List<Instruction> body;

        public Function(PSignatureType signature, List<Instruction> body)
        {
            this.signature = signature;
            this.body = body;
        }

        public IEnumerator<Instruction> GetEnumerator() => body.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString() => signature.ToString();
    }
}
