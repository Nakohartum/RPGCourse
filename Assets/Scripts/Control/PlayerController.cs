using System;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        private Mover _mover;
        private Fighter _fighter;
        private Health _health;

        private void Awake()
        {
            _mover = GetComponent<Mover>();
            _fighter = GetComponent<Fighter>();
            _health = GetComponent<Health>();
        }

        private void Update()
        {
            if (_health.IsDead)
            {
                return;
            }
            if (InteractWithCombat())
            {
                return;
            }

            if (InteractWithMovement())
            {
                return;
            }
            print("Nothing to do");
        }

        private bool InteractWithCombat()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            foreach (var hit in hits)
            {
                var res = hit.collider.GetComponent<CombatTarget>();
                if (res == null)
                {
                    continue;
                }
                if (!_fighter.CanAttack(res.gameObject))
                {
                    continue;
                }
                if (res != null)
                {
                    if (Input.GetMouseButton(0))
                    {
                        _fighter.Attack(res.gameObject);
                    }

                    return true;
                }
            }

            return false;
        }

        private bool InteractWithMovement()
        {
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (hasHit)
            {
                if (Input.GetMouseButton(0))
                {
                    _mover.StartMoveAction(hit.point);
                }

                return true;
            }

            return false;
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}