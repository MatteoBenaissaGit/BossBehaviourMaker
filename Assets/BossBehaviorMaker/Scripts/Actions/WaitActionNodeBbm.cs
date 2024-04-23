using BossBehaviorMaker.Scripts.Runtime;
using UnityEngine;

namespace BossBehaviorMaker.Scripts.Actions
{
    public class WaitActionNodeBbm : ActionNodeBbm
    {
        [SerializeField] public double Duration;

        private float _startTime;

        public override string ToString()
        {
            return "Wait";
        }

        public override string NodeDescription()
        {
            return "This node will wait for a set amount of time before continuing.";
        }

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