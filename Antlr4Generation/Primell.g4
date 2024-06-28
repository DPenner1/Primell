grammar Primell;

program : line? (NL+ line)* EOF;   // there was probably a simpler way to write this grammar rule

line : termSeq outMethod?;

termSeq : mulTerm+ ;

mulTerm: atomTerm                                     #atom
       | mulTerm numUnaryOp                           #numericUnaryOperation 
       | mulTerm listUnaryOp                          #listUnaryOperation
       | mulTerm binaryOp (atomTerm | RTL termSeq)    #binaryOperation
       | mulTerm forEachBlock                         #forEachLeftTerm
       | mulTerm binaryOp '[' termSeq ']'             #forEachRightTerm
       ;             

atomTerm : INT                #integer
         | '-∞'               #negativeInfinity
         | '∞'                #positiveInfinity
         | nullaryOp          #nullaryOperation
         | '(' termSeq? ')'   #parens
         ;

forEachBlock : '[' forEachOperation ']' ;

forEachOperation : binaryOp (atomTerm | RTL termSeq)  #forEachBinary
                 | numUnaryOp                         #forEachNumericUnary
                 | listUnaryOp                        #forEachListUnary
                 ;

baseNullaryOp : OP_VAR1 | OP_VAR2 | OP_VAR3 | OP_READ_LIST | OP_READ_STR ;

baseNumUnaryOp : OP_FACT | OP_NEXT | OP_PREV | OP_ROUND | OP_NEGATE | OP_BIN_NOT;

baseNumBinaryOp : OP_ADD | OP_SUB | OP_MUL | OP_DIV | OP_MOD | OP_POW | OP_LOG | OP_SMALL | OP_BIG
         | OP_INC_RANGE | OP_RANGE | OP_BIN_AND | OP_BIN_OR | OP_BIN_XOR ;

baseListUnaryOp : OP_HEAD | OP_TAIL | OP_DISTINCT | OP_REV | OP_FLATTEN | OP_SORT ;

baseListBinaryOp : OP_COND | OP_NEG_COND | OP_INDEX | OP_INDEX_END | OP_INDEX_OF
                 | OP_JUMP | OP_JUMP_BACK | OP_NEG_JUMP | OP_NEG_JUMP_BACK
                 | OP_LSHIFT | OP_RSHIFT | OP_LROTATE | OP_RROTATE
                 | OP_LIST_DIFF | OP_INTERSECT
                 ;

// Coming back to this after years, plural makes me think I intended multiple mods possible at once
// but never got around to implementing that. Also no idea whey opMods and assignMods are both here and the same.

opMods : (OPMOD_CUT | OPMOD_POW)? ;

assignMods : (OPMOD_CUT | OPMOD_POW)? ;

nullaryOp : baseNullaryOp opMods ;

numUnaryOp : baseNumUnaryOp opMods ;

listUnaryOp : baseListUnaryOp opMods ;

binaryOp : ASSIGN assignMods
         | baseNumBinaryOp opMods (ASSIGN assignMods)?
         | baseListBinaryOp opMods (ASSIGN assignMods)?
         ;

outMethod : OUT_INV | OUT_DEF | OUT_STR ;

/* 
'['=1
']'=2
'\u00f8'=3
'\u221e'=4
'('=5
')'=6
'.'=7

T__0=1
T__1=2
T__2=3
T__3=4
T__4=5
T__5=6
T__6=7*/


INT : [0-9A-Za-zÞþ]+ ; 

RTL : '$' ;

// REF=10 don't yet know what this token will be.
ASSIGN : '=';
USER_OP : 'λ';

OP_VAR1 : ',' ;
OP_VAR2 : ';' ;
OP_VAR3 : '#' ;

OP_READ_LIST : ':_' ;
OP_READ_STR : ':~' ;
OP_FACT : 'Γ' ;
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
OP_INDEX_END : '@>' ;
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