using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Porous
{
    public abstract class Direction
    {
        public abstract Instruction Resolve();
    }

    public class PushIntDirection : Direction
    {
        int toPush;

        public PushIntDirection(int toPush)
        {
            this.toPush = toPush;
        }

        public override Instruction Resolve()
        {
            return new PushIntInstruction(toPush);
        }
    }

    public class PushCharDirection : Direction
    {
        char toPush;

        public PushCharDirection(char toPush)
        {
            this.toPush = toPush;
        }

        public override Instruction Resolve()
        {
            return new PushCharInstruction(toPush);
        }
    }

    public class PushBoolDirection : Direction
    {
        bool toPush;

        public PushBoolDirection(bool toPush)
        {
            this.toPush = toPush;
        }

        public override Instruction Resolve()
        {
            return new PushBoolInstruction(toPush);
        }
    }
}
