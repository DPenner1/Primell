lexer grammar PrimellLexer;

INT : [0-9A-Za-zÞþ]+ ; 
INFINITY : '∞' ;
IDENTIFIER : [,]+ ;

RTL : '$' ;
LTR : '€' ;

CONCAT : ';' ;

ASSIGN : '=' ;

LBRACK : '[' ;
RBRACK : ']' ;
LPAREN : '(' ;
RPAREN : ')' ;
LBRACE : '{' ;
RBRACE : '}' ;
VERT_BAR : '|' ;

OUT_INV : '"~' ;
OUT_DEF : '""' ;
OUT_STR : '"' ;

OPMOD_POW : '^' ;
OPMOD_CUT : '`' ;
OPMOD_FOLD : '!' ;

OP_READ_CODE : '_:' ;
OP_READ_STR : ':"' ;
OP_READ_CSV : ':,' ;
OP_GAMMA : 'Γ' ;
OP_NEXT : '++';
OP_PREV : '--';
OP_ROUND : '+-' ;
OP_NEGATE : '~' ;
OP_HEAD : '_<' ;
OP_TAIL : '_>' ;
OP_DISTINCT : '_*' ;
OP_FLATTEN : '__' ;
OP_PURGE : '_?' ;
OP_REV : '_~' ;
OP_SORT : '_@' ;
OP_ADD : '+' ;
OP_SUB : '-' ;
OP_DIV : '/' ;
OP_MUL : '*' ;
OP_MOD : '%' ;
OP_POW : '**';
OP_LOG : '//';
OP_INC_RANGE : '…';
OP_RANGE : '..' ;
OP_SMALL : '<' ;
OP_BIG : '>' ;
OP_BIT_AND : '`&' ;
OP_BIT_OR : '`|' ;
OP_BIT_XOR : '`^' ;
OP_BIT_NOT : '`~' ;
OP_INDEX : '@' ;
OP_INDEX_OF : '@#' ;
OP_FILT : '*?';
OP_NEG_FILT : '*?~' ;
OP_JUMP : '?/' ;
OP_NEG_JUMP : '?~/' ;
OP_JUMP_BACK : '?\\' ;
OP_NEG_JUMP_BACK : '?~\\' ;
OP_COND : '?' ;
OP_NEG_COND : '?~' ;
OP_WHILE : '?*' ;
OP_NEG_WHILE : '?~*' ;
OP_DO_WHILE : '?+' ;
OP_NEG_DO_WHILE : '?~+' ;
OP_LIST_DIFF : '\\' ;
OP_INTERSECT : '&' ;
OP_CONS : '<::' ;
OP_APPEND : '::>' ;
OP_CONCAT : '<::>' ;

WS : [ \t\r\n]+ -> skip ;

COMMENT : '~~' ~[\r\n]* ;