namespace dpenner1.Primell
{
    public abstract class PLGenerator
    {
        public virtual PLNumber Count // null/NaN indicates unknown 
        {
            get; protected set;
        }

        public virtual bool HasNext()
        {
            if (Count == null || Count.IsNaN) throw new InvalidOperationException();
            return Count > 0;
        }
        public abstract PLObject Next();

        public abstract PLGenerator DeepCopy();

        public PLGenerator Append(PLGenerator secondGenerator)
        {
            if (secondGenerator.Count.IsPositiveInfinity) return DeepCopy(); // can't ever reach second generator anyway

            return new SequencedPLGenerator(this, secondGenerator);
        }
    }

    class ConstantPLGenerator : PLGenerator
    {
        PLObject value;

        public ConstantPLGenerator(PLObject value, PLNumber count)
        {
            if (count.IsNaN) throw new ArgumentException();
            if (count < 0) throw new ArgumentException();

            this.value = value.DeepCopy();
            Count = count;
        }

        public override PLObject Next()
        {
            if (!HasNext()) throw new InvalidOperationException();
            Count -= 1;

            return value.DeepCopy();
        }

        public override PLGenerator DeepCopy()
        {
            return new ConstantPLGenerator(value, Count);
        }
    }

    class PrimePLGenerator : PLGenerator
    {
        PLNumber lastGenerated;
        PLNumber end;
        public PrimePLGenerator(PLNumber start, PLNumber end)
        {
            if (start.IsPositiveInfinity) throw new InvalidOperationException("Cannot start at infinity");
            if (start.IsNaN) throw new InvalidOperationException();
            if (end.IsNaN) throw new InvalidOperationException();

            if (start <= 2) lastGenerated = 1;
            else if (PrimeLib.IsPrime(start)) lastGenerated = start - 1;
            else lastGenerated = start;

            this.end = end;
            if (end.IsPositiveInfinity) Count = end;
            else Count = PLNumber.NaN; //unknown
        }

        public override bool HasNext()
        {
            return PrimeLib.NextHighestPrime(lastGenerated) < end;
        }
        public override PLObject Next()
        {
            if (!HasNext()) throw new InvalidOperationException();
            lastGenerated = PrimeLib.NextHighestPrime(lastGenerated);
            Count -= 1;

            return new PLObject(lastGenerated);
        }

        public override PLGenerator DeepCopy()
        {
            var value = new PrimePLGenerator(lastGenerated + 1, end); // +1 is hack for poor coding
            value.Count = Count;
            return value;
        }
    }

    class IncPLGenerator : PLGenerator
    {
        PLNumber lastGenerated;
        public IncPLGenerator(PLNumber start, PLNumber end)
        {
            if (start.IsInfinity) throw new InvalidOperationException("Cannot start at infinity");
            if (start.IsNaN) throw new InvalidOperationException();
            if (end.IsNaN) throw new InvalidOperationException();

            this.lastGenerated = PLNumber.Ceiling(start) - 1;
            Count = end - start;
        }

        public override PLObject Next()
        {
            lastGenerated += 1;
            Count -= 1;
            return lastGenerated;
        }

        public override PLGenerator DeepCopy()
        {
            return new IncPLGenerator(lastGenerated, lastGenerated + Count);
        }
    }
    
    class PresetPLGenerator : PLGenerator
    {
        List<PLObject> toGenerate = new List<PLObject>();
        int currentIndex;

        public PresetPLGenerator(PLObject singleValueToGenerate)
        {
            toGenerate.Add(singleValueToGenerate);
            currentIndex = 0;
            Count = 1;
        }
        public PresetPLGenerator(List<PLObject> valuesToGenerate)
        {
            foreach (var plobj in valuesToGenerate)
                toGenerate.Add(plobj.DeepCopy());

            currentIndex = 0;
            Count = toGenerate.Count;
        }

        public override PLObject Next()
        {
            var retval = toGenerate[currentIndex];
            currentIndex += 1;
            Count -= 1;
            return retval;
        }

        public override PLGenerator DeepCopy()
        {
            var retval = new PresetPLGenerator(toGenerate);
            retval.currentIndex = currentIndex;
            retval.Count = Count;
            return retval;
        }
    }
    
    class SequencedPLGenerator : PLGenerator
    {
        PLGenerator first;
        PLGenerator second;

