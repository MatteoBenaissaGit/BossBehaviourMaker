using System;
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

        public void Initialize(Vector3 direction, GameObject sender, float speedMultiplier = 1f)
        {
            Sender = sender;
            _direction = direction;

            _speed *= speedMultiplier;

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

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == Sender || other.gameObject.GetComponent<Projectile>())
            {
                return;
            }

            if (other.gameObject.TryGetComponent(out IAttackReceiver attackReceiver))
            {
                attackReceiver.TakeDamageFrom(this.gameObject, _damage);
            }
            Destroy(gameObject);
        }
    }
}