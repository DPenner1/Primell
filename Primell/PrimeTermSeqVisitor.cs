using Antlr4.Runtime.Misc;

namespace dpenner1.Primell
{
    class PrimeTermSequenceVisitor : PrimellBaseVisitor<PLObject>
    {
        private PrimeProgramControl control;
        private Stack<PLObject> placeholders;
        private Stack<PLObject> currentForEach;
        private Stack<bool> incorporate;

        public PrimeTermSequenceVisitor(PrimeProgramControl control)
        {
            this.control = control;
            placeholders = new Stack<PLObject>();
            currentForEach = new Stack<PLObject>();
            incorporate = new Stack<bool>();
        }

        public override PLObject VisitLine([NotNull] PrimellParser.LineContext context)
        {
            return Visit(context.termSeq());
        }

        public override PLObject VisitParens([NotNull] PrimellParser.ParensContext context)
        {
            incorporate.Pop();
            incorporate.Push(false);

            if (context.termSeq() == null) return PLObject.Empty;
            return Visit(context.termSeq());
        }

        public override PLObject VisitTermSeq([NotNull] PrimellParser.TermSeqContext context)
        {
            // There was a hack here to fix list initialization.
            // We use the incorporate stack to see if the object should be incorporated directly in
            // i.e. its objects added to the current term instead of itself being added as a single term
            // To do so, used a stack to keep track of this, but then had to make Add() AddRange() Reduce() available here.
            // Theoretically there might be a way to get reduce to work within PLObject's Add/AddRange, not yet done though.

            var terms = new PLObject();
            foreach (var termContext in context.mulTerm())
            {
                incorporate.Push(true);
                var plobj = Visit(termContext);

                if (incorporate.Pop())
                {
                    terms.AddRange(plobj);
                }
                else terms.Add(plobj);
            }
            return terms.Reduce();
        }

        public override PLObject VisitInteger([NotNull] PrimellParser.IntegerContext context)
        {
            var number = ParseLib.ParseInteger(context.GetText(), control.Settings.SourceBase);
            if (!control.Settings.FreeSource && !PrimeLib.IsPrime(number))
                throw new ArgumentException("NON-PRIME DETECTED");

            return number;
        }

        public override PLObject VisitPositiveInfinity([NotNull] PrimellParser.PositiveInfinityContext context)
        {
            return PLNumber.PositiveInfinity;
        }

        /* Coming back to this in years, I cannot figure out what Reference was meant to be, but it seems interesting...
           all i know from old files is that REF is meant to be atomTerm
        public override PLObject VisitReference([NotNull] PrimeParser.ReferenceContext context)
        {
            if (placeholders.Count == 0) throw new InvalidOperationException("Cannot use reference outside of binary operation");

            return placeholders.Peek();
        }*/

        public override PLObject VisitNullaryOp([NotNull] PrimellParser.NullaryOpContext context)
        {
            switch (context.baseNullaryOp().GetText())
            {
                case ",":
                    return control.EmptyVariable;
                case ";":
                    return control.InfEmptyVariable;
                case "#":
                    return control.InfNumberVariable;
                case ":_":
                    return control.GetListInput();
                case ":~":
                    return control.GetStringInput();
                default: throw new NotImplementedException("Unimplemented Operator");
            }
        }

        public override PLObject VisitNumericUnaryOperation([NotNull] PrimellParser.NumericUnaryOperationContext context)
        {
            Func<PLNumber, PLObject> operation = 
                x => new PLObject(NumUnaryOps[context.numUnaryOp().baseNumUnaryOp().GetText()].Invoke(x));
            var mods = new PLOperationOptions();

            control.LastOperationWasAssignment = false;
            return Visit(context.mulTerm()).NumericUnaryOperation(operation, mods);
        }
        

        public override PLObject VisitListUnaryOperation([NotNull] PrimellParser.ListUnaryOperationContext context)
        {
            return ExecuteListUnaryOperation(Visit(context.mulTerm()), context.listUnaryOp());
        }

        public override PLObject VisitBinaryOperation([NotNull] PrimellParser.BinaryOperationContext context)
        {
            var left = Visit(context.mulTerm());
            placeholders.Push(left.DeepCopy());
            PLObject right;

            if (context.termSeq() != null) right = Visit(context.termSeq());
            else right = Visit(context.atomTerm());

            placeholders.Pop();

            return ExecuteBinaryOperation(left, right, context.binaryOp());
        }

