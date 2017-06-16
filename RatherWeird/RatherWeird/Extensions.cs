using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatherWeird
{
    static class Extensions
    {
        public static void Sort(Users collection)
        {
            List<Player> sorted = collection.OrderBy(x => x.Nickname).ToList();
            for (int i = 0; i < sorted.Count; i++)
                collection.Move(collection.IndexOf(sorted[i]), i);
        }
    }
}
