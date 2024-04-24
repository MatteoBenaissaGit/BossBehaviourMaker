using BossBehaviorMaker.Scripts.Runtime;
using UnityEngine;

namespace BossBehaviorMaker.Scripts.Actions
{
    public class WalkTowardPlayerActionNodeBbm : ActionNodeBbm
    {
        [field:SerializeField] public double Duration { get; set; }
        [field:SerializeField] public double Speed { get; set; }

        private float _timer;
        
        public override string ToString()
        {
            return $"Walk toward player for {Duration}s at speed {Speed}";
        }

        public override string NodeDescription()
        {
            return "This node will make the boss walk toward the player for a certain time.";
        }

        protected override void OnStart()
        {
            Reset();
            m_tree.Runner.OnWalkTowardPlayerForSecondsAtSpeed.Invoke((float)Duration, (float)Speed);
        }

        protected override void OnStop()
        {
            m_tree.Runner.OnIdle?.Invoke();
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
            _timer = (float)Duration;
        }
    }
}