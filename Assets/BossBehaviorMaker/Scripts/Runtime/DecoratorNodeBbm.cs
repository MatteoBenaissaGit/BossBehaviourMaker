using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace BossBehaviorMaker.Scripts.Runtime
{
    public abstract class DecoratorNodeBbm : NodeBbm
    {
        public NodeBbm Child { get; set; }

        public override void AddChild(NodeBbm child)
        {
            Child = child;
        }

        public override void RemoveChild(NodeBbm child)
        {
            if (child == Child)
            {
                Child = null;
            }
        }

        public override List<NodeBbm> GetChildren()
        {
            List<NodeBbm> list = new List<NodeBbm>();
            if (Child != null)
            {
                list.Add(Child);
            }
            return list;
        }
    }
}