using BossBehaviorMaker.Scripts.Runtime;
using UnityEngine;

namespace BossBehaviorMaker.Scripts.Actions
{
    public class DebugLogActionNodeBbm : ActionNodeBbm
    {
        [field:SerializeField] public string Message { get; set; }

        public override string ToString()
        {
            return "Debug Log";
        }

        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override NodeBbmState OnUpdate()
        {
            Debug.Log($"{Message}");
            return NodeBbmState.Success;
        }
    }
}