using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Porous
{
    /// <summary>
    /// Represents a Porous instruction: a specific directive that contains all the necessary information
    /// for the interpreter to execute some action. The type signature of any instruction should be known
    /// ahead-of-time.
    /// </summary>
    public abstract class Instruction 
    {
        public abstract PSignatureType signature { get; }

        /// <summary>
        /// Executes this function on a given stack. Usually called only during interpretation.
        /// </summary>
        /// <param name="stack">The data stack on which to execute this function. 
        /// The types of the values on top of the stack should match the inputs of the signature.</param>
        public abstract void Execute(ref Stack<object> stack);
    }

    /// <summary>
    /// An instruction to push an integer value onto the stack.
    /// </summary>
    public class PushIntInstruction : Instruction
    {
        int toPush;

        public override PSignatureType signature => new(new List<PType>(), new List<PType> { PType.intType });

        public PushIntInstruction(int toPush)
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

        public PushCharInstruction(char toPush)
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

        public PushBoolInstruction(bool toPush)
        {
            this.toPush = toPush;
        }

        public override void Execute(ref Stack<object> stack)
        {
            stack.Push(toPush);
        }
    }
}
