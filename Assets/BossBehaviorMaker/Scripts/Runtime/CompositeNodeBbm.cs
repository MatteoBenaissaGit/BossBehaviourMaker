using System.Collections.Generic;

namespace BossBehaviorMaker.Scripts.Runtime
{
    public abstract class CompositeNodeBbm : NodeBbm
    {
        public List<NodeBbm> Children { get; set; } = new List<NodeBbm>();

        public override void AddChild(NodeBbm child)
        {
            Children.Add(child);
        }

        public override void RemoveChild(NodeBbm child)
        {
            Children.Remove(child);
        }

        public override List<NodeBbm> GetChildren()
        {
            return Children;
        }
    }
}