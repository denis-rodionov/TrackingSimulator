using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationCore.Primitives
{
    public class LimitList<T> : IEnumerable<T>
    {
        List<T> list = new List<T>();

        public int Limit { get; set; }

        public LimitList(int limit)
        {
            Limit = limit;
        }

        public void Add(T item)
        {
            if (list.Count == Limit)
                list.Remove(list.First());
            list.Add(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public void Clear()
        {
            list.Clear();
        }
    }
}
