using BossBehaviourMaker.Scripts.Runtime;
using UnityEngine;

namespace BossBehaviourMaker.Scripts.Actions
{
    public class WaitActionNodeBbm : ActionNodeBbm
    {
        public float Duration { get; set; } = 1f;

        private float _startTime;
        
        protected override void OnStart()
        {
            _startTime = Time.time;
        }

        protected override void OnStop()
        {
        }

        protected override NodeBbmState OnUpdate()
        {
            return Time.time - _startTime > Duration ? NodeBbmState.Success : NodeBbmState.Running;
        }
    }
}