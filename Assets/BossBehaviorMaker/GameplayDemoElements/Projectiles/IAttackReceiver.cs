namespace BossBehaviorMaker.GameplayDemoElements.Projectiles
{
    public interface IAttackReceiver
    {
        public void TakeDamageFrom(Projectile projectile, int damage);
    }
}