using System.Collections;

namespace dpenner1.Primell
{
    [System.Diagnostics.DebuggerDisplay("{DebugString}")]
    public class PLObject : IEnumerable<PLObject>, IEquatable<PLObject>, IComparable<PLObject>
    {
        public static PLObject Empty => new PLObject();

        List<PLObject> Values;
        PLGenerator gen;

        public PLNumber? Atom { get; private set; }

        public bool IsEmpty => Values.Count == 0 && Atom == null;
        public bool IsAtomic => Atom != null;
        public bool IsZero => IsAtomic && Atom.Value.IsZero;

        public PLNumber Count
        {
            get
            {
                if (IsAtomic) return PLNumber.One;

                PLNumber retval = Values.Count;
                if (gen != null)
                {
                    if (gen.Count.IsNaN) GenerateToEnd();
                    retval += gen.Count;
                }
                
                return retval;
            }
        }

        private string DebugString
        {
            // base hashcode is object.hashcode, i.e. based on the address of object
            // Helps for debugging reference stuff
            get
            {
                if (IsAtomic) return Atom.Value.ToString(10) + "@" + base.GetHashCode();

                var retval = "(";
                foreach (var plobj in Values)
                {
                    retval += plobj.DebugString + " ";
                }
                retval = retval.TrimEnd();

                if (HasActiveGenerator)
                    retval += " ...";

                retval += ")";

                return retval + "@" + base.GetHashCode();
                
            }           
        }

        public PLObject()
        {
            Values = new List<PLObject>();
        }

        public PLObject(PLNumber number)
        {
            Atom = number;
            Values = new List<PLObject>();
        }

        public PLObject(List<PLObject> plobjs)
        {
            Values = new List<PLObject>();
            foreach(var plobj in plobjs)
            {
                Values.Add(plobj);
            }

            Reduce();
        }    

        internal PLObject(PLGenerator generator)
        {
            Values = new List<PLObject>();
            gen = generator;

            Reduce();
        }

        #region Operations

        public PLObject NumericUnaryOperation(Func<PLNumber, PLObject> op, PLOperationOptions options)
        {
            var retval = new PLObject();

            if (IsAtomic) retval = op.Invoke(Atom.Value);
            else
            {
                foreach (var plobj in Values)
                {
                    retval.Values.Add(plobj.NumericUnaryOperation(op, options));
                }
                if (gen != null)
                {
                    retval.gen = new UnaryNumericCombinedPLGenerator(gen, op, options);
                }
            }          
            
            return retval.Reduce();
        }

        public PLObject NumericBinaryOperation(PLObject other, Func<PLNumber, PLNumber, PLObject> op, PLOperationOptions options)
        {
            var retval = new PLObject();

            if (IsAtomic && other.IsAtomic) retval = op.Invoke(Atom.Value, other.Atom.Value);
            else if (IsAtomic)
            {
                if (options.Cut)
                {
                    if (other.IsEmpty) return Empty;

                    var newOptions = new PLOperationOptions(options);
                    newOptions.Cut = false; // not super-recursive

                    return NumericBinaryOperation(other.Values[0], op, newOptions);
                }
                else
                {
                    foreach (var plobj in other.Values)
                        retval.Values.Add(NumericBinaryOperation(plobj, op, options));

                    if (other.gen != null)
                        retval.gen = new BinaryNumericCombinedPLGenerator(this, other.gen, op, options);
                }
            }
            else if (other.IsAtomic)
            {
                if (options.Cut)
                {
                    if (IsEmpty) return Empty;

                    var newOptions = new PLOperationOptions(options);
                    newOptions.Cut = false; // not super-recursive

                    return Values[0].NumericBinaryOperation(other, op, newOptions);
                }
                else
                {
                    foreach (var plobj in Values)
                        retval.Values.Add(plobj.NumericBinaryOperation(other, op, options));

                    if (gen != null)
                        retval.gen = new BinaryNumericCombinedPLGenerator(gen, other, op, options);
                }        
            }

            // list and list operation
            else if (options.Power)
            {
                var newOptions = new PLOperationOptions(options);
                newOptions.Power = false; // not super-recursive

                GenerateToEnd();
                other.GenerateToEnd();

                foreach (var left in Values)
                {
                    var row = new PLObject();
                    foreach (var right in other.Values)
                    {
                        row.Values.Add(left.NumericBinaryOperation(right, op, newOptions));                        
                    }
                    retval.Values.Add(row.Reduce());
                }
            }
            else
            {
                // Try to make Values equal lengths (easier logic that way -  at least I thought that at one point)
                while (gen != null && gen.HasNext() && Values.Count < other.Values.Count) GenerateNextValue();
                while (other.gen != null && other.gen.HasNext() && Values.Count > other.Values.Count) other.GenerateNextValue();

                var minCount = Math.Min(Values.Count, other.Values.Count);
                for (int i = 0; i < minCount; i += 1)
                {
                    retval.Values.Add(
                        Values[i].NumericBinaryOperation(other.Values[i], op, options)
                    );
                }

                if (Values.Count > other.Values.Count && !options.Cut)
                {
                    for (int i = minCount; i < Values.Count; i++)
                    {
                        retval.Values.Add(Values[i]);
                    }
                    retval.gen = gen?.DeepCopy();
                }
                else if (Values.Count < other.Values.Count && !options.Cut)
                {
                    for (int i = minCount; i < other.Values.Count; i++)
                    {
                        retval.Values.Add(other.Values[i]);
                    }
                    retval.gen = other.gen?.DeepCopy();
                }
                
                if (Values.Count == other.Values.Count) // combine the generators
                {
                    if (!HasActiveGenerator) retval.gen = other.gen?.DeepCopy();
                    else if (!other.HasActiveGenerator) retval.gen = gen?.DeepCopy();
                    else retval.gen = new BinaryNumericCombinedPLGenerator(gen, other.gen, op, options);
                }
            }

            return retval.Reduce();
        }


