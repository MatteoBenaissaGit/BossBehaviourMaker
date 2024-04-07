using BossBehaviorMaker.Scripts.Runtime;
using UnityEngine;

namespace BossBehaviorMaker.Scripts.Composites
{
    public class SelectorCompositeNodeBbm : CompositeNodeBbm
    {
        [field:SerializeField] public int NodeToRun { get; set; } = 0;

        public override string ToString()
        {
            return "Selector";
        }

        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override NodeBbmState OnUpdate()
        {
            if (NodeToRun + 1 > Children.Count)
            {
                return NodeBbmState.Failure;
            }
            return Children[NodeToRun].Update();
        }
    }
}