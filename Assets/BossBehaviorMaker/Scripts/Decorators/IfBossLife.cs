using BossBehaviorMaker.Scripts.Runtime;
using UnityEngine;

namespace BossBehaviorMaker.Scripts.Decorators
{
    public enum BossLifeCondition
    {
        LessThan,
        LessThanOrEqualTo,
        GreaterThan,
        GreaterThanOrEqualTo,
    }
    
    public class IfBossLife : DecoratorNodeBbm
    {
        [field:SerializeField] public BossLifeCondition Condition { get; set; }
        [field:SerializeField] public double Value { get; set; }
        
        public override string ToString()
        {
            return $"If Boss Life {Condition} {Value}";
        }

        public override string NodeDescription()
        {
            return "This node will run the child node depending on the condition set for the set value.";
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
            bool isConditionMet = Condition switch
            {
                BossLifeCondition.LessThan => m_tree.Runner.BossCurrentLife < Value,
                BossLifeCondition.LessThanOrEqualTo => m_tree.Runner.BossCurrentLife <= Value,
                BossLifeCondition.GreaterThan => m_tree.Runner.BossCurrentLife > Value,
                BossLifeCondition.GreaterThanOrEqualTo => m_tree.Runner.BossCurrentLife >= Value,
                _ => false
            };
            
            return isConditionMet == false ? Child.Update() : NodeBbmState.Success;
        }

        public override void Reset()
        {
            base.Reset();
        }
    }
}