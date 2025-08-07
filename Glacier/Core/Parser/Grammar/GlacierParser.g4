parser grammar GlacierParser;
options { tokenVocab=GlacierLexer; }

//TODO replace fndecl with stmt
program : (expr | stmt | NL)* EOF;

id : ID;


//Const
const_int  : INT    ;
const_float: FLOAT  ;
const_bool : BOOL   ;
const_none : NONE   ;
const_str  : STRING ;

const_val  : const_int | const_float | const_str | const_bool | const_none; 


assign	: id ASSIGN expr;

//Expressions
expr 	: expr EQ  expr
		| expr NEQ expr
		| expr GT  expr
		| expr LT  expr
		| expr GTI expr
		| expr LTI expr
		| addi
		;
		
addi	: addi ADD addi
		| addi SUB addi
		| term
		;

term	: term MUL  term
		| term DIV  term
		| term REM  term
		| term IDIV term
		| pow_expr;
		
pow_expr: atom POW pow_expr  //Pow
		| factor
		;

factor  : SUB factor		 //Factor, -smth
		| atom
		;

atom 	: const_val			 //Literal
		| LPAREN expr RPAREN //Parenthesis
		| fncall
		| strexpr
		| id				 //Variable name
		;

//Statements
stmt	: assign
		| ifstmt
		| fndecl
		| return
		| expr
		| printstmt
		| debugstmt
		| errstmt
		;
		
printstmt	: PRINT LPAREN expr RPAREN;
block		: INDENT (expr | stmt | NL)* DEDENT;
fndecl  	: FUNCDECL id LPAREN (id (COMMA id)*)? RPAREN COLON block;
fncall  	: id LPAREN (expr (COMMA expr)*)? RPAREN;
return 		: RETURN expr;

errstmt		: ERR LPAREN expr RPAREN;
ifstmt 		: IF expr block (ELSE block);
strexpr		: STRCONVERT LPAREN expr RPAREN;
debugstmt   : DEBUG id;
