// Generated from /home/darrell/Projects/Primell/Antlr4Generation/PrimellParser.g4 by ANTLR 4.13.1
import org.antlr.v4.runtime.tree.ParseTreeListener;

/**
 * This interface defines a complete listener for a parse tree produced by
 * {@link PrimellParser}.
 */
public interface PrimellParserListener extends ParseTreeListener {
	/**
	 * Enter a parse tree produced by {@link PrimellParser#line}.
	 * @param ctx the parse tree
	 */
	void enterLine(PrimellParser.LineContext ctx);
	/**
	 * Exit a parse tree produced by {@link PrimellParser#line}.
	 * @param ctx the parse tree
	 */
	void exitLine(PrimellParser.LineContext ctx);
	/**
	 * Enter a parse tree produced by {@link PrimellParser#termSeq}.
	 * @param ctx the parse tree
	 */
	void enterTermSeq(PrimellParser.TermSeqContext ctx);
	/**
	 * Exit a parse tree produced by {@link PrimellParser#termSeq}.
	 * @param ctx the parse tree
	 */
	void exitTermSeq(PrimellParser.TermSeqContext ctx);
	/**
	 * Enter a parse tree produced by {@link PrimellParser#concatRtlTerm}.
	 * @param ctx the parse tree
	 */
	void enterConcatRtlTerm(PrimellParser.ConcatRtlTermContext ctx);
	/**
	 * Exit a parse tree produced by {@link PrimellParser#concatRtlTerm}.
	 * @param ctx the parse tree
	 */
	void exitConcatRtlTerm(PrimellParser.ConcatRtlTermContext ctx);
	/**
	 * Enter a parse tree produced by the {@code passThroughRtl}
	 * labeled alternative in {@link PrimellParser#rtlTerm}.
	 * @param ctx the parse tree
	 */
	void enterPassThroughRtl(PrimellParser.PassThroughRtlContext ctx);
	/**
	 * Exit a parse tree produced by the {@code passThroughRtl}
	 * labeled alternative in {@link PrimellParser#rtlTerm}.
	 * @param ctx the parse tree
	 */
	void exitPassThroughRtl(PrimellParser.PassThroughRtlContext ctx);
	/**
	 * Enter a parse tree produced by the {@code stdAssign}
	 * labeled alternative in {@link PrimellParser#rtlTerm}.
	 * @param ctx the parse tree
	 */
	void enterStdAssign(PrimellParser.StdAssignContext ctx);
	/**
	 * Exit a parse tree produced by the {@code stdAssign}
	 * labeled alternative in {@link PrimellParser#rtlTerm}.
	 * @param ctx the parse tree
	 */
	void exitStdAssign(PrimellParser.StdAssignContext ctx);
	/**
	 * Enter a parse tree produced by the {@code forEachRightAssign}
	 * labeled alternative in {@link PrimellParser#rtlTerm}.
	 * @param ctx the parse tree
	 */
	void enterForEachRightAssign(PrimellParser.ForEachRightAssignContext ctx);
	/**
	 * Exit a parse tree produced by the {@code forEachRightAssign}
	 * labeled alternative in {@link PrimellParser#rtlTerm}.
	 * @param ctx the parse tree
	 */
	void exitForEachRightAssign(PrimellParser.ForEachRightAssignContext ctx);
	/**
	 * Enter a parse tree produced by the {@code forEachLeftAssign}
	 * labeled alternative in {@link PrimellParser#rtlTerm}.
	 * @param ctx the parse tree
	 */
	void enterForEachLeftAssign(PrimellParser.ForEachLeftAssignContext ctx);
	/**
	 * Exit a parse tree produced by the {@code forEachLeftAssign}
	 * labeled alternative in {@link PrimellParser#rtlTerm}.
	 * @param ctx the parse tree
	 */
	void exitForEachLeftAssign(PrimellParser.ForEachLeftAssignContext ctx);
	/**
	 * Enter a parse tree produced by {@link PrimellParser#binaryAssign}.
	 * @param ctx the parse tree
	 */
	void enterBinaryAssign(PrimellParser.BinaryAssignContext ctx);
	/**
	 * Exit a parse tree produced by {@link PrimellParser#binaryAssign}.
	 * @param ctx the parse tree
	 */
	void exitBinaryAssign(PrimellParser.BinaryAssignContext ctx);
	/**
	 * Enter a parse tree produced by the {@code binaryOperation}
	 * labeled alternative in {@link PrimellParser#mulTerm}.
	 * @param ctx the parse tree
	 */
	void enterBinaryOperation(PrimellParser.BinaryOperationContext ctx);
	/**
	 * Exit a parse tree produced by the {@code binaryOperation}
	 * labeled alternative in {@link PrimellParser#mulTerm}.
	 * @param ctx the parse tree
	 */
	void exitBinaryOperation(PrimellParser.BinaryOperationContext ctx);
	/**
	 * Enter a parse tree produced by the {@code forEachLeftBinary}
	 * labeled alternative in {@link PrimellParser#mulTerm}.
	 * @param ctx the parse tree
	 */
	void enterForEachLeftBinary(PrimellParser.ForEachLeftBinaryContext ctx);
	/**
	 * Exit a parse tree produced by the {@code forEachLeftBinary}
	 * labeled alternative in {@link PrimellParser#mulTerm}.
	 * @param ctx the parse tree
	 */
	void exitForEachLeftBinary(PrimellParser.ForEachLeftBinaryContext ctx);
	/**
	 * Enter a parse tree produced by the {@code forEachChain}
	 * labeled alternative in {@link PrimellParser#mulTerm}.
	 * @param ctx the parse tree
	 */
	void enterForEachChain(PrimellParser.ForEachChainContext ctx);
	/**
	 * Exit a parse tree produced by the {@code forEachChain}
	 * labeled alternative in {@link PrimellParser#mulTerm}.
	 * @param ctx the parse tree
	 */
	void exitForEachChain(PrimellParser.ForEachChainContext ctx);
	/**
	 * Enter a parse tree produced by the {@code unaryOperation}
	 * labeled alternative in {@link PrimellParser#mulTerm}.
	 * @param ctx the parse tree
	 */
	void enterUnaryOperation(PrimellParser.UnaryOperationContext ctx);
	/**
	 * Exit a parse tree produced by the {@code unaryOperation}
	 * labeled alternative in {@link PrimellParser#mulTerm}.
	 * @param ctx the parse tree
	 */
	void exitUnaryOperation(PrimellParser.UnaryOperationContext ctx);
	/**
	 * Enter a parse tree produced by the {@code passThroughMulTerm}
	 * labeled alternative in {@link PrimellParser#mulTerm}.
	 * @param ctx the parse tree
	 */
	void enterPassThroughMulTerm(PrimellParser.PassThroughMulTermContext ctx);
	/**
	 * Exit a parse tree produced by the {@code passThroughMulTerm}
	 * labeled alternative in {@link PrimellParser#mulTerm}.
	 * @param ctx the parse tree
	 */
	void exitPassThroughMulTerm(PrimellParser.PassThroughMulTermContext ctx);
	/**
	 * Enter a parse tree produced by the {@code forEachUnary}
	 * labeled alternative in {@link PrimellParser#mulTerm}.
	 * @param ctx the parse tree
	 */
	void enterForEachUnary(PrimellParser.ForEachUnaryContext ctx);
	/**
	 * Exit a parse tree produced by the {@code forEachUnary}
	 * labeled alternative in {@link PrimellParser#mulTerm}.
	 * @param ctx the parse tree
	 */
	void exitForEachUnary(PrimellParser.ForEachUnaryContext ctx);
	/**
	 * Enter a parse tree produced by {@link PrimellParser#binaryOpWithRS}.
	 * @param ctx the parse tree
	 */
	void enterBinaryOpWithRS(PrimellParser.BinaryOpWithRSContext ctx);
	/**
	 * Exit a parse tree produced by {@link PrimellParser#binaryOpWithRS}.
	 * @param ctx the parse tree
	 */
	void exitBinaryOpWithRS(PrimellParser.BinaryOpWithRSContext ctx);
	/**
	 * Enter a parse tree produced by {@link PrimellParser#unaryOrBinaryOp}.
	 * @param ctx the parse tree
	 */
	void enterUnaryOrBinaryOp(PrimellParser.UnaryOrBinaryOpContext ctx);
	/**
	 * Exit a parse tree produced by {@link PrimellParser#unaryOrBinaryOp}.
	 * @param ctx the parse tree
	 */
	void exitUnaryOrBinaryOp(PrimellParser.UnaryOrBinaryOpContext ctx);
	/**
	 * Enter a parse tree produced by the {@code integerOrIdentifier}
	 * labeled alternative in {@link PrimellParser#atomTerm}.
	 * @param ctx the parse tree
	 */
	void enterIntegerOrIdentifier(PrimellParser.IntegerOrIdentifierContext ctx);
	/**
	 * Exit a parse tree produced by the {@code integerOrIdentifier}
	 * labeled alternative in {@link PrimellParser#atomTerm}.
	 * @param ctx the parse tree
	 */
	void exitIntegerOrIdentifier(PrimellParser.IntegerOrIdentifierContext ctx);
	/**
	 * Enter a parse tree produced by the {@code infinity}
	 * labeled alternative in {@link PrimellParser#atomTerm}.
	 * @param ctx the parse tree
	 */
	void enterInfinity(PrimellParser.InfinityContext ctx);
	/**
	 * Exit a parse tree produced by the {@code infinity}
	 * labeled alternative in {@link PrimellParser#atomTerm}.
	 * @param ctx the parse tree
	 */
	void exitInfinity(PrimellParser.InfinityContext ctx);
	/**
	 * Enter a parse tree produced by the {@code nullaryOperation}
	 * labeled alternative in {@link PrimellParser#atomTerm}.
	 * @param ctx the parse tree
	 */
	void enterNullaryOperation(PrimellParser.NullaryOperationContext ctx);
	/**
	 * Exit a parse tree produced by the {@code nullaryOperation}
	 * labeled alternative in {@link PrimellParser#atomTerm}.
	 * @param ctx the parse tree
	 */
	void exitNullaryOperation(PrimellParser.NullaryOperationContext ctx);
	/**
	 * Enter a parse tree produced by the {@code emptyList}
	 * labeled alternative in {@link PrimellParser#atomTerm}.
	 * @param ctx the parse tree
	 */
	void enterEmptyList(PrimellParser.EmptyListContext ctx);
	/**
	 * Exit a parse tree produced by the {@code emptyList}
	 * labeled alternative in {@link PrimellParser#atomTerm}.
	 * @param ctx the parse tree
	 */
	void exitEmptyList(PrimellParser.EmptyListContext ctx);
	/**
	 * Enter a parse tree produced by the {@code parens}
	 * labeled alternative in {@link PrimellParser#atomTerm}.
	 * @param ctx the parse tree
	 */
	void enterParens(PrimellParser.ParensContext ctx);
	/**
	 * Exit a parse tree produced by the {@code parens}
	 * labeled alternative in {@link PrimellParser#atomTerm}.
	 * @param ctx the parse tree
	 */
	void exitParens(PrimellParser.ParensContext ctx);
	/**
	 * Enter a parse tree produced by the {@code string}
	 * labeled alternative in {@link PrimellParser#atomTerm}.
	 * @param ctx the parse tree
	 */
	void enterString(PrimellParser.StringContext ctx);
	/**
	 * Exit a parse tree produced by the {@code string}
	 * labeled alternative in {@link PrimellParser#atomTerm}.
	 * @param ctx the parse tree
	 */
	void exitString(PrimellParser.StringContext ctx);
	/**
	 * Enter a parse tree produced by {@link PrimellParser#intOrId}.
	 * @param ctx the parse tree
	 */
	void enterIntOrId(PrimellParser.IntOrIdContext ctx);
	/**
	 * Exit a parse tree produced by {@link PrimellParser#intOrId}.
	 * @param ctx the parse tree
	 */
	void exitIntOrId(PrimellParser.IntOrIdContext ctx);
	/**
	 * Enter a parse tree produced by {@link PrimellParser#baseNullaryOp}.
	 * @param ctx the parse tree
	 */
	void enterBaseNullaryOp(PrimellParser.BaseNullaryOpContext ctx);
	/**
	 * Exit a parse tree produced by {@link PrimellParser#baseNullaryOp}.
	 * @param ctx the parse tree
	 */
	void exitBaseNullaryOp(PrimellParser.BaseNullaryOpContext ctx);
	/**
	 * Enter a parse tree produced by {@link PrimellParser#baseUnaryOp}.
	 * @param ctx the parse tree
	 */
	void enterBaseUnaryOp(PrimellParser.BaseUnaryOpContext ctx);
	/**
	 * Exit a parse tree produced by {@link PrimellParser#baseUnaryOp}.
	 * @param ctx the parse tree
	 */
	void exitBaseUnaryOp(PrimellParser.BaseUnaryOpContext ctx);
	/**
	 * Enter a parse tree produced by {@link PrimellParser#baseBinaryOp}.
	 * @param ctx the parse tree
	 */
	void enterBaseBinaryOp(PrimellParser.BaseBinaryOpContext ctx);
	/**
	 * Exit a parse tree produced by {@link PrimellParser#baseBinaryOp}.
	 * @param ctx the parse tree
	 */
	void exitBaseBinaryOp(PrimellParser.BaseBinaryOpContext ctx);
	/**
	 * Enter a parse tree produced by {@link PrimellParser#conditionalOp}.
	 * @param ctx the parse tree
	 */
	void enterConditionalOp(PrimellParser.ConditionalOpContext ctx);
	/**
	 * Exit a parse tree produced by {@link PrimellParser#conditionalOp}.
	 * @param ctx the parse tree
	 */
	void exitConditionalOp(PrimellParser.ConditionalOpContext ctx);
	/**
	 * Enter a parse tree produced by {@link PrimellParser#condFunc}.
	 * @param ctx the parse tree
	 */
	void enterCondFunc(PrimellParser.CondFuncContext ctx);
	/**
	 * Exit a parse tree produced by {@link PrimellParser#condFunc}.
	 * @param ctx the parse tree
	 */
	void exitCondFunc(PrimellParser.CondFuncContext ctx);
	/**
	 * Enter a parse tree produced by {@link PrimellParser#opMods}.
	 * @param ctx the parse tree
	 */
	void enterOpMods(PrimellParser.OpModsContext ctx);
	/**
	 * Exit a parse tree produced by {@link PrimellParser#opMods}.
	 * @param ctx the parse tree
	 */
	void exitOpMods(PrimellParser.OpModsContext ctx);
	/**
	 * Enter a parse tree produced by {@link PrimellParser#assignMods}.
	 * @param ctx the parse tree
	 */
	void enterAssignMods(PrimellParser.AssignModsContext ctx);
	/**
	 * Exit a parse tree produced by {@link PrimellParser#assignMods}.
	 * @param ctx the parse tree
	 */
	void exitAssignMods(PrimellParser.AssignModsContext ctx);
	/**
	 * Enter a parse tree produced by {@link PrimellParser#nullaryOp}.
	 * @param ctx the parse tree
	 */
	void enterNullaryOp(PrimellParser.NullaryOpContext ctx);
	/**
	 * Exit a parse tree produced by {@link PrimellParser#nullaryOp}.
	 * @param ctx the parse tree
	 */
	void exitNullaryOp(PrimellParser.NullaryOpContext ctx);
	/**
	 * Enter a parse tree produced by {@link PrimellParser#unaryOp}.
	 * @param ctx the parse tree
	 */
	void enterUnaryOp(PrimellParser.UnaryOpContext ctx);
	/**
	 * Exit a parse tree produced by {@link PrimellParser#unaryOp}.
	 * @param ctx the parse tree
	 */
	void exitUnaryOp(PrimellParser.UnaryOpContext ctx);
	/**
	 * Enter a parse tree produced by {@link PrimellParser#unaryAssign}.
	 * @param ctx the parse tree
	 */
	void enterUnaryAssign(PrimellParser.UnaryAssignContext ctx);
	/**
	 * Exit a parse tree produced by {@link PrimellParser#unaryAssign}.
	 * @param ctx the parse tree
	 */
	void exitUnaryAssign(PrimellParser.UnaryAssignContext ctx);
	/**
	 * Enter a parse tree produced by {@link PrimellParser#binaryOp}.
	 * @param ctx the parse tree
	 */
	void enterBinaryOp(PrimellParser.BinaryOpContext ctx);
	/**
	 * Exit a parse tree produced by {@link PrimellParser#binaryOp}.
	 * @param ctx the parse tree
	 */
	void exitBinaryOp(PrimellParser.BinaryOpContext ctx);
	/**
	 * Enter a parse tree produced by {@link PrimellParser#op_list_diff}.
	 * @param ctx the parse tree
	 */
	void enterOp_list_diff(PrimellParser.Op_list_diffContext ctx);
	/**
	 * Exit a parse tree produced by {@link PrimellParser#op_list_diff}.
	 * @param ctx the parse tree
	 */
	void exitOp_list_diff(PrimellParser.Op_list_diffContext ctx);
	/**
	 * Enter a parse tree produced by {@link PrimellParser#cond_mod_back_jump}.
	 * @param ctx the parse tree
	 */
	void enterCond_mod_back_jump(PrimellParser.Cond_mod_back_jumpContext ctx);
	/**
	 * Exit a parse tree produced by {@link PrimellParser#cond_mod_back_jump}.
	 * @param ctx the parse tree
	 */
	void exitCond_mod_back_jump(PrimellParser.Cond_mod_back_jumpContext ctx);
	/**
	 * Enter a parse tree produced by {@link PrimellParser#op_div}.
	 * @param ctx the parse tree
	 */
	void enterOp_div(PrimellParser.Op_divContext ctx);
	/**
	 * Exit a parse tree produced by {@link PrimellParser#op_div}.
	 * @param ctx the parse tree
	 */
	void exitOp_div(PrimellParser.Op_divContext ctx);
	/**
	 * Enter a parse tree produced by {@link PrimellParser#cond_mod_jump}.
	 * @param ctx the parse tree
	 */
	void enterCond_mod_jump(PrimellParser.Cond_mod_jumpContext ctx);
	/**
	 * Exit a parse tree produced by {@link PrimellParser#cond_mod_jump}.
	 * @param ctx the parse tree
	 */
	void exitCond_mod_jump(PrimellParser.Cond_mod_jumpContext ctx);
	/**
	 * Enter a parse tree produced by {@link PrimellParser#op_max}.
	 * @param ctx the parse tree
	 */
	void enterOp_max(PrimellParser.Op_maxContext ctx);
	/**
	 * Exit a parse tree produced by {@link PrimellParser#op_max}.
	 * @param ctx the parse tree
	 */
	void exitOp_max(PrimellParser.Op_maxContext ctx);
	/**
	 * Enter a parse tree produced by {@link PrimellParser#cond_mod_tail}.
	 * @param ctx the parse tree
	 */
	void enterCond_mod_tail(PrimellParser.Cond_mod_tailContext ctx);
	/**
	 * Exit a parse tree produced by {@link PrimellParser#cond_mod_tail}.
	 * @param ctx the parse tree
	 */
	void exitCond_mod_tail(PrimellParser.Cond_mod_tailContext ctx);
	/**
	 * Enter a parse tree produced by {@link PrimellParser#op_mul}.
	 * @param ctx the parse tree
	 */
	void enterOp_mul(PrimellParser.Op_mulContext ctx);
	/**
	 * Exit a parse tree produced by {@link PrimellParser#op_mul}.
	 * @param ctx the parse tree
	 */
	void exitOp_mul(PrimellParser.Op_mulContext ctx);
	/**
	 * Enter a parse tree produced by {@link PrimellParser#cond_mod_while}.
	 * @param ctx the parse tree
	 */
	void enterCond_mod_while(PrimellParser.Cond_mod_whileContext ctx);
	/**
	 * Exit a parse tree produced by {@link PrimellParser#cond_mod_while}.
	 * @param ctx the parse tree
	 */
	void exitCond_mod_while(PrimellParser.Cond_mod_whileContext ctx);
	/**
	 * Enter a parse tree produced by {@link PrimellParser#op_add}.
	 * @param ctx the parse tree
	 */
	void enterOp_add(PrimellParser.Op_addContext ctx);
	/**
	 * Exit a parse tree produced by {@link PrimellParser#op_add}.
	 * @param ctx the parse tree
	 */
	void exitOp_add(PrimellParser.Op_addContext ctx);
	/**
	 * Enter a parse tree produced by {@link PrimellParser#cond_mod_do_while}.
	 * @param ctx the parse tree
	 */
	void enterCond_mod_do_while(PrimellParser.Cond_mod_do_whileContext ctx);
	/**
	 * Exit a parse tree produced by {@link PrimellParser#cond_mod_do_while}.
	 * @param ctx the parse tree
	 */
	void exitCond_mod_do_while(PrimellParser.Cond_mod_do_whileContext ctx);
	/**
	 * Enter a parse tree produced by {@link PrimellParser#op_neg}.
	 * @param ctx the parse tree
	 */
	void enterOp_neg(PrimellParser.Op_negContext ctx);
	/**
	 * Exit a parse tree produced by {@link PrimellParser#op_neg}.
	 * @param ctx the parse tree
	 */
	void exitOp_neg(PrimellParser.Op_negContext ctx);
	/**
	 * Enter a parse tree produced by {@link PrimellParser#cond_mod_neg}.
	 * @param ctx the parse tree
	 */
	void enterCond_mod_neg(PrimellParser.Cond_mod_negContext ctx);
	/**
	 * Exit a parse tree produced by {@link PrimellParser#cond_mod_neg}.
	 * @param ctx the parse tree
	 */
	void exitCond_mod_neg(PrimellParser.Cond_mod_negContext ctx);
}