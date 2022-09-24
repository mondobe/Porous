using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLexBase;
using TLexBase.Testing;
using TLexCLI;
using UtahBase;
using UtahBase.Testing;
using UtahCLI;

namespace Porous
{
    public static class LexerParser
    {
        private const string lexer = @"
:comment=
REQUIRE /
REQUIRE /
IF lf NEXT ELSE REPEAT
WRAP BACK DELETE

:include=
WORD include DELETE
IF ws { DELETE REPEAT }
ELSE NEXT
IF lf { DELETE NEXT }
ELSE REPEAT
WRAP BACK ADD include

:ops=
TAGCHARS +-*/%<> oper

:boolOps=
TAGCHARS <> nBoolOper

:boolEqOps=
IF nBoolOper NEXT ELSE CANCEL
IF eq SKIP ELSE CANCEL
WRAP BACK ADD oper

:zeroToNine=
TAGCHARS 0123456789 z9

:oneToNine=
TAGCHARS 123456789 o9

:int=
REQUIRE o9
ZEROORMORE z9
WRAP BACK ADD int ADD stmt ADD expr ADD positive

:int=
IF 0 { ADD int ADD stmt ADD expr ADD positive CANCEL }

:negative=
REQUIRE -
REQUIRE int
WRAP BACK ADD int ADD stmt ADD expr

:schar=
IF ' NEXT ELSE CANCEL
NEXT
IF ' NEXT ELSE CANCEL
WRAP BACK ADD char ADD stmt ADD expr

:true=
WORD true ADD bool ADD stmt ADD expr

:false=
WORD false ADD bool ADD stmt ADD expr

:letter=
IF ws CANCEL ELSE SKIP
IF oper CANCEL ELSE SKIP
IF ocb CANCEL ELSE SKIP
IF ccb CANCEL ELSE SKIP
IF ( CANCEL ELSE SKIP
IF ) CANCEL ELSE SKIP
IF eq CANCEL ELSE SKIP
IF expr CANCEL ELSE SKIP
IF colon CANCEL ELSE SKIP
IF include CANCEL ELSE SKIP
IF < CANCEL ELSE SKIP
IF > CANCEL ELSE SKIP
IF & CANCEL ELSE SKIP
IF [ CANCEL ELSE SKIP
IF ] CANCEL ELSE SKIP
ADD letter

:deq=
REQUIRE eq
REQUIRE eq
WRAP BACK ADD letter

:word=
REQUIRE letter
ZEROORMORE letter
WRAP BACK ADD word

:rmws=
IF ws DELETE CANCEL
";

        private const string parser = @"
    # Signature context (expr)

( type* colon : sigStart;
sigStart type* ) : sig;

    # Type context (type)

word :: type;
sig :: type;
[ type ] : type arrType;
& type : type ptrType;

    # Normal context (stmt)

oper :: stmt;
word :: stmt;

{ stmt* } : block;

< word* > : generics;
sig generics : genSig;
sig block : blockType stmt;
genSig block : blockType stmt genBlock;

[ expr* ] : expr array;

    # Global context (decl/body)

word = stmt : global;
";

        public static List<Token> Lex(string program, bool verbose = false)
        {
            Lexer l = CreateLexerFromString.StringToLexer(lexer, verbose);
            return l.ApplyAll(program, verbose, 50000);
        }

        public static List<ParseToken> Parse(List<Token> tox, bool verbose = false)
        {
            Parser p = CreateParserFromString.FromString(parser, verbose);
            return p.ParseTokens(tox, verbose:verbose);
        }
    }
}
