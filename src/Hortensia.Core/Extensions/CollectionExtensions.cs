using Hortensia.Core.Threads;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hortensia.Core.Extensions
{
    public static class CollectionExtensions
    {
        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable => listToClone.Select(item => (T)item.Clone()).ToList();

        public static string ToString(this IList collection, string separator) => collection != null ? string.Join(separator, ToStringArr(collection)) : "<unknown>";

        public static T GetOrDefault<T>(this IList<T> list, int index) => index >= list.Count ? default : list[index];

        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key) => dict.TryGetValue(key, out TValue val) ? val : default;

        public static string ByteArrayToString(this byte[] bytes)
        {
            var output = new StringBuilder(bytes.Length);

            for (int i = 0; i < bytes.Length; i++)
                output.Append(bytes[i].ToString("X2"));

            return output.ToString().ToLower();
        }

        public static string ByteArrayToString(this sbyte[] bytes)
        {
            var output = new StringBuilder(bytes.Length);

            for (int i = 0; i < bytes.Length; i++)
                output.Append(bytes[i].ToString("X2"));

            return output.ToString().ToLower();
        }

        public static bool CompareEnumerable<T>(this IEnumerable<T> ie1, IEnumerable<T> ie2)
        {
            if (ie1.GetType() != ie2.GetType())
                return false;

            var a1 = ie1.ToArray();
            var a2 = ie2.ToArray();

            if (a1.Length != a2.Length)
                return false;

            return !a1.Except(a2).Any() && !a2.Except(a1).Any();
        }

        public static T MaxOf<T, T1>(this IList<T> collection, Func<T, T1> selector) where T1 : IComparable<T1>
        {
            if (collection.Count == 0) return default;

            T maxT = collection[0];
            T1 maxT1 = selector(maxT);

            for (int i = 1; i < collection.Count; i++)
            {
                T1 currentT1 = selector(collection[i]);
                if (currentT1.CompareTo(maxT1) > 0)
                {
                    maxT = collection[i];
                    maxT1 = currentT1;
                }
            }
            return maxT;
        }

        public static T MinOf<T, T1>(this IList<T> collection, Func<T, T1> selector) where T1 : IComparable<T1>
        {
            if (collection.Count == 0) return default;

            T maxT = collection[0];
            T1 maxT1 = selector(maxT);

            for (int i = 1; i < collection.Count; i++)
            {
                T1 currentT1 = selector(collection[i]);
                if (currentT1.CompareTo(maxT1) < 0)
                {
                    maxT = collection[i];
                    maxT1 = currentT1;
                }
            }
            return maxT;
        }

        public static T RandomElementOrDefault<T>(this IEnumerable<T> enumerable)
        {
            AsyncRandom random = new();
            int count = enumerable.Count();

            if (count <= 0)
                return default;

            return enumerable.ElementAt(random.Next(count));
        }

        public static string[] ToStringArr(this IEnumerable collection)
        {
            List<string> strs = new();
            IEnumerator colEnum = collection.GetEnumerator();

            while (colEnum.MoveNext())
            {
                object cur = colEnum.Current;

                if (cur != null)
                    strs.Add(cur.ToString());
            }
            return strs.ToArray();
        }

        public static void MoveToLast<T>(this IList<T> list)
        {
            T item = list.First();
            list.RemoveAt(0);
            list.Add(item);
        }

        public static string ToCSV(this IList list)
        {
            string str = string.Empty;

            if (list.Count == 0)
                return str;

            foreach (var value in list)
                str += value.ToString() + ",";

            str = str.Remove(str.Length - 1);

            return str;
        }

        public static List<T> FromCSV<T>(this string str, char separator = ',')
        {
            if (str == string.Empty)
                return new List<T>();

            List<T> list = new();

            foreach (var value in str.Split(separator))
                list.Add((T)Convert.ChangeType(value, typeof(T)));

            return list;
        }

        public static void FindAndAction<T>(this IEnumerable<T> data, Action<T> action) where T : class
        {
            foreach (var item in data)
                action(item);
        }

        public static T FindWhere<T>(this List<T> data, Func<T, bool> action) where T : class
            => (T)data.FirstOrDefault(action);

        public static IEnumerable<T> FindAllWhere<T>(this List<T> data, Func<T, bool> action) where T : class
            => data.Where(action);
    }
}
