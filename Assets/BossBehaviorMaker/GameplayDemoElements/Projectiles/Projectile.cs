using UnityEngine;

namespace BossBehaviorMaker.GameplayDemoElements.Projectiles
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    public class Projectile : MonoBehaviour
    {
        public GameObject Sender { get; private set; }
        
        [SerializeField] private float _speed = 10f;
        [SerializeField] private float _lifetime = 5f;
        [SerializeField] private int _damage = 1;
        
        private Vector3 _direction;

        public void Initialize(Vector3 direction, GameObject sender)
        {
            Sender = sender;
            _direction = direction;

            transform.position = sender.transform.position + new Vector3(0, 0.5f, 0);
        }

        private void Update()
        {
            _lifetime -= Time.deltaTime;
            if (_lifetime <= 0)
            {
                Destroy(gameObject);
            }

            Vector3 movement = _direction * (_speed * Time.deltaTime);
            transform.position += movement;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject == Sender)
            {
                return;
            }

            if (collision.gameObject.TryGetComponent(out IAttackReceiver attackReceiver))
            {
                attackReceiver.TakeDamageFrom(this.gameObject, _damage);
            }
            Destroy(gameObject);
        }
    }
}