        public PLObject Assign(PLObject value, PLOperationOptions options)
        {
            if (IsEmpty || IsAtomic || options.Power) // replace self with value
            {
                var copy = value.DeepCopy();
                Values = copy.Values;
                gen = copy.gen;
                Atom = copy.Atom;

                Reduce();
            }
            else if (value.IsAtomic) // all items in self becomes the atomic value
            {
                for (PLNumber i = 0; i < Values.Count; i += 1)
                {
                    //Values[(int)i] = new PLObject(value.Atom.Value);
                    if (Values[(int)i].IsAtomic)
                        Values[(int)i].Atom = value.Atom;
                    else
                        Values[(int)i].Assign(value.Atom.Value, options);
                }

                if (gen != null)
                {
                    gen = new ConstantPLGenerator(new PLObject(value.Atom.Value), gen.Count);
                }
            }
            else 
            {
                // list to list assignment: parallel assignment
                // if value is bigger than self, the extra values are ignored
                // if self is bigger than value, extra values are left intact

                // The minute I wrote this code I should have commented how it works.
                for (PLNumber i = 0; i < PLNumber.Min(Values.Count, value.Count); i += 1)
                {
                    if (i >= value.Values.Count) value.GenerateNextValue();

                    var left = Values[(int)i];
                    var right = value.Values[(int)i].DeepCopy();

                    if (left.IsAtomic) {
                        Values[(int)i].Atom = right.Atom;
                        Values[(int)i].Values = right.Values;
                        Values[(int)i].gen = right.gen;
                    } 
                    else left.Assign(right, options);
                }
            }

            return this;
        }

        public PLObject ListDifference(PLObject valuesToRemove)
        {
            if (Count.IsInfinity && valuesToRemove.Count.IsInfinity) throw new InvalidOperationException();

            PLObject retval = this;

            foreach (var toRemove in valuesToRemove)
            {
                retval = retval.RemoveValue(toRemove);
            }
            return retval;
        }

        private PLObject RemoveValue(PLObject toRemove)
        {
            if (IsAtomic)
            {
                if (Atom == toRemove.Atom) return Empty;
                return this;
            }

            var retval = new PLObject();
            bool valueFound = false;
            foreach (var plobj in Values)
            {
                if (!valueFound && plobj.Equals(toRemove)) valueFound = true;
                else retval.Values.Add(plobj.DeepCopy());
            }

            retval.gen = gen?.DeepCopy();
            while (!valueFound && retval.HasActiveGenerator)
            {
                var next = retval.GenerateNextValue();
                if (next.Equals(toRemove)) valueFound = true;
            }

            return retval.Reduce();
        }

        public PLObject IndexOf(PLObject value)
        {
            InfiniteCheck();

            var retval = new PLObject();

            int i = 0;
            foreach (var plobj in this)
            {
                if (plobj.Equals(value)) retval.Values.Add((PLNumber)i);
                i++;
            }

            return retval.Reduce();
        }

        #region List Unary

        public PLObject Head()
        {
            if (IsEmpty) return Empty;
            if (IsAtomic) return this;

            TryGenerateNextValue(); // in case none generated yet
            return Values[0];
        }

        public PLObject Tail()
        {
            TryGenerateNextValue(); // Avoid checking nextvalues count (in case it is NaN, this may result in full generation)
            TryGenerateNextValue();
            if (Values.Count <= 1) return Empty;

            var retval = new PLObject();
            for (int i = 1; i < Values.Count; i++) retval.Values.Add(Values[i]);
            retval.gen = gen?.DeepCopy();  // TODO - not certain deep copy here is correct

            return retval.Reduce();
        }

