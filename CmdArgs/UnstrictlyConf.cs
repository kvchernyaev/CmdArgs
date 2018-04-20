#region usings
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
#endregion



namespace CmdArgs
{
    public class UnstrictlyConf : IReadOnlyCollection<UnstrictlyConf.UnstrictlyConfItem>
    {
        public class UnstrictlyConfItem
        {
            public string Name;
            public string Value;


            public UnstrictlyConfItem(string name, string value)
            {
                Name = name;
                Value = value;
            }
        }



        List<UnstrictlyConfItem> _list;


        public UnstrictlyConf()
        {
            _list = new List<UnstrictlyConfItem>();
        }


        public UnstrictlyConf(List<UnstrictlyConfItem> list)
        {
            _list = list ?? throw new ArgumentNullException(nameof(list));
        }


        public IEnumerator<UnstrictlyConfItem> GetEnumerator() => _list.GetEnumerator();


        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        public int Count => _list.Count;


        public UnstrictlyConfItem this[int i] => _list[i];


        internal void Add(UnstrictlyConfItem item)
        {
            if (_list == null) _list = new List<UnstrictlyConfItem>();
            _list.Add(item);
        }


        internal void AddRange(IEnumerable<UnstrictlyConfItem> items)
        {
            if (_list == null) _list = new List<UnstrictlyConfItem>();
            _list.AddRange(items);
        }
    }
}