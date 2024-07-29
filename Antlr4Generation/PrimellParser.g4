parser grammar PrimellParser;

options {
  tokenVocab = PrimellLexer;
}

line : termSeq outputSpec? COMMENT? EOF;

outputSpec : OUT_INV | OUT_DEF | OUT_STR ;

termSeq : concatMulterm+ ;

concatMulterm : CONCAT? mulTerm ;

mulTerm : atomTerm                                                              #atom
        | mulTerm numUnaryOp                                                    #numericUnaryOperation 
        | mulTerm listUnaryOp                                                   #listUnaryOperation
        | mulTerm binaryOp (atomTerm | RTL termSeq)                             #binaryOperation
        | FOREACH_LEFT termSeq FOREACH_RIGHT listUnaryOp                        #forEachListUnary
        | FOREACH_LEFT termSeq FOREACH_RIGHT binaryOp (atomTerm | RTL termSeq)  #forEachBinary
        | mulTerm FOREACH_LEFT binaryOp termSeq FOREACH_RIGHT                   #forEachRightTerm
        ;             

atomTerm : INT                      #integer
         | INFINITY                 #infinity
         | nullaryOp                #nullaryOperation
         | LPAREN RPAREN            #emptyList
         | LPAREN termSeq RPAREN    #parens
         ;

baseNullaryOp : IDENTIFIER | OP_READ_STR | OP_READ_CSV;

baseNumUnaryOp : OP_GAMMA | OP_NEXT | OP_PREV | OP_ROUND | OP_NEGATE | OP_BIN_NOT;

baseNumBinaryOp : OP_ADD | OP_SUB | OP_MUL | OP_DIV | OP_MOD | OP_POW | OP_LOG | OP_SMALL | OP_BIG
                | OP_INC_RANGE | OP_RANGE | OP_BIN_AND | OP_BIN_OR | OP_BIN_XOR 
                ;

baseListUnaryOp : OP_HEAD | OP_TAIL | OP_DISTINCT | OP_REV | OP_FLATTEN | OP_SORT | OP_READ_CODE 
                ;

baseListBinaryOp : OP_COND | OP_NEG_COND | OP_INDEX_OF
                 | OP_JUMP | OP_JUMP_BACK | OP_NEG_JUMP | OP_NEG_JUMP_BACK
                 | OP_LIST_DIFF | OP_INTERSECT
                 ;

baseListNumericOp : OP_INDEX ;

baseNumericListOp : OP_CONS ;

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
         | baseListNumericOp opMods (ASSIGN assignMods)?
         | baseNumericListOp opMods (ASSIGN assignMods)?
         ;



