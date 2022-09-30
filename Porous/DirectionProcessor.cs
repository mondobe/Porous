using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using UtahBase.Testing;

namespace Porous
{
    public static class DirectionProcessor
    {
        public static List<Direction> ExtractDirections(ParseToken pt)
        {
            if (!pt.tags.Contains("blockType") || pt.children[1].children.Count < 2)
                throw new PorousException(pt, "Attempted to call non-function. Are you sure this is a function?");

            List<Direction> exDirs = new();
            List<ParseToken> tox = pt.children[1].children;

            for (int i = 1; i < tox.Count - 1; i++)
                exDirs.Add(ProcessDirection(tox[i]));

            return exDirs;
        }

        public static Direction ProcessDirection(ParseToken pt)
        {
            if (!pt.tags.Contains("stmt"))
                throw new PorousException(pt, "Non-statement used like a direction. This could be an unimplemented feature.");

            if (pt.tags.Contains("int"))
                return new PushIntDirection(int.Parse(pt.content), pt);

            if (pt.tags.Contains("bool"))
                return new PushBoolDirection(bool.Parse(pt.content), pt);

            if (pt.tags.Contains("char"))
                return new PushCharDirection(pt.content[1], pt);

            if (pt.tags.Contains("oper"))
                return new IntArithmeticDirection(pt.content switch
                {
                    "+" => (int lhs, int rhs) => lhs + rhs,
                    "-" => (int lhs, int rhs) => lhs - rhs,
                    "*" => (int lhs, int rhs) => lhs * rhs,
                    "/" => (int lhs, int rhs) => lhs / rhs,
                    "%" => (int lhs, int rhs) => lhs % rhs,
                    _ => throw new PorousException(pt, "Operation not yet implemented.")
                }, pt);

            if(pt.tags.Contains("blockType"))
            {
                List<Direction> funcBody = new();
                List<ParseToken> directions = pt.children[1].children;
                for (int i = 1; i < directions.Count - 1; i++)
                    funcBody.Add(ProcessDirection(directions[i]));
                return new PushFunctionDirection(
                    new GenericFunction((PSignatureType)PType.ParseType(pt.children[0]), funcBody, new List<string>())
                    , pt);
            }

            return pt.content switch
            {
                ":" => new DoDirection(pt),
                _ => throw new PorousException(pt, "Direction not recognized. Did you forget to include a file?")
            };
        }
    }
}
