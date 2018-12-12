namespace ECTL
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public static class Utils
    {
        public static T[] ToArray<T>(ICollection<T> collection)
        {
            return ToArray<T>(collection, 0);
        }

        public static T[] ToArray<T>(ICollection collection)
        {
            return ToArray<T>(collection, 0);
        }

        public static T[] ToArray<T>(ICollection<T> collection, int nIndex)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            return collection.ToArray<T>();
        }

        public static T[] ToArray<T>(ICollection collection, int nIndex)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            int count = 0;
            T[] array = null;
            lock (collection.SyncRoot)
            {
                count = collection.Count;
                array = new T[count];
                if (count > 0)
                {
                    collection.CopyTo(array, nIndex);
                }
            }
            return array;
        }
    }
}

