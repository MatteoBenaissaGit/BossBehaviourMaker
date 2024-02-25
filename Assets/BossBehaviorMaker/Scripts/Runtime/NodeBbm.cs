using UnityEngine;

namespace BossBehaviorMaker.Scripts.Runtime
{
    public abstract class NodeBbm : ScriptableObject
    {
        public enum NodeBbmState
        {
            Running = 0,
            Success = 1,
            Failure = 2
        }

        public string Guid { get; set; }
        public NodeBbmState State { get; set; } = NodeBbmState.Running;
        
        [SerializeField] private bool _started;

        /// <summary>
        /// Runs when the Node first starts running.
        /// Initialize the Node.
        /// </summary>
        protected abstract void OnStart();
        
        /// <summary>
        /// Runs when the Node stop.
        /// Any cleanup that the node may need to do.
        /// </summary>
        protected abstract void OnStop();
        
        /// <summary>
        /// Runs every update of the node
        /// </summary>
        /// <returns>The state is in once the update is finished.</returns>
        protected abstract NodeBbmState OnUpdate();

        public NodeBbmState Update()
        {
            if (_started == false)
            {
                OnStart();
                _started = true;
            }

            State = OnUpdate();

            if (State != NodeBbmState.Failure && State != NodeBbmState.Success)
            {
                return State;
            }
            
            OnStop();
            _started = false;

            return State;
        }

        public void Restart()
        {
            OnStart();
        }
    }
}