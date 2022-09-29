using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtahBase.Testing;

namespace Porous
{
    /// <summary>
    /// An exception class for Porous compiler errors found while processing or executing
    /// a program. Seeing a PorousException alerts the user that something is either wrong
    /// with their program or this one.
    /// </summary>
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
