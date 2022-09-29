using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Porous
{
    /// <summary>
    /// A generic instruction specified by the user to be executed in a Porous program. 
    /// Directions do not have a given type signature, so they can be things like 
    /// overloaded function calls, stack manipulation operations, and so on.
    /// </summary>
    public abstract class Direction
    {
        /// <summary>
        /// Resolves this direction, given the current type stack, to an appropriate instruction
        /// or throws an error if none is found.
        /// </summary>
        /// <returns>An instruction that fully describes the specifics and implementation of 
        /// this direction.</returns>
        public abstract Instruction Resolve(Stack<PType> typeStack);
    }

    /// <summary>
    /// A direction to push a given integer onto the stack.
    /// </summary>
    public class PushIntDirection : Direction
    {
        int toPush;

        public PushIntDirection(int toPush)
        {
            this.toPush = toPush;
        }

        public override Instruction Resolve(Stack<PType> typeStack)
        {
            return new PushIntInstruction(toPush);
        }
    }

    /// <summary>
    /// A direction to push a given character onto the stack.
    /// </summary>
    public class PushCharDirection : Direction
    {
        char toPush;

        public PushCharDirection(char toPush)
        {
            this.toPush = toPush;
        }

        public override Instruction Resolve(Stack<PType> typeStack)
        {
            return new PushCharInstruction(toPush);
        }
    }

    /// <summary>
    /// A direction to push a given boolean value onto the stack.
    /// </summary>
    public class PushBoolDirection : Direction
    {
        bool toPush;

        public PushBoolDirection(bool toPush)
        {
            this.toPush = toPush;
        }

        public override Instruction Resolve(Stack<PType> typeStack)
        {
            return new PushBoolInstruction(toPush);
        }
    }
}
