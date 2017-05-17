using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xrns2XMod.Extensions
{
    public static class Extensions
    {
        public static IEnumerable<IndexValuePair<T>> WithIndex<T>(this IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException("source");
            var position = 0;
            foreach (T value in source)
            {
                yield return new IndexValuePair<T>(position++, value);
            }
        }
    }

    public class IndexValuePair<T>
    {
        public IndexValuePair(Int32 index, T value)
        {
            this.index = index;
            this.value = value;
        }

        private readonly Int32 index;
        public Int32 Index
        {
            get { return index; }
        }

        private readonly T value;
        public T Value
        {
            get { return value; }
        }
    }
}
