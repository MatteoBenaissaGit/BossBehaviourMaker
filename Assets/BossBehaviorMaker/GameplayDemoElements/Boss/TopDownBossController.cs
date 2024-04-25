using System;
using System.Threading.Tasks;
using BossBehaviorMaker.GameplayDemoElements.Character;
using BossBehaviorMaker.GameplayDemoElements.Projectiles;
using BossBehaviorMaker.Scripts.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BossBehaviorMaker.GameplayDemoElements.Boss
{
    public class TopDownBossController : MonoBehaviour, IAttackReceiver
    {
        [Header("Tree Runner") ,SerializeField] private BossBehaviorTreeRunner _treeRunner;
        
        [Header("References") ,SerializeField] private Transform _canvas;
        [SerializeField] private Image _lifeBar;
        [SerializeField] private TMP_Text _lifeText;
        [SerializeField] private TopDownCharacterController _player;
        [SerializeField] private Animator _bossAnimator;
        [SerializeField] private ParticleSystem _deathParticle;

        [Header("Punch attack Parameters") ,SerializeField] private int _punchAttackDamage;
        [SerializeField] private float _punchAttackDistance;
        [SerializeField] private float _punchAttackRadius;
        [SerializeField] private float _punchAttackCastTime;
        
        [Header("Projectile attack Parameters") ,SerializeField] private Projectile _attackProjectile;
        
        private int _life;
        private int _maxLife;
        private float _walkTowardPlayerTimer;
        private float _walkTowardPlayerSpeed;
        private float _lookTowardPlayerTimer;
        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;

            _life = 10;
            _maxLife = _life;
            UpdateLife();

            _treeRunner.OnIdle += SetIdle;
            _treeRunner.OnAttackIndex += SetAttack;
            _treeRunner.OnWalkTowardPlayerForSecondsAtSpeed += SetWalkTowardPlayerForSeconds;
            _treeRunner.OnLookTowardPlayerForSeconds += x => _lookTowardPlayerTimer = x;
        }

        private void Start()
        {
            _treeRunner.StartBehaviorTree(_player.transform,_life,_maxLife);
        }

        private void Update()
        {
            _canvas.forward = -_camera.transform.forward;

            RunTowardPlayer();
            HandleLookTowardPlayer();
        }

        private void UpdateLife()
        {
            _treeRunner.BossCurrentLife = _life;
            
            float percentage = (float)_life / (float)_maxLife;
            _lifeBar.fillAmount = percentage;
            _lifeText.text = $"{_life} / {_maxLife}";
        }

        public void TakeDamageFrom(GameObject attacker, int damage)
        {
            if (attacker == gameObject)
            {
                return;
            }
            
            _life -= damage;
            UpdateLife();
            if (_life <= 0)
            {
                Die();
            }
        }
        
        private async void Die()
        {
            _treeRunner.OnDie.Invoke();
            
            _deathParticle.Play();
            _deathParticle.transform.parent = null;
            
            gameObject.SetActive(false);
            
            await Task.Delay(1500);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void RunTowardPlayer()
        {
            if (_walkTowardPlayerTimer <= 0 || _player == null || _player.gameObject == null)
            {
                return;
            }
            
            _walkTowardPlayerTimer -= Time.deltaTime;
            LookTowardPlayer();
        }

        private void LookTowardPlayer(float lookSpeedLerp = 0.1f)
        {
            Vector3 direction = (_player.transform.position - transform.position).normalized;
            direction.y = 0;
            direction.Normalize();
            transform.position += direction * (_walkTowardPlayerSpeed * Time.deltaTime);
            transform.forward = Vector3.Lerp(transform.forward, direction, lookSpeedLerp);
        }

        private void SetWalkTowardPlayerForSeconds(float seconds, float speed)
        {
            if (_player == null || _player.gameObject == null)
            {
                return;
            }
            
            _walkTowardPlayerSpeed = speed;
            _walkTowardPlayerTimer = seconds;
            
            _bossAnimator.SetBool("IsWalking",true);
        }

        private void SetIdle()
        {
            _walkTowardPlayerSpeed = 0;
            _walkTowardPlayerTimer = 0;
            
            _bossAnimator.SetBool("IsWalking",false);
        }

        private RaycastHit[] _hits = new RaycastHit[16];
        private async void SetAttack(int index)
        {
            switch (index)
            {
                case 0:
                    _bossAnimator.SetTrigger("Attack1");
                    await Task.Delay((int)(_punchAttackCastTime * 1000));
                    if (Application.isPlaying == false)
                    {
                        return;
                    }
                    int hits = Physics.SphereCastNonAlloc(
                        transform.position + (transform.forward * _punchAttackDistance), 
                        _punchAttackRadius, 
                        transform.forward,
                        _hits, 
                        0);
                    for (int i = 0; i < hits; i++)
                    {
                        if (_hits[i].collider.TryGetComponent(out IAttackReceiver attackReceiver))
                        {
                            attackReceiver.TakeDamageFrom(gameObject,_punchAttackDamage);
                        }
                    }
                    break;
                case 1:
                    _bossAnimator.SetTrigger("Attack1");
                    await Task.Delay(250);
                    if (Application.isPlaying == false)
                    {
                        return;
                    }
                    for (int i = -1; i <= 1; i++)
                    {
                        Projectile projectile = Instantiate(_attackProjectile);
                        projectile.transform.position = transform.position + transform.forward * 2;
                        Vector3 direction = Quaternion.Euler(0, i * 15, 0) * transform.forward;
                        projectile.Initialize(direction, gameObject, 0.65f);
                    }
                    break;
            }
        }
        
        private void HandleLookTowardPlayer()
{
            if (_lookTowardPlayerTimer <= 0)
            {
                return;
            }
            
            _lookTowardPlayerTimer -= Time.deltaTime;
            LookTowardPlayer(0.05f);
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + (transform.forward * _punchAttackDistance),_punchAttackRadius);
        }

#endif
    }
}
