using System;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] private float _weaponRange = 2f;
        [SerializeField] private float _timeBetweenAttacks = 1f;
        [SerializeField] private float _weaponDamage = 5f;
        private Mover _mover;
        private Health _combatTarget;
        private Animator _animator;
        private float _timeSinceLastAttack = Mathf.Infinity;

        private void Awake()
        {
            _mover = GetComponent<Mover>();
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;
            if (_combatTarget == null)
            {
                return;
            }

            if (_combatTarget.IsDead)
            {
                return;
            }
            if (!GetIsInRange())
            {
                _mover.MoveTo(_combatTarget.transform.position);
            }
            else
            {
                _mover.Cancel();
                AttackBehaviour();
            }
        }

        private void AttackBehaviour()
        {
            transform.LookAt(_combatTarget.transform);
            if (_timeSinceLastAttack > _timeBetweenAttacks)
            {
                TriggerAttack();
                _timeSinceLastAttack = 0;
            }
        }

        private void TriggerAttack()
        {
            _animator.ResetTrigger("stopAttack");
            _animator.SetTrigger("attack");
        }

        public bool CanAttack(GameObject combatTarget)
        {
            
            if (combatTarget == null)
            {
                return false;
            }
            var health = combatTarget.GetComponent<Health>();
            if (health.IsDead)
            {
                return false;
            }

            return true;
        }

        private bool GetIsInRange()
        {
            bool isInRange = Vector3.Distance(transform.position, _combatTarget.transform.position) < _weaponRange;
            return isInRange;
        }

        public void Attack(GameObject target)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            _combatTarget = target.GetComponent<Health>();
        }

        public void Cancel()
        {
            _combatTarget = null;
            StopAttack();
        }

        private void StopAttack()
        {
            _animator.ResetTrigger("attack");
            _animator.SetTrigger("stopAttack");
        }

        private void Hit()
        {
            if (_combatTarget != null)
            {
                _combatTarget.TakeDamage(_weaponDamage);
            }
        }
    }
}