        // structured this way cause of historical reason
        private PLObject NumericBinaryOperation(PLObject left, PLObject right, string opText, PLOperationOptions options)
        {
            Func<PLNumber, PLNumber, PLObject> operation;

            switch (opText)
            {
                case "..":
                    operation = (x, y) => PrimeLib.PrimeRange(x, y, false);
                    break;
                case "…":
                    operation = (x, y) => PrimeLib.PrimeRange(x, y, true);
                    break;
                case "[..":
                    operation = (x, y) => new PLObject(new IncPLGenerator(x, y));
                    break;
                default: // operation implemented by PLNumber
                    if (!NumBinaryOps.ContainsKey(opText)) throw new NotImplementedException("Unimplemented Operator");
                    operation = (x, y) => NumBinaryOps[opText].Invoke(x, y);
                    break;
            }

            return left.NumericBinaryOperation(right, operation, options);
        }

        // structured this way cause of historical reason
        private PLObject ListBinaryOperation(PLObject left, PLObject right, string opText, PLOperationOptions options)
        {
            switch (opText)
            {
                case "?": return Conditional(left, right, false);
                case "?~": return Conditional(left, right, true);
                case "?/": return ConditionalBranch(left, right, false, false);
                case "?\\": return ConditionalBranch(left, right, false, true);
                case "?~/": return ConditionalBranch(left, right, true, false);
                case "?~\\": return ConditionalBranch(left, right, true, true);
                case "@": return left.ListNumberOperation(left.Index, right, options);
                case "@#": return left.IndexOf(right);
                case ">>": return left.ListNumberOperation(left.ShiftRight, right, options);
                case "<<": return left.ListNumberOperation(left.ShiftLeft, right, options);
                case "\\": return left.ListDifference(right);
                default: throw new NotImplementedException("Unimplemented Operator");
            }
        }

        public override PLObject VisitForEachLeftTerm([NotNull] PrimellParser.ForEachLeftTermContext context)
        {
            currentForEach.Push(Visit(context.mulTerm()));
            Visit(context.forEachBlock());
            return currentForEach.Pop();
        }

        public override PLObject VisitForEachRightTerm([NotNull] PrimellParser.ForEachRightTermContext context)
        {
            var retval = new List<PLObject>();
            var left = Visit(context.mulTerm());
            placeholders.Push(left.DeepCopy());

            foreach (var plobj in Visit(context.termSeq()))
            {
                retval.Add(ExecuteBinaryOperation(left, plobj, context.binaryOp()));
            }

            placeholders.Pop();
            return new PLObject(retval);
        }

        public override PLObject VisitForEachBinary([NotNull] PrimellParser.ForEachBinaryContext context)
        {
            var values = new List<PLObject>();
            foreach (var plobj in currentForEach.Pop())
            {
                var left = plobj;
                placeholders.Push(left.DeepCopy());

                PLObject right;
                if (context.termSeq() != null) right = Visit(context.termSeq());
                else right = Visit(context.atomTerm());
                placeholders.Pop();

                values.Add(ExecuteBinaryOperation(left, right, context.binaryOp()));
            }

            var retval = new PLObject(values);
            currentForEach.Push(retval);
            return retval;
        }

        public override PLObject VisitForEachNumericUnary([NotNull] PrimellParser.ForEachNumericUnaryContext context)
        {
            // TODO - copied  from VisitNumericUnary
            Func<PLNumber, PLObject> operation =
                x => new PLObject(NumUnaryOps[context.numUnaryOp().baseNumUnaryOp().GetText()].Invoke(x));
            var mods = new PLOperationOptions();

            control.LastOperationWasAssignment = false; 

            // No need for fancy logic, numeric unary operations have foreach semantics by default
            var retval = currentForEach.Pop();
            currentForEach.Push(retval.NumericUnaryOperation(operation, mods));
            return retval;
        }

        public override PLObject VisitForEachListUnary([NotNull] PrimellParser.ForEachListUnaryContext context)
        {
            // TODO - copied  from VisitListUnary
            var values = new List<PLObject>();
            foreach (var plobj in currentForEach.Pop())
            {
                values.Add(ExecuteListUnaryOperation(plobj, context.listUnaryOp()));
            }

            var retval = new PLObject(values);
            currentForEach.Push(retval);
            return retval;
        }

