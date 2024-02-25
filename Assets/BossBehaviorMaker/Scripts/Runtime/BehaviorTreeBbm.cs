using System.Collections.Generic;
using UnityEngine;

namespace BossBehaviorMaker.Scripts.Runtime
{
    [CreateAssetMenu(fileName = "BehaviourTreeBbm", menuName = "Tools/BehaviourTreeBbm")]
    public class BehaviorTreeBbm : ScriptableObject
    {
        public NodeBbm RootNode { get; set; }
        public NodeBbm.NodeBbmState TreeState { get; set; } = NodeBbm.NodeBbmState.Running;
        public List<NodeBbm> Nodes { get; set; } = new List<NodeBbm>();

        public NodeBbm.NodeBbmState Update()
        {
            if (RootNode == null)
            {
                Debug.LogWarning($"{name} needs a root node in order to properly run. Please add one.", this);
                TreeState = NodeBbm.NodeBbmState.Failure;
                return TreeState;
            }
            
            if (TreeState == NodeBbm.NodeBbmState.Running) 
            { 
                TreeState = RootNode.Update();
            }

            return TreeState;
        }
    }
}