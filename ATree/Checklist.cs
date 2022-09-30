using System.Collections.Generic;

namespace ATree
{
    public class Checklist
    {
        public List<CheckListItem> Items = new List<CheckListItem>();

        public CheckListItem[] Flatten()
        {
            List<CheckListItem> list = new List<CheckListItem>();
            list.AddRange(Items);
            foreach (var item in Items)
            {                
                item.GetSubTree(list);
            }
            return list.ToArray();
        }
    }

}
