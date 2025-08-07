lexer grammar GlacierLexer;
tokens { INDENT, DEDENT }

//Indent handler
@lexer::header {
using Glacier.Core.Parser.AntlrDenter.AntlrDenter;
}

@lexer::members {
private DenterHelper denter;
  
public override IToken NextToken()
{
    if (denter == null)
    {
        denter = DenterHelper.Builder()
            .Nl(NL)
            .Indent(GlacierParser.INDENT)
            .Dedent(GlacierParser.DEDENT)
            .PullToken(base.NextToken);
    }

    return denter.NextToken();
}
}

//New line
NL: ('\r'? '\n' '\t'*);

//Special
COMMA   : ','   ;
DOT     : '.'   ;
LPAREN  : '('   ;
RPAREN  : ')'   ;
LCURLY  : '{'   ;
RCURLY  : '}'   ;
COLON   : ':'   ;


//OPs
POW     : '**'  ;

MUL     : '*'   ;
DIV     : '/'   ;
REM		: '%'	;
IDIV    : '//'  ;

ADD     : '+'   ;
SUB     : '-'   ;



//Comparison
EQ      : '=='  ;
NEQ     : '!='  ;

LT      : '<'   ;
GT      : '>'   ;

LTI     : '<='  ;
GTI     : '>='  ;

//Assign
ASSIGN  : '<-' | '=' ;


//Literals
fragment DIGIT : [0-9];
fragment DIGITS: [0-9]+;
BOOL  : 'True' | 'False';
INT   : DIGITS;
FLOAT : DIGITS DOT DIGITS;
NONE  : 'None';
STRING: '\'' .*? '\'';

//Keywords
AND     	: 'and' ;
OR      	: 'or'  ;
NOT     	: 'not' ;

IF			: 'if'	;
ELSE		: 'else';
DATADECL  	: 'data';
FUNCDECL	: 'fn'  ;
RETURN 	 	: 'ret' ;
WITH    	: 'with';

PRINT		: 'print';
ERR			: 'err'	 ;
STRCONVERT	: 'str'	 ;

DEBUG		: '@';
//Identifier
ID    	: [a-zA-Z_][a-zA-Z_0-9]*;

//Whitespace
WS      : [ \t]+ -> skip;

//Comments
COMM : '#' ~[\r\n]* -> skip ;