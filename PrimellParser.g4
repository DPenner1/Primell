parser grammar PrimellParser;

options {
  tokenVocab = PrimellLexer;
}

// TODO : at some point learn how to prevent whitespace in certain contexts

line : termSeq COMMENT? EOF
     ;

termSeq : concatRtlTerm+ ;

concatRtlTerm : CONCAT? rtlTerm ;

rtlTerm : mulTerm                                                                   #passThroughRtl
        | mulTerm binaryAssign (rtlTerm | RTL termSeq)                              #stdAssign
        | mulTerm binaryAssign L_BRACK termSeq R_BRACK                   #forEachRightAssign
        | L_BRACK termSeq R_BRACK binaryAssign (rtlTerm | RTL termSeq)   #forEachLeftAssign
        ; // TODO : rtl assign stuff hasn't been updated with the latest mulTerm mirrors

binaryAssign : ASSIGN assignMods binaryOp? ;

mulTerm : atomTerm                                                  #passThroughMulTerm
        | mulTerm unaryOp                                           #unaryOperation
        | mulTerm binaryOpWithRS                                    #binaryOperation
        | L_BRACK termSeq R_BRACK unaryOp                           #forEachUnary
        | L_BRACK termSeq R_BRACK binaryOpWithRS                    #forEachLeftBinary
        | L_BRACK termSeq VERT_BAR unaryOrBinaryOp+ R_BRACK opMods  #forEachChain 
        ;     // not sure I want opMods here, but I want to know if i accidentally break this

binaryOpWithRS : binaryOp atomTerm
               | binaryOp RTL termSeq
               | L_BRACK binaryOp termSeq R_BRACK
               ;

unaryOrBinaryOp : unaryOp | binaryOpWithRS ;

atomTerm : intOrId                    #integerOrIdentifier   // the silly one
         | INFINITY                   #infinity
         | nullaryOp                  #nullaryOperation
         | L_PAREN R_PAREN            #emptyList
         | L_BRACK R_BRACK            #emptyList  // i don't think it ever matters which empty list
         | L_PAREN termSeq R_PAREN    #parens
         | STRING                     #string
         ;

intOrId : INT_OR_ID (DOT INT_OR_ID)* ;

// Note; numeric/list op distinction isn't syntactical, it's functional!
baseNullaryOp : OP_NULLARY
              ;

baseUnaryOp : OP_UNARY 
            | OP_USER_UNARY 
            | op_neg | op_branch_a | op_branch_f | op_branch_b   // re-used symbols
            ;
            
baseBinaryOp : OP_BINARY 
             | OP_USER_BINARY 
             | op_add | op_mul | op_div | op_max | op_list_diff   // re-used symbols
             | conditionalOp
             ;

conditionalOp : OP_COND condBranch? condLoop? cond_mod_neg? cond_mod_tail? ;

condBranch : cond_branch_f | cond_branch_b ;

condLoop : cond_loop_while | cond_loop_do_while ;

opMods : (OPMOD_CUT | OPMOD_POW)? ;

assignMods : (OPMOD_CUT | OPMOD_POW)? ;

nullaryOp : baseNullaryOp opMods ;

unaryOp : unaryAssign? baseUnaryOp opMods ;

unaryAssign : ASSIGN assignMods ;   

binaryOp : baseBinaryOp opMods ;


// a few symbols get used (or probably will) in different contexts so need to be distinguished by parser (or lexer mode, but that seemed tougher)
// (I've used underscores instead of camelCase for these parser rules that are basically just tokens)

op_list_diff : B_SLASH ;

op_div : F_SLASH ;

op_branch_b : BRANCH_B ;
cond_branch_b : BRANCH_B ;

op_branch_f : BRANCH_F ;
cond_branch_f : BRANCH_F ;

op_branch_a : BRANCH_A ;
cond_branch_a : BRANCH_A ;

op_max : TAIL ;
cond_mod_tail : TAIL ;

op_mul :  STAR ;
cond_loop_while : STAR ;

op_add : PLUS ;
cond_loop_do_while : PLUS ;

op_neg : NEGATE ;
cond_mod_neg : NEGATE ;