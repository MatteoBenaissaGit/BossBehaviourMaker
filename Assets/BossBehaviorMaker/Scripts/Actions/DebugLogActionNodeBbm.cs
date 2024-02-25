using BossBehaviorMaker.Scripts.Runtime;
using UnityEngine;

namespace BossBehaviorMaker.Scripts.Actions
{
    public class DebugLogActionNodeBbm : ActionNodeBbm
    {
        public string Message { get; set; }
        
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