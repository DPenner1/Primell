lexer grammar PrimellLexer;

INT : [0-9A-Za-zÞþ]+ ; 
INFINITY : '∞' ;
IDENTIFIER : [,]+ ;

RTL : '$' ;

ASSIGN : '=' ;

FOREACH_LEFT : '[' ;
FOREACH_RIGHT : ']' ;
LPAREN : '(' ;
RPAREN : ')' ;

OP_READ_CODE : ':_' ;
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
OP_BIN_AND : '`&' ;
OP_BIN_OR : '`|' ;
OP_BIN_XOR : '`^' ;
OP_BIN_NOT : '`~' ;
OP_LSHIFT : '<<' ;
OP_RSHIFT : '>>' ;
OP_LROTATE : '<<<' ;
OP_RROTATE : '>>>' ;
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
OP_LIST_DIFF : '\\' ;
OP_INTERSECT : '&' ;
OUT_INV : '"~' ;
OUT_DEF : '""' ;
OUT_STR : '"' ;
// OUT_MOD_VERT=65  don't know what this is yet
OPMOD_POW : '^' ;
OPMOD_CUT : '`' ;
OPMOD_FOLD : '!' ;

WS : [ \t\r]+ -> skip ;  // new line \n will be checked for
NL : '\n';