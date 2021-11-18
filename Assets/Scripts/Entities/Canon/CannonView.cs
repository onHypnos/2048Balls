using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Core
{
    public class CannonView : MonoBehaviour
    {
        private Animator _animator;
        [SerializeField] private bool _playerControlled;
        private Vector3 _currentTarget;
        private bool _aimLineActive = false;
        public bool PlayerControl => _playerControlled;
        [SerializeField]private List<GameObject> _aimSpheres;
        [SerializeField]private GameObject _aimSphereExample;
        [SerializeField] private ParticleSystem _fireParticle;
        private Transform _aiTarget;
        [SerializeField] private MMFeedbacks _sound;
        
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
            if (!_playerControlled)
            {
                StartCoroutine(StartAIMovings());
            }else if (_aimSphereExample != null)
            {
                _aimSpheres = new List<GameObject>();
                for (int i = 0; i < 10; i++)
                {
                    _aimSpheres.Add(Instantiate(_aimSphereExample,transform.position, Quaternion.identity, transform));
                }
            }
        }

        private IEnumerator StartAIMovings()
        {
            //TODO AiShitHere
            StartCoroutine(UpdateAiTarget());
            while (this != null)
            {
                Debug.LogWarning("AI strategy not set");
                
                yield return new WaitForSeconds(2f);
            }

            yield return null;
        }

        private IEnumerator UpdateAiTarget()
        {
            if (this == null)
            {
                yield break;
            }
            
            yield return new WaitForSeconds(2f);
        }

        private Ray _rayForward;
        private RaycastHit hitInfo;
        public void Update() //rename to Execute on refactoring
        {
            if (_aimLineActive)
            {
                _rayForward = new Ray(transform.position+Vector3.up*0.5f, transform.forward);
                if(Physics.Raycast(_rayForward, out hitInfo, float.PositiveInfinity, 1<<7))
                {
                    for (int i = 0; i < _aimSpheres.Count; i++)
                    {
                        _aimSpheres[i].transform.position = Vector3.Lerp(transform.position + Vector3.up * 0.5f,
                            transform.position + Vector3.up * 0.5f + transform.forward * hitInfo.distance, (1f* i )/ (_aimSpheres.Count-1));
                    }
                }
                else
                {
                    for (int i = 0; i < _aimSpheres.Count; i++)
                    {
                        _aimSpheres[i].transform.position = Vector3.Lerp(transform.position + Vector3.up * 0.5f,
                            transform.position + Vector3.up * 0.5f + transform.forward * 10f, (1f* i )/ (_aimSpheres.Count-1));
                    }
                }
            }
        }

        public void Attack(BallView view)
        {
            view.transform.position = transform.position + Vector3.up * 0.45f; //TODO выделить новую переменную
            view.gameObject.SetActive(true);
            view.RigidBody.AddForce((_currentTarget - view.transform.position).normalized * 15f, ForceMode.Impulse);
            
            _animator.SetBool("Attack", true);
        }

        public void OnAnimatorAttack()
        {
            
            if (_fireParticle.gameObject.activeSelf)
            {
                _fireParticle.Play();
            }
            else
            {
                _fireParticle.gameObject.SetActive(true);
                _fireParticle.Play();
            }
            _sound.PlayFeedbacks();
            _animator.SetBool("Attack", false);
        }

        public void ActivateAimLine()
        {
            _aimLineActive = true;
        }

        public void DeactivateAimLine()
        {
            for (int i = 0; i < _aimSpheres.Count; i++)
            {
                _aimSpheres[i].transform.position = transform.position + Vector3.down * 5f;
            }
            _aimLineActive = false;
        }

        public void UpdateRotation(Vector3 target)
        {
            _currentTarget.x = target.x;
            _currentTarget.y = transform.position.y;
            _currentTarget.z = target.z;
             transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation(_currentTarget - transform.position ), Time.deltaTime * 10f);
        }
    }
}