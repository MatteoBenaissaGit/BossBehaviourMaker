using BossBehaviorMaker.Scripts.Runtime;
using UnityEngine;

namespace BossBehaviorMaker.Scripts.Actions
{
    public class DebugLogActionNodeBbm : ActionNodeBbm
    {
        [SerializeField] public string Message;

        public override string ToString()
        {
            return "Debug Log";
        }

        public override string NodeDescription()
        {
            return "This node will print a debug message to the console.";
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