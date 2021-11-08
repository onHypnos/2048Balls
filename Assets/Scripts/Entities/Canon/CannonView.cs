using System;
using UnityEngine;

namespace Core
{
    public class CannonView : MonoBehaviour
    {
        private Animator _animator;
        [SerializeField] private bool _playerControlled;
        private Vector3 _currentTarget;
        public bool PlayerControl => _playerControlled;

        private void Awake()
        {
            if (_animator == null)
            {
                if (TryGetComponent(out _animator))
                {
                    
                }
                else
                {
                    Debug.Log("Animator not Set",this.gameObject);
                }
            }
        }


        public void Attack(BallView view)
        {
            view.transform.position = transform.position + Vector3.up * 0.45f;//TODO выделить новую переменную
            view.gameObject.SetActive(true);
            view.RigidBody.AddForce((_currentTarget - view.transform.position).normalized * 15f, ForceMode.Impulse);
            _animator.SetTrigger("Attack");
            
        }

        public void ActivateAimLine()
        {
            
        }

        public void DeactivateAimLine()
        {
            
        }

        public void UpdateRotation(Vector3 target)
        {
            _currentTarget = target;
            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation(target - transform.position ), Time.deltaTime * 10f);
        }
    }
}