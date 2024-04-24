using System;
using BossBehaviorMaker.Scripts.Actions;
using BossBehaviorMaker.Scripts.Composites;
using BossBehaviorMaker.Scripts.Decorators;
using UnityEngine;

namespace BossBehaviorMaker.Scripts.Runtime
{
    /// <summary>
    /// The purpose of this class if to be put on the same game object that holds the boss controller and act as
    /// an interface between the behavior tree and the boss controller you made.
    /// You can subscribe to the actions from you boss controller and you need to update the boss values of this script
    /// to make it work properly.
    /// </summary>
    public class BossBehaviorTreeRunner : MonoBehaviour
    {
        // Events, subscribe to them in your boss controller script to get Events from the tree
        public Action OnStart { get; set; }
        public Action OnIdle { get; set; }
        public Action<float, float> OnWalkTowardPlayerForSecondsAtSpeed { get; set; }
        public Action<int> OnAttackIndex { get; set; }
        public Action OnDie { get; set; }

        // References to player and boss value, update them from your boss controller
        public Transform PlayerTransform { get; private set; }
        public float BossCurrentLife { get; set; }
        public float BossMaxLife { get; private set; }
        
        // Tree reference field to fill in the editor 
        [SerializeField] private BehaviorTreeBbm _tree;

        private bool _isStarted;

        /// <summary>
        /// This method start the behavior tree, call it whenever you want your boss to start acting with the behavior tree
        /// </summary>
        /// <param name="playerTransform">The transform component of the player the boss is refering to</param>
        /// <param name="baseBossLife">The current boss life</param>
        /// <param name="bossMaxLife">The max boss life</param>
        public void StartBehaviorTree(Transform playerTransform, int baseBossLife, int bossMaxLife)
        {
            BossCurrentLife = baseBossLife;
            BossMaxLife = bossMaxLife;

            PlayerTransform = playerTransform;

            _tree.Runner = this;
            _tree.Initialize();
            
            OnStart?.Invoke();
            _isStarted = true;
            _tree.TreeState = NodeBbm.NodeBbmState.Running;
        }

        private void Update()
        {
            if (_tree == null || _isStarted == false)
            {
                return;
            }
            _tree.Update();
        }
    }
}