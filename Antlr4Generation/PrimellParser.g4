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
        | mulTerm binaryAssign L_BRACK termSeq R_BRACK                   #forEachRightAssign
        | LBRACE termSeq RBRACE binaryAssign (rtlTerm | RTL termSeq)   #forEachLeftAssign
        ; // TODO : rtl assign stuff hasn't been updated with the latest mulTerm mirrors

binaryAssign : ASSIGN assignMods binaryOp? ;

mulTerm : atomTerm                                          #passThroughMulTerm
        | mulTerm unaryOp                                   #unaryOperation
        | mulTerm binaryOpWithRS                            #binaryOperation
        | L_BRACK termSeq R_BRACK unaryOp                     #forEachUnary
        | L_BRACK termSeq R_BRACK binaryOpWithRS              #forEachLeftBinary
        | L_BRACK termSeq VERT_BAR unaryOrBinaryOp+ R_BRACK   #forEachChain
        ;          

binaryOpWithRS : binaryOp atomTerm
               | binaryOp RTL termSeq
               | L_BRACK binaryOp termSeq R_BRACK
               ;

unaryOrBinaryOp : unaryOp | binaryOpWithRS ;

atomTerm : INT                      #integer
         | INFINITY                 #infinity
         | nullaryOp                #nullaryOperation
         | L_PAREN R_PAREN            #emptyList
         | L_BRACK R_BRACK            #emptyList  // i don't think it ever matters which
         | L_PAREN termSeq R_PAREN    #parens
         ;

// Note; numeric/list op distinction isn't syntactical, it's functional!
baseNullaryOp : IDENTIFIER | OP_READ_STR | OP_READ_CSV
              ;

baseUnaryOp : OP_GAMMA | OP_NEXT | OP_PREV | OP_ROUND | op_neg | OP_BIT_NOT   // numeric unary
            | OP_HEAD | OP_TAIL | OP_DISTINCT | OP_REV | OP_FLATTEN | OP_SORT | OP_READ_CODE   // list unary
            ;
            
baseBinaryOp : op_add | OP_SUB | op_mul | op_div | OP_MOD | OP_POW | OP_LOG   // num binary (math)
             | OP_BIT_AND | OP_BIT_OR | OP_BIT_XOR                            // num binary (bitwise)
             | OP_INC_RANGE | OP_RANGE | OP_MIN | op_max                      // num binary (misc)
             | op_list_diff | OP_INTERSECT | OP_INDEX_OF | OP_CONCAT          // list binary
             | OP_INDEX | OP_APPEND                                           // list numeric
             | OP_CONS                                                        // numeric list
             | conditionalOp   // separate for easier handling
             ;

conditionalOp : OP_COND condMods ;

condMods : cond_mod_neg? condFuncMod? cond_mod_tail? ;

condFuncMod : cond_mod_jump | cond_mod_back_jump | cond_mod_while | cond_mod_do_while ;

opMods : (OPMOD_CUT | OPMOD_POW)? ;

assignMods : (OPMOD_CUT | OPMOD_POW)? ;

nullaryOp : baseNullaryOp opMods ;

unaryOp : unaryAssign? baseUnaryOp opMods ;

unaryAssign : ASSIGN assignMods ;   

binaryOp : baseBinaryOp opMods ;


// a few symbols get used in different contexts so need to be distinguished by parser

op_list_diff : B_SLASH ;
cond_mod_back_jump : B_SLASH ;

op_div : F_SLASH ;
cond_mod_jump : F_SLASH ;

op_max : TAIL ;
cond_mod_tail : TAIL ;

op_mul :  STAR ;
cond_mod_while : STAR ;

op_add : PLUS ;
cond_mod_do_while : PLUS ;

op_neg : NEGATE ;
cond_mod_neg : NEGATE ;