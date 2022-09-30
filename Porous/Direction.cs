﻿using System;
using System.Collections.Generic;
using System.Linq;
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

        public override Instruction Resolve(Stack<PType> typeStack)
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

        public override Instruction Resolve(Stack<PType> typeStack)
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

        public override Instruction Resolve(Stack<PType> typeStack)
        {
            return new PushBoolInstruction(toPush, token);
        }
    }

    public class IntArithmeticDirection : Direction
    {
        Func<int, int, int> operation;

        public IntArithmeticDirection(Func<int, int, int> operation, ParseToken token) : base(token)
        {
            this.operation = operation;
        }

        public override Instruction Resolve(Stack<PType> typeStack)
        {
            List<PType> pList = typeStack.ToList();
            if (pList[0] != PType.intType || pList[1] != PType.intType)
                throw new PorousException("Arithmetic operations must be called on two integers.");

            return new IntArithmeticInstruction(operation, token);
        }
    }
}
