using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Thesis.TwoHopCover.Treewidth
{
    public class Index : IIndex
    {
        private Dictionary<int, Dictionary<int, Entry>> Entries { get; set; } = new Dictionary<int, Dictionary<int, Entry>>();

        public void Query(out int length, int v1, int v2)
        {
            length = int.MaxValue;

            if (Entries.TryGetValue(v1, out var v1SubIndex) && Entries.TryGetValue(v2, out var v2SubIndex))
                foreach (var key in v1SubIndex.Keys)
                    if (v2SubIndex.TryGetValue(key, out var v2Entry))
                        length = Math.Min(length, v1SubIndex[key].Length + v2Entry.Length);
        }

        public int[] Query(int v1, int v2)
        {
            var length = int.MaxValue;
            var path = Array.Empty<int>();
            var pathNode = 0;

            if (Entries.TryGetValue(v1, out var v1SubIndex) && Entries.TryGetValue(v2, out var v2SubIndex))
            {
                foreach (var key in v1SubIndex.Keys)
                {
                    if (v2SubIndex.TryGetValue(key, out var v2Entry))
                    {
                        var pathLength = v1SubIndex[key].Length + v2Entry.Length;

                        if (pathLength >= length)
                            continue;

                        length = pathLength;
                        pathNode = key;
                    }
                }

                var path1 = GetPath(v1, pathNode);
                var path2 = GetPath(pathNode, v2);

                path2.Add(v2);
                path = path1.Union(path2).ToArray();
            }

            return path;
        }

        /// <summary>
        /// Gets path from v1 to v2. Does not include v2.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private List<int> GetPath(int v1, int v2)
        {
            var path = new LinkedList<int>();
            path.AddFirst(v2);
            path.AddFirst(v1);

            var current = path.First;

            while (current.Next != null)
            {
                Entry entry;
                var x = current.Value;
                var y = current.Next.Value;
                if (Entries[x].ContainsKey(y))
                    entry = Entries[x][y];
                else
                    entry = Entries[y][x];

                if (entry.Middle != -1)
                    path.AddAfter(current, entry.Middle);
                else
                    current = current.Next;
            }

            return path.ToList();
        }

        public void AddOrUpdate(int v1, int v2, int length, int middle)
        {
            if (!Entries.TryGetValue(v1, out var subIndex))
                Entries[v1] = subIndex = new Dictionary<int, Entry>();
            subIndex[v2] = new Entry() { Length = length, To = v2, Middle = middle };
        }

        public int Size { 
            get {
                var size = 0;
                foreach (var value in Entries.Values)
                    size += value.Keys.Count;
                return size; 
            } 
        }
    }
}
