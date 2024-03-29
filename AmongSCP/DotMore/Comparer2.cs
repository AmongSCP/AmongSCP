﻿//https://github.com/mattmc3/dotmore/blob/master/dotmore/Collections/Generic/Comparer2.cs

using System;
using System.Collections.Generic;
using System.Linq;

namespace mattmc3.dotmore.Collections.Generic {
    public class Comparer2<T> : Comparer<T> {
        private readonly Comparison<T> _compareFunction;

        public Comparer2(Comparison<T> comparison) {
            if (comparison == null) throw new ArgumentNullException("comparison");
            _compareFunction = comparison;
        }

        public override int Compare(T arg1, T arg2) {
            return _compareFunction(arg1, arg2);
        }
    }
}