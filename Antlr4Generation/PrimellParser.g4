parser grammar PrimellParser;

options {
  tokenVocab = PrimellLexer;
}

// TODO : at some point learn how to prevent whitespace in certain contexts

line : termSeq outputSpec? COMMENT? EOF;

outputSpec : OUT_INV | OUT_DEF | OUT_STR ;

termSeq : concatRtlTerm+ ;

concatRtlTerm : CONCAT? rtlTerm ;

rtlTerm : mulTerm                                                                   #passThroughRtl
        | mulTerm binaryAssign (rtlTerm | RTL termSeq)                              #stdAssign
        | mulTerm binaryAssign L_BRACK termSeq R_BRACK                   #forEachRightAssign
        | L_BRACK termSeq R_BRACK binaryAssign (rtlTerm | RTL termSeq)   #forEachLeftAssign
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

atomTerm : intOrId                    #integerOrIdentifier   // the silly one
         | INFINITY                   #infinity
         | nullaryOp                  #nullaryOperation
         | L_PAREN R_PAREN            #emptyList
         | L_BRACK R_BRACK            #emptyList  // i don't think it ever matters which
         | L_PAREN termSeq R_PAREN    #parens
         ;

intOrId : INT_OR_ID (DOT INT_OR_ID)* ;

// Note; numeric/list op distinction isn't syntactical, it's functional!
baseNullaryOp : OP_NULLARY
              ;

baseUnaryOp : OP_UNARY 
            | OP_USER_UNARY 
            | op_neg
            ;
            
baseBinaryOp : OP_BINARY 
             | OP_USER_BINARY 
             | op_add | op_mul | op_div | op_max | op_list_diff   // re-used symbols
             | conditionalOp
             ;

conditionalOp : OP_COND condFunc? cond_mod_neg? cond_mod_tail? ;

condFunc : cond_mod_jump | cond_mod_back_jump | cond_mod_while | cond_mod_do_while ;

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