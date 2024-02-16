using System.Collections.Generic;

namespace BossBehaviourMaker.Scripts.Runtime
{
    public abstract class CompositeNodeBbm : NodeBbm
    {
        public List<NodeBbm> Children { get; set; } = new List<NodeBbm>();
    }
}