        private PLObject ExecuteListUnaryOperation(PLObject plobj, PrimellParser.ListUnaryOpContext context)
        {
            var options = ParseLib.ParseOptions(context.opMods()?.GetText());

            control.LastOperationWasAssignment = false;
            switch (context.baseListUnaryOp().GetText())
            {
                case "_<": return plobj.Head();
                case "_>": return plobj.Tail();
                //case "_?": return value.Purge();
                case "_*": return plobj.Distinct(options.Power);
                case "_~": return plobj.Reverse(options.Power);
                case "__": return plobj.Flatten(options.Power);
                case "_@": return plobj.Sort(options.Power);
                default: throw new NotImplementedException("Unimplemented Operator");
            }
        }

        private PLObject ExecuteBinaryOperation(PLObject left, PLObject right, PrimellParser.BinaryOpContext context)
        {
            bool isAssign = context.ASSIGN() != null;
            var assignOptions = ParseLib.ParseOptions(context.assignMods()?.GetText());
            var opOptions = ParseLib.ParseOptions(context.opMods()?.GetText());

            var result = right;

            if (context.baseNumBinaryOp() != null)
                result = NumericBinaryOperation(left, right, context.baseNumBinaryOp().GetText(), opOptions);
            else if (context.baseListBinaryOp() != null)
                result = ListBinaryOperation(left, right, context.baseListBinaryOp().GetText(), opOptions);

            if (isAssign)
                result = left.Assign(result, assignOptions);

            control.LastOperationWasAssignment = isAssign;

            return result;
        }

        private PLObject ConditionalBranch(PLObject left, PLObject right, bool negative, bool jumpBack)
        {
            bool truth = IsTruthValue(left);
            if (truth && !negative || !truth && negative)
            {
                var retval = new PLObject();
                foreach (var plobj in right.Head())
                {
                    if (plobj.IsAtomic)
                    {
                        var num = plobj.Atom.Value;
                        int offset = (int)(num.IsInteger ? num : PrimeLib.RoundToPrime(num));
                        if (jumpBack) offset = -offset;

                        retval = control.ExecuteRelativeLine(offset);
                    }
                    else
                    {
                        // TODO - Actually test this portion of code
                        foreach (var nestedobj in plobj)
                        {
                            //retval.Add(ConditionalBranch(left, nestedobj, negative, jumpBack));
                            throw new NotImplementedException("Nested lists in conditional head not yet implemented");
                        }
                    }       
                }
                return retval;
            }

            return right.Tail();
        }

        private PLObject Conditional(PLObject left, PLObject right, bool negative)
        {
            var truth = IsTruthValue(left);
            if (truth && !negative || !truth && negative) return right.Head();
            return right.Tail();
        }

        private bool IsTruthValue(PLObject plobj)
        {
            var def = control.Settings.TruthDefinition;

            if (plobj.IsEmpty)
                return def.EmptyIsTrue;

            if (plobj.IsAtomic)
            {
                if (def.UsePrimeBoolean)
                    return PrimeLib.IsPrime(plobj.Atom.Value);
                else
                    return !plobj.IsZero;
            }

            // infinite lists may hang
            foreach (var nestedobj in plobj)
            {
                var truthValue = IsTruthValue(nestedobj);
                if (truthValue && !def.RequireAllTrue) return true;
                if (!truthValue && def.RequireAllTrue) return false;
            }

            // We have either an all true list or all false list now.
            return def.RequireAllTrue;
        }

        

        public static Dictionary<string, Func<PLNumber, PLNumber, PLNumber>> NumBinaryOps
            = new Dictionary<string, Func<PLNumber, PLNumber, PLNumber>>
        {
            { "+", PLNumber.Add },
            { "-", PLNumber.Subtract },
            { "/", PLNumber.Divide },
            { "*", PLNumber.Multiply },
            //{ "**", PLNumber.Pow },
            //{ "//", PLNumber.Log },
            //{ "%", PLNumber.Mod },
            { "<", PLNumber.Min },
            { ">", PLNumber.Max },
        };

        public static Dictionary<string, Func<PLNumber, PLNumber>> NumUnaryOps
            = new Dictionary<string, Func<PLNumber, PLNumber>>
            {
                { "++", PrimeLib.NextHighestPrime },
                { "--", PrimeLib.NextLowestPrime },
                { "+-", PrimeLib.RoundToPrime },
                { "~", PLNumber.Negate },
                //{ "!", PLNumber.Factorial },
            };

        
    }

    public class PLOperationOptions
    {
        public bool Cut { get; set; }
        public bool Power { get; set; }

        public PLOperationOptions() { }
        public PLOperationOptions(PLOperationOptions options)
        {
            Cut = options.Cut;
            Power = options.Power;
        }
    }
}
