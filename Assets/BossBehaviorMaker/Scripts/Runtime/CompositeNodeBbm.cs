using System.Collections.Generic;
using UnityEngine;

namespace BossBehaviorMaker.Scripts.Runtime
{
    public abstract class CompositeNodeBbm : NodeBbm
    {
        [SerializeField] public List<NodeBbm> Children;

        public override void AddChild(NodeBbm child)
        {
            Children ??= new List<NodeBbm>();
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