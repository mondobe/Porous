using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtahBase.Testing;

namespace Porous
{
    public class PorousException : Exception
    {
        public PorousException() : base()
        {
            throw new Exception("Porous program is malformed. No other information available.");
        }

        public PorousException(string message) : base(message)
        {
            throw new Exception("Porous program is malformed: " + message);
        }

        public PorousException(ParseToken pt) : base()
        {
            throw new Exception("Porous program is malformed at " + pt.toSimpleString);
        }

        public PorousException(ParseToken pt, string message) : base(message)
        {
            throw new Exception("Porous program is malformed at " + pt.toSimpleString + ": " + message);
        }
    }
}
