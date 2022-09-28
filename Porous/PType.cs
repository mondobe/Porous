using System.Security.Cryptography.X509Certificates;
using UtahBase.Testing;

namespace Porous
{
    public class PType 
    {
        public string name;

        public PType(string name)
        {
            this.name = name;
        }

        public static PType intType = new("int"), 
            charType = new("char"), 
            boolType = new("bool");

        public static PType ParseType(ParseToken pt)
        {
            if (!pt.tags.Contains("type"))
                throw new PorousException(pt, "Non-type value being used like a type.");
            if (pt.tags.Contains("sig"))
            {
                PSignatureType sig = new(new List<PType>(), new List<PType>());
                for (int i = 1; i < pt.children.Count - 1; i++)
                    sig.outs.Add(ParseType(pt.children[i]));
                for (int i = 1; i < pt.children[0].children.Count - 1; i++)
                    sig.ins.Add(ParseType(pt.children[0].children[i]));
                return sig;
            }
            return pt.content switch
            {
                "int" => intType,
                "char" => charType,
                "bool" => boolType,
                _ => throw new PorousException(pt, "Unrecognized type. Are you sure this is a type?")
            };
        }

        public override string ToString()
        {
            return name;
        }
    }

    public class PSignatureType : PType
    {
        public List<PType> ins, outs;

        public PSignatureType(List<PType> ins, List<PType> outs, string name = "") : base(name)
        {
            this.ins = ins;
            this.outs = outs;
        }

        public override string ToString()
        {
            string toRet = "(";
            foreach (PType i in ins)
                toRet += i + " ";
            toRet += ":";
            foreach (PType o in outs)
                toRet += " " + o;
            return toRet + ")";
        }
    }
}