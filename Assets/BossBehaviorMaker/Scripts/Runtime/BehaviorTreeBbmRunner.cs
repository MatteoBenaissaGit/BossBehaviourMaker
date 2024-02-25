using BossBehaviorMaker.Scripts.Actions;
using BossBehaviorMaker.Scripts.Composites;
using BossBehaviorMaker.Scripts.Decorators;
using UnityEngine;

namespace BossBehaviorMaker.Scripts.Runtime
{
    public class BehaviorTreeBbmRunner : MonoBehaviour
    {
        [SerializeField] private BehaviorTreeBbm _tree;
        
        private void Start()
        {
            _tree = ScriptableObject.CreateInstance<BehaviorTreeBbm>();
            
            DebugLogActionNodeBbm debugLogNode1 = ScriptableObject.CreateInstance<DebugLogActionNodeBbm>();
            debugLogNode1.Message = "1";
            DebugLogActionNodeBbm debugLogNode2 = ScriptableObject.CreateInstance<DebugLogActionNodeBbm>();
            debugLogNode2.Message = "2";

            WaitActionNodeBbm waitNode = ScriptableObject.CreateInstance<WaitActionNodeBbm>();
            waitNode.Duration = 0.5f;
            WaitActionNodeBbm waitNode2 = ScriptableObject.CreateInstance<WaitActionNodeBbm>();
            waitNode2.Duration = 1f;

            SequencerCompositeNodeBbm sequencer = ScriptableObject.CreateInstance<SequencerCompositeNodeBbm>();
            sequencer.Children.Add(debugLogNode1);
            sequencer.Children.Add(waitNode);
            sequencer.Children.Add(debugLogNode2);
            sequencer.Children.Add(waitNode2);
            
            RepeatDecoratorNodeBbm repeatNode = ScriptableObject.CreateInstance<RepeatDecoratorNodeBbm>();
            repeatNode.NumberOfRepetitions = 3;
            repeatNode.Child = sequencer;
            
            _tree.RootNode = repeatNode;
        }

        private void Update()
        {
            _tree.Update();
        }
    }
}