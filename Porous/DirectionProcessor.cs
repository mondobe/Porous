using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtahBase.Testing;

namespace Porous
{
    /// <summary>
    /// A static catch-all class for the process of resolving directions from ParseTokens.
    /// Most of the class is dedicated to the ProcessDirection method.
    /// </summary>
    public static class DirectionProcessor
    {
        public static Dictionary<string, Direction> workingDict = new();
        public static List<string> headers = new();

        /// <summary>
        /// Initialize the processor with empty values.
        /// </summary>
        public static void Initalize()
        {
            workingDict = new Dictionary<string, Direction>();
            headers = new List<string>();
        }

        /// <summary>
        /// From a given ParseToken describing a generic function, extracts all of the directions from it.
        /// </summary>
        /// <param name="pt">The ParseToken describing the function.</param>
        /// <returns>A list of directions describing the nature of the function without specific signatures.</returns>
        /// <exception cref="PorousException">Thrown if the input ParseToken was not recognized by the parser 
        /// as a function.</exception>
        public static List<Direction> ExtractDirections(ParseToken pt)
        {
            // Make sure the given ParseToken describes a function.
            if (!pt.tags.Contains("blockType") || pt.children[1].children.Count < 2)
                throw new PorousException(pt, "Attempted to call non-function. Are you sure this is a function?");

            // Add each direction into a list
            List<Direction> exDirs = new();
            List<ParseToken> tox = pt.children[1].children;

            // Process the directions from the input's children
            for (int i = 1; i < tox.Count - 1; i++)
                exDirs.Add(ProcessDirection(tox[i]));

            return exDirs;
        }

        /// <summary>
        /// Processes a direction from a given ParseToken.
        /// </summary>
        /// <param name="pt">The ParseToken that presumably describes a direction.</param>
        /// <returns>A generic direction to be part of a function or global.</returns>
        /// <exception cref="PorousException">Thrown if the input ParseToken is not a statement.</exception>
        public static Direction ProcessDirection(ParseToken pt)
        {
            // Throw an error if a statement is not recieved
            if (!pt.tags.Contains("stmt"))
                throw new PorousException(pt, "Non-statement used like a direction. This could be an unimplemented feature.");

            // Parse an external call
            if (pt.tags.Contains("extern"))
                return new ExternDirection(pt.content.Substring(2), pt);

            // Parse a PushIntDirection
            if (pt.tags.Contains("int"))
                return new PushIntDirection(int.Parse(pt.content), pt);

            // Parse a PushBoolDirection
            if (pt.tags.Contains("bool"))
                return new PushBoolDirection(bool.Parse(pt.content), pt);

            // Parse a PushCharDirection
            if (pt.tags.Contains("char"))
                return new PushCharDirection(pt.content[1], pt);

            // Parse a mathematical operation
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

            // Parse a mathematical operation
            if (pt.tags.Contains("boolOp"))
                return new ComparisonDirection(pt.content switch
                {
                    ">" => (int lhs, int rhs) => lhs > rhs,
                    "<" => (int lhs, int rhs) => lhs < rhs,
                    ">=" => (int lhs, int rhs) => lhs >= rhs,
                    "<=" => (int lhs, int rhs) => lhs <= rhs,
                    _ => throw new PorousException(pt, "Operation not yet implemented.")
                }, pt);

            // Parse a function block
            if(pt.tags.Contains("blockType"))
            {
                List<Direction> funcBody = new();

                // Process each of the directions in the function
                List<ParseToken> directions = pt.children[1].children;
                for (int i = 1; i < directions.Count - 1; i++)
                    funcBody.Add(ProcessDirection(directions[i]));

                // Parse the signature of the function and return the PushFunctionDirection
                return new PushFunctionDirection(
                    new GenericFunction((PSignatureType)PType.ParseType(pt.children[0]), funcBody, new List<string>())
                    , pt);
            }

            // Parse a global call
            if (headers.Contains(pt.content))
                return new CallDirection(pt.content, pt);

            // Parse miscellaneous directions
            return pt.content switch
            {
                ":" => new DoDirection(pt),
                "dup" => new DupDirection(pt),
                "drop" => new DropDirection(pt),
                "swap" => new SwapDirection(pt),
                "over" => new OverDirection(pt),
                "?" => new ChooseDirection(pt),
                "==" => new EqualsDirection(pt),
                "." => new PutDirection(pt),
                "!" => new NotDirection(pt),
                _ => throw new PorousException(pt, "Direction not recognized. Did you forget to include a file?")
            };
        }
    }
}
