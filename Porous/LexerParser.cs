﻿using System;
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
:string=
REQUIRE dq
IF dq NEXT ELSE REPEAT
WRAP BACK ADD string ADD stmt ADD expr

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

        /// <summary>
        /// Lexes a given string consistently with the lexer rules of Porous. Lexer brought to you by T-Lex.
        /// </summary>
        /// <param name="program">The given string, usually direct from the user.</param>
        /// <param name="verbose">Whether or not to display debug information about the lexing process 
        /// (activating this makes the program slower!)</param>
        /// <returns>A series of tokens that more fully describes the nature of the program</returns>
        public static List<Token> Lex(string program, bool verbose = false)
        {
            Lexer l = CreateLexerFromString.StringToLexer(lexer, verbose);
            return l.ApplyAll(program, verbose, 50000);
        }

        /// <summary>
        /// Parses a string of tokens consistently with the parser rules of Porous. Parser brought to you by Utah.
        /// </summary>
        /// <param name="tox">The given string of tokens, usually from the lexer (<see cref="Lex(string, bool)"/></param>
        /// <param name="verbose">Whether or not to display debug information about the parsing process</param>
        /// <returns>A tree of ParseTokens that can be easily processed to extract a full program</returns>
        public static List<ParseToken> Parse(List<Token> tox, bool verbose = false)
        {
            Parser p = CreateParserFromString.FromString(parser, verbose);
            return p.ParseTokens(tox, verbose:verbose);
        }
    }
}
