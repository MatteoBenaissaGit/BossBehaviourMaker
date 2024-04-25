using BossBehaviorMaker.Scripts.Runtime;
using UnityEngine;

namespace BossBehaviorMaker.Scripts.Actions
{
    public class GoBackToRoot : ActionNodeBbm
    {
        public override string ToString()
        {
            return "Go back to root";
        }

        public override string NodeDescription()
        {
            return "This node send the current boss behaviour back to the designed node";
        }

        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override NodeBbmState OnUpdate()
        {
            m_tree.Initialize();
            return NodeBbmState.Success;
        }

        public override void Reset()
        {
            base.Reset();
        }
    }
}