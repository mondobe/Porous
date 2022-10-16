using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Porous
{
    public class ExternalCall
    {
        public PSignatureType signature;
        public ExternalActionDelegate action;

        public static Dictionary<string, ExternalCall> calls = new()
        {
            { "printInt", new ExternalCall(new PSignatureType(new List<PType>{ PType.intType }, new List<PType>()),
                Program.PrintInt) },
            { "hello", new ExternalCall(new PSignatureType(new List<PType>(), new List<PType>()),
                Program.HelloWorld) },
            { "nl", new ExternalCall(new PSignatureType(new List<PType>(), new List<PType>()),
                Program.NewLine) }
        };

        public ExternalCall(PSignatureType signature, ExternalActionDelegate action)
        {
            this.signature = signature;
            this.action = action;
        }
    }

    public delegate void ExternalActionDelegate(ref Stack<object> stack);
}
