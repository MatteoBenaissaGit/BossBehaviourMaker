using UnityEngine;

namespace BossBehaviorMaker.GameplayDemoElements.Projectiles
{
    public interface IAttackReceiver
    {
        public void TakeDamageFrom(GameObject attacker, int damage);
    }
}