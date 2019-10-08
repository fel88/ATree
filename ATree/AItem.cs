using System;
using System.Collections.Generic;
using System.Drawing;

namespace ATree
{
    public class AItem
    {
        public static int NewId = 0;
        public int Id { get; set; }
        public AItem()
        {
            Id = NewId++;
        }
        public List<AItem> Parents = new List<AItem>();
        public string Name { get; set; }
        public float Radius { get; set; } = 40;
        public Brush Brush = Brushes.LightYellow;
        public PointF Position;
        public bool Done { get; set; }
        public bool IsProgress { get; set; }
        public int Progress { get; set; }
        public List<AItem> Childs = new List<AItem>();
        public List<AInfoItem> Infos = new List<AInfoItem>();

        internal void AddChild(AItem aItem)
        {
            Childs.Add(aItem);
            aItem.Parents.Add(this);
        }
    }
}

