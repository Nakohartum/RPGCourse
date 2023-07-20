using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace RPG.Core
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private float _healthPoints = 100f;
        private Animator _animator;
        private ActionScheduler _actionScheduler;
        public bool IsDead { get; private set; } = false;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _actionScheduler = GetComponent<ActionScheduler>();
        }

        public void TakeDamage(float damage)
        {
            _healthPoints = Mathf.Max(_healthPoints - damage, 0);
            if (_healthPoints == 0 && !IsDead)
            {
                Die();
            }
            print($"{_healthPoints}");
        }

        private void Die()
        {
            IsDead = true;
            _animator.SetTrigger("die");
            _actionScheduler.CancelCurrentAction();
        }
    }
}