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
        | mulTerm binaryAssign LBRACK termSeq RBRACK                   #forEachRightAssign
        | LBRACE termSeq RBRACE binaryAssign (rtlTerm | RTL termSeq)   #forEachLeftAssign
        ; // TODO : rtl assign stuff hasn't been updated with the latest mulTerm mirrors

binaryAssign : ASSIGN assignMods binaryOp? ;

mulTerm : atomTerm                                          #passThroughMulTerm
        | mulTerm unaryOp                                   #unaryOperation
        | mulTerm binaryOpWithRS                            #binaryOperation
        | LBRACK termSeq RBRACK unaryOp                     #forEachUnary
        | LBRACK termSeq RBRACK binaryOpWithRS              #forEachLeftBinary
        | LBRACK termSeq VERT_BAR unaryOrBinaryOp+ RBRACK   #forEachChain
        ;          

binaryOpWithRS : binaryOp atomTerm
               | binaryOp RTL termSeq
               | LBRACK binaryOp termSeq RBRACK
               ;

unaryOrBinaryOp : unaryOp | binaryOpWithRS ;

atomTerm : INT                      #integer
         | INFINITY                 #infinity
         | nullaryOp                #nullaryOperation
         | LPAREN RPAREN            #emptyList
         | LBRACK RBRACK            #emptyList  // i don't think it ever matters which
         | LPAREN termSeq RPAREN    #parens
         ;

// Note; numeric/list op distinction isn't syntactical, it's functional!
baseNullaryOp : IDENTIFIER | OP_READ_STR | OP_READ_CSV
              ;

baseUnaryOp : OP_GAMMA | OP_NEXT | OP_PREV | OP_ROUND | OP_NEGATE | OP_BIT_NOT   // numeric unary
            | OP_HEAD | OP_TAIL | OP_DISTINCT | OP_REV | OP_FLATTEN | OP_SORT | OP_READ_CODE   // list unary
            ;
            
baseBinaryOp : OP_ADD | OP_SUB | OP_MUL | OP_DIV | OP_MOD | OP_POW | OP_LOG   // num binary (math)
             | OP_BIT_AND | OP_BIT_OR | OP_BIT_XOR                            // num binary (bitwise)
             | OP_INC_RANGE | OP_RANGE | OP_SMALL | OP_BIG                    // num binary (misc)
             | OP_LIST_DIFF | OP_INTERSECT | OP_INDEX_OF | OP_CONCAT          // list binary
             | OP_INDEX | OP_APPEND                                           // list numeric
             | OP_CONS                                                        // numeric list
                // Conditionals (list binary, but I may have to section them off later for easier handling)
             | OP_COND | OP_NEG_COND 
             | OP_JUMP | OP_JUMP_BACK | OP_NEG_JUMP | OP_NEG_JUMP_BACK
             | OP_WHILE | OP_NEG_WHILE | OP_DO_WHILE | OP_NEG_DO_WHILE
             ;

opMods : (OPMOD_CUT | OPMOD_POW)? ;

assignMods : (OPMOD_CUT | OPMOD_POW)? ;

nullaryOp : baseNullaryOp opMods ;

unaryOp : unaryAssign? baseUnaryOp opMods ;

unaryAssign : ASSIGN assignMods ;   

binaryOp : baseBinaryOp opMods ;



