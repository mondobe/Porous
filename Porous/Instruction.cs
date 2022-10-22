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

    /// <summary>
    /// An instruction to push a callable function onto the stack. 
    /// Generics are automatically resolved when a function is pushed.
    /// </summary>
    public class PushFunctionInstruction : Instruction
    {
        Function toPush;

        public override PSignatureType signature => new(new List<PType>(), new List<PType> { toPush.signature });

        public PushFunctionInstruction(GenericFunction toPush, Stack<PType> typeStack, ParseToken token) : base(token)
        {
            this.toPush = toPush.TypeCheck(typeStack);
        }

        public override void Execute(ref Stack<object> stack)
        {
            stack.Push(toPush);
        }
    }

    /// <summary>
    /// An instruction designed to represent multiple binary arithmetic operators without extra boilerplate.
    /// </summary>
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

    /// <summary>
    /// An instruction that executes whatever function is on the top of the stack.
    /// </summary>
    public class DoInstruction : Instruction
    {
        PSignatureType topSig;

        public override PSignatureType signature
        {
            get
            {
                PSignatureType toRet = new(new List<PType>(), new List<PType>());
                toRet.ins.AddRange(topSig.ins);
                toRet.outs.AddRange(topSig.outs);
                toRet.ins.Add(topSig);
                toRet.outs.Add(topSig);
                return toRet;
            }
        }

        public DoInstruction(Stack<PType> typeStack, ParseToken token) : base(token) 
        {
            topSig = (PSignatureType)typeStack.Peek();
        }

        public override void Execute(ref Stack<object> stack)
        {
            Function func = (Function)stack.Pop();
            func.Execute(ref stack);
            stack.Push(func);
        }
    }

    public class DupInstruction : Instruction
    {
        public PType type;

        public override PSignatureType signature => new(new List<PType> { type }, new List<PType> { type, type });

        public DupInstruction(PType type, ParseToken token) : base(token)
        {
            this.type = type;
        }

        public override void Execute(ref Stack<object> stack)
        {
            object obj = stack.Pop();
            stack.Push(obj);
            stack.Push(obj);
        }
    }

    public class DropInstruction : Instruction
    {
        public PType type;

        public override PSignatureType signature => new(new List<PType> { type }, new List<PType> { });

        public DropInstruction(PType type, ParseToken token) : base(token)
        {
            this.type = type;
        }

        public override void Execute(ref Stack<object> stack)
        {
            stack.Pop();
        }
    }

    public class SwapInstruction : Instruction
    {
        public PType topType, bottomType;

        public override PSignatureType signature => new(new List<PType> { bottomType, topType }, new List<PType> { topType, bottomType });

        public SwapInstruction(PType bottomType, PType topType, ParseToken token) : base(token)
        {
            this.bottomType = bottomType;
            this.topType = topType;
        }

        public override void Execute(ref Stack<object> stack)
        {
            object top = stack.Pop();
            object bottom = stack.Pop();
            stack.Push(top);
            stack.Push(bottom);
        }
    }

    public class OverInstruction : Instruction
    {
        public PType topType, bottomType;

        public override PSignatureType signature => new(new List<PType> { bottomType, topType }, new List<PType> { bottomType, topType, bottomType });

        public OverInstruction(PType bottomType, PType topType, ParseToken token) : base(token)
        {
            this.bottomType = bottomType;
            this.topType = topType;
        }

        public override void Execute(ref Stack<object> stack)
        {
            object top = stack.Pop();
            object bottom = stack.Peek();
            stack.Push(top);
            stack.Push(bottom);
        }
    }

    public class ChooseInstruction : Instruction
    {
        public PType type;

        public override PSignatureType signature => new(new List<PType> { type, type, PType.boolType }, new List<PType> { type });

        public ChooseInstruction(PType type, ParseToken token) : base(token)
        {
            this.type = type;
        }

        public override void Execute(ref Stack<object> stack)
        {
            bool b = (bool)stack.Pop();
            object top = stack.Pop();
            object bottom = stack.Pop();
            stack.Push(b ? top : bottom);
        }
    }

    public class EqualsInstruction : Instruction
    {
        public PType type;

        public override PSignatureType signature => new(new List<PType> { type, type }, new List<PType> { PType.boolType });
        
        public EqualsInstruction(PType type, ParseToken token) : base(token)
        {
            this.type = type;
        }

        public override void Execute(ref Stack<object> stack)
        {
            object top = stack.Pop();
            object bottom = stack.Pop();
            stack.Push(bottom == top);
        }
    }

    public class PutInstruction : Instruction
    {
        public override PSignatureType signature => new(new List<PType> { PType.charType }, new List<PType> { });

        public PutInstruction(ParseToken token) : base(token) { }

        public override void Execute(ref Stack<object> stack)
        {
            Console.Write((char)stack.Pop());
        }
    }

    public class ComparisonInstruction : Instruction
    {
        Func<int, int, bool> operation;

        public override PSignatureType signature => new(new List<PType> { PType.intType, PType.intType }, new List<PType> { PType.boolType });

        public ComparisonInstruction(Func<int, int, bool> operation, ParseToken token) : base(token) 
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

    public class NotInstruction : Instruction
    {
        public override PSignatureType signature => new(new List<PType> { PType.boolType }, new List<PType> { PType.boolType });

        public NotInstruction(ParseToken token) : base(token) { }

        public override void Execute(ref Stack<object> stack)
        {
            bool top = (bool)stack.Pop();
            stack.Push(!top);
        }
    }

    public class ExternInstruction : Instruction
    {
        ExternalCall call;

        public override PSignatureType signature => call.signature;

        public ExternInstruction(ExternalCall call, ParseToken token) : base(token)
        {
            this.call = call;
        }

        public override void Execute(ref Stack<object> stack)
        {
            call.action(ref stack);
        }
    }

    public class WhileInstruction : Instruction
    {
        readonly Stack<PType> typeStack;

        public override PSignatureType signature => new(new List<PType> {
            new PSignatureType(new List<PType>(), new List<PType>{ PType.boolType }), PType.boolType
        }, new List<PType>
        {
            new PSignatureType(new List<PType>(), new List<PType>{ PType.boolType })
        });
        
        public WhileInstruction(ParseToken token) : base(token)
        {
            typeStack = new Stack<PType>();
            typeStack.Push(PType.boolType);
        }

        public override void Execute(ref Stack<object> stack)
        {
            bool keepGoing = (bool)stack.Pop();
            Function fn = (Function)stack.Pop();
            while(keepGoing)
            {
                fn.Execute(ref stack);
                keepGoing = (bool)stack.Pop();
            }
            stack.Push(fn);
        }
    }

    public class CurryInstruction : Instruction
    {
        List<PType> types;
        PSignatureType initial, final;

        public override PSignatureType signature => new(types.Concat(new List<PType> { initial }).ToList(), new List<PType> { final });

        public CurryInstruction(List<PType> types, PSignatureType initial, ParseToken token) : base(token)
        {
            this.types = types;
            this.initial = initial;
            final = initial.Duplicate();
            final.Curry(types);
        }

        public override void Execute(ref Stack<object> stack)
        {
            Function fn = (Function)stack.Pop();
            fn.Curry(types, ref stack);
            stack.Push(fn);
        }
    }
}
