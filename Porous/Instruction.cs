using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Porous
{
    public abstract class Instruction 
    {
        public abstract PSignatureType signature { get; }

        public abstract void Execute(ref Stack<object> stack);
    }

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
