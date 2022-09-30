using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtahBase.Testing;

namespace Porous
{
    /// <summary>
    /// Represents a Porous instruction: a specific directive that contains all the necessary information
    /// for the interpreter to execute some action. The type signature of any instruction should be known
    /// ahead-of-time.
    /// </summary>
    public abstract class Instruction 
    {
        public ParseToken token;

        protected Instruction(ParseToken token)
        {
            this.token = token;
        }

        public abstract PSignatureType signature { get; }

        /// <summary>
        /// Executes this function on a given stack. Usually called only during interpretation.
        /// </summary>
        /// <param name="stack">The data stack on which to execute this function. 
        /// The types of the values on top of the stack should match the inputs of the signature.</param>
        public abstract void Execute(ref Stack<object> stack);

        public override string ToString()
        {
            return token.toSimpleString;
        }
    }

    /// <summary>
    /// An instruction to push an integer value onto the stack.
    /// </summary>
    public class PushIntInstruction : Instruction
    {
        int toPush;

        public override PSignatureType signature => new(new List<PType>(), new List<PType> { PType.intType });

        public PushIntInstruction(int toPush, ParseToken token) : base(token)
        {
            this.toPush = toPush;
        }

        public override void Execute(ref Stack<object> stack)
        {
            stack.Push(toPush);
        }
    }

    /// <summary>
    /// An instruction to push a character value onto the stack.
    /// </summary>
    public class PushCharInstruction : Instruction
    {
        char toPush;

        public override PSignatureType signature => new(new List<PType>(), new List<PType> { PType.charType });

        public PushCharInstruction(char toPush, ParseToken token) : base(token)
        {
            this.toPush = toPush;
        }

        public override void Execute(ref Stack<object> stack)
        {
            stack.Push(toPush);
        }
    }

    /// <summary>
    /// An instruction to push a boolean value onto the stack.
    /// </summary>
    public class PushBoolInstruction : Instruction
    {
        bool toPush;

        public override PSignatureType signature => new(new List<PType>(), new List<PType> { PType.boolType });

        public PushBoolInstruction(bool toPush, ParseToken token) : base(token)
        {
            this.toPush = toPush;
        }

        public override void Execute(ref Stack<object> stack)
        {
            stack.Push(toPush);
        }
    }

    public class IntArithmeticInstruction : Instruction
    {
        Func<int, int, int> operation;

        public override PSignatureType signature => new(new List<PType> { PType.intType, PType.intType }, new List<PType> { PType.intType });

        public IntArithmeticInstruction(Func<int, int, int> operation, ParseToken token) : base(token) 
        {
            this.operation = operation;
        }

        public override void Execute(ref Stack<object> stack)
        {
            int rhs = (int)stack.Pop();
            int lhs = (int)stack.Pop();
            stack.Push(operation.Invoke(lhs, rhs));
        }
    }
}
