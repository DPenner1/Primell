lexer grammar PrimellLexer;

INFINITY : '∞' ;

RTL : '$' ;
LTR : '€' ;

CONCAT : ';' ;

ASSIGN : '=' ;

L_BRACK : '[' ;
R_BRACK : ']' ;
L_PAREN : '(' ;
R_PAREN : ')' ;
VERT_BAR : '|' ;

DOT: '.' ;

// these symbols get re-used as both ops and mods in different contexts (or probably will)
// in theory, i could use modes, but i think this ones easier in the parser
TAIL : '>' ;
PLUS : '+' ;
STAR : '*' ;
NEGATE : '~' ;
F_SLASH: '/' ;
B_SLASH: '\\' ;

D_QUOTE: '"' -> skip, pushMode(IN_STR) ; 

OPMOD_POW : '^' ;
OPMOD_CUT : '`' ;

OP_NULLARY : ':"' | ':,' ;

OP_UNARY :  '_:' | '_<' | '_>' | '_*' | '__' | '_?' | '_~' | '_@' | ':>"' | ':>,' | ':>_' | ':@"' | ':@,' // list unary
         | '++' | '--' | '+-' | '`~' | '!|' | '!\\' | '!/' // numeric unary
         ;

OP_USER_UNARY : '_' INT_OR_ID | '#' INT_OR_ID ;

OP_BINARY : '-' |'%' | '**' | '//' | '…' | '..' | '<' | '`&' | '`|'  | '`^' // numeric binary
          | '<::>' | '*?' | '*?~' | '&' | ':@_' | ':@>_' | ':@>"' | ':@>,' | '@_'     // list binary
          | '@' | '::>'// list-numeric
          | '<::'  //numeric-list
          ;

OP_USER_BINARY : '_' INT_OR_ID '_' | '_' INT_OR_ID '#' | '#' INT_OR_ID '_' | '#' INT_OR_ID '#' ;

OP_COND : '?' ;

WS : [ \t\r\n]+ -> channel(HIDDEN) ;

// this is objectively silly
INT_OR_ID : INT_OR_ID_CHAR+ ;  
fragment INT_OR_ID_CHAR : [0-9A-Za-zÞþ,] ;
// TODO - later to be even less restrictive

COMMENT : '~~' [\u0000-\uFFFF]*;

mode IN_STR;

STRING: ~["\u0000-\u001F]+ ;  // TODO - could be more detailed

InStr_D_QUOTE: '"' -> skip, popMode ;
