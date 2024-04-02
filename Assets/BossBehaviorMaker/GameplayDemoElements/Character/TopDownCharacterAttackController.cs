using System;
using BossBehaviorMaker.GameplayDemoElements.Projectiles;
using UnityEngine;

namespace BossBehaviorMaker.GameplayDemoElements.Character
{
    public class TopDownCharacterAttackController : MonoBehaviour
    {
        [SerializeField] private Projectile _projectilePrefab;
        [SerializeField] private Transform _attackAimFeedback;
        [SerializeField] private Animator _characterAnimator;
        [SerializeField] private LayerMask _groundLayer;

        private RaycastHit[] _hits = new RaycastHit[4];
        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
        }
        
        private void Update()
        {
            int hits = Physics.RaycastNonAlloc(_camera.ScreenPointToRay(Input.mousePosition), _hits, Single.MaxValue, _groundLayer);
            _attackAimFeedback.gameObject.SetActive(hits > 0);
            for (int i = 0; i < hits; i++)
            {
                _attackAimFeedback.transform.position = _hits[i].point;
            }

            if (Input.GetMouseButtonDown(0))
            {
                Projectile projectile = Instantiate(_projectilePrefab);
                Vector3 direction = (_attackAimFeedback.position - transform.position);
                direction.y = 0;
                direction.Normalize();
                projectile.Initialize(direction, gameObject);
                _characterAnimator?.SetTrigger("Attack");
            }
        }
    }
}