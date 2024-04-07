using System;
using BossBehaviorMaker.GameplayDemoElements.Character;
using BossBehaviorMaker.GameplayDemoElements.Projectiles;
using BossBehaviorMaker.Scripts.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BossBehaviorMaker.GameplayDemoElements.Boss
{
    public class BossController : MonoBehaviour, IAttackReceiver
    {
        [Header("Tree Runner") ,SerializeField] private BossBehaviorTreeRunner _treeRunner;
        
        [Header("References") ,SerializeField] private Transform _canvas;
        [SerializeField] private Image _lifeBar;
        [SerializeField] private TMP_Text _lifeText;
        [SerializeField] private TopDownCharacterController _player;

        private int _life;
        private int _maxLife;
        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;

            _life = 10;
            _maxLife = _life;
            UpdateLife();
        }

        private void Start()
        {
            _treeRunner.StartBehaviorTree(_player.transform,_life,_maxLife);
        }

        private void Update()
        {
            _canvas.forward = -_camera.transform.forward;
        }

        private void UpdateLife()
        {
            float percentage = (float)_life / (float)_maxLife;
            _lifeBar.fillAmount = percentage;
            _lifeText.text = $"{_life} / {_maxLife}";
            _treeRunner.BossCurrentLife = _life;
        }

        public void TakeDamageFrom(Projectile projectile, int damage)
        {
            _life -= damage;
            UpdateLife();
        }
    }
}
