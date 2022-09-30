﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Porous
{
    public class Interpreter
    {
        public Stack<object> dataStack;
        public Stack<PType> typeStack;

        public Interpreter()
        {
            dataStack = new Stack<object>();
            typeStack = new Stack<PType>();
        }

        public void DebugInterpret(Direction direction, bool verbose = true)
        {
            if (verbose)
                Console.WriteLine("Doing " + direction);
            Instruction instruction = direction.Resolve(typeStack);
            instruction.signature.Execute(ref typeStack);
            instruction.Execute(ref dataStack);
        }

        public string DebugToString()
        {
            string toRet = "";

            int count = dataStack.Count;

            for (int i = 0; i < count; i++)
                toRet += dataStack.Pop() + "\t- " + typeStack.Pop() + "\n";

            return toRet;
        }
    }
}