        public PLObject Reverse(bool recursive)
        {
            GenerateToEnd();
            var retval = DeepCopy();
            retval.Values.Reverse();

            if (recursive)
            {
                foreach (var plobj in retval.Values) plobj.Values = plobj.Reverse(recursive).Values;
            }

            return retval;
        }

        public PLObject Flatten(bool recursive)
        {
            GenerateToEnd();
            if (IsAtomic) return this;

            var retval = new PLObject();
            foreach (var plobj in this)
            {
                if (plobj.IsAtomic) retval.Values.Add(plobj);
                else
                {
                    foreach (var nestedobj in plobj)
                    {
                        if (!recursive || nestedobj.IsAtomic)
                            retval.Values.Add(nestedobj.DeepCopy());
                        else retval.Values.AddRange(nestedobj.Flatten(recursive).Values);
                    }
                }
            }

            return retval;
        }

        public PLObject Sort(bool recursive)
        {
            GenerateToEnd();
            if (IsAtomic) return this;

            var retval = new PLObject(DeepCopy().Values.OrderBy(x => x.Count).ThenBy(x => x.Atom).ToList());

            if (recursive)
            {
                foreach (var plobj in retval.Values)
                {
                    plobj.Values = plobj.Sort(recursive).Values;
                }
            }

            return retval;
        }

        public PLObject Distinct(bool recursive)
        {
            GenerateToEnd();
            if (IsAtomic) return this;      

            if (recursive)
            {
                var values = new List<PLObject>();
                foreach (var plobj in Values)
                {
                    foreach (var unique in plobj.Distinct(recursive))
                    {
                        if (!values.Contains(unique)) values.Add(unique.DeepCopy());
                    }
                }

                return new PLObject(values);
            }
            else
            {
                var retval = DeepCopy();
                retval.Values = retval.Values.Distinct().ToList();
                return retval.Reduce();
            }
        }

        #endregion List Unary

        #region List x Number

        public PLObject ListNumberOperation(Func<PLNumber, PLObject> op, PLObject other, PLOperationOptions options)
        {
            other.InfiniteCheck();

            var retval = new PLObject();
            foreach (var i in other)
            {
                if (i.IsAtomic) retval.Values.Add(op.Invoke(i.Atom.Value));
                else retval.Values.Add(ListNumberOperation(op, i, options));
            }

            // Kind of like reduce. But can't call it because we need to be referentially transparent.
            // i.e. Retval must be the exact same object when atomic, not the wrapper object
            // This block of comments is probably not explained that well
            while (retval.Values.Count == 1) retval = retval.Values[0];

            return retval;
        }

        public PLObject Index(PLNumber index)
        {
            if (index.IsNaN) return Empty;
            index = ToInteger(index);

            // Use generator to generate up to needed value
            while (gen != null && gen.HasNext() && Values.Count <= index) GenerateNextValue();

            // Indexing off the end of the list, implicitly fill with empties.
            while (Values.Count <= index) Values.Add(Empty);

            return Values[(int)index.Numerator];
        }

        public PLObject ShiftRight(PLNumber positions)
        {
            if (positions.IsNaN) return this;

            positions = ToInteger(positions);
            if (positions == 0) return this;
            if (positions < 0) return ShiftLeft(PLNumber.Negate(positions));

            var retval = DeepCopy();
            if (IsAtomic) retval.Values.Add(new PLObject(Atom.Value));

            for (PLNumber i = 0; i < positions; i += 1)
            {
                retval.Values.Insert(0, Empty);
            }
            
            return retval;
        }   

        public PLObject ShiftLeft(PLNumber positions)
        {
            if (positions.IsNaN) return this;

            positions = ToInteger(positions);
            if (positions == 0) return this;
            if (positions < 0) return ShiftRight(-positions);

            var retval = DeepCopy();
            retval.Atom = null;

            for (PLNumber i = 0; i < positions; i += 1)
            {
                TryGenerateNextValue();
                if (Values.Count == 0) break;
                retval.Values.RemoveAt(0);
            }

            return retval;
        }

        #endregion List x Number

        #endregion Operations

        #region Public helpers

        public PLObject DeepCopy()
        {
            var retval = new PLObject();
            foreach (var plobj in Values) retval.Values.Add(plobj.DeepCopy());
            retval.gen = gen?.DeepCopy();
            retval.Atom = Atom; // immutable object
            return retval;
        }

        public void Add(PLObject plobj)
        {
            if (IsAtomic) {
                Values.Add(Atom.Value);
                Atom = null;
            }

            if (HasActiveGenerator)
            {
                gen.Append(new PresetPLGenerator(plobj));
            }
            else
            {
                Values.Add(plobj);
            }
        }

