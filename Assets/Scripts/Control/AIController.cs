using System;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] private float _chaseDistance = 5f;
        [SerializeField] private float _suspicionTime = 3f;
        [SerializeField] private float _waypointTolerance = 1f;
        [SerializeField] private float _dwellingTime = 1f;
        [SerializeField] private float _timeSinceArrivedToThePoint = Mathf.Infinity;
        [SerializeField] private PatrolPath _patrolPath;
        private Fighter _fighter;
        private GameObject _player;
        private Health _health;
        private Mover _mover;
        private ActionScheduler _actionScheduler;
        private Vector3 _guardPosition;
        private float _timeSinceLastSawPlayer = Mathf.Infinity;
        private int _currentWaypointIndex = 0;

        private void Awake()
        {
            _player = GameObject.FindWithTag("Player");
            _fighter = GetComponent<Fighter>();
            _health = GetComponent<Health>();
            _mover = GetComponent<Mover>();
            _actionScheduler = GetComponent<ActionScheduler>();
            _guardPosition = transform.position;
        }

        private void Update()
        {
            if (_health.IsDead)
            {
                return;
            }
            if (Vector3.Distance(transform.position, _player.transform.position) < _chaseDistance && _fighter.CanAttack(_player))
            {
                AttackBehaviour();
            }
            else if (_timeSinceLastSawPlayer < _suspicionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        private void UpdateTimers()
        {
            _timeSinceLastSawPlayer += Time.deltaTime;
            _timeSinceArrivedToThePoint += Time.deltaTime;
        }

        private void SuspicionBehaviour()
        {
            _actionScheduler.CancelCurrentAction();
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = _guardPosition;
            if (_patrolPath != null)
            {
                if (AtWaypoint())
                {
                    _timeSinceArrivedToThePoint = 0f;
                    CycleWaypoint();
                }

                nextPosition = GetCurrentWaypoint();
            }

            if (_timeSinceArrivedToThePoint > _dwellingTime)
            {
                _mover.StartMoveAction(nextPosition);
                
            }
        }

        private Vector3 GetCurrentWaypoint()
        {
            return _patrolPath.GetWaypoint(_currentWaypointIndex);
        }

        private void CycleWaypoint()
        {
            _currentWaypointIndex = _patrolPath.GetNextIndex(_currentWaypointIndex);
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < _waypointTolerance;
        }

        private void AttackBehaviour()
        {
            _fighter.Attack(_player);
            _timeSinceLastSawPlayer = 0f;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _chaseDistance);
        }
    }
}