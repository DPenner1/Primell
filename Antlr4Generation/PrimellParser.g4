parser grammar PrimellParser;

options {
  tokenVocab = PrimellLexer;
}

line : termSeq outputSpec? COMMENT? EOF;

outputSpec : OUT_INV | OUT_DEF | OUT_STR ;

termSeq : concatRtlTerm+ ;

concatRtlTerm : CONCAT? rtlTerm ;

rtlTerm : mulTerm                                                                   #passThroughRtl
        | mulTerm binaryAssign (rtlTerm | RTL termSeq)                              #stdAssign
        | mulTerm FOREACH_LEFT binaryAssign termSeq FOREACH_RIGHT                   #forEachRightAssign
        | FOREACH_LEFT termSeq binaryAssign FOREACH_RIGHT (rtlTerm | RTL termSeq)   #forEachLeftAssign
        ;

binaryAssign : ASSIGN assignMods binaryOp? ;

mulTerm : atomTerm                                                              #passThrough
        | mulTerm unaryAssign? unaryOp                                          #unaryOperation
        | mulTerm binaryOp (atomTerm | RTL termSeq)                             #binaryOperation
        | FOREACH_LEFT termSeq FOREACH_RIGHT unaryOp                            #forEachUnary
        | FOREACH_LEFT termSeq binaryOp FOREACH_RIGHT (atomTerm | RTL termSeq)  #forEachLeftBinary
        | mulTerm FOREACH_LEFT binaryOp termSeq FOREACH_RIGHT                   #forEachRightBinary
        ;

unaryAssign : ASSIGN assignMods ;             

atomTerm : INT                      #integer
         | INFINITY                 #infinity
         | nullaryOp                #nullaryOperation
         | LPAREN RPAREN            #emptyList
         | LPAREN termSeq RPAREN    #parens
         ;

baseNullaryOp : IDENTIFIER | OP_READ_STR | OP_READ_CSV
              ;

baseNumUnaryOp : OP_GAMMA | OP_NEXT | OP_PREV | OP_ROUND | OP_NEGATE | OP_BIN_NOT 
               ;

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

opMods : (OPMOD_CUT | OPMOD_POW)? ;

assignMods : (OPMOD_CUT | OPMOD_POW)? ;

nullaryOp : baseNullaryOp opMods ;

unaryOp : baseUnaryOp opMods ;

baseUnaryOp : baseNumUnaryOp
            | baseListUnaryOp
            ;

binaryOp : baseBinaryOp opMods ;

baseBinaryOp : baseNumBinaryOp
             | baseListBinaryOp
             | baseListNumericOp
             | baseNumericListOp
             ;



