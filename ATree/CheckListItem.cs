using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace ATree
{
    public class CheckListItem
    {
        public CheckListItem Parent;
        public void GetSubTree(List<CheckListItem> l)
        {
            l.AddRange(Childs);
            foreach (var item in Childs)
            {
                item.GetSubTree(l);
            }
        }
        public CheckListStatusTypeEnum Status { get; set; }
        public DateTime? PlannedFinishDate { get; set; }
        public string Name { get; set; }
        public List<CheckListItem> Childs = new List<CheckListItem>();
        public void AddChild(CheckListItem item)
        {
            item.Parent = this;
            Childs.Add(item);
        }

        internal void Detach()
        {
            if (Parent == null) return;
            Parent.Childs.Remove(this);
            Parent = null;
        }

        internal void ParseFromXml(XElement item)
        {
            Name = item.Element("name").Value;
            Status = (CheckListStatusTypeEnum)Enum.Parse(typeof(CheckListStatusTypeEnum), item.Attribute("status").Value);
            DateTime time;
            if (DateTime.TryParse(item.Attribute("plannedFinish").Value, out time))
            {
                PlannedFinishDate = time;
            }

            var ch = item.Element("childs");
            if (ch == null) return;
            foreach (var citem in ch.Elements("item"))
            {
                CheckListItem cc = new CheckListItem();
                cc.ParseFromXml(citem);
                AddChild(cc);
            }
        }
    }

}
