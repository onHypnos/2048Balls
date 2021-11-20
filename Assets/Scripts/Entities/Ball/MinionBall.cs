using System;
using UnityEngine;

namespace Core
{
    public class MinionBall : MonoBehaviour
    {
        [SerializeField]private MinionView view;
        private Vector3 basePosition = Vector3.zero;
        [SerializeField]private Rigidbody _rigidbody;
        [SerializeField] private SphereCollider _collider;

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.layer == 7)
            {
                view.ActivateRealBall();
            }
            else if (other.gameObject.layer == 6 || other.gameObject.layer == 9)
            {
                view.DefeatMinion();
                DisableDanger();
            }
        }

        private void DisableDanger()
        {
            _rigidbody.useGravity = true;
            _collider.isTrigger = true;
        }
    }
}