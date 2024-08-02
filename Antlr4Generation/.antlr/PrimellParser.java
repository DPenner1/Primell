// Generated from /home/darrell/Projects/Primell/Antlr4Generation/PrimellParser.g4 by ANTLR 4.13.1
import org.antlr.v4.runtime.atn.*;
import org.antlr.v4.runtime.dfa.DFA;
import org.antlr.v4.runtime.*;
import org.antlr.v4.runtime.misc.*;
import org.antlr.v4.runtime.tree.*;
import java.util.List;
import java.util.Iterator;
import java.util.ArrayList;

@SuppressWarnings({"all", "warnings", "unchecked", "unused", "cast", "CheckReturnValue"})
public class PrimellParser extends Parser {
	static { RuntimeMetaData.checkVersion("4.13.1", RuntimeMetaData.VERSION); }

	protected static final DFA[] _decisionToDFA;
	protected static final PredictionContextCache _sharedContextCache =
		new PredictionContextCache();
	public static final int
		INFINITY=1, RTL=2, LTR=3, CONCAT=4, ASSIGN=5, L_BRACK=6, R_BRACK=7, L_PAREN=8, 
		R_PAREN=9, VERT_BAR=10, DOT=11, TAIL=12, PLUS=13, STAR=14, NEGATE=15, 
		F_SLASH=16, B_SLASH=17, D_QUOTE=18, OPMOD_POW=19, OPMOD_CUT=20, OP_NULLARY=21, 
		OP_UNARY=22, OP_USER_UNARY=23, OP_BINARY=24, OP_USER_BINARY=25, OP_COND=26, 
		WS=27, INT_OR_ID=28, COMMENT=29, STRING=30, InStr_D_QUOTE=31;
	public static final int
		RULE_line = 0, RULE_termSeq = 1, RULE_concatRtlTerm = 2, RULE_rtlTerm = 3, 
		RULE_binaryAssign = 4, RULE_mulTerm = 5, RULE_binaryOpWithRS = 6, RULE_unaryOrBinaryOp = 7, 
		RULE_atomTerm = 8, RULE_intOrId = 9, RULE_baseNullaryOp = 10, RULE_baseUnaryOp = 11, 
		RULE_baseBinaryOp = 12, RULE_conditionalOp = 13, RULE_condFunc = 14, RULE_opMods = 15, 
		RULE_assignMods = 16, RULE_nullaryOp = 17, RULE_unaryOp = 18, RULE_unaryAssign = 19, 
		RULE_binaryOp = 20, RULE_op_list_diff = 21, RULE_cond_mod_back_jump = 22, 
		RULE_op_div = 23, RULE_cond_mod_jump = 24, RULE_op_max = 25, RULE_cond_mod_tail = 26, 
		RULE_op_mul = 27, RULE_cond_mod_while = 28, RULE_op_add = 29, RULE_cond_mod_do_while = 30, 
		RULE_op_neg = 31, RULE_cond_mod_neg = 32;
	private static String[] makeRuleNames() {
		return new String[] {
			"line", "termSeq", "concatRtlTerm", "rtlTerm", "binaryAssign", "mulTerm", 
			"binaryOpWithRS", "unaryOrBinaryOp", "atomTerm", "intOrId", "baseNullaryOp", 
			"baseUnaryOp", "baseBinaryOp", "conditionalOp", "condFunc", "opMods", 
			"assignMods", "nullaryOp", "unaryOp", "unaryAssign", "binaryOp", "op_list_diff", 
			"cond_mod_back_jump", "op_div", "cond_mod_jump", "op_max", "cond_mod_tail", 
			"op_mul", "cond_mod_while", "op_add", "cond_mod_do_while", "op_neg", 
			"cond_mod_neg"
		};
	}
	public static final String[] ruleNames = makeRuleNames();

	private static String[] makeLiteralNames() {
		return new String[] {
			null, "'\\u221E'", "'$'", "'\\u20AC'", "';'", "'='", "'['", "']'", "'('", 
			"')'", "'|'", "'.'", "'>'", "'+'", "'*'", "'~'", "'/'", "'\\'", null, 
			"'^'", "'`'", null, null, null, null, null, "'?'"
		};
	}
	private static final String[] _LITERAL_NAMES = makeLiteralNames();
	private static String[] makeSymbolicNames() {
		return new String[] {
			null, "INFINITY", "RTL", "LTR", "CONCAT", "ASSIGN", "L_BRACK", "R_BRACK", 
			"L_PAREN", "R_PAREN", "VERT_BAR", "DOT", "TAIL", "PLUS", "STAR", "NEGATE", 
			"F_SLASH", "B_SLASH", "D_QUOTE", "OPMOD_POW", "OPMOD_CUT", "OP_NULLARY", 
			"OP_UNARY", "OP_USER_UNARY", "OP_BINARY", "OP_USER_BINARY", "OP_COND", 
			"WS", "INT_OR_ID", "COMMENT", "STRING", "InStr_D_QUOTE"
		};
	}
	private static final String[] _SYMBOLIC_NAMES = makeSymbolicNames();
	public static final Vocabulary VOCABULARY = new VocabularyImpl(_LITERAL_NAMES, _SYMBOLIC_NAMES);

	/**
	 * @deprecated Use {@link #VOCABULARY} instead.
	 */
	@Deprecated
	public static final String[] tokenNames;
	static {
		tokenNames = new String[_SYMBOLIC_NAMES.length];
		for (int i = 0; i < tokenNames.length; i++) {
			tokenNames[i] = VOCABULARY.getLiteralName(i);
			if (tokenNames[i] == null) {
				tokenNames[i] = VOCABULARY.getSymbolicName(i);
			}

			if (tokenNames[i] == null) {
				tokenNames[i] = "<INVALID>";
			}
		}
	}

	@Override
	@Deprecated
	public String[] getTokenNames() {
		return tokenNames;
	}

	@Override

	public Vocabulary getVocabulary() {
		return VOCABULARY;
	}

	@Override
	public String getGrammarFileName() { return "PrimellParser.g4"; }

	@Override
	public String[] getRuleNames() { return ruleNames; }

	@Override
	public String getSerializedATN() { return _serializedATN; }

	@Override
	public ATN getATN() { return _ATN; }

	public PrimellParser(TokenStream input) {
		super(input);
		_interp = new ParserATNSimulator(this,_ATN,_decisionToDFA,_sharedContextCache);
	}

	@SuppressWarnings("CheckReturnValue")
	public static class LineContext extends ParserRuleContext {
		public TermSeqContext termSeq() {
			return getRuleContext(TermSeqContext.class,0);
		}
		public TerminalNode EOF() { return getToken(PrimellParser.EOF, 0); }
		public TerminalNode COMMENT() { return getToken(PrimellParser.COMMENT, 0); }
		public LineContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_line; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterLine(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitLine(this);
		}
	}

	public final LineContext line() throws RecognitionException {
		LineContext _localctx = new LineContext(_ctx, getState());
		enterRule(_localctx, 0, RULE_line);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(66);
			termSeq();
			setState(68);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==COMMENT) {
				{
				setState(67);
				match(COMMENT);
				}
			}

			setState(70);
			match(EOF);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class TermSeqContext extends ParserRuleContext {
		public List<ConcatRtlTermContext> concatRtlTerm() {
			return getRuleContexts(ConcatRtlTermContext.class);
		}
		public ConcatRtlTermContext concatRtlTerm(int i) {
			return getRuleContext(ConcatRtlTermContext.class,i);
		}
		public TermSeqContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_termSeq; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterTermSeq(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitTermSeq(this);
		}
	}

