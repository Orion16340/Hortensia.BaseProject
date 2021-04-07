using System;
using System.Linq;

namespace Hortensia.Core.Extensions
{
    public static class ArrayExtensions
    {
        public static string ToString<T>(this T[] source, string separator) where T : IConvertible => string.Join(separator, source);

        public static T[] Clone<T>(this T[] arrayToClone) where T : ICloneable => arrayToClone.Select(item => (T)item.Clone()).ToArray();

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        public static T[] RemoveAt<T>(this T[] source, int index)
        {
            T[] dest = new T[source.Length - 1];

            if (index > 0)
                Array.Copy(source, 0, dest, 0, index);

            if (index < source.Length - 1)
                Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

            return dest;
        }
    }
}
