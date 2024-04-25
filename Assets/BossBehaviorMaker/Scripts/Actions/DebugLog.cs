using BossBehaviorMaker.Scripts.Runtime;
using UnityEngine;

namespace BossBehaviorMaker.Scripts.Actions
{
    public class DebugLog : ActionNodeBbm
    {
        [field:SerializeField] public string Message { get; set; }

        public override string ToString()
        {
            return $"Debug Log \"{(Message.Length >= 5 ? Message.Substring(0,5) + "..." : Message)}\"";
        }

        public override string NodeDescription()
        {
            return "This node will print a debug message to the console.";
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
            Debug.Log($"{Message}");
            return NodeBbmState.Success;
        }

        public override void Reset()
        {
            base.Reset();
        }
    }
}