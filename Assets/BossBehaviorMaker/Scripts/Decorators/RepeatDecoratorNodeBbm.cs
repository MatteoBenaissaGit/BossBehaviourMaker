using System;
using BossBehaviorMaker.Scripts.Runtime;
using UnityEngine;

namespace BossBehaviorMaker.Scripts.Decorators
{
    public class RepeatDecoratorNodeBbm : DecoratorNodeBbm
    {
        /// <summary>
        /// Set the number of times to repeat the child node.
        /// -1 let the node run until success.
        /// -2 let the node run until failure.
        /// </summary>
        [field:SerializeField] public int NumberOfRepetitions { get; set; } = -1;

        private int _currentRepetitions;

        public override string ToString()
        {
            return "Repeat";
        }

        protected override void OnStart()
        {
            _currentRepetitions = 0;
        }

        protected override void OnStop()
        {
        }

        protected override NodeBbmState OnUpdate()
        {
            Child.Update();
            if (Child.State == NodeBbmState.Success)
            {
                _currentRepetitions++;
                Child.Restart();
            }

            if (NumberOfRepetitions >= 0)
            {
                return _currentRepetitions >= NumberOfRepetitions ? NodeBbmState.Success : NodeBbmState.Running;
            }
            
            if (NumberOfRepetitions == -1 && Child.State == NodeBbmState.Success)
            {
                switch (Child.State)
                {
                    case NodeBbmState.Success:
                        return NodeBbmState.Success;
                        break;
                    case NodeBbmState.Failure:
                        return NodeBbmState.Success;
                        break;
                }
            }

            return NodeBbmState.Running;
        }
    }
}