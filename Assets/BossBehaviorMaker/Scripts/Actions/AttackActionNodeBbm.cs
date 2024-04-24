using BossBehaviorMaker.Scripts.Runtime;
using UnityEngine;

namespace BossBehaviorMaker.Scripts.Actions
{
    public class AttackActionNodeBbm : ActionNodeBbm
    {
        [field:SerializeField] public int AttackIndex { get; set; } 
        
        public override string ToString()
        {
            return $"Attack (index:{AttackIndex})";
        }

        public override string NodeDescription()
        {
            return "This node will launch the attack event with the set attack index parameter";
        }

        protected override void OnStart()
        {
            Reset();
        }

        protected override void OnStop()
        {
        }

        protected override NodeBbmState OnUpdate()
        {
            m_tree.Runner.OnAttackIndex?.Invoke(AttackIndex);
            return NodeBbmState.Success;
        }

        public override void Reset()
        {
            base.Reset();
        }
    }
}