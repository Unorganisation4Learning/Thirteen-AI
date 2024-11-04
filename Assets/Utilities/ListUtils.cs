
namespace Game.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public static class ListUtils
    {
        public static void SafeForEach<T>(this List<T> list, System.Action<T> handler) where T : class
        {
            list.ForEach((item) =>
            {
                if (item != null)
                {
                    handler.Invoke(item);
                }
            });
        }

        public static T SafeFind<T>(this List<T> list, System.Func<T, bool> predicate) where T : class
        {
            return list.Find((item) =>
            {
                if (item != null)
                {
                    return predicate.Invoke(item);
                }
                return false;
            });
        }

        public static int SafeFindIndex<T>(this List<T> list, System.Func<T, bool> predicate) where T : class
        {
            return list.FindIndex((item) =>
            {
                if (item != null)
                {
                    return predicate.Invoke(item);
                }
                return false;
            });
        }

        public static bool IsEmpty(this ICollection collection)
        {
            return collection == null || collection.Count == 0;
        }

        public static bool NotEmpty(this ICollection collection)
        {
            return !collection.IsEmpty();
        }

        public static bool IsCountEqual(this ICollection collection, int count)
        {
            return collection != null && collection.Count == count;
        }

        public static bool IsCountGreaterThan(this ICollection collection, int count)
        {
            return collection != null && collection.Count > count;
        }

        public static bool IsCountGreaterOrEqual(this ICollection collection, int count)
        {
            return collection != null && collection.Count >= count;
        }

        public static bool IsCountLess(this ICollection collection, int count)
        {
            return collection != null && collection.Count < count;
        }

        public static bool IsCountLessOrEqual(this ICollection collection, int count)
        {
            return collection != null && collection.Count <= count;
        }

        public static bool IsIndex(this ICollection collection, int index)
        {
            return index >= 0 && index < collection.SafeCount();
        }

        public static bool TryGetValue<T>(this IList<T> list, int index, out T value)
        {
            value = default;
            if (list != null && index >= 0 && index < list.Count)
            {
                value = list[index];
                return true;
            }
            return false;
        }

        public static bool RemoveAt<T>(this IList<T> list, Predicate<T> predicate)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (predicate.Invoke(list[i]))
                {
                    list.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public static bool AddIfNotExist<T>(this IList<T> list, T item, Predicate<T> predicate)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (predicate.Invoke(list[i]))
                {
                    return false;
                }
            }

            list.Add(item);
            return true;
        }

        public static int SafeCount(this ICollection collection)
        {
            return collection != null ? collection.Count : 0;
        }

        public static int FindIndex<T>(this IList<T> list, Predicate<T> predicate)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (predicate.Invoke(list[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public static bool CompareValue<T>(this IList<T> thisLs, IList<T> otherLs)
        {
            if (thisLs == null && otherLs == null)
            {
                return true;
            }

            if (thisLs == null || otherLs == null)
            {
                return false;
            }

            if (thisLs.Count != otherLs.Count)
            {
                return false;
            }

            int count = thisLs.Count;

            for (int i = 0; i < count; i++)
            {
                if (!thisLs[i].Equals(otherLs[i]))
                {
                    return false;
                }
            }

            return true;
        }
        
        public static bool CompareValue<T>(this IList<T> thisLs, IList<T> otherLs, Func<T, T, bool> comparer)
        {
            if (thisLs == null && otherLs == null)
            {
                return true;
            }

            if (thisLs == null || otherLs == null)
            {
                return false;
            }

            if (thisLs.Count != otherLs.Count)
            {
                return false;
            }

            int count = thisLs.Count;

            for (int i = 0; i < count; i++)
            {
                if (!comparer.Invoke(thisLs[i], otherLs[i]))
                {
                    return false;
                }
            }

            return true;
        }
    
        public static List<object> ToList(this IEnumerator it)
        {
            List<object> result = new List<object>();
            while(it.MoveNext())
            {
                result.Add(it.Current);
            }
            return result;
        }
    }
}
