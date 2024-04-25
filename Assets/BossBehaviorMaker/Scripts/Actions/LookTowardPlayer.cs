using BossBehaviorMaker.Scripts.Runtime;
using UnityEngine;

namespace BossBehaviorMaker.Scripts.Actions
{
    public class LookTowardPlayer : ActionNodeBbm
    {
        [field:SerializeField] public double LookTime { get; set; }
        
        private float _timer;
        
        public override string ToString()
        {
            return $"Look player for {LookTime}s";
        }

        public override string NodeDescription()
        {
            return "This node will make the boss look toward the player for a certain number of seconds";
        }

        protected override void OnStart()
        {
            Reset();
            m_tree.Runner.OnLookTowardPlayerForSeconds?.Invoke((float)LookTime);
        }

        protected override void OnStop()
        {
        }

        protected override NodeBbmState OnUpdate()
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                return NodeBbmState.Success;
            }
            return NodeBbmState.Running;
        }

        public override void Reset()
        {
            base.Reset();
            _timer = (float)LookTime;
        }
    }
}