        public override PLNumber Count
        {
            get
            {
                return first.Count + second.Count;
            }

            protected set
            {
                throw new InvalidOperationException();
            }
        }

        public SequencedPLGenerator(PLGenerator firstGenerator, PLGenerator secondGenerator)
        {
            first = firstGenerator.DeepCopy();
            second = secondGenerator.DeepCopy();
        }

        public override bool HasNext()
        {
            if (first.HasNext()) return true;
            return second.HasNext();
        }

        public override PLObject Next()
        {
            if (!HasNext()) throw new InvalidOperationException();
            if (first.HasNext()) return first.Next();
            return second.Next();
        }

        public override PLGenerator DeepCopy()
        {
            var retval = new SequencedPLGenerator(first, second);
            return retval;
        }
    }

    class BinaryNumericCombinedPLGenerator : PLGenerator
    {
        PLGenerator first;
        PLObject firstObj;
        PLGenerator second;
        PLObject secondObj;

        Func<PLNumber, PLNumber, PLObject> operation;
        PLOperationOptions options;

        public BinaryNumericCombinedPLGenerator(PLGenerator firstGenerator, PLGenerator secondGenerator,
            Func<PLNumber, PLNumber, PLObject> operation, PLOperationOptions options)
        {
            first = firstGenerator.DeepCopy();
            second = secondGenerator.DeepCopy();
            this.operation = operation;
            this.options = options;
        }

        public BinaryNumericCombinedPLGenerator(PLObject firstObj, PLGenerator secondGenerator,
            Func<PLNumber, PLNumber, PLObject> operation, PLOperationOptions options)
        {
            this.firstObj = firstObj.DeepCopy();
            second = secondGenerator.DeepCopy();
            this.operation = operation;
            this.options = options;
        }

        public BinaryNumericCombinedPLGenerator(PLGenerator firstGenerator, PLObject secondObj,
            Func<PLNumber, PLNumber, PLObject> operation, PLOperationOptions options)
        {
            first = firstGenerator.DeepCopy();
            this.secondObj = secondObj.DeepCopy();
            this.operation = operation;
            this.options = options;
        }

        public override PLNumber Count
        {
            get
            {
                if (firstObj != null) return second.Count;
                if (secondObj != null) return first.Count;
                return first.Count + second.Count;
            }

            protected set
            {
                throw new InvalidOperationException();
            }
        }

        public override PLGenerator DeepCopy()
        {
            if (firstObj != null) return new BinaryNumericCombinedPLGenerator(firstObj, second, operation, options);
            if (secondObj != null) return new BinaryNumericCombinedPLGenerator(first, secondObj, operation, options);
            return new BinaryNumericCombinedPLGenerator(first, second, operation, options);
        }
        public override bool HasNext()
        {
            if (firstObj != null) return second.HasNext();
            if (secondObj != null) return first.HasNext();

            if (options.Cut) return first.HasNext() && second.HasNext();
            return first.HasNext() || second.HasNext();
        }

        public override PLObject Next()
        {
            if (!HasNext()) throw new InvalidOperationException();

            PLObject left;
            if (firstObj != null) left = firstObj;
            else if (first.HasNext())
                left = first.Next();
            else // first generator ran out, just return second
                return second.Next();

            PLObject right;
            if (secondObj != null) right = secondObj;
            else if (second.HasNext())
                right = second.Next();
            else // second generator ran out, just return first
                return left; // note at this stage, first has already been generated, don't generate again

            return left.NumericBinaryOperation(right, operation, options);
        }
    }


    class UnaryNumericCombinedPLGenerator : PLGenerator
    {
        PLGenerator generator;
        Func<PLNumber, PLObject> operation;
        PLOperationOptions options;

        public UnaryNumericCombinedPLGenerator(PLGenerator generator, Func<PLNumber, PLObject> operation, PLOperationOptions options)
        {
            this.generator = generator.DeepCopy();
            this.operation = operation;
            this.options = options;
        }

        public override PLNumber Count
        {
            get
            {
                return generator.Count;
            }

            protected set
            {
                throw new InvalidOperationException();
            }
        }

        public override PLGenerator DeepCopy()
        {
            return new UnaryNumericCombinedPLGenerator(generator, operation, options);
        }
        public override bool HasNext()
        {
            return generator.HasNext();
        }

        public override PLObject Next()
        {
            return generator.Next().NumericUnaryOperation(operation, options);
        }
    }
}
