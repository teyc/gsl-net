using System;
using System.Linq;
using System.Collections.Generic;

namespace Gsl.Infrastructure
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> Transpose<T>(this IEnumerable<IEnumerable<T>> list)
        {
            return
                list.First().Any()
                    // generate list of heads of lists   | 'concat' it with the rest of the matrix transposed
                    ? (new[] { list.Select(x => x.First()) }).Concat(Transpose(list.Select(x => x.Skip(1))))
                    : (IEnumerable<IEnumerable<T>>)new List<List<T>>();
        }
    }
}