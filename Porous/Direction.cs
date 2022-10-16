using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UtahBase.Testing;

namespace Porous
{
    /// <summary>
    /// A generic instruction specified by the user to be executed in a Porous program. 
    /// Directions do not have a given type signature, so they can be things like 
    /// overloaded function calls, stack manipulation operations, and so on.
    /// </summary>
    public abstract class Direction
    {
        public ParseToken token;

        protected Direction(ParseToken token)
        {
            this.token = token;
        }

        /// <summary>
        /// Resolves this direction, given the current type stack, to an appropriate instruction
        /// or throws an error if none is found.
        /// </summary>
        /// <returns>An instruction that fully describes the specifics and implementation of 
        /// this direction.</returns>
        public abstract Instruction Resolve(Stack<PType> typeStack);

        public override string ToString()
        {
            return token.toSimpleString;
        }
    }

    /// <summary>
    /// A direction to push a given integer onto the stack.
    /// </summary>
    public class PushIntDirection : Direction
    {
        int toPush;

        public PushIntDirection(int toPush, ParseToken token) : base(token)
        {
            this.toPush = toPush;
        }

        public override PushIntInstruction Resolve(Stack<PType> typeStack)
        {
            return new PushIntInstruction(toPush, token);
        }
    }

    /// <summary>
    /// A direction to push a given character onto the stack.
    /// </summary>
    public class PushCharDirection : Direction
    {
        char toPush;

        public PushCharDirection(char toPush, ParseToken token) : base(token)
        {
            this.toPush = toPush;
        }

        public override PushCharInstruction Resolve(Stack<PType> typeStack)
        {
            return new PushCharInstruction(toPush, token);
        }
    }

    /// <summary>
    /// A direction to push a given boolean value onto the stack.
    /// </summary>
    public class PushBoolDirection : Direction
    {
        bool toPush;

        public PushBoolDirection(bool toPush, ParseToken token) : base(token)
        {
            this.toPush = toPush;
        }

        public override PushBoolInstruction Resolve(Stack<PType> typeStack)
        {
            return new PushBoolInstruction(toPush, token);
        }
    }

    public class PushFunctionDirection : Direction
    {
        GenericFunction toPush;

        public PushFunctionDirection(GenericFunction toPush, ParseToken token) : base(token)
        {
            this.toPush = toPush;
        }

        public override PushFunctionInstruction Resolve(Stack<PType> typeStack)
        {
            return new PushFunctionInstruction(toPush, token);
        }
    }

    public class IntArithmeticDirection : Direction
    {
        Func<int, int, int> operation;

        public IntArithmeticDirection(Func<int, int, int> operation, ParseToken token) : base(token)
        {
            this.operation = operation;
        }

        public override IntArithmeticInstruction Resolve(Stack<PType> typeStack)
        {
            List<PType> pList = typeStack.ToList();
            if (pList[0] != PType.intType || pList[1] != PType.intType)
                throw new PorousException("Arithmetic operations must be called on two integers.");

            return new IntArithmeticInstruction(operation, token);
        }
    }

    public class DoDirection : Direction
    {
        public DoDirection(ParseToken token) : base(token) { }

        public override DoInstruction Resolve(Stack<PType> typeStack)
        {
            if (typeStack.Peek() is PSignatureType s)
                return new DoInstruction(s, typeStack, token);
            throw new PorousException(token, "Tried to call something that was not a function.");
        }
    }

    public class CallDirection : Direction
    {
        string toCall;

        public CallDirection(string toCall, ParseToken token) : base(token)
        {
            this.toCall = toCall;
        }

        public override Instruction Resolve(Stack<PType> typeStack)
        {
            return DirectionProcessor.workingDict[toCall].Resolve(typeStack);
        }
    }

    public class DupDirection : Direction
    {
        public DupDirection(ParseToken token) : base(token) { }

        public override DupInstruction Resolve(Stack<PType> typeStack)
        {
            return new DupInstruction(typeStack.Peek(), token);
        }
    }

    public class DropDirection : Direction
    {
        public DropDirection(ParseToken token) : base(token) { }

        public override DropInstruction Resolve(Stack<PType> typeStack)
        {
            return new DropInstruction(typeStack.Peek(), token);
        }
    }

    public class SwapDirection : Direction
    {
        public SwapDirection(ParseToken token) : base(token) { }

        public override SwapInstruction Resolve(Stack<PType> typeStack)
        {
            List<PType> pList = typeStack.ToList();
            return new SwapInstruction(pList[1], pList[0], token);
        }
    }

    public class OverDirection : Direction
    {
        public OverDirection(ParseToken token) : base(token) { }

        public override OverInstruction Resolve(Stack<PType> typeStack)
        {
            List<PType> pList = typeStack.ToList();
            return new OverInstruction(pList[1], pList[0], token);
        }
    }

    public class ChooseDirection : Direction
    {
        public ChooseDirection(ParseToken token) : base(token) { }

        public override ChooseInstruction Resolve(Stack<PType> typeStack)
        {
            List<PType> pList = typeStack.ToList();
            if (pList[0] != PType.boolType)
                throw new PorousException(token, "Tried to call ? without a boolean value on top of the stack.");
            if (!pList[1].Equals(pList[2]))
                throw new PorousException(token, "Tried to call ? with two values of different types: " 
                    + pList[1] + " / " + pList[2]);
            return new ChooseInstruction(pList[1], token);
        }
    }

    public class EqualsDirection : Direction
    {
        public EqualsDirection(ParseToken token) : base(token) { }

        public override EqualsInstruction Resolve(Stack<PType> typeStack)
        {
            List<PType> pList = typeStack.ToList();
            if (!pList[0].Equals(pList[1]))
                throw new PorousException(token, "Tried to compare two values that are not of the same type: " 
                    + pList[0] + " / " + pList[1]);
            return new EqualsInstruction(pList[1], token);
        }
    }

    public class PutDirection : Direction
    {
        public PutDirection(ParseToken token) : base(token) { }

        public override Instruction Resolve(Stack<PType> typeStack)
        {
            return new PutInstruction(token);
        }
    }

    public class ComparisonDirection : Direction
    {
        Func<int, int, bool> operation;

        public ComparisonDirection(Func<int, int, bool> operation, ParseToken token) : base(token)
        {
            this.operation = operation;
        }

        public override ComparisonInstruction Resolve(Stack<PType> typeStack)
        {
            List<PType> pList = typeStack.ToList();
            if (!pList[0].Equals(PType.intType) || !pList[1].Equals(PType.intType))
                throw new PorousException("Comparison operations must be called on two integers.");

            return new ComparisonInstruction(operation, token);
        }
    }

    public class NotDirection : Direction
    {
        public NotDirection(ParseToken token) : base(token) { }

        public override NotInstruction Resolve(Stack<PType> typeStack)
        {
            return new NotInstruction(token);
        }
    }

    public class ExternDirection : Direction
    {
        string call;

        public ExternDirection(string call, ParseToken token) : base(token)
        {
            this.call = call;
        }

        public override ExternInstruction Resolve(Stack<PType> typeStack)
        {
            return new ExternInstruction(ExternalCall.calls[call], token);
        }
    }
}
