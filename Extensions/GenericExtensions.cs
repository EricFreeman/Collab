using System;
using System.Collections.Generic;
using System.Linq;

namespace Extensions
{
    public static class GenericExtensions
    {
        public static bool In<T>(this T source, params T[] list)
        {
            return list.Contains(source);
        }

        public static void Each<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var thing in source)
            {
                action(thing);
            }
        }
    }
}