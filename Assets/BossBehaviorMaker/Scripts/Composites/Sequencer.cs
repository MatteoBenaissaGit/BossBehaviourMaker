using System;
using System.Text;
using BossBehaviorMaker.Scripts.Runtime;
using UnityEngine;

namespace BossBehaviorMaker.Scripts.Composites
{
    public class Sequencer : CompositeNodeBbm
    {
        [field:SerializeField] public string SequenceName { get; set; }
        
        private int _current;

        public override string ToString()
        {
            return $"{((SequenceName == null || SequenceName.Length <= 0) ? "Sequencer" : "Sequence : " + SequenceName)}";
        }

        public override string NodeDescription()
        {
            StringBuilder nodeOrder = new StringBuilder();
            if (Children == null || Children.Count <= 0)
            {
                nodeOrder.Append("No children");
            }
            else
            {
                for (int i = 0; i < Children.Count; i++)
                {
                    nodeOrder.Append($"{i+1} => {Children[i].ToString()}\n");
                }
            }
            
            return "This node will run each child node in order until one of them fails.\n" +
                   "\nNode order :\n" +
                   $"\n{nodeOrder}";
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
            // If the node has no children, return failure.
            if (Children.Count <= 0)
            {
                Debug.LogWarning("Sequencer Node has no children.");
                return NodeBbmState.Failure;
            }
            
            // If the current child node is running, keep running it.
            // If the current child node fails, return failure.
            // If the current child node succeeds, move to the next child node.
            NodeBbmState state = Children[_current]!.Update();
            switch (state)
            {
                case NodeBbmState.Running:
                    return NodeBbmState.Running;
                case NodeBbmState.Success:
                    if (_current + 1 >= Children.Count)
                    {
                        return NodeBbmState.Success;
                    }
                    else
                    {
                        //Debug.Log($"seq : {_current}/{Children.Count} => {_current + 1}/{Children.Count}");
                        _current++;
                        return NodeBbmState.Running;
                    }
                case NodeBbmState.Failure:
                    return NodeBbmState.Failure;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void Reset()
        {
            base.Reset();
            _current = 0;
        }
    }
}