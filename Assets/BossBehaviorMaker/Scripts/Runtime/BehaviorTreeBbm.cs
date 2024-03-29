﻿using System.Collections.Generic;
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

        public void AddChild(NodeBbm parent, NodeBbm child)
        {
            if (Nodes.Contains(parent) == false)
            {
                return;
            }
            parent.AddChild(child);

            if (Nodes.Contains(child))
            {
                return;
            }
            Nodes.Add(child);
        }
        
        public void RemoveChild(NodeBbm parent, NodeBbm child)
        {
            if (Nodes.Contains(parent) == false)
            {
                return;
            }    
            parent.RemoveChild(child);
        }

        public List<NodeBbm> GetChildren(NodeBbm parent)
        {
            return Nodes.Contains(parent) ? parent.GetChildren() : new List<NodeBbm>();
        }
    }
}