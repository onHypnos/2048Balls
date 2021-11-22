using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core
{
    public enum RiderState
    {
        Stay,
    }

    public class RiderView : MonoBehaviour
    {
        public LevelController Controller;
        public Transform Ball;
        public Vector3 PositionRelativeBall;
        private Rigidbody[] _rigs;
        public Rigidbody _hipsRig;

        [SerializeField] private Animator _animator;
        //Animator

        public void Awake()
        {
            _animator.SetBool("Fight", true);
            _rigs = null;
            TurnOffRigidBody();
        }

        public void SetController(LevelController controller)
        {
            Controller = controller;
            controller.AddRider(this);
        }

        public void SetBall(Transform ball, Vector3 positionRelativeBall)
        {
            Ball = ball;
        }

        public void KillEnemy()
        {
            ExplodeRider();
            Controller.KillRider(this);
        }

        public void ExecuteMoving(Vector3 castlePosition)
        {
            transform.position = Ball.position + PositionRelativeBall;
            transform.LookAt(castlePosition + Ball.forward,Vector3.up);
        }

        [Button]
        public void ExplodeRider()
        {
            transform.localScale = Vector3.one;
            TurnOnRigidBody(true,
                Vector3.up * 200f * transform.localScale.x + Vector3.forward * Random.Range(-3f, 3f) * 10f + Vector3.right * Random.Range(-3f, 3f) * 10f);
        }

        //ригидбади
        [Button]
        public void Reset()
        {
            transform.position = Vector3.zero;
            TurnOffRigidBody();
        }

        [Button]
        public void TurnOnRigidBody()
        {
            TurnOnRigidBody(false, Vector3.zero);
        }

        public void TurnOnRigidBody(bool addForce, Vector3 forceDirection)
        {
            if (_rigs == null)
            {
                _rigs = GetComponentsInChildren<Rigidbody>();
            }

            if (addForce)
            {
                for (int i = 0; i < _rigs.Length; i++)
                {
                    _rigs[i].isKinematic = false;
                    _rigs[i].useGravity = true;
                }
                _hipsRig.AddForce(forceDirection, ForceMode.Impulse);
            }
            else
            {
                for (int i = 0; i < _rigs.Length; i++)
                {
                    _rigs[i].isKinematic = false;
                    _rigs[i].useGravity = true;
                }
            }

            _animator.enabled = false;
        }
        
        [Button]
        public void TurnOffRigidBody()
        {
            if (_rigs == null)
            {
                _rigs = GetComponentsInChildren<Rigidbody>();
            }

            for (int i = 0; i < _rigs.Length; i++)
            {
                _rigs[i].isKinematic = true;
                _rigs[i].useGravity = false;
                _rigs[i].velocity = Vector3.zero;
                _rigs[i].angularVelocity = Vector3.zero;
            }

            _animator.enabled = true;
        }
    }
}