        public void AddRange(PLObject plobj)
        {
            if (plobj.IsAtomic) Add(plobj);
            else
            {
                if (HasActiveGenerator && plobj.Values.Count > 0)
                {
                    gen.Append(new PresetPLGenerator(plobj.Values));
                    if (plobj.HasActiveGenerator) gen.Append(plobj.gen.DeepCopy());
                }
                else
                {
                    Values.AddRange(plobj.Values);
                    gen = plobj.gen?.DeepCopy();
                }
            }
        }

        public static implicit operator PLObject(PLNumber num) => new PLObject(num);

        #endregion Public helpers

        #region Private helpers

        private PLNumber ToInteger(PLNumber value)
        {
            if (value.IsInfinity) throw new InvalidOperationException("Attempted to compute infinitely");
            return PLNumber.RoundToInteger(value);
        }

        internal PLObject Reduce()
        {
            // Try to get two values generated
            TryGenerateNextValue();
            TryGenerateNextValue();

            if (Values.Count == 1)
            {               
                var value = Values[0];

                Values = value.Values;
                gen = value.gen;
                Atom = value.Atom;

                return value.Reduce();  // If i've coded stuff right, this is unnecessary - but doesn't hurt
            }

            return this;
        }

        private void InfiniteCheck()
        {
            if (gen != null && gen.Count.IsPositiveInfinity)
                throw new InvalidOperationException("Tried to compute infinitely");
        }

        private void GenerateToEnd()
        {
            InfiniteCheck();
            while (TryGenerateNextValue());
        }

        private bool TryGenerateNextValue()
        {
            if (!HasActiveGenerator) return false;
            GenerateNextValue();
            return true;
        }

        private PLObject GenerateNextValue()
        {
            var value = gen.Next();
            Values.Add(value);
            return value;
        }

        private bool HasActiveGenerator => gen != null && gen.HasNext();

        #endregion Private helpers

        #region Overrides and implementations

        public string ToString(int @base)
        {
            if (IsAtomic) return Atom.Value.ToString(@base);

            GenerateToEnd();

            var retval = "(";
            foreach (var plobj in Values)
            {
                retval += plobj.ToString(@base) + " ";
            }

            return retval.TrimEnd() + ")";
        }

        public override bool Equals(object obj)
        {
            var plobj = obj as PLObject;
            if (plobj == null) return false;
            return Equals(plobj);
        }

        public bool Equals(PLObject other)
        {
            if (Atom != other.Atom) return false; // TODO - NaN?

            // Try to make Values equal lengths
            while (gen != null && gen.HasNext() && Values.Count < other.Values.Count) GenerateNextValue();
            while (other.gen != null && other.gen.HasNext() && Values.Count > other.Values.Count) other.GenerateNextValue();

            // Counts don't match, they can't be equal
            if (Values.Count != other.Values.Count) return false;

            // Try to find unequal value
            for (int i = 0; i < Values.Count; i++)
            {
                if (!Values[i].Equals(other.Values[i])) return false;
            }

            // no more values to generate, must be equal
            if (gen == null || !gen.HasNext() && other.gen == null || !other.gen.HasNext()) return true;

            // Try to avoid generating by checking counts
            if (!gen.Count.IsNaN && !other.gen.Count.IsNaN && gen.Count != other.gen.Count) return false;

            while (true) // might loop forever if they are equal
            {
                if (!GenerateNextValue().Equals(other.GenerateNextValue())) return false;
            }

            //Values.so
        }

        // Equal values must return the same hash, but can't necessarily compute to infinity to find this out.
        // So we arbitrarily stop at 100. This ensures all equal lists have same hashcode, though there may
        // collisions for lists with more than 100 values
        public override int GetHashCode()
        {
            const int QuantityToHash = 100;

            if (IsAtomic) return Atom.Value.GetHashCode();
            int hash = 0x12345678;

            for (int i = 0; i < QuantityToHash; i++)
            {
                if (Values.Count < QuantityToHash)
                {
                    if (!TryGenerateNextValue()) return hash;
                }
                hash ^= Values[i].GetHashCode(); // don't know if ^ is good here.
            }
            return hash;
        }

        public IEnumerator<PLObject> GetEnumerator()
        {
            if (IsAtomic) yield return Atom.Value;
            else
            {
                foreach (var plobj in Values) yield return plobj;
                while (gen != null && gen.HasNext()) yield return GenerateNextValue();
            }         
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // should result in order by length, then by value (for atomic objects)
        public int CompareTo(PLObject other)
        {
            var countCompare = Count.CompareTo(other.Count);

            if (countCompare == 0 && IsAtomic && other.IsAtomic)
                return Atom.Value.CompareTo(other.Atom.Value);

            return countCompare;
        }

        #endregion Overrides and implementations
    }
}
