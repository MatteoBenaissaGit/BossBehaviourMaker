using BossBehaviorMaker.Scripts.Runtime;
using UnityEngine;

namespace BossBehaviorMaker.Scripts.Composites
{
    public class SelectorCompositeNodeBbm : CompositeNodeBbm
    {
        [field:SerializeField] public int NodeToRun { get; set; }

        public override string ToString()
        {
            return "Selector";
        }

        public override string NodeDescription()
        {
            return "This node will run each child node in order until one of them succeeds.";
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