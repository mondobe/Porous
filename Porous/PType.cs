using UtahBase.Testing;

namespace Porous
{
    /// <summary>
    /// Represents a type in Porous. Classes derived from this can hold more specific information about 
    /// their type.
    /// </summary>
    public class PType 
    {
        /// <summary>
        /// The name of this type. Used for debug purposes only.
        /// </summary>
        public string name;

        /// <summary>
        /// Creates a new type with this name. Should be called only to create the int, char, and bool types.
        /// </summary>
        /// <param name="name">The name of the type to be created.</param>
        public PType(string name)
        {
            this.name = name;
        }

        public static PType intType = new("int"), 
            charType = new("char"), 
            boolType = new("bool");

        /// <summary>
        /// Automatically determines the type referred to by a given ParseToken or throws an error if none is found.
        /// </summary>
        /// <param name="pt">The ParseToken, which presumably refers to a certain type.</param>
        /// <returns>The best guess as to what the type is, following Porous type syntax rules.</returns>
        /// <exception cref="PorousException">The type cannot be determined from the given ParseToken.</exception>
        public static PType ParseType(ParseToken pt)
        {
            // Make sure it is a type in the first place.
            if (!pt.tags.Contains("type"))
                throw new PorousException(pt, "Non-type value being used like a type.");

            // Is it a signature?
            if (pt.tags.Contains("sig"))
            {
                PSignatureType sig = new(new List<PType>(), new List<PType>());

                // Process the given signature for inputs and outputs
                /* The structure of a signature looks like this internally in Utah:
                 *               ------signature-------------
                 *              /              |              \
                 *             /               |               \
                 *    -----sigstart----    type type ... type   )
                 *   /       |         \
                 *  /        |          \
                 * (  type type .. type  :
                 */
                for (int i = 1; i < pt.children.Count - 1; i++)
                    sig.outs.Add(ParseType(pt.children[i]));

                for (int i = 1; i < pt.children[0].children.Count - 1; i++)
                    sig.ins.Add(ParseType(pt.children[0].children[i]));
                return sig;
            }

            // Is it the basic types recognized by Porous?
            return pt.content switch
            {
                "int" => intType,
                "char" => charType,
                "bool" => boolType,
                // Otherwise, throw an error.
                _ => throw new PorousException(pt, "Unrecognized type. Are you sure this is a type?")
            };
        }

        public override string ToString()
        {
            return name;
        }
    }

    /// <summary>
    /// A class representing the signature type in Porous: similar to a function pointer,
    /// with inputs and outputs specified.
    /// </summary><example>
    /// When a function is evaluated during type checking, the stack is checked against the input values.
    /// If they match, the function's type signature is executed: the input values are popped off the stack,
    /// and the output values are pushed on. Here's an example of a function with the following signature:
    /// (int char : bool (: char))
    /// 
    /// initial stack                                               output stack
    ///     
    ///  X  (int char : bool (:char))   - Function to be evaluated      
    ///  X  char                        - matches "char"                (: char)
    ///  X  int                         - matches "int"                 bool
    ///     int                                                         int
    ///     bool                                                        bool
    ///     char[]                                                      char[]
    /// </example>
    public class PSignatureType : PType
    {
        public List<PType> ins, outs;

        // Create a signature type, given the input and output types.
        public PSignatureType(List<PType> ins, List<PType> outs) : base("")
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