	public final TermSeqContext termSeq() throws RecognitionException {
		TermSeqContext _localctx = new TermSeqContext(_ctx, getState());
		enterRule(_localctx, 2, RULE_termSeq);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(73); 
			_errHandler.sync(this);
			_alt = 1;
			do {
				switch (_alt) {
				case 1:
					{
					{
					setState(72);
					concatRtlTerm();
					}
					}
					break;
				default:
					throw new NoViableAltException(this);
				}
				setState(75); 
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,1,_ctx);
			} while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER );
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class ConcatRtlTermContext extends ParserRuleContext {
		public RtlTermContext rtlTerm() {
			return getRuleContext(RtlTermContext.class,0);
		}
		public TerminalNode CONCAT() { return getToken(PrimellParser.CONCAT, 0); }
		public ConcatRtlTermContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_concatRtlTerm; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterConcatRtlTerm(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitConcatRtlTerm(this);
		}
	}

	public final ConcatRtlTermContext concatRtlTerm() throws RecognitionException {
		ConcatRtlTermContext _localctx = new ConcatRtlTermContext(_ctx, getState());
		enterRule(_localctx, 4, RULE_concatRtlTerm);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(78);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==CONCAT) {
				{
				setState(77);
				match(CONCAT);
				}
			}

			setState(80);
			rtlTerm();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class RtlTermContext extends ParserRuleContext {
		public RtlTermContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_rtlTerm; }
	 
		public RtlTermContext() { }
		public void copyFrom(RtlTermContext ctx) {
			super.copyFrom(ctx);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class StdAssignContext extends RtlTermContext {
		public MulTermContext mulTerm() {
			return getRuleContext(MulTermContext.class,0);
		}
		public BinaryAssignContext binaryAssign() {
			return getRuleContext(BinaryAssignContext.class,0);
		}
		public RtlTermContext rtlTerm() {
			return getRuleContext(RtlTermContext.class,0);
		}
		public TerminalNode RTL() { return getToken(PrimellParser.RTL, 0); }
		public TermSeqContext termSeq() {
			return getRuleContext(TermSeqContext.class,0);
		}
		public StdAssignContext(RtlTermContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterStdAssign(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitStdAssign(this);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class ForEachLeftAssignContext extends RtlTermContext {
		public TerminalNode L_BRACK() { return getToken(PrimellParser.L_BRACK, 0); }
		public List<TermSeqContext> termSeq() {
			return getRuleContexts(TermSeqContext.class);
		}
		public TermSeqContext termSeq(int i) {
			return getRuleContext(TermSeqContext.class,i);
		}
		public TerminalNode R_BRACK() { return getToken(PrimellParser.R_BRACK, 0); }
		public BinaryAssignContext binaryAssign() {
			return getRuleContext(BinaryAssignContext.class,0);
		}
		public RtlTermContext rtlTerm() {
			return getRuleContext(RtlTermContext.class,0);
		}
		public TerminalNode RTL() { return getToken(PrimellParser.RTL, 0); }
		public ForEachLeftAssignContext(RtlTermContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterForEachLeftAssign(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitForEachLeftAssign(this);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class ForEachRightAssignContext extends RtlTermContext {
		public MulTermContext mulTerm() {
			return getRuleContext(MulTermContext.class,0);
		}
		public BinaryAssignContext binaryAssign() {
			return getRuleContext(BinaryAssignContext.class,0);
		}
		public TerminalNode L_BRACK() { return getToken(PrimellParser.L_BRACK, 0); }
		public TermSeqContext termSeq() {
			return getRuleContext(TermSeqContext.class,0);
		}
		public TerminalNode R_BRACK() { return getToken(PrimellParser.R_BRACK, 0); }
		public ForEachRightAssignContext(RtlTermContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterForEachRightAssign(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitForEachRightAssign(this);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class PassThroughRtlContext extends RtlTermContext {
		public MulTermContext mulTerm() {
			return getRuleContext(MulTermContext.class,0);
		}
		public PassThroughRtlContext(RtlTermContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterPassThroughRtl(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitPassThroughRtl(this);
		}
	}

	public final RtlTermContext rtlTerm() throws RecognitionException {
		RtlTermContext _localctx = new RtlTermContext(_ctx, getState());
		enterRule(_localctx, 6, RULE_rtlTerm);
		try {
			setState(105);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,5,_ctx) ) {
			case 1:
				_localctx = new PassThroughRtlContext(_localctx);
				enterOuterAlt(_localctx, 1);
				{
				setState(82);
				mulTerm(0);
				}
				break;
			case 2:
				_localctx = new StdAssignContext(_localctx);
				enterOuterAlt(_localctx, 2);
				{
				setState(83);
				mulTerm(0);
				setState(84);
				binaryAssign();
				setState(88);
				_errHandler.sync(this);
				switch (_input.LA(1)) {
				case INFINITY:
				case L_BRACK:
				case L_PAREN:
				case OP_NULLARY:
				case INT_OR_ID:
				case STRING:
					{
					setState(85);
					rtlTerm();
					}
					break;
				case RTL:
					{
					setState(86);
					match(RTL);
					setState(87);
					termSeq();
					}
					break;
				default:
					throw new NoViableAltException(this);
				}
				}
				break;
			case 3:
				_localctx = new ForEachRightAssignContext(_localctx);
				enterOuterAlt(_localctx, 3);
				{
				setState(90);
				mulTerm(0);
				setState(91);
				binaryAssign();
				setState(92);
				match(L_BRACK);
				setState(93);
				termSeq();
				setState(94);
				match(R_BRACK);
				}
				break;
			case 4:
				_localctx = new ForEachLeftAssignContext(_localctx);
				enterOuterAlt(_localctx, 4);
				{
				setState(96);
				match(L_BRACK);
				setState(97);
				termSeq();
				setState(98);
				match(R_BRACK);
				setState(99);
				binaryAssign();
				setState(103);
				_errHandler.sync(this);
				switch (_input.LA(1)) {
				case INFINITY:
				case L_BRACK:
				case L_PAREN:
				case OP_NULLARY:
				case INT_OR_ID:
				case STRING:
					{
					setState(100);
					rtlTerm();
					}
					break;
				case RTL:
					{
					setState(101);
					match(RTL);
					setState(102);
					termSeq();
					}
					break;
				default:
					throw new NoViableAltException(this);
				}
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class BinaryAssignContext extends ParserRuleContext {
		public TerminalNode ASSIGN() { return getToken(PrimellParser.ASSIGN, 0); }
		public AssignModsContext assignMods() {
			return getRuleContext(AssignModsContext.class,0);
		}
		public BinaryOpContext binaryOp() {
			return getRuleContext(BinaryOpContext.class,0);
		}
		public BinaryAssignContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_binaryAssign; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterBinaryAssign(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitBinaryAssign(this);
		}
	}

	public final BinaryAssignContext binaryAssign() throws RecognitionException {
		BinaryAssignContext _localctx = new BinaryAssignContext(_ctx, getState());
		enterRule(_localctx, 8, RULE_binaryAssign);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(107);
			match(ASSIGN);
			setState(108);
			assignMods();
			setState(110);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & 117665792L) != 0)) {
				{
				setState(109);
				binaryOp();
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class MulTermContext extends ParserRuleContext {
		public MulTermContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_mulTerm; }
	 
		public MulTermContext() { }
		public void copyFrom(MulTermContext ctx) {
			super.copyFrom(ctx);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class BinaryOperationContext extends MulTermContext {
		public MulTermContext mulTerm() {
			return getRuleContext(MulTermContext.class,0);
		}
		public BinaryOpWithRSContext binaryOpWithRS() {
			return getRuleContext(BinaryOpWithRSContext.class,0);
		}
		public BinaryOperationContext(MulTermContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterBinaryOperation(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitBinaryOperation(this);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class ForEachLeftBinaryContext extends MulTermContext {
		public TerminalNode L_BRACK() { return getToken(PrimellParser.L_BRACK, 0); }
		public TermSeqContext termSeq() {
			return getRuleContext(TermSeqContext.class,0);
		}
		public TerminalNode R_BRACK() { return getToken(PrimellParser.R_BRACK, 0); }
		public BinaryOpWithRSContext binaryOpWithRS() {
			return getRuleContext(BinaryOpWithRSContext.class,0);
		}
		public ForEachLeftBinaryContext(MulTermContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterForEachLeftBinary(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitForEachLeftBinary(this);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class ForEachChainContext extends MulTermContext {
		public TerminalNode L_BRACK() { return getToken(PrimellParser.L_BRACK, 0); }
		public TermSeqContext termSeq() {
			return getRuleContext(TermSeqContext.class,0);
		}
		public TerminalNode VERT_BAR() { return getToken(PrimellParser.VERT_BAR, 0); }
		public TerminalNode R_BRACK() { return getToken(PrimellParser.R_BRACK, 0); }
		public List<UnaryOrBinaryOpContext> unaryOrBinaryOp() {
			return getRuleContexts(UnaryOrBinaryOpContext.class);
		}
		public UnaryOrBinaryOpContext unaryOrBinaryOp(int i) {
			return getRuleContext(UnaryOrBinaryOpContext.class,i);
		}
		public ForEachChainContext(MulTermContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterForEachChain(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitForEachChain(this);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class UnaryOperationContext extends MulTermContext {
		public MulTermContext mulTerm() {
			return getRuleContext(MulTermContext.class,0);
		}
		public UnaryOpContext unaryOp() {
			return getRuleContext(UnaryOpContext.class,0);
		}
		public UnaryOperationContext(MulTermContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterUnaryOperation(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitUnaryOperation(this);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class PassThroughMulTermContext extends MulTermContext {
		public AtomTermContext atomTerm() {
			return getRuleContext(AtomTermContext.class,0);
		}
		public PassThroughMulTermContext(MulTermContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterPassThroughMulTerm(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitPassThroughMulTerm(this);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class ForEachUnaryContext extends MulTermContext {
		public TerminalNode L_BRACK() { return getToken(PrimellParser.L_BRACK, 0); }
		public TermSeqContext termSeq() {
			return getRuleContext(TermSeqContext.class,0);
		}
		public TerminalNode R_BRACK() { return getToken(PrimellParser.R_BRACK, 0); }
		public UnaryOpContext unaryOp() {
			return getRuleContext(UnaryOpContext.class,0);
		}
		public ForEachUnaryContext(MulTermContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterForEachUnary(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitForEachUnary(this);
		}
	}

	public final MulTermContext mulTerm() throws RecognitionException {
		return mulTerm(0);
	}

	private MulTermContext mulTerm(int _p) throws RecognitionException {
		ParserRuleContext _parentctx = _ctx;
		int _parentState = getState();
		MulTermContext _localctx = new MulTermContext(_ctx, _parentState);
		MulTermContext _prevctx = _localctx;
		int _startState = 10;
		enterRecursionRule(_localctx, 10, RULE_mulTerm, _p);
		int _la;
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(134);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,8,_ctx) ) {
			case 1:
				{
				_localctx = new PassThroughMulTermContext(_localctx);
				_ctx = _localctx;
				_prevctx = _localctx;

				setState(113);
				atomTerm();
				}
				break;
			case 2:
				{
				_localctx = new ForEachUnaryContext(_localctx);
				_ctx = _localctx;
				_prevctx = _localctx;
				setState(114);
				match(L_BRACK);
				setState(115);
				termSeq();
				setState(116);
				match(R_BRACK);
				setState(117);
				unaryOp();
				}
				break;
			case 3:
				{
				_localctx = new ForEachLeftBinaryContext(_localctx);
				_ctx = _localctx;
				_prevctx = _localctx;
				setState(119);
				match(L_BRACK);
				setState(120);
				termSeq();
				setState(121);
				match(R_BRACK);
				setState(122);
				binaryOpWithRS();
				}
				break;
			case 4:
				{
				_localctx = new ForEachChainContext(_localctx);
				_ctx = _localctx;
				_prevctx = _localctx;
				setState(124);
				match(L_BRACK);
				setState(125);
				termSeq();
				setState(126);
				match(VERT_BAR);
				setState(128); 
				_errHandler.sync(this);
				_la = _input.LA(1);
				do {
					{
					{
					setState(127);
					unaryOrBinaryOp();
					}
					}
					setState(130); 
					_errHandler.sync(this);
					_la = _input.LA(1);
				} while ( (((_la) & ~0x3f) == 0 && ((1L << _la) & 130281568L) != 0) );
				setState(132);
				match(R_BRACK);
				}
				break;
			}
			_ctx.stop = _input.LT(-1);
			setState(142);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,10,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					if ( _parseListeners!=null ) triggerExitRuleEvent();
					_prevctx = _localctx;
					{
					setState(140);
					_errHandler.sync(this);
					switch ( getInterpreter().adaptivePredict(_input,9,_ctx) ) {
					case 1:
						{
						_localctx = new UnaryOperationContext(new MulTermContext(_parentctx, _parentState));
						pushNewRecursionContext(_localctx, _startState, RULE_mulTerm);
						setState(136);
						if (!(precpred(_ctx, 5))) throw new FailedPredicateException(this, "precpred(_ctx, 5)");
						setState(137);
						unaryOp();
						}
						break;
					case 2:
						{
						_localctx = new BinaryOperationContext(new MulTermContext(_parentctx, _parentState));
						pushNewRecursionContext(_localctx, _startState, RULE_mulTerm);
						setState(138);
						if (!(precpred(_ctx, 4))) throw new FailedPredicateException(this, "precpred(_ctx, 4)");
						setState(139);
						binaryOpWithRS();
						}
						break;
					}
					} 
				}
				setState(144);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,10,_ctx);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			unrollRecursionContexts(_parentctx);
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class BinaryOpWithRSContext extends ParserRuleContext {
		public BinaryOpContext binaryOp() {
			return getRuleContext(BinaryOpContext.class,0);
		}
		public AtomTermContext atomTerm() {
			return getRuleContext(AtomTermContext.class,0);
		}
		public TerminalNode RTL() { return getToken(PrimellParser.RTL, 0); }
		public TermSeqContext termSeq() {
			return getRuleContext(TermSeqContext.class,0);
		}
		public TerminalNode L_BRACK() { return getToken(PrimellParser.L_BRACK, 0); }
		public TerminalNode R_BRACK() { return getToken(PrimellParser.R_BRACK, 0); }
		public BinaryOpWithRSContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_binaryOpWithRS; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterBinaryOpWithRS(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitBinaryOpWithRS(this);
		}
	}

	public final BinaryOpWithRSContext binaryOpWithRS() throws RecognitionException {
		BinaryOpWithRSContext _localctx = new BinaryOpWithRSContext(_ctx, getState());
		enterRule(_localctx, 12, RULE_binaryOpWithRS);
		try {
			setState(157);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,11,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(145);
				binaryOp();
				setState(146);
				atomTerm();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(148);
				binaryOp();
				setState(149);
				match(RTL);
				setState(150);
				termSeq();
				}
				break;
			case 3:
				enterOuterAlt(_localctx, 3);
				{
				setState(152);
				match(L_BRACK);
				setState(153);
				binaryOp();
				setState(154);
				termSeq();
				setState(155);
				match(R_BRACK);
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class UnaryOrBinaryOpContext extends ParserRuleContext {
		public UnaryOpContext unaryOp() {
			return getRuleContext(UnaryOpContext.class,0);
		}
		public BinaryOpWithRSContext binaryOpWithRS() {
			return getRuleContext(BinaryOpWithRSContext.class,0);
		}
		public UnaryOrBinaryOpContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_unaryOrBinaryOp; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterUnaryOrBinaryOp(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitUnaryOrBinaryOp(this);
		}
	}

	public final UnaryOrBinaryOpContext unaryOrBinaryOp() throws RecognitionException {
		UnaryOrBinaryOpContext _localctx = new UnaryOrBinaryOpContext(_ctx, getState());
		enterRule(_localctx, 14, RULE_unaryOrBinaryOp);
		try {
			setState(161);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case ASSIGN:
			case NEGATE:
			case OP_UNARY:
			case OP_USER_UNARY:
				enterOuterAlt(_localctx, 1);
				{
				setState(159);
				unaryOp();
				}
				break;
			case L_BRACK:
			case TAIL:
			case PLUS:
			case STAR:
			case F_SLASH:
			case B_SLASH:
			case OP_BINARY:
			case OP_USER_BINARY:
			case OP_COND:
				enterOuterAlt(_localctx, 2);
				{
				setState(160);
				binaryOpWithRS();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class AtomTermContext extends ParserRuleContext {
		public AtomTermContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_atomTerm; }
	 
		public AtomTermContext() { }
		public void copyFrom(AtomTermContext ctx) {
			super.copyFrom(ctx);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class IntegerOrIdentifierContext extends AtomTermContext {
		public IntOrIdContext intOrId() {
			return getRuleContext(IntOrIdContext.class,0);
		}
		public IntegerOrIdentifierContext(AtomTermContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterIntegerOrIdentifier(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitIntegerOrIdentifier(this);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class NullaryOperationContext extends AtomTermContext {
		public NullaryOpContext nullaryOp() {
			return getRuleContext(NullaryOpContext.class,0);
		}
		public NullaryOperationContext(AtomTermContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterNullaryOperation(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitNullaryOperation(this);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class EmptyListContext extends AtomTermContext {
		public TerminalNode L_PAREN() { return getToken(PrimellParser.L_PAREN, 0); }
		public TerminalNode R_PAREN() { return getToken(PrimellParser.R_PAREN, 0); }
		public TerminalNode L_BRACK() { return getToken(PrimellParser.L_BRACK, 0); }
		public TerminalNode R_BRACK() { return getToken(PrimellParser.R_BRACK, 0); }
		public EmptyListContext(AtomTermContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterEmptyList(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitEmptyList(this);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class ParensContext extends AtomTermContext {
		public TerminalNode L_PAREN() { return getToken(PrimellParser.L_PAREN, 0); }
		public TermSeqContext termSeq() {
			return getRuleContext(TermSeqContext.class,0);
		}
		public TerminalNode R_PAREN() { return getToken(PrimellParser.R_PAREN, 0); }
		public ParensContext(AtomTermContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterParens(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitParens(this);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class StringContext extends AtomTermContext {
		public TerminalNode STRING() { return getToken(PrimellParser.STRING, 0); }
		public StringContext(AtomTermContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterString(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitString(this);
		}
	}
	@SuppressWarnings("CheckReturnValue")
	public static class InfinityContext extends AtomTermContext {
		public TerminalNode INFINITY() { return getToken(PrimellParser.INFINITY, 0); }
		public InfinityContext(AtomTermContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterInfinity(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitInfinity(this);
		}
	}

	public final AtomTermContext atomTerm() throws RecognitionException {
		AtomTermContext _localctx = new AtomTermContext(_ctx, getState());
		enterRule(_localctx, 16, RULE_atomTerm);
		try {
			setState(175);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,13,_ctx) ) {
			case 1:
				_localctx = new IntegerOrIdentifierContext(_localctx);
				enterOuterAlt(_localctx, 1);
				{
				setState(163);
				intOrId();
				}
				break;
			case 2:
				_localctx = new InfinityContext(_localctx);
				enterOuterAlt(_localctx, 2);
				{
				setState(164);
				match(INFINITY);
				}
				break;
			case 3:
				_localctx = new NullaryOperationContext(_localctx);
				enterOuterAlt(_localctx, 3);
				{
				setState(165);
				nullaryOp();
				}
				break;
			case 4:
				_localctx = new EmptyListContext(_localctx);
				enterOuterAlt(_localctx, 4);
				{
				setState(166);
				match(L_PAREN);
				setState(167);
				match(R_PAREN);
				}
				break;
			case 5:
				_localctx = new EmptyListContext(_localctx);
				enterOuterAlt(_localctx, 5);
				{
				setState(168);
				match(L_BRACK);
				setState(169);
				match(R_BRACK);
				}
				break;
			case 6:
				_localctx = new ParensContext(_localctx);
				enterOuterAlt(_localctx, 6);
				{
				setState(170);
				match(L_PAREN);
				setState(171);
				termSeq();
				setState(172);
				match(R_PAREN);
				}
				break;
			case 7:
				_localctx = new StringContext(_localctx);
				enterOuterAlt(_localctx, 7);
				{
				setState(174);
				match(STRING);
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class IntOrIdContext extends ParserRuleContext {
		public List<TerminalNode> INT_OR_ID() { return getTokens(PrimellParser.INT_OR_ID); }
		public TerminalNode INT_OR_ID(int i) {
			return getToken(PrimellParser.INT_OR_ID, i);
		}
		public List<TerminalNode> DOT() { return getTokens(PrimellParser.DOT); }
		public TerminalNode DOT(int i) {
			return getToken(PrimellParser.DOT, i);
		}
		public IntOrIdContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_intOrId; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterIntOrId(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitIntOrId(this);
		}
	}

	public final IntOrIdContext intOrId() throws RecognitionException {
		IntOrIdContext _localctx = new IntOrIdContext(_ctx, getState());
		enterRule(_localctx, 18, RULE_intOrId);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(177);
			match(INT_OR_ID);
			setState(182);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,14,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					{
					{
					setState(178);
					match(DOT);
					setState(179);
					match(INT_OR_ID);
					}
					} 
				}
				setState(184);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,14,_ctx);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class BaseNullaryOpContext extends ParserRuleContext {
		public TerminalNode OP_NULLARY() { return getToken(PrimellParser.OP_NULLARY, 0); }
		public BaseNullaryOpContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_baseNullaryOp; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterBaseNullaryOp(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitBaseNullaryOp(this);
		}
	}

	public final BaseNullaryOpContext baseNullaryOp() throws RecognitionException {
		BaseNullaryOpContext _localctx = new BaseNullaryOpContext(_ctx, getState());
		enterRule(_localctx, 20, RULE_baseNullaryOp);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(185);
			match(OP_NULLARY);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class BaseUnaryOpContext extends ParserRuleContext {
		public TerminalNode OP_UNARY() { return getToken(PrimellParser.OP_UNARY, 0); }
		public TerminalNode OP_USER_UNARY() { return getToken(PrimellParser.OP_USER_UNARY, 0); }
		public Op_negContext op_neg() {
			return getRuleContext(Op_negContext.class,0);
		}
		public BaseUnaryOpContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_baseUnaryOp; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterBaseUnaryOp(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitBaseUnaryOp(this);
		}
	}

	public final BaseUnaryOpContext baseUnaryOp() throws RecognitionException {
		BaseUnaryOpContext _localctx = new BaseUnaryOpContext(_ctx, getState());
		enterRule(_localctx, 22, RULE_baseUnaryOp);
		try {
			setState(190);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case OP_UNARY:
				enterOuterAlt(_localctx, 1);
				{
				setState(187);
				match(OP_UNARY);
				}
				break;
			case OP_USER_UNARY:
				enterOuterAlt(_localctx, 2);
				{
				setState(188);
				match(OP_USER_UNARY);
				}
				break;
			case NEGATE:
				enterOuterAlt(_localctx, 3);
				{
				setState(189);
				op_neg();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class BaseBinaryOpContext extends ParserRuleContext {
		public TerminalNode OP_BINARY() { return getToken(PrimellParser.OP_BINARY, 0); }
		public TerminalNode OP_USER_BINARY() { return getToken(PrimellParser.OP_USER_BINARY, 0); }
		public Op_addContext op_add() {
			return getRuleContext(Op_addContext.class,0);
		}
		public Op_mulContext op_mul() {
			return getRuleContext(Op_mulContext.class,0);
		}
		public Op_divContext op_div() {
			return getRuleContext(Op_divContext.class,0);
		}
		public Op_maxContext op_max() {
			return getRuleContext(Op_maxContext.class,0);
		}
		public Op_list_diffContext op_list_diff() {
			return getRuleContext(Op_list_diffContext.class,0);
		}
		public ConditionalOpContext conditionalOp() {
			return getRuleContext(ConditionalOpContext.class,0);
		}
		public BaseBinaryOpContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_baseBinaryOp; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterBaseBinaryOp(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitBaseBinaryOp(this);
		}
	}

	public final BaseBinaryOpContext baseBinaryOp() throws RecognitionException {
		BaseBinaryOpContext _localctx = new BaseBinaryOpContext(_ctx, getState());
		enterRule(_localctx, 24, RULE_baseBinaryOp);
		try {
			setState(200);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case OP_BINARY:
				enterOuterAlt(_localctx, 1);
				{
				setState(192);
				match(OP_BINARY);
				}
				break;
			case OP_USER_BINARY:
				enterOuterAlt(_localctx, 2);
				{
				setState(193);
				match(OP_USER_BINARY);
				}
				break;
			case PLUS:
				enterOuterAlt(_localctx, 3);
				{
				setState(194);
				op_add();
				}
				break;
			case STAR:
				enterOuterAlt(_localctx, 4);
				{
				setState(195);
				op_mul();
				}
				break;
			case F_SLASH:
				enterOuterAlt(_localctx, 5);
				{
				setState(196);
				op_div();
				}
				break;
			case TAIL:
				enterOuterAlt(_localctx, 6);
				{
				setState(197);
				op_max();
				}
				break;
			case B_SLASH:
				enterOuterAlt(_localctx, 7);
				{
				setState(198);
				op_list_diff();
				}
				break;
			case OP_COND:
				enterOuterAlt(_localctx, 8);
				{
				setState(199);
				conditionalOp();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class ConditionalOpContext extends ParserRuleContext {
		public TerminalNode OP_COND() { return getToken(PrimellParser.OP_COND, 0); }
		public CondFuncContext condFunc() {
			return getRuleContext(CondFuncContext.class,0);
		}
		public Cond_mod_negContext cond_mod_neg() {
			return getRuleContext(Cond_mod_negContext.class,0);
		}
		public Cond_mod_tailContext cond_mod_tail() {
			return getRuleContext(Cond_mod_tailContext.class,0);
		}
		public ConditionalOpContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_conditionalOp; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterConditionalOp(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitConditionalOp(this);
		}
	}

	public final ConditionalOpContext conditionalOp() throws RecognitionException {
		ConditionalOpContext _localctx = new ConditionalOpContext(_ctx, getState());
		enterRule(_localctx, 26, RULE_conditionalOp);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(202);
			match(OP_COND);
			setState(204);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & 221184L) != 0)) {
				{
				setState(203);
				condFunc();
				}
			}

			setState(207);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==NEGATE) {
				{
				setState(206);
				cond_mod_neg();
				}
			}

			setState(210);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==TAIL) {
				{
				setState(209);
				cond_mod_tail();
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class CondFuncContext extends ParserRuleContext {
		public Cond_mod_jumpContext cond_mod_jump() {
			return getRuleContext(Cond_mod_jumpContext.class,0);
		}
		public Cond_mod_back_jumpContext cond_mod_back_jump() {
			return getRuleContext(Cond_mod_back_jumpContext.class,0);
		}
		public Cond_mod_whileContext cond_mod_while() {
			return getRuleContext(Cond_mod_whileContext.class,0);
		}
		public Cond_mod_do_whileContext cond_mod_do_while() {
			return getRuleContext(Cond_mod_do_whileContext.class,0);
		}
		public CondFuncContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_condFunc; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterCondFunc(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitCondFunc(this);
		}
	}

	public final CondFuncContext condFunc() throws RecognitionException {
		CondFuncContext _localctx = new CondFuncContext(_ctx, getState());
		enterRule(_localctx, 28, RULE_condFunc);
		try {
			setState(216);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case F_SLASH:
				enterOuterAlt(_localctx, 1);
				{
				setState(212);
				cond_mod_jump();
				}
				break;
			case B_SLASH:
				enterOuterAlt(_localctx, 2);
				{
				setState(213);
				cond_mod_back_jump();
				}
				break;
			case STAR:
				enterOuterAlt(_localctx, 3);
				{
				setState(214);
				cond_mod_while();
				}
				break;
			case PLUS:
				enterOuterAlt(_localctx, 4);
				{
				setState(215);
				cond_mod_do_while();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class OpModsContext extends ParserRuleContext {
		public TerminalNode OPMOD_CUT() { return getToken(PrimellParser.OPMOD_CUT, 0); }
		public TerminalNode OPMOD_POW() { return getToken(PrimellParser.OPMOD_POW, 0); }
		public OpModsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_opMods; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterOpMods(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitOpMods(this);
		}
	}

	public final OpModsContext opMods() throws RecognitionException {
		OpModsContext _localctx = new OpModsContext(_ctx, getState());
		enterRule(_localctx, 30, RULE_opMods);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(219);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,21,_ctx) ) {
			case 1:
				{
				setState(218);
				_la = _input.LA(1);
				if ( !(_la==OPMOD_POW || _la==OPMOD_CUT) ) {
				_errHandler.recoverInline(this);
				}
				else {
					if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
					_errHandler.reportMatch(this);
					consume();
				}
				}
				break;
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class AssignModsContext extends ParserRuleContext {
		public TerminalNode OPMOD_CUT() { return getToken(PrimellParser.OPMOD_CUT, 0); }
		public TerminalNode OPMOD_POW() { return getToken(PrimellParser.OPMOD_POW, 0); }
		public AssignModsContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_assignMods; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterAssignMods(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitAssignMods(this);
		}
	}

	public final AssignModsContext assignMods() throws RecognitionException {
		AssignModsContext _localctx = new AssignModsContext(_ctx, getState());
		enterRule(_localctx, 32, RULE_assignMods);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(222);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==OPMOD_POW || _la==OPMOD_CUT) {
				{
				setState(221);
				_la = _input.LA(1);
				if ( !(_la==OPMOD_POW || _la==OPMOD_CUT) ) {
				_errHandler.recoverInline(this);
				}
				else {
					if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
					_errHandler.reportMatch(this);
					consume();
				}
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class NullaryOpContext extends ParserRuleContext {
		public BaseNullaryOpContext baseNullaryOp() {
			return getRuleContext(BaseNullaryOpContext.class,0);
		}
		public OpModsContext opMods() {
			return getRuleContext(OpModsContext.class,0);
		}
		public NullaryOpContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_nullaryOp; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterNullaryOp(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitNullaryOp(this);
		}
	}

	public final NullaryOpContext nullaryOp() throws RecognitionException {
		NullaryOpContext _localctx = new NullaryOpContext(_ctx, getState());
		enterRule(_localctx, 34, RULE_nullaryOp);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(224);
			baseNullaryOp();
			setState(225);
			opMods();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class UnaryOpContext extends ParserRuleContext {
		public BaseUnaryOpContext baseUnaryOp() {
			return getRuleContext(BaseUnaryOpContext.class,0);
		}
		public OpModsContext opMods() {
			return getRuleContext(OpModsContext.class,0);
		}
		public UnaryAssignContext unaryAssign() {
			return getRuleContext(UnaryAssignContext.class,0);
		}
		public UnaryOpContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_unaryOp; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterUnaryOp(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitUnaryOp(this);
		}
	}

	public final UnaryOpContext unaryOp() throws RecognitionException {
		UnaryOpContext _localctx = new UnaryOpContext(_ctx, getState());
		enterRule(_localctx, 36, RULE_unaryOp);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(228);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==ASSIGN) {
				{
				setState(227);
				unaryAssign();
				}
			}

			setState(230);
			baseUnaryOp();
			setState(231);
			opMods();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class UnaryAssignContext extends ParserRuleContext {
		public TerminalNode ASSIGN() { return getToken(PrimellParser.ASSIGN, 0); }
		public AssignModsContext assignMods() {
			return getRuleContext(AssignModsContext.class,0);
		}
		public UnaryAssignContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_unaryAssign; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterUnaryAssign(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitUnaryAssign(this);
		}
	}

	public final UnaryAssignContext unaryAssign() throws RecognitionException {
		UnaryAssignContext _localctx = new UnaryAssignContext(_ctx, getState());
		enterRule(_localctx, 38, RULE_unaryAssign);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(233);
			match(ASSIGN);
			setState(234);
			assignMods();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class BinaryOpContext extends ParserRuleContext {
		public BaseBinaryOpContext baseBinaryOp() {
			return getRuleContext(BaseBinaryOpContext.class,0);
		}
		public OpModsContext opMods() {
			return getRuleContext(OpModsContext.class,0);
		}
		public BinaryOpContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_binaryOp; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterBinaryOp(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitBinaryOp(this);
		}
	}

	public final BinaryOpContext binaryOp() throws RecognitionException {
		BinaryOpContext _localctx = new BinaryOpContext(_ctx, getState());
		enterRule(_localctx, 40, RULE_binaryOp);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(236);
			baseBinaryOp();
			setState(237);
			opMods();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class Op_list_diffContext extends ParserRuleContext {
		public TerminalNode B_SLASH() { return getToken(PrimellParser.B_SLASH, 0); }
		public Op_list_diffContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_op_list_diff; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterOp_list_diff(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitOp_list_diff(this);
		}
	}

	public final Op_list_diffContext op_list_diff() throws RecognitionException {
		Op_list_diffContext _localctx = new Op_list_diffContext(_ctx, getState());
		enterRule(_localctx, 42, RULE_op_list_diff);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(239);
			match(B_SLASH);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class Cond_mod_back_jumpContext extends ParserRuleContext {
		public TerminalNode B_SLASH() { return getToken(PrimellParser.B_SLASH, 0); }
		public Cond_mod_back_jumpContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_cond_mod_back_jump; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterCond_mod_back_jump(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitCond_mod_back_jump(this);
		}
	}

	public final Cond_mod_back_jumpContext cond_mod_back_jump() throws RecognitionException {
		Cond_mod_back_jumpContext _localctx = new Cond_mod_back_jumpContext(_ctx, getState());
		enterRule(_localctx, 44, RULE_cond_mod_back_jump);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(241);
			match(B_SLASH);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class Op_divContext extends ParserRuleContext {
		public TerminalNode F_SLASH() { return getToken(PrimellParser.F_SLASH, 0); }
		public Op_divContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_op_div; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterOp_div(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitOp_div(this);
		}
	}

	public final Op_divContext op_div() throws RecognitionException {
		Op_divContext _localctx = new Op_divContext(_ctx, getState());
		enterRule(_localctx, 46, RULE_op_div);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(243);
			match(F_SLASH);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class Cond_mod_jumpContext extends ParserRuleContext {
		public TerminalNode F_SLASH() { return getToken(PrimellParser.F_SLASH, 0); }
		public Cond_mod_jumpContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_cond_mod_jump; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterCond_mod_jump(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitCond_mod_jump(this);
		}
	}

	public final Cond_mod_jumpContext cond_mod_jump() throws RecognitionException {
		Cond_mod_jumpContext _localctx = new Cond_mod_jumpContext(_ctx, getState());
		enterRule(_localctx, 48, RULE_cond_mod_jump);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(245);
			match(F_SLASH);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class Op_maxContext extends ParserRuleContext {
		public TerminalNode TAIL() { return getToken(PrimellParser.TAIL, 0); }
		public Op_maxContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_op_max; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterOp_max(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitOp_max(this);
		}
	}

	public final Op_maxContext op_max() throws RecognitionException {
		Op_maxContext _localctx = new Op_maxContext(_ctx, getState());
		enterRule(_localctx, 50, RULE_op_max);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(247);
			match(TAIL);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class Cond_mod_tailContext extends ParserRuleContext {
		public TerminalNode TAIL() { return getToken(PrimellParser.TAIL, 0); }
		public Cond_mod_tailContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_cond_mod_tail; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterCond_mod_tail(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitCond_mod_tail(this);
		}
	}

	public final Cond_mod_tailContext cond_mod_tail() throws RecognitionException {
		Cond_mod_tailContext _localctx = new Cond_mod_tailContext(_ctx, getState());
		enterRule(_localctx, 52, RULE_cond_mod_tail);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(249);
			match(TAIL);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class Op_mulContext extends ParserRuleContext {
		public TerminalNode STAR() { return getToken(PrimellParser.STAR, 0); }
		public Op_mulContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_op_mul; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterOp_mul(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitOp_mul(this);
		}
	}

	public final Op_mulContext op_mul() throws RecognitionException {
		Op_mulContext _localctx = new Op_mulContext(_ctx, getState());
		enterRule(_localctx, 54, RULE_op_mul);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(251);
			match(STAR);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class Cond_mod_whileContext extends ParserRuleContext {
		public TerminalNode STAR() { return getToken(PrimellParser.STAR, 0); }
		public Cond_mod_whileContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_cond_mod_while; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterCond_mod_while(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitCond_mod_while(this);
		}
	}

	public final Cond_mod_whileContext cond_mod_while() throws RecognitionException {
		Cond_mod_whileContext _localctx = new Cond_mod_whileContext(_ctx, getState());
		enterRule(_localctx, 56, RULE_cond_mod_while);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(253);
			match(STAR);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class Op_addContext extends ParserRuleContext {
		public TerminalNode PLUS() { return getToken(PrimellParser.PLUS, 0); }
		public Op_addContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_op_add; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterOp_add(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitOp_add(this);
		}
	}

	public final Op_addContext op_add() throws RecognitionException {
		Op_addContext _localctx = new Op_addContext(_ctx, getState());
		enterRule(_localctx, 58, RULE_op_add);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(255);
			match(PLUS);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class Cond_mod_do_whileContext extends ParserRuleContext {
		public TerminalNode PLUS() { return getToken(PrimellParser.PLUS, 0); }
		public Cond_mod_do_whileContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_cond_mod_do_while; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterCond_mod_do_while(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitCond_mod_do_while(this);
		}
	}

	public final Cond_mod_do_whileContext cond_mod_do_while() throws RecognitionException {
		Cond_mod_do_whileContext _localctx = new Cond_mod_do_whileContext(_ctx, getState());
		enterRule(_localctx, 60, RULE_cond_mod_do_while);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(257);
			match(PLUS);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class Op_negContext extends ParserRuleContext {
		public TerminalNode NEGATE() { return getToken(PrimellParser.NEGATE, 0); }
		public Op_negContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_op_neg; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterOp_neg(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitOp_neg(this);
		}
	}

	public final Op_negContext op_neg() throws RecognitionException {
		Op_negContext _localctx = new Op_negContext(_ctx, getState());
		enterRule(_localctx, 62, RULE_op_neg);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(259);
			match(NEGATE);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	@SuppressWarnings("CheckReturnValue")
	public static class Cond_mod_negContext extends ParserRuleContext {
		public TerminalNode NEGATE() { return getToken(PrimellParser.NEGATE, 0); }
		public Cond_mod_negContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_cond_mod_neg; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).enterCond_mod_neg(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof PrimellParserListener ) ((PrimellParserListener)listener).exitCond_mod_neg(this);
		}
	}

	public final Cond_mod_negContext cond_mod_neg() throws RecognitionException {
		Cond_mod_negContext _localctx = new Cond_mod_negContext(_ctx, getState());
		enterRule(_localctx, 64, RULE_cond_mod_neg);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(261);
			match(NEGATE);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public boolean sempred(RuleContext _localctx, int ruleIndex, int predIndex) {
		switch (ruleIndex) {
		case 5:
			return mulTerm_sempred((MulTermContext)_localctx, predIndex);
		}
		return true;
	}
	private boolean mulTerm_sempred(MulTermContext _localctx, int predIndex) {
		switch (predIndex) {
		case 0:
			return precpred(_ctx, 5);
		case 1:
			return precpred(_ctx, 4);
		}
		return true;
	}

	public static final String _serializedATN =
		"\u0004\u0001\u001f\u0108\u0002\u0000\u0007\u0000\u0002\u0001\u0007\u0001"+
		"\u0002\u0002\u0007\u0002\u0002\u0003\u0007\u0003\u0002\u0004\u0007\u0004"+
		"\u0002\u0005\u0007\u0005\u0002\u0006\u0007\u0006\u0002\u0007\u0007\u0007"+
		"\u0002\b\u0007\b\u0002\t\u0007\t\u0002\n\u0007\n\u0002\u000b\u0007\u000b"+
		"\u0002\f\u0007\f\u0002\r\u0007\r\u0002\u000e\u0007\u000e\u0002\u000f\u0007"+
		"\u000f\u0002\u0010\u0007\u0010\u0002\u0011\u0007\u0011\u0002\u0012\u0007"+
		"\u0012\u0002\u0013\u0007\u0013\u0002\u0014\u0007\u0014\u0002\u0015\u0007"+
		"\u0015\u0002\u0016\u0007\u0016\u0002\u0017\u0007\u0017\u0002\u0018\u0007"+
		"\u0018\u0002\u0019\u0007\u0019\u0002\u001a\u0007\u001a\u0002\u001b\u0007"+
		"\u001b\u0002\u001c\u0007\u001c\u0002\u001d\u0007\u001d\u0002\u001e\u0007"+
		"\u001e\u0002\u001f\u0007\u001f\u0002 \u0007 \u0001\u0000\u0001\u0000\u0003"+
		"\u0000E\b\u0000\u0001\u0000\u0001\u0000\u0001\u0001\u0004\u0001J\b\u0001"+
		"\u000b\u0001\f\u0001K\u0001\u0002\u0003\u0002O\b\u0002\u0001\u0002\u0001"+
		"\u0002\u0001\u0003\u0001\u0003\u0001\u0003\u0001\u0003\u0001\u0003\u0001"+
		"\u0003\u0003\u0003Y\b\u0003\u0001\u0003\u0001\u0003\u0001\u0003\u0001"+
		"\u0003\u0001\u0003\u0001\u0003\u0001\u0003\u0001\u0003\u0001\u0003\u0001"+
		"\u0003\u0001\u0003\u0001\u0003\u0001\u0003\u0003\u0003h\b\u0003\u0003"+
		"\u0003j\b\u0003\u0001\u0004\u0001\u0004\u0001\u0004\u0003\u0004o\b\u0004"+
		"\u0001\u0005\u0001\u0005\u0001\u0005\u0001\u0005\u0001\u0005\u0001\u0005"+
		"\u0001\u0005\u0001\u0005\u0001\u0005\u0001\u0005\u0001\u0005\u0001\u0005"+
		"\u0001\u0005\u0001\u0005\u0001\u0005\u0001\u0005\u0004\u0005\u0081\b\u0005"+
		"\u000b\u0005\f\u0005\u0082\u0001\u0005\u0001\u0005\u0003\u0005\u0087\b"+
		"\u0005\u0001\u0005\u0001\u0005\u0001\u0005\u0001\u0005\u0005\u0005\u008d"+
		"\b\u0005\n\u0005\f\u0005\u0090\t\u0005\u0001\u0006\u0001\u0006\u0001\u0006"+
		"\u0001\u0006\u0001\u0006\u0001\u0006\u0001\u0006\u0001\u0006\u0001\u0006"+
		"\u0001\u0006\u0001\u0006\u0001\u0006\u0003\u0006\u009e\b\u0006\u0001\u0007"+
		"\u0001\u0007\u0003\u0007\u00a2\b\u0007\u0001\b\u0001\b\u0001\b\u0001\b"+
		"\u0001\b\u0001\b\u0001\b\u0001\b\u0001\b\u0001\b\u0001\b\u0001\b\u0003"+
		"\b\u00b0\b\b\u0001\t\u0001\t\u0001\t\u0005\t\u00b5\b\t\n\t\f\t\u00b8\t"+
		"\t\u0001\n\u0001\n\u0001\u000b\u0001\u000b\u0001\u000b\u0003\u000b\u00bf"+
		"\b\u000b\u0001\f\u0001\f\u0001\f\u0001\f\u0001\f\u0001\f\u0001\f\u0001"+
		"\f\u0003\f\u00c9\b\f\u0001\r\u0001\r\u0003\r\u00cd\b\r\u0001\r\u0003\r"+
		"\u00d0\b\r\u0001\r\u0003\r\u00d3\b\r\u0001\u000e\u0001\u000e\u0001\u000e"+
		"\u0001\u000e\u0003\u000e\u00d9\b\u000e\u0001\u000f\u0003\u000f\u00dc\b"+
		"\u000f\u0001\u0010\u0003\u0010\u00df\b\u0010\u0001\u0011\u0001\u0011\u0001"+
		"\u0011\u0001\u0012\u0003\u0012\u00e5\b\u0012\u0001\u0012\u0001\u0012\u0001"+
		"\u0012\u0001\u0013\u0001\u0013\u0001\u0013\u0001\u0014\u0001\u0014\u0001"+
		"\u0014\u0001\u0015\u0001\u0015\u0001\u0016\u0001\u0016\u0001\u0017\u0001"+
		"\u0017\u0001\u0018\u0001\u0018\u0001\u0019\u0001\u0019\u0001\u001a\u0001"+
		"\u001a\u0001\u001b\u0001\u001b\u0001\u001c\u0001\u001c\u0001\u001d\u0001"+
		"\u001d\u0001\u001e\u0001\u001e\u0001\u001f\u0001\u001f\u0001 \u0001 \u0001"+
		" \u0000\u0001\n!\u0000\u0002\u0004\u0006\b\n\f\u000e\u0010\u0012\u0014"+
		"\u0016\u0018\u001a\u001c\u001e \"$&(*,.02468:<>@\u0000\u0001\u0001\u0000"+
		"\u0013\u0014\u0111\u0000B\u0001\u0000\u0000\u0000\u0002I\u0001\u0000\u0000"+
		"\u0000\u0004N\u0001\u0000\u0000\u0000\u0006i\u0001\u0000\u0000\u0000\b"+
		"k\u0001\u0000\u0000\u0000\n\u0086\u0001\u0000\u0000\u0000\f\u009d\u0001"+
		"\u0000\u0000\u0000\u000e\u00a1\u0001\u0000\u0000\u0000\u0010\u00af\u0001"+
		"\u0000\u0000\u0000\u0012\u00b1\u0001\u0000\u0000\u0000\u0014\u00b9\u0001"+
		"\u0000\u0000\u0000\u0016\u00be\u0001\u0000\u0000\u0000\u0018\u00c8\u0001"+
		"\u0000\u0000\u0000\u001a\u00ca\u0001\u0000\u0000\u0000\u001c\u00d8\u0001"+
		"\u0000\u0000\u0000\u001e\u00db\u0001\u0000\u0000\u0000 \u00de\u0001\u0000"+
		"\u0000\u0000\"\u00e0\u0001\u0000\u0000\u0000$\u00e4\u0001\u0000\u0000"+
		"\u0000&\u00e9\u0001\u0000\u0000\u0000(\u00ec\u0001\u0000\u0000\u0000*"+
		"\u00ef\u0001\u0000\u0000\u0000,\u00f1\u0001\u0000\u0000\u0000.\u00f3\u0001"+
		"\u0000\u0000\u00000\u00f5\u0001\u0000\u0000\u00002\u00f7\u0001\u0000\u0000"+
		"\u00004\u00f9\u0001\u0000\u0000\u00006\u00fb\u0001\u0000\u0000\u00008"+
		"\u00fd\u0001\u0000\u0000\u0000:\u00ff\u0001\u0000\u0000\u0000<\u0101\u0001"+
		"\u0000\u0000\u0000>\u0103\u0001\u0000\u0000\u0000@\u0105\u0001\u0000\u0000"+
		"\u0000BD\u0003\u0002\u0001\u0000CE\u0005\u001d\u0000\u0000DC\u0001\u0000"+
		"\u0000\u0000DE\u0001\u0000\u0000\u0000EF\u0001\u0000\u0000\u0000FG\u0005"+
		"\u0000\u0000\u0001G\u0001\u0001\u0000\u0000\u0000HJ\u0003\u0004\u0002"+
		"\u0000IH\u0001\u0000\u0000\u0000JK\u0001\u0000\u0000\u0000KI\u0001\u0000"+
		"\u0000\u0000KL\u0001\u0000\u0000\u0000L\u0003\u0001\u0000\u0000\u0000"+
		"MO\u0005\u0004\u0000\u0000NM\u0001\u0000\u0000\u0000NO\u0001\u0000\u0000"+
		"\u0000OP\u0001\u0000\u0000\u0000PQ\u0003\u0006\u0003\u0000Q\u0005\u0001"+
		"\u0000\u0000\u0000Rj\u0003\n\u0005\u0000ST\u0003\n\u0005\u0000TX\u0003"+
		"\b\u0004\u0000UY\u0003\u0006\u0003\u0000VW\u0005\u0002\u0000\u0000WY\u0003"+
		"\u0002\u0001\u0000XU\u0001\u0000\u0000\u0000XV\u0001\u0000\u0000\u0000"+
		"Yj\u0001\u0000\u0000\u0000Z[\u0003\n\u0005\u0000[\\\u0003\b\u0004\u0000"+
		"\\]\u0005\u0006\u0000\u0000]^\u0003\u0002\u0001\u0000^_\u0005\u0007\u0000"+
		"\u0000_j\u0001\u0000\u0000\u0000`a\u0005\u0006\u0000\u0000ab\u0003\u0002"+
		"\u0001\u0000bc\u0005\u0007\u0000\u0000cg\u0003\b\u0004\u0000dh\u0003\u0006"+
		"\u0003\u0000ef\u0005\u0002\u0000\u0000fh\u0003\u0002\u0001\u0000gd\u0001"+
		"\u0000\u0000\u0000ge\u0001\u0000\u0000\u0000hj\u0001\u0000\u0000\u0000"+
		"iR\u0001\u0000\u0000\u0000iS\u0001\u0000\u0000\u0000iZ\u0001\u0000\u0000"+
		"\u0000i`\u0001\u0000\u0000\u0000j\u0007\u0001\u0000\u0000\u0000kl\u0005"+
		"\u0005\u0000\u0000ln\u0003 \u0010\u0000mo\u0003(\u0014\u0000nm\u0001\u0000"+
		"\u0000\u0000no\u0001\u0000\u0000\u0000o\t\u0001\u0000\u0000\u0000pq\u0006"+
		"\u0005\uffff\uffff\u0000q\u0087\u0003\u0010\b\u0000rs\u0005\u0006\u0000"+
		"\u0000st\u0003\u0002\u0001\u0000tu\u0005\u0007\u0000\u0000uv\u0003$\u0012"+
		"\u0000v\u0087\u0001\u0000\u0000\u0000wx\u0005\u0006\u0000\u0000xy\u0003"+
		"\u0002\u0001\u0000yz\u0005\u0007\u0000\u0000z{\u0003\f\u0006\u0000{\u0087"+
		"\u0001\u0000\u0000\u0000|}\u0005\u0006\u0000\u0000}~\u0003\u0002\u0001"+
		"\u0000~\u0080\u0005\n\u0000\u0000\u007f\u0081\u0003\u000e\u0007\u0000"+
		"\u0080\u007f\u0001\u0000\u0000\u0000\u0081\u0082\u0001\u0000\u0000\u0000"+
		"\u0082\u0080\u0001\u0000\u0000\u0000\u0082\u0083\u0001\u0000\u0000\u0000"+
		"\u0083\u0084\u0001\u0000\u0000\u0000\u0084\u0085\u0005\u0007\u0000\u0000"+
		"\u0085\u0087\u0001\u0000\u0000\u0000\u0086p\u0001\u0000\u0000\u0000\u0086"+
		"r\u0001\u0000\u0000\u0000\u0086w\u0001\u0000\u0000\u0000\u0086|\u0001"+
		"\u0000\u0000\u0000\u0087\u008e\u0001\u0000\u0000\u0000\u0088\u0089\n\u0005"+
		"\u0000\u0000\u0089\u008d\u0003$\u0012\u0000\u008a\u008b\n\u0004\u0000"+
		"\u0000\u008b\u008d\u0003\f\u0006\u0000\u008c\u0088\u0001\u0000\u0000\u0000"+
		"\u008c\u008a\u0001\u0000\u0000\u0000\u008d\u0090\u0001\u0000\u0000\u0000"+
		"\u008e\u008c\u0001\u0000\u0000\u0000\u008e\u008f\u0001\u0000\u0000\u0000"+
		"\u008f\u000b\u0001\u0000\u0000\u0000\u0090\u008e\u0001\u0000\u0000\u0000"+
		"\u0091\u0092\u0003(\u0014\u0000\u0092\u0093\u0003\u0010\b\u0000\u0093"+
		"\u009e\u0001\u0000\u0000\u0000\u0094\u0095\u0003(\u0014\u0000\u0095\u0096"+
		"\u0005\u0002\u0000\u0000\u0096\u0097\u0003\u0002\u0001\u0000\u0097\u009e"+
		"\u0001\u0000\u0000\u0000\u0098\u0099\u0005\u0006\u0000\u0000\u0099\u009a"+
		"\u0003(\u0014\u0000\u009a\u009b\u0003\u0002\u0001\u0000\u009b\u009c\u0005"+
		"\u0007\u0000\u0000\u009c\u009e\u0001\u0000\u0000\u0000\u009d\u0091\u0001"+
		"\u0000\u0000\u0000\u009d\u0094\u0001\u0000\u0000\u0000\u009d\u0098\u0001"+
		"\u0000\u0000\u0000\u009e\r\u0001\u0000\u0000\u0000\u009f\u00a2\u0003$"+
		"\u0012\u0000\u00a0\u00a2\u0003\f\u0006\u0000\u00a1\u009f\u0001\u0000\u0000"+
		"\u0000\u00a1\u00a0\u0001\u0000\u0000\u0000\u00a2\u000f\u0001\u0000\u0000"+
		"\u0000\u00a3\u00b0\u0003\u0012\t\u0000\u00a4\u00b0\u0005\u0001\u0000\u0000"+
		"\u00a5\u00b0\u0003\"\u0011\u0000\u00a6\u00a7\u0005\b\u0000\u0000\u00a7"+
		"\u00b0\u0005\t\u0000\u0000\u00a8\u00a9\u0005\u0006\u0000\u0000\u00a9\u00b0"+
		"\u0005\u0007\u0000\u0000\u00aa\u00ab\u0005\b\u0000\u0000\u00ab\u00ac\u0003"+
		"\u0002\u0001\u0000\u00ac\u00ad\u0005\t\u0000\u0000\u00ad\u00b0\u0001\u0000"+
		"\u0000\u0000\u00ae\u00b0\u0005\u001e\u0000\u0000\u00af\u00a3\u0001\u0000"+
		"\u0000\u0000\u00af\u00a4\u0001\u0000\u0000\u0000\u00af\u00a5\u0001\u0000"+
		"\u0000\u0000\u00af\u00a6\u0001\u0000\u0000\u0000\u00af\u00a8\u0001\u0000"+
		"\u0000\u0000\u00af\u00aa\u0001\u0000\u0000\u0000\u00af\u00ae\u0001\u0000"+
		"\u0000\u0000\u00b0\u0011\u0001\u0000\u0000\u0000\u00b1\u00b6\u0005\u001c"+
		"\u0000\u0000\u00b2\u00b3\u0005\u000b\u0000\u0000\u00b3\u00b5\u0005\u001c"+
		"\u0000\u0000\u00b4\u00b2\u0001\u0000\u0000\u0000\u00b5\u00b8\u0001\u0000"+
		"\u0000\u0000\u00b6\u00b4\u0001\u0000\u0000\u0000\u00b6\u00b7\u0001\u0000"+
		"\u0000\u0000\u00b7\u0013\u0001\u0000\u0000\u0000\u00b8\u00b6\u0001\u0000"+
		"\u0000\u0000\u00b9\u00ba\u0005\u0015\u0000\u0000\u00ba\u0015\u0001\u0000"+
		"\u0000\u0000\u00bb\u00bf\u0005\u0016\u0000\u0000\u00bc\u00bf\u0005\u0017"+
		"\u0000\u0000\u00bd\u00bf\u0003>\u001f\u0000\u00be\u00bb\u0001\u0000\u0000"+
		"\u0000\u00be\u00bc\u0001\u0000\u0000\u0000\u00be\u00bd\u0001\u0000\u0000"+
		"\u0000\u00bf\u0017\u0001\u0000\u0000\u0000\u00c0\u00c9\u0005\u0018\u0000"+
		"\u0000\u00c1\u00c9\u0005\u0019\u0000\u0000\u00c2\u00c9\u0003:\u001d\u0000"+
		"\u00c3\u00c9\u00036\u001b\u0000\u00c4\u00c9\u0003.\u0017\u0000\u00c5\u00c9"+
		"\u00032\u0019\u0000\u00c6\u00c9\u0003*\u0015\u0000\u00c7\u00c9\u0003\u001a"+
		"\r\u0000\u00c8\u00c0\u0001\u0000\u0000\u0000\u00c8\u00c1\u0001\u0000\u0000"+
		"\u0000\u00c8\u00c2\u0001\u0000\u0000\u0000\u00c8\u00c3\u0001\u0000\u0000"+
		"\u0000\u00c8\u00c4\u0001\u0000\u0000\u0000\u00c8\u00c5\u0001\u0000\u0000"+
		"\u0000\u00c8\u00c6\u0001\u0000\u0000\u0000\u00c8\u00c7\u0001\u0000\u0000"+
		"\u0000\u00c9\u0019\u0001\u0000\u0000\u0000\u00ca\u00cc\u0005\u001a\u0000"+
		"\u0000\u00cb\u00cd\u0003\u001c\u000e\u0000\u00cc\u00cb\u0001\u0000\u0000"+
		"\u0000\u00cc\u00cd\u0001\u0000\u0000\u0000\u00cd\u00cf\u0001\u0000\u0000"+
		"\u0000\u00ce\u00d0\u0003@ \u0000\u00cf\u00ce\u0001\u0000\u0000\u0000\u00cf"+
		"\u00d0\u0001\u0000\u0000\u0000\u00d0\u00d2\u0001\u0000\u0000\u0000\u00d1"+
		"\u00d3\u00034\u001a\u0000\u00d2\u00d1\u0001\u0000\u0000\u0000\u00d2\u00d3"+
		"\u0001\u0000\u0000\u0000\u00d3\u001b\u0001\u0000\u0000\u0000\u00d4\u00d9"+
		"\u00030\u0018\u0000\u00d5\u00d9\u0003,\u0016\u0000\u00d6\u00d9\u00038"+
		"\u001c\u0000\u00d7\u00d9\u0003<\u001e\u0000\u00d8\u00d4\u0001\u0000\u0000"+
		"\u0000\u00d8\u00d5\u0001\u0000\u0000\u0000\u00d8\u00d6\u0001\u0000\u0000"+
		"\u0000\u00d8\u00d7\u0001\u0000\u0000\u0000\u00d9\u001d\u0001\u0000\u0000"+
		"\u0000\u00da\u00dc\u0007\u0000\u0000\u0000\u00db\u00da\u0001\u0000\u0000"+
		"\u0000\u00db\u00dc\u0001\u0000\u0000\u0000\u00dc\u001f\u0001\u0000\u0000"+
		"\u0000\u00dd\u00df\u0007\u0000\u0000\u0000\u00de\u00dd\u0001\u0000\u0000"+
		"\u0000\u00de\u00df\u0001\u0000\u0000\u0000\u00df!\u0001\u0000\u0000\u0000"+
		"\u00e0\u00e1\u0003\u0014\n\u0000\u00e1\u00e2\u0003\u001e\u000f\u0000\u00e2"+
		"#\u0001\u0000\u0000\u0000\u00e3\u00e5\u0003&\u0013\u0000\u00e4\u00e3\u0001"+
		"\u0000\u0000\u0000\u00e4\u00e5\u0001\u0000\u0000\u0000\u00e5\u00e6\u0001"+
		"\u0000\u0000\u0000\u00e6\u00e7\u0003\u0016\u000b\u0000\u00e7\u00e8\u0003"+
		"\u001e\u000f\u0000\u00e8%\u0001\u0000\u0000\u0000\u00e9\u00ea\u0005\u0005"+
		"\u0000\u0000\u00ea\u00eb\u0003 \u0010\u0000\u00eb\'\u0001\u0000\u0000"+
		"\u0000\u00ec\u00ed\u0003\u0018\f\u0000\u00ed\u00ee\u0003\u001e\u000f\u0000"+
		"\u00ee)\u0001\u0000\u0000\u0000\u00ef\u00f0\u0005\u0011\u0000\u0000\u00f0"+
		"+\u0001\u0000\u0000\u0000\u00f1\u00f2\u0005\u0011\u0000\u0000\u00f2-\u0001"+
		"\u0000\u0000\u0000\u00f3\u00f4\u0005\u0010\u0000\u0000\u00f4/\u0001\u0000"+
		"\u0000\u0000\u00f5\u00f6\u0005\u0010\u0000\u0000\u00f61\u0001\u0000\u0000"+
		"\u0000\u00f7\u00f8\u0005\f\u0000\u0000\u00f83\u0001\u0000\u0000\u0000"+
		"\u00f9\u00fa\u0005\f\u0000\u0000\u00fa5\u0001\u0000\u0000\u0000\u00fb"+
		"\u00fc\u0005\u000e\u0000\u0000\u00fc7\u0001\u0000\u0000\u0000\u00fd\u00fe"+
		"\u0005\u000e\u0000\u0000\u00fe9\u0001\u0000\u0000\u0000\u00ff\u0100\u0005"+
		"\r\u0000\u0000\u0100;\u0001\u0000\u0000\u0000\u0101\u0102\u0005\r\u0000"+
		"\u0000\u0102=\u0001\u0000\u0000\u0000\u0103\u0104\u0005\u000f\u0000\u0000"+
		"\u0104?\u0001\u0000\u0000\u0000\u0105\u0106\u0005\u000f\u0000\u0000\u0106"+
		"A\u0001\u0000\u0000\u0000\u0018DKNXgin\u0082\u0086\u008c\u008e\u009d\u00a1"+
		"\u00af\u00b6\u00be\u00c8\u00cc\u00cf\u00d2\u00d8\u00db\u00de\u00e4";
	public static final ATN _ATN =
		new ATNDeserializer().deserialize(_serializedATN.toCharArray());
	static {
		_decisionToDFA = new DFA[_ATN.getNumberOfDecisions()];
		for (int i = 0; i < _ATN.getNumberOfDecisions(); i++) {
			_decisionToDFA[i] = new DFA(_ATN.getDecisionState(i), i);
		}
	}
}