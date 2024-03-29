﻿using BossBehaviorMaker.Scripts.Runtime;
using UnityEngine;

namespace BossBehaviorMaker.Scripts.Composites
{
    public class SequencerCompositeNodeBbm : CompositeNodeBbm
    {
        private int _current;

        public override string ToString()
        {
            return "Sequencer";
        }

        protected override void OnStart()
        {
            _current = 0;
        }

        protected override void OnStop()
        {
        }

        protected override NodeBbmState OnUpdate()
        {
            if (Children.Count <= 0)
            {
                Debug.LogWarning("Sequencer Node has no children.");
                return NodeBbmState.Failure;
            }
            
            return Children[_current]!.Update() switch
            {
                NodeBbmState.Running => NodeBbmState.Running,
                NodeBbmState.Failure => NodeBbmState.Failure,
                NodeBbmState.Success => ++_current >= Children.Count ? 
                    NodeBbmState.Success : NodeBbmState.Running,
                _ => NodeBbmState.Failure
            };
        }
    }
}