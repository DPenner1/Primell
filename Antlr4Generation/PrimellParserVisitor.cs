//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.13.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from /home/darrell/Projects/Prime/Antlr4Generation/PrimellParser.g4 by ANTLR 4.13.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete generic visitor for a parse tree produced
/// by <see cref="PrimellParser"/>.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.13.1")]
[System.CLSCompliant(false)]
public interface IPrimellParserVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by <see cref="PrimellParser.line"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitLine([NotNull] PrimellParser.LineContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PrimellParser.outputSpec"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitOutputSpec([NotNull] PrimellParser.OutputSpecContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PrimellParser.termSeq"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTermSeq([NotNull] PrimellParser.TermSeqContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>listUnaryOperation</c>
	/// labeled alternative in <see cref="PrimellParser.mulTerm"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitListUnaryOperation([NotNull] PrimellParser.ListUnaryOperationContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>forEachLeftTerm</c>
	/// labeled alternative in <see cref="PrimellParser.mulTerm"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitForEachLeftTerm([NotNull] PrimellParser.ForEachLeftTermContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>forEachRightTerm</c>
	/// labeled alternative in <see cref="PrimellParser.mulTerm"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitForEachRightTerm([NotNull] PrimellParser.ForEachRightTermContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>binaryOperation</c>
	/// labeled alternative in <see cref="PrimellParser.mulTerm"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBinaryOperation([NotNull] PrimellParser.BinaryOperationContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>atom</c>
	/// labeled alternative in <see cref="PrimellParser.mulTerm"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAtom([NotNull] PrimellParser.AtomContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>numericUnaryOperation</c>
	/// labeled alternative in <see cref="PrimellParser.mulTerm"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNumericUnaryOperation([NotNull] PrimellParser.NumericUnaryOperationContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>integer</c>
	/// labeled alternative in <see cref="PrimellParser.atomTerm"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitInteger([NotNull] PrimellParser.IntegerContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>infinity</c>
	/// labeled alternative in <see cref="PrimellParser.atomTerm"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitInfinity([NotNull] PrimellParser.InfinityContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>nullaryOperation</c>
	/// labeled alternative in <see cref="PrimellParser.atomTerm"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNullaryOperation([NotNull] PrimellParser.NullaryOperationContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>emptyList</c>
	/// labeled alternative in <see cref="PrimellParser.atomTerm"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitEmptyList([NotNull] PrimellParser.EmptyListContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>parens</c>
	/// labeled alternative in <see cref="PrimellParser.atomTerm"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitParens([NotNull] PrimellParser.ParensContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PrimellParser.forEachBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitForEachBlock([NotNull] PrimellParser.ForEachBlockContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>forEachBinary</c>
	/// labeled alternative in <see cref="PrimellParser.forEachOperation"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitForEachBinary([NotNull] PrimellParser.ForEachBinaryContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>forEachNumericUnary</c>
	/// labeled alternative in <see cref="PrimellParser.forEachOperation"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitForEachNumericUnary([NotNull] PrimellParser.ForEachNumericUnaryContext context);
	/// <summary>
	/// Visit a parse tree produced by the <c>forEachListUnary</c>
	/// labeled alternative in <see cref="PrimellParser.forEachOperation"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitForEachListUnary([NotNull] PrimellParser.ForEachListUnaryContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PrimellParser.baseNullaryOp"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBaseNullaryOp([NotNull] PrimellParser.BaseNullaryOpContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PrimellParser.baseNumUnaryOp"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBaseNumUnaryOp([NotNull] PrimellParser.BaseNumUnaryOpContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PrimellParser.baseNumBinaryOp"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBaseNumBinaryOp([NotNull] PrimellParser.BaseNumBinaryOpContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PrimellParser.baseListUnaryOp"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBaseListUnaryOp([NotNull] PrimellParser.BaseListUnaryOpContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PrimellParser.baseListBinaryOp"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBaseListBinaryOp([NotNull] PrimellParser.BaseListBinaryOpContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PrimellParser.opMods"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitOpMods([NotNull] PrimellParser.OpModsContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PrimellParser.assignMods"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAssignMods([NotNull] PrimellParser.AssignModsContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PrimellParser.nullaryOp"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNullaryOp([NotNull] PrimellParser.NullaryOpContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PrimellParser.numUnaryOp"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNumUnaryOp([NotNull] PrimellParser.NumUnaryOpContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PrimellParser.listUnaryOp"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitListUnaryOp([NotNull] PrimellParser.ListUnaryOpContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="PrimellParser.binaryOp"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitBinaryOp([NotNull] PrimellParser.BinaryOpContext context);
}
